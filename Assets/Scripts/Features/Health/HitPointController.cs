using UnityEngine;

namespace Feature.Health
{
    public class HitPointController : MonoBehaviour, IDamagable
    {
        [SerializeField] private float damageMultiplier = 1f;

        private Logic.Player.Player owner;
        private HealthController health;

        private void Start()
        {
            owner = GetComponentInParent<Logic.Player.Player>();
            health = GetComponentInParent<HealthController>();
        }

        public bool Damage(Logic.Player.Player damageBy, float damage, bool ignoreDamageBy = false)
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
}