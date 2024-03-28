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
    
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int fireRate;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int clipSize;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private int clipsCount;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private ZoomPreset[] zooms;
    
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private Transform projectileSpawnPoint;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private Projectile projectile;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private float projectileSpeed;
    [FoldoutGroup(PROJECTILE_GROUP)]
    [SerializeField] private float projectileRange;
    [FoldoutGroup(SETTINGS_GROUP)]
    [SerializeField] private float projectileDamage;

    [FoldoutGroup(RELOADING_GROUP)]
    [SerializeField] private bool autoReload;
    [FoldoutGroup(RELOADING_GROUP)]
    [SerializeField] private float reloadTime = 2f;

    [FoldoutGroup(VFX_GROUP), SerializeField] private ParticleSystem muzzle;

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
    public int CurrentAmmoCount { get; private set; }
    public int CurrrentClipsCount { get; private set; }
    private bool isDryFiring;
    private ZoomPreset currentZoom;
    private PlayerWeaponsManager weaponsManager;

    private void Start()
    {
        weaponsManager = GetComponentInParent<PlayerWeaponsManager>();

        CurrentAmmoCount = clipSize;
        CurrrentClipsCount = clipsCount;

        ResetZoom();
    }

    private void OnDisable()
    {
        fireCoroutine = null;
        reloadCoroutine = null;
    }

    public void Fire()
    {
        if (fireCoroutine == null)
            fireCoroutine = StartCoroutine(FireCoroutine());
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
                Reload();
            else
                DryFire();

            fireCoroutine = null;
            yield break;
        }

        CurrentAmmoCount--;
        weaponsManager.UpdateAmmoCountUI(CurrentAmmoCount);
        muzzle.Play();
        sfxSource.PlayOneShot(fireSFXs[Random.Range(0, fireSFXs.Length)]);

        var proj = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation, null);
        proj.Init(projectileSpeed, projectileRange, projectileDamage);
        proj.RegisterOwner(GetComponentInParent<PlayerInfo>());

        yield return new WaitForSeconds(60f / fireRate);

        fireCoroutine = null;
    }

    public void Reload()
    {
        if (CurrrentClipsCount == 0)
        {
            DryFire();
            return;
        }

        if (reloadCoroutine == null)
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
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
        var currentZoomIndex = zooms.ToList().FindIndex(x=>x ==  currentZoom);

        if (currentZoomIndex == zooms.Length - 1)
            currentZoomIndex = 0;
        else
            currentZoomIndex++;

        currentZoom = zooms[currentZoomIndex];
        WeaponInfoUI.Instance.SetZoomText(currentZoom.Zoom);
        CameraZoomController.Instance.SetLensSize(currentZoom.LensSize);
    }
}