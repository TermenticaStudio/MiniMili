using Feature.Flip;
using Feature.OverlapDetector;
using Mirror;
using UnityEngine;

namespace Feature.Player.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : NetworkBehaviour
    {
        [Header("Movement Setting")]
        [SerializeField] private float walkForce = 0.8f;
        [SerializeField] private float walkMaxSpeed = 3f;
        [SerializeField] private float crawlForce = 0.4f;
        [SerializeField] private float crawlMaxSpeed = 1.5f;
        [SerializeField] private Animator feetAnimator;
        [SerializeField] private CapsuleCollider2D characterCollider;
        [SerializeField] private float standHeight = 1.9f;
        [SerializeField] private float crawlHeight = 1.4f;
        private bool isCrawling;

        private float targetMaxSpeed;
        private float targetAcceleration;
        private float currentMaxSpeed;
        private float currentAcceleration;

        private float speedAnimationInput;
        const string SPEED_ANIM = "Speed";
        const string DIR_ANIM = "Dir";

        [Header("Footstep")]
        [SerializeField] private AudioSource footstepAS;
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private float footstepDistance = 0.2f;
        [SerializeField] private Vector2 pitchRandomness = new Vector2(0.8f, 1);
        private Vector2 lastFootstepPos;

        private Rigidbody2D rb;
        private OverlapDetectorController _groundDetector;
        private OverlapDetectorController _ceilDetector;
        private FlipController _flipController;
        private bool _isGrounded;
        private bool _isCeilingAbove;
        private Vector2 _directionInput;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            lastFootstepPos = transform.position;
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            SlipBrake();
            CheckGround();
            CheckCeiling();

            isCrawling = _directionInput.y <= -0.4f && _isGrounded;

            if (_isCeilingAbove && _isGrounded)
                isCrawling = true;

            SetSpeed();

            characterCollider.size = new Vector2(characterCollider.size.x, isCrawling ? crawlHeight : standHeight);

            currentMaxSpeed = Mathf.Lerp(currentMaxSpeed, targetMaxSpeed, Time.deltaTime * 5f);
            currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * 5f);

            speedAnimationInput = Mathf.Lerp(speedAnimationInput, (IsFlipped() ? -1 : 1) * _directionInput.x, Time.deltaTime * 5);

            CmdUpdateAnimator(speedAnimationInput, isCrawling ? -1 : 0);

            Footstep();
        }

        private void FixedUpdate()
        {
            rb.freezeRotation = _isGrounded;

            if (!_isGrounded)
                return;

            Move();
        }
        [Command]
        private void CmdUpdateAnimator(float speed, float dir)
        {
            if (isServerOnly) 
            {
                feetAnimator.SetFloat(SPEED_ANIM, speed);
                feetAnimator.SetFloat(DIR_ANIM, dir);
            }
            // Update animator parameters on the server
            RpcUpdateAnimator(speed, dir);

        }

        [ClientRpc]
        private void RpcUpdateAnimator(float speed, float dir)
        {
            // Update animator parameters on all clients
            feetAnimator.SetFloat(SPEED_ANIM, speed);
            feetAnimator.SetFloat(DIR_ANIM, dir);
        }
        private void Move()
        {
            if (rb.velocity.magnitude > currentMaxSpeed)
                return;

            rb.AddForce(new Vector2(_directionInput.x * currentAcceleration * 1000f, 0));
        }

        private void SetSpeed()
        {
            if (isCrawling)
            {
                targetMaxSpeed = crawlMaxSpeed * Mathf.Abs(_directionInput.x);
                targetAcceleration = crawlForce;
            }
            else
            {
                targetMaxSpeed = walkMaxSpeed * Mathf.Abs(_directionInput.x);
                targetAcceleration = walkForce;
            }
        }

        private void SlipBrake()
        {
            if (_directionInput.magnitude > 0.1f || !_isGrounded)
            {
                rb.drag = 0;
                return;
            }

            rb.drag = 100;
        }

        private void Footstep()
        {
            if (!_isGrounded)
                return;

            if (_directionInput.x == 0)
                return;

            if (footstepClips.Length == 0)
                return;

            if (Vector2.Distance(transform.position, lastFootstepPos) < footstepDistance)
                return;

            lastFootstepPos = transform.position;
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