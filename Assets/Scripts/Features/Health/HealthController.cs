using Feature.Audio;
using Mirror;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Feature.Health
{
    public class HealthController : NetworkBehaviour
    {
        [SerializeField] private float health;
        [SerializeField] private bool canRegenerateHealth;
        [SerializeField] private float regeneratePerSecond;
        [SerializeField] private AudioClip[] deathSFX;
        [SerializeField] private bool destroyOnDie;

        private float currentHealth;
        private bool isDead = true;

        public bool IsDead { get => isDead; }
        public Logic.Player.Player LastDamageBy { get; private set; }

        public event Action OnDie;
        public event Action OnRevive;
        public event Action<float, float> OnUpdateHealth;

        private void Start()
        {
            Revive();
        }

        public void Revive()
        {
            if (!isDead)
                return;

            currentHealth = health;
            isDead = false;
            OnUpdateHealth?.Invoke(currentHealth, health);
            OnRevive?.Invoke();
        }

        private void Update()
        {
            RegenerateHealth();

            if (Input.GetKeyDown(KeyCode.V))
                Die();

            if (Input.GetKey(KeyCode.B))
                Damage(0.5f, null);
        }

        private void RegenerateHealth()
        {
            if (isDead)
                return;

            if (!canRegenerateHealth)
                return;

            AddHealth(Time.deltaTime * regeneratePerSecond);
        }

        public void AddHealth(float health)
        {
            currentHealth += health;
            currentHealth = Mathf.Clamp(currentHealth, 0, this.health);
            OnUpdateHealth?.Invoke(currentHealth, this.health);
        }

        public bool IsHealthFull() => currentHealth == health;
        [Server]
        public void Damage(float amount, Logic.Player.Player damageBy)
        {
            if (isDead)
                return;

            currentHealth -= amount;
            RpcDamage();
            LastDamageBy = damageBy;

            if (currentHealth <= 0)
                Die();
        }
        [ClientRpc]
        private void RpcDamage()
        {
            OnUpdateHealth?.Invoke(currentHealth, health);

        }
        [Server]
        private void Die()
        {
            if (isDead)
                return;

            currentHealth = 0;
            isDead = true;
            RpcDie();

            if (destroyOnDie)
                NetworkServer.UnSpawn(gameObject);
               // Destroy(gameObject);
        }
        [ClientRpc]
        private void RpcDie()
        {
            if (deathSFX.Length > 0)
            {
                var randomClip = deathSFX[Random.Range(0, deathSFX.Length)];
                AudioManager.Instance.Play2DSFX(randomClip, transform.position);
            }

            OnUpdateHealth?.Invoke(currentHealth, health);
            OnDie?.Invoke();

        }
    }
}