using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float regeneratePerSecond;

    [SyncVar]
    private float currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        currentHealth += Time.deltaTime * regeneratePerSecond;
        currentHealth = Mathf.Clamp(currentHealth, 0, health);

        WeaponInfoUI.Instance.SetHealth(currentHealth / health);
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

    }
}