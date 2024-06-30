using Logic.Player;
using System.Collections.Generic;
using UnityEngine;

public class PickupDetector : MonoBehaviour
{
    private PlayerWeaponsManager weaponsManager;
    private Player player;

    private List<IPickup> pickupables = new List<IPickup>();

    private void Start()
    {
        player = GetComponentInParent<Player>();
        weaponsManager = GetComponentInParent<PlayerWeaponsManager>();
    }

    private void Update()
    {
        var nearItem = GetNearestItem();

        if (nearItem == null)
            return;

        var weapon = (PickupWeapon)nearItem;

        if (weapon != null)
        {
            weaponsManager.NotifyWeaponNearby(weapon);
            return;
        }
    }

    public void Pickup()
    {
        var item = GetNearestItem();
        item.Pickup();
    }

    public IPickup GetNearestItem()
    {
        if (player.Health.IsDead)
            return null;

        if (!IsAnyPickupable())
            return null;

        IPickup item = null;
        var distance = 0f;

        foreach (var pick in pickupables)
        {
            if (item == null)
            {
                item = pick;
                distance = Vector2.Distance(transform.position, pick.GetPosition());
            }
            else
            {
                var newDistance = Vector2.Distance(transform.position, pick.GetPosition());

                if (newDistance < distance)
                {
                    item = pick;
                    distance = newDistance;
                }
            }
        }

        return item;
    }

    public bool IsAnyPickupable()
    {
        return pickupables.Count > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pickup = collision.GetComponent<IPickup>();

        if (pickup == null)
            return;

        if (!pickupables.Contains(pickup))
            pickupables.Add(pickup);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pickup = collision.GetComponent<IPickup>();

        if (pickup == null)
            return;

        if (pickupables.Contains(pickup))
            pickupables.Remove(pickup);
    }
}