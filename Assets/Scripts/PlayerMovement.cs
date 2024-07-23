using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float walkAcceleration = 0.8f;
    [SerializeField] private float walkMaxSpeed = 3f;
    [SerializeField] private float crawlAcceleration = 0.4f;
    [SerializeField] private float crawlMaxSpeed = 1.5f;
    [SerializeField] private float deceleration = 2f;
    [SerializeField] private Animator feetAnimator;
    [SerializeField] private CapsuleCollider2D characterCollider;
    [SerializeField] private float standHeight = 1.9f;
    [SerializeField] private float crawlHeight = 1.4f;
    private bool isCrawling;
    private bool canStand;

    private float targetMaxSpeed;
    private float targetAcceleration;
    private float currentMaxSpeed;
    private float currentAcceleration;

    private float speedAnimationInput;
    const string SPEED_ANIM = "Speed";
    const string DIR_ANIM = "Dir";

    [Header("Ground Checking")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Ceiling Checking")]
    [SerializeField] private Transform ceilChecker;
    [SerializeField] private LayerMask ceilLayer;
    [SerializeField] private float ceilCheckRadius = 0.1f;

    [Header("Footstep")]
    [SerializeField] private AudioSource footstepAS;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepDistance = 0.2f;
    [SerializeField] private Vector2 pitchRandomness = new Vector2(0.8f, 1);
    private Vector2 lastFootstepPos;

    private Rigidbody2D rb;
    private PlayerAim playerAim;
    private Health playerHealth;

    public bool IsGrounded { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAim = GetComponent<PlayerAim>();
        playerHealth = GetComponent<Health>();

        lastFootstepPos = transform.position;
    }

    private void Update()
    {
        if (playerHealth.IsDead)
            return;

        isCrawling = PlayerInput.Instance.GetMovement().y <= -0.4f && IsGrounded;

        canStand = !CheckCeiling();

        if (!canStand && IsGrounded)
            isCrawling = true;

        SetSpeed();

        characterCollider.size = new Vector2(characterCollider.size.x, isCrawling ? crawlHeight : standHeight);

        currentMaxSpeed = Mathf.Lerp(currentMaxSpeed, targetMaxSpeed, Time.deltaTime * 5f);
        currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * 5f);

        feetAnimator.SetFloat(DIR_ANIM, isCrawling ? -1 : 0);
        speedAnimationInput = Mathf.Lerp(speedAnimationInput, (playerAim.IsFlipped ? -1 : 1) * PlayerInput.Instance.GetMovement().x, Time.deltaTime * 5);

        feetAnimator.SetFloat(SPEED_ANIM, speedAnimationInput);
        IsGrounded = CheckGround();

        if (IsGrounded)
            rb.gravityScale = 0;
        else
            rb.gravityScale = 1;

        Footstep();
    }

    private void FixedUpdate()
    {
        SlipBrake();

        if (playerHealth.IsDead)
            return;

        rb.freezeRotation = IsGrounded;

        if (!IsGrounded)
            return;

        if (Mathf.Abs(PlayerInput.Instance.GetMovement().x) <= 0.1f)
        {
            Decelerate();
            return;
        }

        Accelerate();
    }

    private void Accelerate()
    {
        rb.velocity += new Vector2(PlayerInput.Instance.GetMovement().x * currentAcceleration, 0);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentMaxSpeed);
    }

    private void Decelerate()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * deceleration);
    }

    private void SetSpeed()
    {
        if (isCrawling)
        {
            targetMaxSpeed = crawlMaxSpeed;
            targetAcceleration = crawlAcceleration;
        }
        else
        {
            targetMaxSpeed = walkMaxSpeed;
            targetAcceleration = walkAcceleration;
        }
    }

    private bool CheckGround()
    {
        if (Physics2D.OverlapCircleAll(new Vector2(groundChecker.position.x, groundChecker.position.y), groundCheckRadius, groundLayer).Length > 0)
            return true;

        return false;
    }

    private bool CheckCeiling()
    {
        if (Physics2D.OverlapCircleAll(new Vector2(ceilChecker.position.x, ceilChecker.position.y), ceilCheckRadius, ceilLayer).Length > 0)
            return true;

        return false;
    }

    private void SlipBrake()
    {
        if (PlayerInput.Instance.GetMovement().magnitude > 0.1f || !IsGrounded)
        {
            rb.drag = 0;
            return;
        }

        rb.drag = 100;
    }

    private void Footstep()
    {
        if (!IsGrounded)
            return;

        if (PlayerInput.Instance.GetMovement().x == 0)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (groundChecker)
            Gizmos.DrawWireSphere(groundChecker.position, groundCheckRadius);

        if (ceilChecker)
            Gizmos.DrawWireSphere(ceilChecker.position, ceilCheckRadius);
    }
}