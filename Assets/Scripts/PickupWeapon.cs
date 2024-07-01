using System;
using UnityEngine;

public class PickupWeapon : MonoBehaviour, IPickup
{
    [SerializeField] private AudioClip pickSFX;
    [SerializeField] private WeaponPreset preset;

    private Ammo ammoLeft;
    private bool isDropped;

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

    public void Init(int clipLeft, int ammoLeft)
    {
        this.ammoLeft = new Ammo(clipLeft, ammoLeft);
        isDropped = true;
    }

    public void Pickup()
    {
        AudioManager.Instance.PlaySFX(pickSFX);
        Destroy(gameObject);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public Ammo GetAmmo()
    {
        if (isDropped)
            return ammoLeft;

        return new Ammo(preset.clipsCount, preset.clipSize);
    }
}