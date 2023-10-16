using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private FixedJoystick joystick;

    [Header("Movement Setting")]
    [SerializeField] private float acceleration = 0.3f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float deceleration = 2f;

    [Header("Ground Checking")]
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    public bool IsGrounded { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        IsGrounded = CheckGround();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded)
            return;

        if (Mathf.Abs(joystick.Direction.x) <= 0.1f)
        {
            Decelerate();
            return;
        }

        Accelerate();
    }

    private void Accelerate()
    {
        rb.velocity += new Vector2(joystick.Direction.x * acceleration, 0);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Decelerate()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * deceleration);
    }

    private bool CheckGround()
    {
        var distanceToGround = groundCheckCollider.bounds.extents.y;
        return Physics2D.Raycast(transform.position, Vector2.down, distanceToGround + 0.1f, groundLayer);
    }
}