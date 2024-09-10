using Feature.Flip;
using Feature.OverlapDetector;
using UnityEngine;
using Mirror;

namespace Feature.Player.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : NetworkBehaviour
    {
        [SerializeField] private MovementData data;

        public MovementData Data { get => data; }

        [SerializeField] private Animator feetAnimator;
        [SerializeField] private CapsuleCollider2D characterCollider;
        private bool _isCrawling;
        private float _targetMaxSpeed;
        private float _targetAcceleration;
        private float _currentMaxSpeed;
        private float _currentAcceleration;

        private float _speedAnimationInput;
        private const string _SPEED_ANIM = "Speed";
        private const string _DIR_ANIM = "Dir";

        [Header("Footstep")]
        [SerializeField] private AudioSource footstepAS;
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private float footstepDistance = 0.2f;
        [SerializeField] private Vector2 pitchRandomness = new Vector2(0.8f, 1);
        private Vector2 _lastFootstepPos;

        private Rigidbody2D _rb;
        private OverlapDetectorController _groundDetector;
        private OverlapDetectorController _ceilDetector;
        private FlipController _flipController;
        private bool _isGrounded;
        private bool _isCeilingAbove;
        private Vector2 _directionInput;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            _lastFootstepPos = transform.position;
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            SlipBrake();
            CheckGround();
            CheckCeiling();

            _isCrawling = _directionInput.y <= -0.4f && _isGrounded;

            if (_isCeilingAbove && _isGrounded)
                _isCrawling = true;

            SetSpeed();

            characterCollider.size = new Vector2(characterCollider.size.x, _isCrawling ? data.crawlHeight : data.standHeight);

            _currentMaxSpeed = Mathf.Lerp(_currentMaxSpeed, _targetMaxSpeed, Time.deltaTime * 5f);
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, _targetAcceleration, Time.deltaTime * 5f);

            _speedAnimationInput = Mathf.Lerp(_speedAnimationInput, (IsFlipped() ? -1 : 1) * _directionInput.x, Time.deltaTime * 5);

            CmdUpdateAnimator(_speedAnimationInput, _isCrawling ? -1 : 0);

            Footstep();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            _rb.freezeRotation = _isGrounded;

            if (!_isGrounded)
                return;

            Move();
        }

        [Command]
        private void CmdUpdateAnimator(float speed, float dir)
        {
            if (isServerOnly)
            {
                feetAnimator.SetFloat(_SPEED_ANIM, speed);
                feetAnimator.SetFloat(_DIR_ANIM, dir);
            }
            RpcUpdateAnimator(speed, dir);

        }

        [ClientRpc]
        private void RpcUpdateAnimator(float speed, float dir)
        {
            feetAnimator.SetFloat(_SPEED_ANIM, speed);
            feetAnimator.SetFloat(_DIR_ANIM, dir);
        }

        private void Move()
        {
            if (_rb.velocity.magnitude > _currentMaxSpeed)
                return;

            _rb.AddForce(new Vector2(_directionInput.x * _currentAcceleration * 1000f, 0));
        }

        private void SetSpeed()
        {
            if (_isCrawling)
            {
                _targetMaxSpeed = data.crawlMaxSpeed * Mathf.Abs(_directionInput.x);
                _targetAcceleration = data.crawlForce;
            }
            else
            {
                _targetMaxSpeed = data.walkMaxSpeed * Mathf.Abs(_directionInput.x);
                _targetAcceleration = data.walkForce;
            }
        }

        private void SlipBrake()
        {
            if (_directionInput.magnitude > 0.1f || !_isGrounded)
            {
                _rb.drag = 0;
                return;
            }

            _rb.drag = 50;
        }

        private void Footstep()
        {
            if (!_isGrounded)
                return;

            if (_directionInput.x == 0)
                return;

            if (footstepClips.Length == 0)
                return;

            if (Vector2.Distance(transform.position, _lastFootstepPos) < footstepDistance)
                return;

            _lastFootstepPos = transform.position;
            var currentClip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAS.clip = currentClip;
            footstepAS.pitch = Random.Range(pitchRandomness.x, pitchRandomness.y);
            footstepAS.Play();
        }

        public void InjectGroundDetector(OverlapDetectorController detector)
        {
            _groundDetector = detector;
        }

        private void CheckGround()
        {
            if (_groundDetector == null)
            {
                _isGrounded = true;
                return;
            }

            _isGrounded = _groundDetector.IsOverlapped;
        }

        public void InjectCeilDetector(OverlapDetectorController detector)
        {
            _ceilDetector = detector;
        }

        private void CheckCeiling()
        {
            if (_ceilDetector == null)
            {
                _isCeilingAbove = false;
                return;
            }

            _isCeilingAbove = _ceilDetector.IsOverlapped;
        }

        public void FeedDirectionInput(Vector2 input)
        {
            _directionInput = input;
        }

        public void InjectFlipController(FlipController controller)
        {
            _flipController = controller;
        }

        private bool IsFlipped()
        {
            return _flipController && _flipController.IsFlipped;
        }
    }
}