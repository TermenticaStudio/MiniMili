using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform skinPivot;
    [SerializeField] private Transform rightHand;

    [Header("Left Hand")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Vector2 leftHandMinMaxRotation;

    [Header("Head")]
    [SerializeField] private Transform headPivot;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private Vector2 minMaxRotationLimit;

    private Vector2 lastAimDirection;

    // [SyncVar]
    private bool isFlipped;

    public bool IsFlipped { get => isFlipped; }

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        //    if (!isLocalPlayer)
        //        return;

        if (playerHealth.IsDead)
            return;

        Aim();
    }

    private void Aim()
    {
        if (PlayerInput.Instance.GetAim().magnitude != 0)
            lastAimDirection = PlayerInput.Instance.GetAim();

        var rot_z = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        rightHand.rotation = Quaternion.Euler(0, 0, rot_z);
        leftHand.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rot_z, leftHandMinMaxRotation.x, leftHandMinMaxRotation.y));
        headPivot.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rot_z, minMaxRotationLimit.x, minMaxRotationLimit.y));

        var dot = Vector2.Dot(rightHand.right, Vector2.right);

        if (dot < 0)
            Flip(-1);
        else
            Flip(1);

        // Fix rotation if player is flipped
        if (isFlipped)
        {
            rightHand.rotation = Quaternion.Euler(0, 0, rot_z + 180);
            leftHand.rotation = Quaternion.Euler(0, 0, rot_z + 180);
            headPivot.rotation = Quaternion.Euler(0, 0, rot_z + 180);

            rightHand.localScale = Vector3.one;
            leftHand.localScale = Vector3.one;
            headPivot.localScale = Vector3.one;
        }
    }

    private void Flip(int value)
    {
        CmdFlip(value);
        skinPivot.localScale = new Vector3(value, 1, 1);
    }

    //[Command]
    private void CmdFlip(int value)
    {
        isFlipped = value > 0 ? false : true;
    }
}