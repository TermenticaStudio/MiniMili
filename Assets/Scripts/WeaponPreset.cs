using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Preset", menuName = "Presets/Weapon")]
public class WeaponPreset : ScriptableObject
{
    const string SETTINGS_GROUP = "Settings";
    const string PROJECTILE_GROUP = "Projectile";
    const string RECOIL_GROUP = "Recoil";
    const string RELOADING_GROUP = "Reloading";
    const string SFX_GROUP = "SFX";
    const string INFO_GROUP = "Info";

    [FoldoutGroup(SETTINGS_GROUP)]
    public string id;
    [FoldoutGroup(SETTINGS_GROUP)]
    public int fireRate;
    [FoldoutGroup(SETTINGS_GROUP)]
    public int clipSize;
    [FoldoutGroup(SETTINGS_GROUP)]
    public int clipsCount;
    [FoldoutGroup(SETTINGS_GROUP)]
    public ZoomPreset[] zooms;
    [FoldoutGroup(SETTINGS_GROUP)]
    public bool isOwnedByDefault;
    [FoldoutGroup(SETTINGS_GROUP)]
    public PickupWeapon pickup;

    [FoldoutGroup(PROJECTILE_GROUP)]
    public Projectile projectile;
    [FoldoutGroup(PROJECTILE_GROUP)]
    public uint projectileCountPerShot = 1;
    [FoldoutGroup(PROJECTILE_GROUP), ShowIf("@projectileCountPerShot > 1")]
    public Vector2 minMaxAngleBetweenPerShot = new Vector2(-10, 10);
    [FoldoutGroup(PROJECTILE_GROUP)]
    public float projectileSpeed;
    [FoldoutGroup(PROJECTILE_GROUP)]
    public float projectileRange;
    [FoldoutGroup(PROJECTILE_GROUP)]
    public float projectileDamage;

    [FoldoutGroup(RECOIL_GROUP)]
    public float recoilPower;

    [FoldoutGroup(RELOADING_GROUP)]
    public bool autoReload;
    [FoldoutGroup(RELOADING_GROUP)]
    public float reloadTime = 2f;

    [LabelText("Fire SFX's")]
    [FoldoutGroup(SFX_GROUP)]
    public AudioClip[] fireSFXs;
    [FoldoutGroup(SFX_GROUP)]
    public AudioClip reloadSFX;
    [FoldoutGroup(SFX_GROUP)]
    public AudioClip dryFire;

    [FoldoutGroup(INFO_GROUP)]
    public Sprite icon;
    public Sprite Icon { get => icon; }
    [FoldoutGroup(INFO_GROUP)]
    public new string name;
    public string Name { get => name; }
}