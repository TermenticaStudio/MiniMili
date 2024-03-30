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

    private Coroutine jetPackActivation;
    private Coroutine chargeFuelDelay;

    private Rigidbody2D rigid;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody2D>();

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

        JetpackMove();
    }

    private void UpdateUI()
    {
        WeaponInfoUI.Instance.SetJetpackFuel(Mathf.Lerp(0, 1, jetPackFuel / initFuel));
    }

    private void JetpackMove()
    {
        if (PlayerInput.Instance.GetMovement().y <= 0 || jetPackFuel == 0)
        {
            jetPackActive = false;

            if (PlayerInput.Instance.GetMovement().y <= 0)
            {
                ChargeFuel();

                if (chargeFuelDelay == null)
                    chargeFuelDelay = StartCoroutine(ChargeFuelCoroutine());
            }

            if (jetPackActivation == null)
                rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, fallingMaxVelocity);

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
            jetPackActive = true;
        }

        if (jetPackActive)
        {
            UseFuel();
            rigid.AddForce((Vector2.up * jetPackForce * PlayerInput.Instance.GetMovement().y) + (Vector2.right * (jetPackForce / 2f) * PlayerInput.Instance.GetMovement().x), ForceMode2D.Force);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxVelocity);
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
        jetPackActive = true;
        jetPackActivation = null;
    }

    private void JetpackParticles()
    {
        foreach (var item in jetpackParticles)
            item.enableEmission = jetPackActive;
    }
}