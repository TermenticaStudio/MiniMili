using Feature.Health;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private float damagePerSecond;
    private List<HealthController> damagables = new();

    private void Update()
    {
        foreach (var player in damagables)
        {
            player.Damage(damagePerSecond * Time.deltaTime, null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerH = collision.gameObject.GetComponent<HealthController>();

        if (playerH == null)
            return;

        if (!damagables.Contains(playerH))
            damagables.Add(playerH);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerH = collision.gameObject.GetComponent<HealthController>();

        if (playerH == null)
            return;

        if (damagables.Contains(playerH))
            damagables.Remove(playerH);
    }
}