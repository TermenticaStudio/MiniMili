using DG.Tweening;
using Logic.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
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
    private PlayerWeaponsManager weaponsManager;
    private PlayerAim playerAim;
    private Player player;

    private Transform defaultParent;
    private Vector3 defaultPos;

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
        weaponsManager = GetComponentInParent<PlayerWeaponsManager>();
        playerAim = GetComponentInParent<PlayerAim>();
        player = GetComponentInParent<Player>();

        defaultParent = transform.parent;
        defaultPos = transform.localPosition;

        player.Health.OnRevive += OnRevivePlayer;
        player.Health.OnDie += OnPlayerDie;

        OnRevivePlayer();
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
                AudioManager.Instance.Play2DSFX(clip, transform.position, player.MainCamera.transform.position);
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
        yield return new WaitForSeconds(preset.preFireDuration);
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
        currentZoom = preset.zooms[0];
        WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);

        CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
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

    private void SelectZoom(ZoomPreset preset)
    {
        currentZoom = preset;
        WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);
        CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
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
        instance.Init(CurrentClipsCount, CurrentAmmoCount);
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
}