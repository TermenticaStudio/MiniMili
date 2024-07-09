using Logic.Player;
using UnityEngine;

public class HitPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private float damageMultiplier = 1f;

    private Player owner;
    private Health health;

    private void Start()
    {
        owner = GetComponentInParent<Player>();
        health = GetComponentInParent<Health>();
    }

    public bool Damage(Player damageBy, float damage, bool ignoreDamageBy = false)
    {
        if (health == null)
            return false;

        if (!ignoreDamageBy)
            if (damageBy != null && owner == damageBy)
                return false;

        health.Damage(damage * damageMultiplier, damageBy);
        return true;
    }
}