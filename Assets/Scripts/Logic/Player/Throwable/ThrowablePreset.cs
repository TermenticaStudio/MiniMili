using Sirenix.OdinInspector;
using UnityEngine;

namespace Logic.Player.ThrowablesSystem
{
    [CreateAssetMenu(menuName = "Presets/Throwable", fileName = "Throwable")]
    public class ThrowablePreset : ScriptableObject
    {
        const string INFO_GROUP = "Info";
        const string SETTINGS_GROUP = "Settings";

        [FoldoutGroup(SETTINGS_GROUP)]
        public ThrowableObject ThrowObject;
        [FoldoutGroup(SETTINGS_GROUP)]
        public bool isOwnedByDefault;
        [FoldoutGroup(SETTINGS_GROUP)]
        public int startCount = 3;
        [FoldoutGroup(SETTINGS_GROUP)]
        public ThrowablePickup pickup;

        [FoldoutGroup(INFO_GROUP)]
        public string Id;
        [FoldoutGroup(INFO_GROUP)]
        public Sprite Icon;
        [FoldoutGroup(INFO_GROUP)]
        public string Name;
    }
}