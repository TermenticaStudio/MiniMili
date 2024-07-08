using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Player.WeaponsSystem
{
    public class Weapon : MonoBehaviour
    {
        const string SETTINGS_GROUP = "Settings";
        const string VFX_GROUP = "VFX";
        const string SFX_GROUP = "SFX";
        const string PROJECTILE_GROUP = "Projectile";
        const string RECOIL_GROUP = "Recoil";

        [FoldoutGroup(SETTINGS_GROUP)]
        [SerializeField] private WeaponPreset preset;
        [FoldoutGroup(SETTINGS_GROUP)]
        [SerializeField] private Transform holsterPos;
        [FoldoutGroup(SETTINGS_GROUP)]
        [SerializeField] private string defaultLayer = "Default";
        [FoldoutGroup(SETTINGS_GROUP)]
        [SerializeField] private string holsterLayer;

        [FoldoutGroup(PROJECTILE_GROUP)]
        [SerializeField] private Transform projectileSpawnPoint;

        [FoldoutGroup(RECOIL_GROUP)]
        [SerializeField] private Transform recoilPivot;

        [FoldoutGroup(VFX_GROUP), SerializeField]
        private ParticleSystem muzzle;
        [FoldoutGroup(VFX_GROUP), SerializeField]
        private ParticleSystem shellDrop;

        [FoldoutGroup(SFX_GROUP), SerializeField] private AudioSource sfxSource;

        private Coroutine fireCoroutine;
        private Coroutine reloadCoroutine;
        private bool isDryFiring;
        private ZoomPreset currentZoom;
        private WeaponsManager weaponsManager;
        private PlayerAim playerAim;
        private Player player;

        private Transform defaultParent;
        private Vector3 defaultPos;
        private bool preFirePending;

        public int CurrentAmmoCount { get; private set; }
        public int CurrentClipsCount { get; private set; }
        public int ClipSize { get; private set; }
        public bool IsOwned { get; private set; }
        public string ID { get => preset.id; }
        public bool IsActive { get; private set; }
        public WeaponPreset Preset { get => preset; }

        public event Action OnStartPreFire;
        public event Action OnEndPreFire;

        public void Init()
        {
            weaponsManager = GetComponentInParent<WeaponsManager>();
            playerAim = GetComponentInParent<PlayerAim>();
            player = GetComponentInParent<Player>();

            defaultParent = transform.parent;
            defaultPos = transform.localPosition;

            player.Health.OnRevive += OnRevivePlayer;
            player.Health.OnDie += OnPlayerDie;

            OnRevivePlayer();
            SetAsDeactive();
        }

        private void OnPlayerDie()
        {
            if (!IsActive)
                return;

            Drop();
        }

        private void OnDisable()
        {
            fireCoroutine = null;
            reloadCoroutine = null;
        }

        private void OnDestroy()
        {
            if (player)
            {
                player.Health.OnRevive -= OnRevivePlayer;
                player.Health.OnDie -= OnPlayerDie;
            }
        }

        private void OnRevivePlayer()
        {
            CurrentAmmoCount = preset.clipSize;
            CurrentClipsCount = preset.clipsCount;
            ClipSize = preset.clipSize;

            IsOwned = preset.isOwnedByDefault;

            ResetZoom();
        }

        public void Fire()
        {
            if (!preset.isFirearm)
                return;

            if (fireCoroutine == null)
                fireCoroutine = StartCoroutine(FireCoroutine());
        }

        private IEnumerator FireCoroutine()
        {
            if (IsReloadNeeded())
                yield break;

            yield return PreFire();

            for (int b = 0; b < FireCount(); b++)
            {
                CurrentAmmoCount--;
                weaponsManager.UpdateAmmoCountUI(CurrentAmmoCount, ClipSize);

                muzzle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(playerAim.IsFlipped ? 1 : 0, 0, 0);
                muzzle.Play();

                shellDrop.Emit(1);

                if (preset.fireSFXs.Length > 0)
                {
                    var clip = preset.fireSFXs[Random.Range(0, preset.fireSFXs.Length)];
                    AudioManager.Instance.Play2DSFX(clip, transform.position);
                }

                projectileSpawnPoint.localRotation = Quaternion.Euler(0, 0, playerAim.IsFlipped ? 180 : 0);

                for (int i = 0; i < preset.projectileCountPerShot; i++)
                {
                    var rot = Quaternion.Euler(projectileSpawnPoint.eulerAngles);

                    if (preset.projectileCountPerShot > 1)
                        rot *= Quaternion.Euler(Vector3.forward * Random.Range(preset.minMaxAngleBetweenPerShot.x, preset.minMaxAngleBetweenPerShot.y));

                    // Accuracy
                    var minRan = Mathf.Lerp(WeaponPreset.minRanAccuracyDegree, 0, preset.accuracy);
                    var maxRan = Mathf.Lerp(WeaponPreset.maxRanAccuracyDegree, 0, preset.accuracy);
                    rot *= Quaternion.Euler(Vector3.forward * Random.Range(minRan, maxRan));
                    //

                    CreateProjectile(rot);
                }

                Recoil();

                yield return new WaitForSeconds(60f / preset.fireRate);

                if (preset.fireMode == FireMode.Burst)
                {
                    if (b == FireCount() - 1)
                    {
                        yield return new WaitForSeconds(preset.burstCooldown);
                    }
                }
            }

            fireCoroutine = null;
        }

        private IEnumerator PreFire()
        {
            if (!preset.enablePreFire)
                yield break;

            OnStartPreFire?.Invoke();
            preFirePending = true;
            yield return new WaitForSeconds(preset.preFireDuration);
            preFirePending = false;
            OnEndPreFire?.Invoke();
        }

        private bool IsReloadNeeded()
        {
            if (CurrentAmmoCount != 0)
                return false;

            if (preset.autoReload)
                Reload();
            else
                DryFire();

            fireCoroutine = null;
            return true;
        }

        public void CancelFire()
        {
            isDryFiring = false;

            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);

            fireCoroutine = null;

            if (preFirePending)
            {
                OnEndPreFire?.Invoke();
                preFirePending = false;
            }
        }

        private int FireCount() => preset.fireMode == FireMode.Burst ? preset.firePerBurst : 1;

        private void CreateProjectile(Quaternion rot)
        {
            var projectilePr = PrefabPool.Instance.Get("Bullet").GetComponent<Projectile>();
            projectilePr.Init(player, projectileSpawnPoint.position, rot, preset.projectileSpeed, preset.projectileRange, preset.projectileDamage);
        }

        public void Reload()
        {
            if (CurrentClipsCount == 0)
            {
                DryFire();
                return;
            }

            if (reloadCoroutine == null)
                reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }

        private IEnumerator ReloadCoroutine()
        {
            weaponsManager.StartReload(preset.reloadTime);
            sfxSource.PlayOneShot(preset.reloadSFX);
            yield return new WaitForSeconds(preset.reloadTime);
            CurrentAmmoCount = preset.clipSize;
            CurrentClipsCount--;
            weaponsManager.UpdateAmmoCountUI(CurrentAmmoCount, ClipSize);
            weaponsManager.UpdateClipCountUI(GetTotalLeftAmmo());
            reloadCoroutine = null;
        }

        public void IncreaseClips(int count = 1)
        {
            CurrentClipsCount += count;
            weaponsManager.UpdateClipCountUI(GetTotalLeftAmmo());
        }

        public void SetAmmo(PickupWeapon.Ammo ammo)
        {
            CurrentAmmoCount = ammo.AmmoLeft;
            CurrentClipsCount = ammo.ClipLeft;
        }

        private void DryFire()
        {
            if (isDryFiring)
                return;

            sfxSource.PlayOneShot(preset.dryFire);
            isDryFiring = true;
        }

        public void ResetZoom()
        {
            SelectZoom(preset.zooms[0]);
        }

        public void ChangeZoom()
        {
            var currentZoomIndex = preset.zooms.ToList().FindIndex(x => x == currentZoom);

            if (currentZoomIndex == preset.zooms.Length - 1)
                currentZoomIndex = 0;
            else
                currentZoomIndex++;

            SelectZoom(preset.zooms[currentZoomIndex]);
        }

        private void SelectZoom(ZoomPreset zoomPreset)
        {
            currentZoom = zoomPreset;
            WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);
            CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
            AudioManager.Instance.PlaySFX(preset.changeZoom);
        }

        public void SelectLastZoom()
        {
            if (currentZoom == null)
            {
                SelectZoom(preset.zooms[0]);
                return;
            }

            SelectZoom(currentZoom);
        }

        public void Recoil(float? overridePower = null)
        {
            recoilPivot.transform.DOLocalRotate(Vector3.forward * (overridePower.HasValue ? overridePower.Value : preset.recoilPower), 0);
            recoilPivot.transform.DOLocalRotate(Vector3.zero, 0.2f);
        }

        public void Drop()
        {
            var instance = Instantiate(preset.pickup, transform.position, transform.rotation, null);
            instance.SetAsDropped(CurrentClipsCount, CurrentAmmoCount);
            DisownWeapon();
        }

        public void SetAsActive()
        {
            IsActive = true;
            gameObject.SetActive(true);
            GetInHand();
        }

        public void SetAsDeactive()
        {
            IsActive = false;

            if (IsOwned)
                Holster();
            else
                gameObject.SetActive(false);
        }

        public void OwnWeapon()
        {
            IsOwned = true;
        }

        public void DisownWeapon()
        {
            IsOwned = false;
            SetAsDeactive();
        }

        public void Holster()
        {
            transform.SetParent(holsterPos);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (var rend in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                rend.sortingLayerName = holsterLayer;
            }
        }

        public void GetInHand()
        {
            transform.SetParent(defaultParent);
            transform.SetLocalPositionAndRotation(defaultPos, Quaternion.identity);

            foreach (var rend in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                rend.sortingLayerName = defaultLayer;
            }
        }

        public int GetTotalLeftAmmo()
        {
            return CurrentClipsCount * preset.clipSize;
        }

        public void MeleeAttack()
        {
            if (!CanMelee())
                return;

            DOVirtual.Float(0, 1, 0.8f, value =>
            {
                recoilPivot.DOLocalRotate(Vector3.forward * preset.meleeRotationCurve.Evaluate(value), 0);
            });

            AudioManager.Instance.Play2DSFX(preset.melee, transform.position);
            player.Rigidbody.AddForce(preset.meleeForce * new Vector2(playerAim.IsFlipped ? -1 : 1, 1), ForceMode2D.Impulse);

            var cols = Physics2D.OverlapCircleAll(transform.position, preset.meleeRange);

            foreach (var col in cols)
            {
                var forward = transform.TransformDirection(Vector3.right);
                var toOther = Vector3.Normalize(col.transform.position - transform.position);

                if (Vector3.Dot(forward, toOther) <= 0)
                {
                    // Target is in back, so we ignore it
                    continue;
                }

                // Target is in front
                var damagable = col.GetComponent<IDamagable>();
                damagable?.Damage(player, preset.meleeDamage);
            }
        }

        public bool CanMelee() => preset.melee;
    }
}