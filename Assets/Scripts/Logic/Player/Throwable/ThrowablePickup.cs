using UnityEngine;

namespace Logic.Player.ThrowablesSystem
{
    public class ThrowablePickup : PickupObject
    {
        [SerializeField] private ThrowablePreset preset;

        public string ID { get => preset.Id; }
    }
}