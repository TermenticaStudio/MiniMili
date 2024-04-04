using Mirror;
using System.Collections;
using System.Linq;
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
    [SerializeField] private Button quitButton;

    public Vector2 MovementJoystickDirection { get => movementJoystick.Direction; }

    public Vector2 AimJoystickDirection { get => aimJoystick.Direction; }

    public bool IsShooting { get => aimJoystick.Direction.magnitude >= 0.9f; }

    public bool IsReloading { get; private set; }

    public bool IsSwitching { get; private set; }

    public bool IsChangingZoom { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        reloadButton.onClick.AddListener(ReloadInput);
        swichButton.onClick.AddListener(SwitchInput);
        zoomButton.onClick.AddListener(ChangeZoomInput);

        quitButton.onClick.AddListener(() =>
        {
            var localPlayer = FindObjectsOfType<PlayerInfo>().SingleOrDefault(x=>x.isLocalPlayer);

            if (localPlayer.isServer)
                NetworkManager.singleton.StopHost();
            else
                NetworkManager.singleton.StopClient();
        });
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
}