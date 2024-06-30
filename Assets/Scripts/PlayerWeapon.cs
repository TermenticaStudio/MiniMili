using DG.Tweening;
using Logic.Player;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    const string SETTINGS_GROUP = "Settings";
    const string RELOADING_GROUP = "Reloading";
    const string VFX_GROUP = "VFX";
    const string SFX_GROUP = "SFX";
    const string INFO_GROUP = "Info";
    const string PROJECTILE_GROUP = "Projectile";
    const string RECOIL_GROUP = "Recoil";

    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private string id;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int fireRate;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int clipSize;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int clipsCount;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private ZoomPreset[] zooms;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private bool isOwnedByDefault;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private PickupWeapon pickup;

    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private Transform projectileSpawnPoint;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private Projectile projectile;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private uint projectileCountPerShot = 1;
    [FoldoutGroup(PROJECTILE_GROUP), ShowIf("@projectileCountPerShot > 1")]
    [SerializeField] private Vector2 minMaxAngleBetweenPerShot = new Vector2(-10, 10);
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private float projectileSpeed;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private float projectileRange;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private float projectileDamage;

    [FoldoutGroup(RECOIL_GROUP)]
    [SerializeField] private Transform recoilPivot;
    [FoldoutGroup(RECOIL_GROUP)]
    [SerializeField] private float recoilPower;

    [FoldoutGroup(RELOADING_GROUP)]
    [SerializeField] private bool autoReload;
    [FoldoutGroup(RELOADING_GROUP)]
    [SerializeField] private float reloadTime = 2f;

    [FoldoutGroup(VFX_GROUP), SerializeField]
    private ParticleSystem muzzle;
    [FoldoutGroup(VFX_GROUP), SerializeField]
    private ParticleSystem shellDrop;

    [FoldoutGroup(SFX_GROUP), SerializeField] private AudioSource sfxSource;
    [LabelText("Fire SFX's")]
    [FoldoutGroup(SFX_GROUP), SerializeField] private AudioClip[] fireSFXs;
    [FoldoutGroup(SFX_GROUP)]
    [SerializeField] private AudioClip reloadSFX;
    [FoldoutGroup(SFX_GROUP)]
    [SerializeField] private AudioClip dryFire;

    [FoldoutGroup(INFO_GROUP)]
    [SerializeField] private Sprite icon;
    public Sprite Icon { get => icon; }
    [FoldoutGroup(INFO_GROUP)]
    [SerializeField] private new string name;
    public string Name { get => name; }

    private Coroutine fireCoroutine;
    private Coroutine reloadCoroutine;
    private bool isDryFiring;
    private ZoomPreset currentZoom;
    private PlayerWeaponsManager weaponsManager;
    private PlayerInfo playerInfo;
    private PlayerAim playerAim;
    private Health playerHealth;

    public int CurrentAmmoCount { get; private set; }
    public int CurrrentClipsCount { get; private set; }
    public bool IsOwned { get; private set; }
    public string ID { get => id; }
    public bool IsActive { get; private set; }

    public void Init()
    {
        weaponsManager = GetComponentInParent<PlayerWeaponsManager>();
        playerInfo = GetComponentInParent<PlayerInfo>();
        playerAim = GetComponentInParent<PlayerAim>();
        playerHealth = GetComponentInParent<Health>();

        playerHealth.OnRevive += OnRevivePlayer;
        playerHealth.OnDie += OnPlayerDie;

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
        if (playerHealth)
        {
            playerHealth.OnRevive -= OnRevivePlayer;
            playerHealth.OnDie -= OnPlayerDie;
        }
    }

    private void OnRevivePlayer()
    {
        CurrentAmmoCount = clipSize;
        CurrrentClipsCount = clipsCount;
        IsOwned = isOwnedByDefault;

        ResetZoom();
    }

    //  [Command]
    public void CmdFire()
    {
        FireRpc();
    }

    // [ClientRpc]
    private void FireRpc()
    {
        if (fireCoroutine == null)
        {
            //Debug.Log($"Player {netId} shitted bullet");
            fireCoroutine = StartCoroutine(FireCoroutine());
        }
    }

    public void ResetFire()
    {
        isDryFiring = false;
    }

    private IEnumerator FireCoroutine()
    {
        if (CurrentAmmoCount == 0)
        {
            if (autoReload)
                CmdReload();
            else
                DryFire();

            fireCoroutine = null;
            yield break;
        }

        CurrentAmmoCount--;
        weaponsManager.UpdateAmmoCountUI(CurrentAmmoCount);

        muzzle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(playerAim.IsFlipped ? 0 : 1, 0, 0);
        muzzle.Play();
        Recoil();

        shellDrop.Emit(1);

        sfxSource.PlayOneShot(fireSFXs[Random.Range(0, fireSFXs.Length)]);

        projectileSpawnPoint.localRotation = Quaternion.Euler(0, 0, playerAim.IsFlipped ? 180 : 0);
        for (int i = 0; i < projectileCountPerShot; i++)
        {
            var rot = Quaternion.Euler(projectileSpawnPoint.eulerAngles);

            if (projectileCountPerShot > 1)
                rot = Quaternion.Euler(projectileSpawnPoint.eulerAngles + Vector3.forward * Random.Range(minMaxAngleBetweenPerShot.x, minMaxAngleBetweenPerShot.y));

            CreateProjectile(rot);
        }

        yield return new WaitForSeconds(60f / fireRate);

        fireCoroutine = null;
    }

    //  [Command]
    private void CreateProjectile(Quaternion rot)
    {
        var projectilePr = PrefabPool.Instance.Get("Bullet").GetComponent<Projectile>();
        //var proj = Instantiate(projectilePr, projectileSpawnPoint.position, rot, null);
        //NetworkServer.Spawn(proj.gameObject);
        projectilePr.Init(playerInfo, projectileSpawnPoint.position, rot, projectileSpeed, projectileRange, projectileDamage);
    }

    //  [Command]
    public void CmdReload()
    {
        ReloadRpc();
    }

    // [ClientRpc]
    private void ReloadRpc()
    {
        if (CurrrentClipsCount == 0)
        {
            DryFire();
            return;
        }

        if (reloadCoroutine == null)
        {
            //Debug.Log($"Player {netId} reloaded");
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        sfxSource.PlayOneShot(reloadSFX);
        yield return new WaitForSeconds(reloadTime);
        CurrentAmmoCount = clipSize;
        weaponsManager.UpdateAmmoCountUI(CurrentAmmoCount);
        CurrrentClipsCount--;
        weaponsManager.UpdateClipCountUI(CurrrentClipsCount);
        reloadCoroutine = null;
    }

    public void IncreaseClips(int count = 1)
    {
        CurrentAmmoCount += count;
    }

    private void DryFire()
    {
        if (isDryFiring)
            return;

        sfxSource.PlayOneShot(dryFire);
        isDryFiring = true;
    }

    public void ResetZoom()
    {
        currentZoom = zooms[0];
        WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);

        CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
    }

    public void ChangeZoom()
    {
        var currentZoomIndex = zooms.ToList().FindIndex(x => x == currentZoom);

        if (currentZoomIndex == zooms.Length - 1)
            currentZoomIndex = 0;
        else
            currentZoomIndex++;

        currentZoom = zooms[currentZoomIndex];
        WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);
        CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
    }

    public void Recoil()
    {
        recoilPivot.transform.DOLocalRotate(Vector3.forward * recoilPower, 0);
        recoilPivot.transform.DOLocalRotate(Vector3.zero, 0.2f);
    }

    public void Drop()
    {
        var instance = Instantiate(pickup, transform.position, transform.rotation, null);
        instance.Init(id, CurrrentClipsCount, CurrentAmmoCount);
        gameObject.SetActive(false);
        IsOwned = false;
        SetAsDeactive();
    }

    public void SetAsActive()
    {
        IsActive = true;
    }

    public void SetAsDeactive()
    {
        IsActive = false;
    }
}