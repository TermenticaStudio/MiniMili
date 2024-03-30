using Mirror;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] private Transform skinPivot;
    [SerializeField] private Transform weaponPivot;

    [Header("Head")]
    [SerializeField] private Transform headPivot;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private Vector2 minMaxRotationLimit;

    private Vector2 lastAimDirection;

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        Aim();
    }

    private void Aim()
    {
        if (PlayerInput.Instance.GetAim().magnitude != 0)
            lastAimDirection = PlayerInput.Instance.GetAim();

        var rot_z = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, rot_z);

        var headRot = Quaternion.Euler(0, 0, Mathf.Clamp(rot_z, minMaxRotationLimit.x, minMaxRotationLimit.y));
        headPivot.rotation = Quaternion.Lerp(headPivot.rotation, headRot, Time.deltaTime * lerpSpeed);

        var dot = Vector2.Dot(weaponPivot.right, Vector2.right);

        if (dot < 0)
            Flip(-1);
        else
            Flip(1);
    }

    private void Flip(int value)
    {
        skinPivot.localScale = new Vector3(value, 1, 1);
        weaponPivot.localScale = new Vector3(1, value, 1);
    }
}