using Logic.Player;
using UnityEngine;

public class HealthBox : PickupObject
{
    [SerializeField] private float healthAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponentInParent<Player>();

        if (player == null)
            return;

        if (player.Health.IsHealthFull())
            return;

        player.Health.AddHealth(healthAmount);
        Destroy(gameObject);
    }
}