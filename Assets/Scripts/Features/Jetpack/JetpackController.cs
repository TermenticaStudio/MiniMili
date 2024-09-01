using Feature.OverlapDetector;
using System.Collections;
using UnityEngine;
using Mirror;

namespace Feature.Jetpack
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JetpackController : NetworkBehaviour
    {
        [SerializeField] private JetpackData data;

        [Header("VFX")]
        [SerializeField] private JetpackFlameController[] jetpackFlames;

        public JetpackData Data { get => data; }

        [SyncVar] private bool _enableJetpack;
        [SyncVar] private bool _isJetPackActive;
        [SyncVar] private bool _isJetpackActivating;
        [SyncVar] private float _throttle;
        [SyncVar] private bool _isGrounded;
        [SyncVar] private float _currentFuel;
        [SyncVar] private bool _isChargingFuel;
        [SyncVar] private bool _isChargingFuelActivating;

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
            if (!isLocalPlayer) return;

            _throttle = Mathf.Clamp01(_directionInput.y);

            CmdCheckGround();
            UpdateUI();
            CmdJetpackParticles();
            CmdJetpackMove();
            CmdStartRefuel();
            CmdChargeFuel();
            CmdUseFuel();
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            if (_isJetPackActive)
                _rigid.AddForce((Vector2.up * data.jetPackForce * _directionInput.y) + (Vector2.right * (data.jetPackForce / 2f) * _directionInput.x), ForceMode2D.Force);

            if (!_isGrounded)
            {
                if (_isJetPackActive || _isJetpackActivating)
                    _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, data.flyingMaxVelocity);
                else
                    _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, data.fallingMaxVelocity);
            }
        }

        private void UpdateUI()
        {
            if (isLocalPlayer)
            {
                WeaponInfoUI.Instance.SetJetpackFuel(Mathf.Lerp(0, 1, _currentFuel / data.jetPackFuel));
            }
        }

        [Command]
        private void CmdJetpackMove()
        {
            if (_isJetpackActivating)
                return;

            if (_isGrounded)
            {
                RpcDeactivateJetpack();

                if (!IsEnoughVerticalInput())
                    return;

                if (!_isJetpackActivating)
                    StartCoroutine(StartJetpackDelayed());
            }

            if (_currentFuel == 0 || _directionInput.y <= 0)
            {
                RpcDeactivateJetpack();
            }
            else
            {
                if (!_isJetpackActivating && IsEnoughVerticalInput())
                    RpcActivateJetpack();
            }
        }

        [Command]
        private void CmdChargeFuel()
        {
            if (!_enableJetpack)
                return;

            if (_isChargingFuelActivating)
                return;

            if (!_isChargingFuel)
                return;

            _currentFuel += Time.deltaTime * data.fuelChargeMutliplier;
            _currentFuel = Mathf.Clamp(_currentFuel, 0, data.jetPackFuel);
        }

        [Command]
        private void CmdStartRefuel()
        {
            if (_isJetPackActive)
                return;

            if (_isChargingFuel)
                return;

            if (_isChargingFuelActivating)
                return;

            if (IsEnoughVerticalInput())
                return;

            StartCoroutine(StartRefuelCoroutine());
        }
        private IEnumerator StartRefuelCoroutine()
        {
            _isChargingFuelActivating = true;
            yield return new WaitForSeconds(data.refuelDelay);
            _isChargingFuel = true;
            _isChargingFuelActivating = false;
        }
        [Command]
        private void CmdUseFuel()
        {
            if (!_isJetPackActive)
                return;

            _isChargingFuel = false;
            _currentFuel -= Time.deltaTime * data.fuelUsageMultiplier * Mathf.Lerp(0.5f, 1, _throttle);
            _currentFuel = Mathf.Clamp(_currentFuel, 0, data.jetPackFuel);
        }

        private IEnumerator StartJetpackDelayed()
        {
            _rigid.AddForce(transform.up * data.jetPackLaunchForce, ForceMode2D.Impulse);
            _isJetpackActivating = true;

            yield return new WaitForSeconds(data.launchDelay);
            RpcActivateJetpack();
            _isJetpackActivating = false;
        }

        [ClientRpc]
        private void RpcActivateJetpack()
        {
            _isJetPackActive = true;

            foreach (var flame in jetpackFlames)
                flame.ActivateFlame(true);
        }

        [ClientRpc]
        private void RpcDeactivateJetpack()
        {
            _isJetPackActive = false;

            foreach (var flame in jetpackFlames)
                flame.ActivateFlame(false);
        }

        [Command]
        private void CmdJetpackParticles()
        {
            RpcJetpackParticles(_throttle);
        }

        [ClientRpc]
        private void RpcJetpackParticles(float throttle)
        {
            foreach (var flame in jetpackFlames)
                flame.SetPower(throttle);
        }

        public void FeedDirectionInput(Vector2 input)
        {
            if (isLocalPlayer)
            {
                _directionInput = input;
            }
        }

        public void ResetFuel()
        {
            _currentFuel = data.jetPackFuel;
        }

        public void PurgeFuel()
        {
            _currentFuel = 0;
        }

        public void EnableJetpack()
        {
            if (isLocalPlayer)
            {
                _enableJetpack = true;
            }
        }

        public void DisableJetpack()
        {
            if (isLocalPlayer)
            {
                _enableJetpack = false;
            }
        }

        public void InjectGroundDetector(OverlapDetectorController groundDetector)
        {
            _groundDetector = groundDetector;
        }

        [Command]
        private void CmdCheckGround()
        {
            if (_groundDetector == null)
            {
                _isGrounded = false;
                return;
            }

            _isGrounded = _groundDetector.IsOverlapped;
        }

        private bool IsEnoughVerticalInput() => _directionInput.y >= 0.8f;
    }
}
