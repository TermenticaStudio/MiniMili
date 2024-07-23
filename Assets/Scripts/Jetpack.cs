using System.Collections;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    [SerializeField] private float jetPackForce;
    [SerializeField] private float jetPackLaunchForce;
    [SerializeField] private float launchDelay = 0.65f;
    [SerializeField] private float jetPackFuel = 10;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float fuelUsageMultiplier = 2f;
    [SerializeField] private float fuelChargeMutliplier = 1f;
    [SerializeField] private float fallingMaxVelocity;
    [SerializeField] private JetpackFlame[] jetpackFlames;

    private float currentFuel;
    private bool jetPackActive;
    private bool isChargingFuel;
    private float throttle;
    private Quaternion targetRotation;

    private Coroutine jetPackActivation;
    private Coroutine chargeFuelDelay;

    private Rigidbody2D rigid;
    private PlayerMovement playerMovement;
    private Health playerHealth;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<Health>();

        currentFuel = jetPackFuel;

        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
        playerHealth.OnDie += OnDie;
    }

    private void OnDie()
    {
        currentFuel = 0;
    }

    private void OnDestroy()
    {
        PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
        playerHealth.OnDie -= OnDie;
    }

    private void OnSpawnPlayer(Logic.Player.PlayerInfo obj)
    {
        if (!obj.IsLocal)
            return;

        currentFuel = jetPackFuel;
    }

    private void Update()
    {
        if (playerHealth.IsDead)
            return;

        throttle = Mathf.Clamp01(PlayerInput.Instance.GetMovement().y);

        playerMovement.OverrideMovementVelocity(jetPackActive || jetPackActivation != null);

        UpdateUI();
        JetpackParticles();
        JetpackMove();
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void FixedUpdate()
    {
        if (playerHealth.IsDead)
            return;

        if (jetPackActive)
        {
            rigid.AddForce((Vector2.up * jetPackForce * PlayerInput.Instance.GetMovement().y) + (Vector2.right * (jetPackForce / 2f) * PlayerInput.Instance.GetMovement().x), ForceMode2D.Force);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxVelocity);
        }
    }

    private void UpdateUI()
    {
        WeaponInfoUI.Instance.SetJetpackFuel(Mathf.Lerp(0, 1, currentFuel / jetPackFuel));
    }

    private void JetpackMove()
    {
        if (jetPackActivation != null)
            return;

        if (playerMovement.IsGrounded)
        {
            CmdDeactivateJetpack();
            StartRefuel();
            targetRotation = Quaternion.identity;

            if (PlayerInput.Instance.GetMovement().y < 0.8f)
                return;

            if (jetPackActivation == null)
                jetPackActivation = StartCoroutine(StartJetpackDelayed());
        }

        if (jetPackActive)
        {
            UseFuel();
            var angle = Mathf.Atan2(PlayerInput.Instance.GetMovement().y, PlayerInput.Instance.GetMovement().x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
        }
        else
        {
            StartRefuel();
            targetRotation = Quaternion.identity;
        }

        if (currentFuel == 0 || PlayerInput.Instance.GetMovement().y <= 0)
        {
            CmdDeactivateJetpack();
        }
        else
        {
            if (jetPackActivation == null)
                CmdActivateJetpack();
        }
    }

    private void ChargeFuel()
    {
        if (!isChargingFuel)
            return;

        currentFuel += Time.deltaTime * fuelChargeMutliplier;
        currentFuel = Mathf.Clamp(currentFuel, 0, jetPackFuel);
    }

    private IEnumerator ChargeFuelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        isChargingFuel = true;
        chargeFuelDelay = null;
    }

    private void StartRefuel()
    {
        if (isChargingFuel == false)
        {
            if (chargeFuelDelay == null)
                chargeFuelDelay = StartCoroutine(ChargeFuelCoroutine());

            return;
        }

        ChargeFuel();
    }

    private void UseFuel()
    {
        isChargingFuel = false;
        currentFuel -= Time.deltaTime * fuelUsageMultiplier * Mathf.Lerp(0.5f, 1, throttle);
        currentFuel = Mathf.Clamp(currentFuel, 0, jetPackFuel);
    }

    private IEnumerator StartJetpackDelayed()
    {
        playerMovement.OverrideMovementVelocity(true);
        rigid.drag = 0;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.up * jetPackLaunchForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(launchDelay);
        CmdActivateJetpack();
        jetPackActivation = null;
    }

    private void JetpackParticles()
    {
        foreach (var flame in jetpackFlames)
            flame.SetPower(throttle);
    }

    private void CmdActivateJetpack()
    {
        jetPackActive = true;

        foreach (var flame in jetpackFlames)
            flame.ActivateFlame(true);
    }

    private void CmdDeactivateJetpack()
    {
        jetPackActive = false;

        foreach (var flame in jetpackFlames)
        {
            flame.ActivateFlame(false);
        }
    }
}