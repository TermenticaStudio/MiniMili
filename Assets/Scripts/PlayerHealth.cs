using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float regeneratePerSecond;

    //[SyncVar]
    private float currentHealth;
    // [SyncVar]
    private bool isDead;

    public bool IsDead { get => isDead; }

    private PlayerInfo playerInfo;

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
        currentHealth = health;

        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnDisable()
    {
        PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    //}

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();
    //    PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
    //}

    private void OnSpawnPlayer(PlayerInfo obj)
    {
        currentHealth = health;
        WeaponInfoUI.Instance.SetHealth(currentHealth / health);
        isDead = false;
    }

    private void Update()
    {
        //if (!isLocalPlayer)
        //    return;

        if (isDead)
            return;

        currentHealth += Time.deltaTime * regeneratePerSecond;
        currentHealth = Mathf.Clamp(currentHealth, 0, health);

        WeaponInfoUI.Instance.SetHealth(currentHealth / health);
    }

    //[ClientRpc]
    public void Damage(float amount)
    {
        if (isDead)
            return;

        Debug.Log($"damage added {amount}");
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        //if (!playerInfo.isLocalPlayer)
        //    return;

        currentHealth = 0;
        WeaponInfoUI.Instance.SetHealth(currentHealth / health);
        isDead = true;
        CameraController.Instance.SetTarget(null);

        PlayerSpawnHandler.Instance.RequestForPlayerRespawn(playerInfo);
    }
}