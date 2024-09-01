using Logic.Player.WeaponsSystem;
using Mirror;
using System;
using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    [Serializable]
    public class PickupWeapon : PickupObject
    {
        [SerializeField] private WeaponPreset preset;

        public Ammo ammoLeft;
        public bool isDropped;

        public string ID { get => preset.id; }

        [Serializable]
        public struct Ammo
        {
            public int ClipLeft;
            public int AmmoLeft;

            public Ammo(int clipLeft, int ammoLeft)
            {
                ClipLeft = clipLeft;
                AmmoLeft = ammoLeft;
            }
        }

        public void SetAsDropped(int clips, int ammo)
        {
            ammoLeft = new Ammo(clips, ammo);
            isDropped = true;
        }

        public Ammo GetAmmo()
        {
            if (isDropped)
                return ammoLeft;

            return new Ammo(preset.clipsCount, preset.clipSize);
        }
    }
}