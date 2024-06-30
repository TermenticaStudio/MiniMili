using Logic.Player;
using Mirror;
using UnityEngine;

public class HitPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private float damageMultiplier = 1f;

    private Health health;
    private PlayerInfo owner;

    private void Start()
    {
        health = GetComponentInParent<Health>();
        owner = GetComponentInParent<PlayerInfo>();
    }

    [ServerCallback]
    public bool Damage(PlayerInfo owner, float damage)
    {
        if (this.owner == owner)
            return false;

        health.Damage(damage * damageMultiplier);
        return true;
    }
}