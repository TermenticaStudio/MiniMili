using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jetpack : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float jetPackForce;
    [SerializeField] private float jetPackLaunchForce;
    [SerializeField] private float jetPackFuel = 10;
    [SerializeField] private float maxVelocity;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private float fuelUsageMultiplier = 2f;
    [SerializeField] private float fuelChargeMutliplier = 1f;
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private float fallingMaxVelocity;
    [SerializeField] private LayerMask groundLayer;

    private float initFuel;
    private bool isGrounded;
    private bool jetPackActive;
    private bool isChargingFuel;

    private Coroutine jetPackActivation;
    private Coroutine chargeFuelDelay;

    // Start is called before the first frame update
    void Start()
    {
        initFuel = jetPackFuel;
    }

    private void Update()
    {
        isGrounded = CheckGround();

        JetpackMove();
        UpdateUI();
    }

    private void UpdateUI()
    {
        fuelSlider.value = Mathf.Lerp(0, 1, jetPackFuel / initFuel);
    }

    private void JetpackMove()
    {
        if (joystick.Direction.y <= 0 || jetPackFuel == 0)
        {
            jetPackActive = false;

            if (joystick.Direction.y <= 0)
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

        if (isGrounded)
        {
            if (joystick.Direction.y < 0.9f)
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
            rigid.AddForce((Vector2.up * jetPackForce * joystick.Direction.y) + (Vector2.right * (jetPackForce / 2f) * joystick.Direction.x), ForceMode2D.Force);
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

    private bool CheckGround()
    {
        var distanceToGround = groundCheckCollider.bounds.extents.y;
        return Physics2D.Raycast(transform.position, Vector2.down, distanceToGround + 0.1f, groundLayer);
    }

    private IEnumerator StartJetpackDelayed()
    {
        yield return new WaitForSeconds(0.65f);
        jetPackActive = true;
        jetPackActivation = null;
    }
}
