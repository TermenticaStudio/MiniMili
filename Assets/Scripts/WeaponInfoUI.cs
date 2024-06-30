using Logic.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoUI : MonoBehaviour
{
    public static WeaponInfoUI Instance;

    [SerializeField] private TextMeshProUGUI clipCountText;
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Image weaponIconImage;

    [SerializeField] private Slider jetpackFuelSlider;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private TextMeshProUGUI zoomText;

    [Header("Replace Weapon")]
    [SerializeField] private GameObject replaceWeapon;
    [SerializeField] private Image currentWeaponImage;
    [SerializeField] private Image newWeaponImage;
    [SerializeField] private Button replaceWeaponBtn;

    private PlayerWeaponsManager weaponsManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnDisable()
    {
        if (PlayerSpawnHandler.Instance == null)
            return;

        PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;

        if (weaponsManager == null)
            return;

        weaponsManager.OnChangeClipCount -= OnChangeClipsCount;
        weaponsManager.OnChangeAmmoCount -= OnChangeAmmoCount;
        weaponsManager.OnChangeWeapon -= OnChangeWeapon;
        weaponsManager.OnWeaponNearby -= OnWeaponNearby;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    //}

    private void OnSpawnPlayer(PlayerInfo obj)
    {
        weaponsManager = obj.GetComponent<PlayerWeaponsManager>();

        weaponsManager.OnChangeClipCount += OnChangeClipsCount;
        weaponsManager.OnChangeAmmoCount += OnChangeAmmoCount;
        weaponsManager.OnChangeWeapon += OnChangeWeapon;
        weaponsManager.OnWeaponNearby += OnWeaponNearby;

        weaponsManager.UpdateUI();
    }

    private void OnWeaponNearby(PlayerWeapon current, PlayerWeapon newWeapon)
    {
        if (current == null || newWeapon == null)
        {
            replaceWeapon.SetActive(false);
            return;
        }

        replaceWeapon.SetActive(true);
        currentWeaponImage.sprite = current.Icon;
        newWeaponImage.sprite = newWeapon.Icon;
    }

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();

    //    PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;

    //    if (weaponsManager == null)
    //        return;

    //    weaponsManager.OnChangeClipCount -= OnChangeClipsCount;
    //    weaponsManager.OnChangeAmmoCount -= OnChangeAmmoCount;
    //    weaponsManager.OnChangeWeapon -= OnChangeWeapon;
    //}

    private void OnChangeWeapon(PlayerWeapon weapon)
    {
        weaponNameText.text = weapon.Name;
        weaponIconImage.sprite = weapon.Icon;
    }

    private void OnChangeClipsCount(int count)
    {
        clipCountText.text = count.ToString();
    }

    private void OnChangeAmmoCount(int count)
    {
        ammoCountText.text = count.ToString();
    }

    public void SetJetpackFuel(float amount)
    {
        jetpackFuelSlider.value = amount;
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }

    public void SetZoomText(int zoom)
    {
        zoomText.text = $"X{zoom}";
    }
}