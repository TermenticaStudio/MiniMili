using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private FixedJoystick aimJoystick;
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button swichButton;
    [SerializeField] private Button zoomButton;
    [SerializeField] private Button replaceWeaponButton;
    [SerializeField] private Button meleeButton;
    [SerializeField] private Button switchThrowablesButton;
    [SerializeField] private Button throwButton;

    public Vector2 MovementJoystickDirection { get => movementJoystick.Direction; }

    public Vector2 AimJoystickDirection { get => aimJoystick.Direction; }

    public bool IsShooting { get => aimJoystick.Direction.magnitude >= 0.9f && aimJoystick.IsPointerDown; }

    public bool IsReloading { get; private set; }

    public bool IsSwitching { get; private set; }

    public bool IsChangingZoom { get; private set; }

    public bool IsReplacing { get; private set; }

    public bool IsMeleeing { get; private set; }

    public bool IsSwitchingThrowables { get; private set; }

    public bool IsThrowing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        reloadButton.onClick.AddListener(ReloadInput);
        swichButton.onClick.AddListener(SwitchInput);
        zoomButton.onClick.AddListener(ChangeZoomInput);
        replaceWeaponButton.onClick.AddListener(ReplaceInput); ;
        meleeButton.onClick.AddListener(MeleeInput);
        switchThrowablesButton.onClick.AddListener(SwitchThrowablesInput);
        throwButton.onClick.AddListener(ThrowInput);
    }

    public void ResetInput()
    {
        movementJoystick.SetInput(Vector2.zero);
        movementJoystick.SetHandlePosition(Vector2.zero);

        aimJoystick.SetInput(Vector2.zero);
        aimJoystick.SetHandlePosition(Vector2.zero);
    }

    public Vector2 GetMovement()
    {
        return MovementJoystickDirection;
    }

    public Vector2 GetAim()
    {
        return AimJoystickDirection;
    }

    private void ReloadInput()
    {
        if (IsReloading)
            return;

        StartCoroutine(ReloadInputCoroutine());
    }

    private IEnumerator ReloadInputCoroutine()
    {
        IsReloading = true;

        yield return new WaitForEndOfFrame();

        IsReloading = false;
    }

    private void SwitchInput()
    {
        if (IsSwitching)
            return;

        StartCoroutine(SwitchInputCoroutine());
    }

    private IEnumerator SwitchInputCoroutine()
    {
        IsSwitching = true;

        yield return new WaitForEndOfFrame();

        IsSwitching = false;
    }

    private void ReplaceInput()
    {
        if (IsReplacing)
            return;

        StartCoroutine(ReplaceInputCoroutine());
    }

    private IEnumerator ReplaceInputCoroutine()
    {
        IsReplacing = true;

        yield return new WaitForEndOfFrame();

        IsReplacing = false;
    }

    private void ChangeZoomInput()
    {
        if (IsChangingZoom)
            return;

        StartCoroutine(ChangeZoomCoroutine());
    }

    private IEnumerator ChangeZoomCoroutine()
    {
        IsChangingZoom = true;

        yield return new WaitForEndOfFrame();

        IsChangingZoom = false;
    }

    private void MeleeInput()
    {
        if (IsMeleeing)
            return;

        StartCoroutine(MeleeCoroutine());
    }

    private IEnumerator MeleeCoroutine()
    {
        IsMeleeing = true;

        yield return new WaitForEndOfFrame();

        IsMeleeing = false;
    }

    private void SwitchThrowablesInput()
    {
        if (IsSwitchingThrowables)
            return;

        StartCoroutine(SwitchThrowablesCoroutine());
    }

    private IEnumerator SwitchThrowablesCoroutine()
    {
        IsSwitchingThrowables = true;

        yield return new WaitForEndOfFrame();

        IsSwitchingThrowables = false;
    }

    private void ThrowInput()
    {
        if (IsThrowing)
            return;

        StartCoroutine(ThrowCoroutine());
    }

    private IEnumerator ThrowCoroutine()
    {
        IsThrowing = true;

        yield return new WaitForEndOfFrame();

        IsThrowing = false;
    }
}