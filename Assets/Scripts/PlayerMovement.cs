using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float acceleration = 0.3f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float deceleration = 2f;

    [Header("Ground Checking")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Footstep")]
    [SerializeField] private AudioSource footstepAS;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepDistance = 0.2f;
    private Vector2 lastFootstepPos;

    private Rigidbody2D rb;
    private PlayerInput playerInput;

    public bool IsGrounded { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        lastFootstepPos = transform.position;
    }

    private void Update()
    {
        IsGrounded = CheckGround();
        Footstep();
    }

    private void FixedUpdate()
    {
        rb.freezeRotation = IsGrounded;

        if (!IsGrounded)
            return;

        if (Mathf.Abs(playerInput.MovementJoystickDirection.x) <= 0.1f)
        {
            Decelerate();
            return;
        }

        Accelerate();
    }

    private void Accelerate()
    {
        rb.velocity += new Vector2(playerInput.MovementJoystickDirection.x * acceleration, 0);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Decelerate()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * deceleration);
    }

    private bool CheckGround()
    {
        if (Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), groundCheckRadius, groundLayer).Length > 0)
            return true;

        return false;
    }

    private void Footstep()
    {
        if (footstepClips.Length == 0)
            return;

        if (Vector2.Distance(transform.position, lastFootstepPos) < footstepDistance)
            return;

        lastFootstepPos = transform.position;
        footstepAS.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
    }
}