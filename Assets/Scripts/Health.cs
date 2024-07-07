using Logic.Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private bool canRegenerateHealth;
    [SerializeField] private float regeneratePerSecond;
    [SerializeField] private AudioClip[] deathSFX;
    [SerializeField] private bool destroyOnDie;

    private float currentHealth;
    private bool isDead = true;

    public bool IsDead { get => isDead; }
    public Player LastDamageBy { get; private set; }

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

    public void Damage(float amount, Player damageBy)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        OnUpdateHealth?.Invoke(currentHealth, health);
        LastDamageBy = damageBy;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        currentHealth = 0;
        isDead = true;

        if (deathSFX.Length > 0)
        {
            var randomClip = deathSFX[Random.Range(0, deathSFX.Length)];
            AudioManager.Instance.Play2DSFX(randomClip, transform.position, Camera.main.transform.position);
        }

        OnUpdateHealth?.Invoke(currentHealth, health);
        OnDie?.Invoke();

        if (destroyOnDie)
            Destroy(gameObject);
    }
}