using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float regeneratePerSecond;
    [SerializeField] private Dismantle[] dismantles;
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private float dismantleForce;

    //[SyncVar]
    private float currentHealth;
    // [SyncVar]
    private bool isDead;

    public bool IsDead { get => isDead; }

    private PlayerInfo playerInfo;
    private PlayerWeaponsManager weaponsManager;
    private Rigidbody2D rigid;

    [Serializable]
    public class Dismantle
    {
        public GameObject Part;
        public Vector2 ForceDirection;
        public bool IsHead;
    }

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
        weaponsManager = GetComponent<PlayerWeaponsManager>();
        rigid = GetComponent<Rigidbody2D>();
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
        FixDismantles();
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

        if (Input.GetKeyDown(KeyCode.V))
            Die();

        if (Input.GetKeyDown(KeyCode.S))
            FixDismantles();
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
        DismantleBody();

        PlayerSpawnHandler.Instance.RequestForPlayerRespawn(playerInfo);
    }

    private void DismantleBody()
    {
        weaponsManager.SelectNoWeapon();
        playerInfo.HideName();

        foreach (var item in dismantles)
        {
            var copy = Instantiate(item.Part, item.Part.transform.position, item.Part.transform.rotation, null);
            var rigid = copy.AddComponent<Rigidbody2D>();
            rigid.velocity = this.rigid.velocity;
            rigid.AddForce(item.ForceDirection * dismantleForce, ForceMode2D.Impulse);
            var collider = copy.GetComponentInChildren<Collider2D>();
            collider.isTrigger = false;
            Destroy(copy, 5f);

            item.Part.SetActive(false);

            var bloodPoses = copy.GetComponentsInChildren<BloodPosition>();

            foreach (var pos in bloodPoses)
                Instantiate(bloodParticle, pos.transform.position, pos.transform.rotation, pos.transform);

            // if (item.IsHead)
            //   CameraController.Instance.SetTarget(copy.transform);
        }
    }

    private void FixDismantles()
    {
        foreach (var item in dismantles)
            item.Part.SetActive(true);

        weaponsManager.SelectLastWeapon();
        playerInfo.ShowName();
    }
}