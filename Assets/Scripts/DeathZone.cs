using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private float damagePerSecond;
    private List<PlayerHealth> players = new();

    private void Update()
    {
        foreach (var player in players)
        {
            player.Damage(damagePerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerH = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerH == null)
            return;

        if (!players.Contains(playerH))
            players.Add(playerH);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerH = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerH == null)
            return;

        if (players.Contains(playerH))
            players.Remove(playerH);
    }
}