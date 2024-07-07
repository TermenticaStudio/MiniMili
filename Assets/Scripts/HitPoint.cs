using Logic.Player;
using Mirror;
using UnityEngine;

public class HitPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private float damageMultiplier = 1f;

    private Health health;
    private Player owner;

    private void Start()
    {
        health = GetComponentInParent<Health>();
        owner = GetComponentInParent<Player>();
    }

    [ServerCallback]
    public bool Damage(Player owner, float damage)
    {
        if (this.owner == owner)
            return false;

        health.Damage(damage * damageMultiplier, owner);
        return true;
    }
}