using UnityEngine;

public class PickupWeapon : MonoBehaviour, IPickup
{
    private string id;
    private int clipsLeft;
    private int ammoLeft;

    public bool CanPicked { get; private set; }
    public string ID { get => id; }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void Init(string id, int clips, int ammo)
    {
        this.id = id;
        clipsLeft = clips;
        ammoLeft = ammo;
    }

    public void Pickup()
    {
        Destroy(gameObject);
    }
}