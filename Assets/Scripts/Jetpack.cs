using Mirror;
using System.Collections;
using UnityEngine;

public class Jetpack : NetworkBehaviour
{
    [SerializeField] private float jetPackForce;
    [SerializeField] private float jetPackLaunchForce;
    [SerializeField] private float jetPackFuel = 10;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float fuelUsageMultiplier = 2f;
    [SerializeField] private float fuelChargeMutliplier = 1f;
    [SerializeField] private float fallingMaxVelocity;
    [SerializeField] private ParticleSystem[] jetpackParticles;

    private float initFuel;
    [SyncVar]
    private bool jetPackActive;
    private bool isChargingFuel;
    private Quaternion currentRotation;

    private Coroutine jetPackActivation;
    private Coroutine chargeFuelDelay;

    private Rigidbody2D rigid;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();

        initFuel = jetPackFuel;
    }

    private void Update()
    {
        UpdateUI();
        JetpackParticles();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (playerHealth.IsDead)
            return;

        JetpackMove();
        rigid.SetRotation(Quaternion.Lerp(transform.rotation, currentRotation, Time.deltaTime * 5f));
    }

    private void UpdateUI()
    {
        if (!isLocalPlayer)
            return;

        WeaponInfoUI.Instance.SetJetpackFuel(Mathf.Lerp(0, 1, jetPackFuel / initFuel));
    }

    private void JetpackMove()
    {
        if (PlayerInput.Instance.GetMovement().y <= 0 || jetPackFuel == 0)
        {
            CmdDeactivateJetpack();

            if (PlayerInput.Instance.GetMovement().y <= 0)
            {
                ChargeFuel();

                if (chargeFuelDelay == null)
                    chargeFuelDelay = StartCoroutine(ChargeFuelCoroutine());
            }

            if (jetPackActivation == null)
                rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, fallingMaxVelocity);

            currentRotation = Quaternion.identity;
            return;
        }

        chargeFuelDelay = null;

        if (playerMovement.IsGrounded)
        {
            if (PlayerInput.Instance.GetMovement().y < 0.9f)
                return;

            rigid.AddForce(Vector2.up * jetPackLaunchForce, ForceMode2D.Impulse);

            if (jetPackActivation == null)
                jetPackActivation = StartCoroutine(StartJetpackDelayed());
        }
        else if (jetPackActivation == null)
        {
            CmdActivateJetpack();
        }

        if (jetPackActive)
        {
            UseFuel();
            rigid.AddForce((Vector2.up * jetPackForce * PlayerInput.Instance.GetMovement().y) + (Vector2.right * (jetPackForce / 2f) * PlayerInput.Instance.GetMovement().x), ForceMode2D.Force);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxVelocity);

            var angle = Mathf.Atan2(PlayerInput.Instance.GetMovement().y, PlayerInput.Instance.GetMovement().x) * Mathf.Rad2Deg;
            currentRotation = Quaternion.Euler(0f, 0f, angle - 90);
        }
        else
        {
            currentRotation = Quaternion.identity;
        }
    }

    private void ChargeFuel()
    {
        if (!isChargingFuel)
            return;

        jetPackFuel += Time.deltaTime * fuelChargeMutliplier;
        jetPackFuel = Mathf.Clamp(jetPackFuel, 0, initFuel);
    }

    private IEnumerator ChargeFuelCoroutine()
    {
        isChargingFuel = false;
        yield return new WaitForSeconds(1f);
        isChargingFuel = true;
    }

    private void UseFuel()
    {
        jetPackFuel -= Time.deltaTime * fuelUsageMultiplier;
        jetPackFuel = Mathf.Clamp(jetPackFuel, 0, initFuel);
    }

    private IEnumerator StartJetpackDelayed()
    {
        yield return new WaitForSeconds(0.65f);
        CmdActivateJetpack();
        jetPackActivation = null;
    }

    private void JetpackParticles()
    {
        foreach (var item in jetpackParticles)
            item.enableEmission = jetPackActive;
    }

    [Command]
    private void CmdActivateJetpack()
    {
        jetPackActive = true;
    }

    [Command]
    private void CmdDeactivateJetpack()
    {
        jetPackActive = false;
    }
}