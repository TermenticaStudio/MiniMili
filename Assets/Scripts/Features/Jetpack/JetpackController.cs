using Feature.OverlapDetector;
using System.Collections;
using UnityEngine;

namespace Feature.Jetpack
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JetpackController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float jetPackForce;
        [SerializeField] private float jetPackLaunchForce;
        [SerializeField] private float launchDelay = 0.65f;
        [SerializeField] private float flyingMaxVelocity;
        [SerializeField] private float fallingMaxVelocity;

        [Header("Fuel")]
        [SerializeField] private float jetPackFuel = 10;
        [SerializeField] private float fuelUsageMultiplier = 2f;
        [SerializeField] private float fuelChargeMutliplier = 1f;
        [SerializeField] private float refuelDelay = 1;

        [Header("VFX")]
        [SerializeField] private JetpackFlameController[] jetpackFlames;

        private bool _enableJetpack;
        private bool _isJetPackActive;
        private bool _isJetpackActivating;
        private float _throttle;
        private bool _isGrounded;
        private float _currentFuel;
        private bool _isChargingFuel;
        private bool _isChargingFuelActivating;
        private Vector2 _directionInput;
        private Rigidbody2D _rigid;
        private OverlapDetectorController _groundDetector;

        private void Start()
        {
            _rigid = GetComponent<Rigidbody2D>();
            ResetFuel();
        }

        private void Update()
        {
            _throttle = Mathf.Clamp01(_directionInput.y);

            CheckGround();
            UpdateUI();
            JetpackParticles();
            JetpackMove();
            StartRefuel();
            ChargeFuel();
            UseFuel();
        }

        private void FixedUpdate()
        {
            if (_isJetPackActive)
                _rigid.AddForce((Vector2.up * jetPackForce * _directionInput.y) + (Vector2.right * (jetPackForce / 2f) * _directionInput.x), ForceMode2D.Force);

            if (!_isGrounded)
            {
                if (_isJetPackActive || _isJetpackActivating)
                    _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, flyingMaxVelocity);
                else
                    _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, fallingMaxVelocity);
            }
        }

        private void UpdateUI()
        {
            WeaponInfoUI.Instance.SetJetpackFuel(Mathf.Lerp(0, 1, _currentFuel / jetPackFuel));
        }

        private void JetpackMove()
        {
            if (_isJetpackActivating)
                return;

            if (_isGrounded)
            {
                DeactivateJetpack();

                if (_directionInput.y < 0.8f)
                    return;

                if (!_isJetpackActivating)
                    StartCoroutine(StartJetpackDelayed());
            }

            if (_currentFuel == 0 || _directionInput.y <= 0)
            {
                DeactivateJetpack();
            }
            else
            {
                if (!_isJetpackActivating)
                    ActivateJetpack();
            }
        }

        private void ChargeFuel()
        {
            if (!_enableJetpack)
                return;

            if (_isChargingFuelActivating)
                return;

            if (!_isChargingFuel)
                return;

            _currentFuel += Time.deltaTime * fuelChargeMutliplier;
            _currentFuel = Mathf.Clamp(_currentFuel, 0, jetPackFuel);
        }

        private IEnumerator StartRefuelCoroutine()
        {
            _isChargingFuelActivating = true;
            yield return new WaitForSeconds(refuelDelay);
            _isChargingFuel = true;
            _isChargingFuelActivating = false;
        }

        private void StartRefuel()
        {
            if (_isJetPackActive)
                return;

            if (_isChargingFuel)
                return;

            if (_isChargingFuelActivating)
                return;

            StartCoroutine(StartRefuelCoroutine());
        }

        private void UseFuel()
        {
            if (!_isJetPackActive)
                return;

            _isChargingFuel = false;
            _currentFuel -= Time.deltaTime * fuelUsageMultiplier * Mathf.Lerp(0.5f, 1, _throttle);
            _currentFuel = Mathf.Clamp(_currentFuel, 0, jetPackFuel);
        }

        private IEnumerator StartJetpackDelayed()
        {
            _rigid.AddForce(transform.up * jetPackLaunchForce, ForceMode2D.Impulse);
            _isJetpackActivating = true;
            yield return new WaitForSeconds(launchDelay);
            ActivateJetpack();
            _isJetpackActivating = false;
        }

        private void JetpackParticles()
        {
            foreach (var flame in jetpackFlames)
                flame.SetPower(_throttle);
        }

        private void ActivateJetpack()
        {
            _isJetPackActive = true;

            foreach (var flame in jetpackFlames)
                flame.ActivateFlame(true);
        }

        private void DeactivateJetpack()
        {
            _isJetPackActive = false;

            foreach (var flame in jetpackFlames)
                flame.ActivateFlame(false);
        }

        public void FeedDirectionInput(Vector2 input)
        {
            _directionInput = input;
        }

        public void ResetFuel()
        {
            _currentFuel = jetPackFuel;
        }

        public void PurgeFuel()
        {
            _currentFuel = 0;
        }

        public void EnableJetpack()
        {
            _enableJetpack = true;
        }

        public void DisableJetpack()
        {
            _enableJetpack = false;
        }

        public void InjectGroundDetector(OverlapDetectorController groundDetector)
        {
            _groundDetector = groundDetector;
        }

        private void CheckGround()
        {
            if (_groundDetector == null)
            {
                _isGrounded = false;
                return;
            }

            _isGrounded = _groundDetector.IsOverlapped;
        }
    }
}