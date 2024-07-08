using Sirenix.OdinInspector;
using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    [CreateAssetMenu(fileName = "Weapon Preset", menuName = "Presets/Weapon")]
    public class WeaponPreset : ScriptableObject
    {
        const string SETTINGS_GROUP = "Settings";
        const string PRE_FIRE_GROUP = "Pre Fire";
        const string PROJECTILE_GROUP = "Projectile";
        const string RECOIL_GROUP = "Recoil";
        const string RELOADING_GROUP = "Reloading";
        const string SFX_GROUP = "SFX";
        const string MELEE_GROUP = "Melee";
        const string INFO_GROUP = "Info";

        [FoldoutGroup(SETTINGS_GROUP)]
        public string id;
        [FoldoutGroup(SETTINGS_GROUP)]
        public bool isFirearm;
        [FoldoutGroup(SETTINGS_GROUP)]
        public int fireRate;
        [FoldoutGroup(SETTINGS_GROUP)]
        public FireMode fireMode;
        [FoldoutGroup(SETTINGS_GROUP), ShowIf("@fireMode == FireMode.Burst")]
        public int firePerBurst = 3;
        [FoldoutGroup(SETTINGS_GROUP), ShowIf("@fireMode == FireMode.Burst")]
        public float burstCooldown = 0.3f;
        [FoldoutGroup(SETTINGS_GROUP)]
        [Range(0, 1)] public float accuracy = 1;
        public const float minRanAccuracyDegree = -15;
        public const float maxRanAccuracyDegree = 15;
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

        [FoldoutGroup(PRE_FIRE_GROUP)]
        [LabelText("Enable")] public bool enablePreFire;
        [FoldoutGroup(PRE_FIRE_GROUP), ShowIf("@enablePreFire")]
        public float preFireDuration = 1f;

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

        [FoldoutGroup(MELEE_GROUP)]
        [LabelText("Enable")] public bool enableMelee;
        [FoldoutGroup(MELEE_GROUP), ShowIf("@enableMelee")]
        [LabelText("Force")] public Vector2 meleeForce;
        [FoldoutGroup(MELEE_GROUP), ShowIf("@enableMelee")]
        [LabelText("Damage")] public float meleeDamage;
        [FoldoutGroup(MELEE_GROUP), ShowIf("@enableMelee")]
        [LabelText("Range")] public float meleeRange;
        [FoldoutGroup(MELEE_GROUP), ShowIf("@enableMelee")]
        [LabelText("Rotation Curve")] public AnimationCurve meleeRotationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.1f, 70), new Keyframe(0.26f, -25), new Keyframe(1, 0) });

        [LabelText("Fire SFX's")]
        [FoldoutGroup(SFX_GROUP)]
        public AudioClip[] fireSFXs;
        [FoldoutGroup(SFX_GROUP)]
        public AudioClip reloadSFX;
        [FoldoutGroup(SFX_GROUP)]
        public AudioClip dryFire;
        [FoldoutGroup(SFX_GROUP)]
        public AudioClip changeZoom;
        [FoldoutGroup(SFX_GROUP)]
        public AudioClip melee;

        [FoldoutGroup(INFO_GROUP)]
        public Sprite icon;
        public Sprite Icon { get => icon; }
        [FoldoutGroup(INFO_GROUP)]
        public new string name;
        public string Name { get => name; }
    }
}