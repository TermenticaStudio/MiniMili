using Logic.Player;
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

    public bool Damage(Player owner, float damage)
    {
        if (owner != null)
            if (this.owner == owner)
                return false;

        health.Damage(damage * damageMultiplier, owner);
        return true;
    }
}