using Logic.Player;
using Logic.Player.ThrowablesSystem;
using Logic.Player.WeaponsSystem;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(10)]
public class PickupDetector : MonoBehaviour
{
    private WeaponsManager weaponsManager;
    private ThrowablesManager playerThrowables;
    private Player player;

    private List<PickupObject> pickupables = new List<PickupObject>();

    private void Start()
    {
        Setup();
    }
    private void Setup()
    {
        player = GetComponentInParent<Player>();
        weaponsManager = GetComponentInParent<WeaponsManager>();
        playerThrowables = GetComponentInParent<ThrowablesManager>();
    }
    private void UpdateNearestItemNotification()
    {
        if (player == null) Setup();
        if (player.Health.IsDead)
            return;

        var nearItem = GetNearestItem();

        if (nearItem == null)
        {
            weaponsManager.NotifyWeaponNearby(null);
            playerThrowables.NotifyThrowableNearby(null);
            return;
        }

        var weapon = nearItem.GetComponent<PickupWeapon>();
        weaponsManager.NotifyWeaponNearby(weapon);

        var throwable = nearItem.GetComponent<ThrowablePickup>();
        playerThrowables.NotifyThrowableNearby(throwable);
    }

    public PickupObject GetNearestItem()
    {
        if (!IsAnyPickupable())
            return null;

        PickupObject item = null;
        var distance = 0f;

        foreach (var pick in pickupables)
        {
            if (item == null)
            {
                item = pick;
                distance = Vector2.Distance(transform.position, pick.transform.position);
            }
            else
            {
                var newDistance = Vector2.Distance(transform.position, pick.transform.position);

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
        var pickup = collision.GetComponent<PickupObject>();

        if (pickup == null)
            return;

        if (!pickupables.Contains(pickup))
            pickupables.Add(pickup);

        UpdateNearestItemNotification();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pickup = collision.GetComponent<PickupObject>();

        if (pickup == null)
            return;

        if (pickupables.Contains(pickup))
            pickupables.Remove(pickup);

        UpdateNearestItemNotification();
    }
}