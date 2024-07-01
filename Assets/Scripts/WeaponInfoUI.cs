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

        OnWeaponNearby(null, null);
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
        currentWeaponImage.sprite = current.Preset.Icon;
        newWeaponImage.sprite = newWeapon.Preset.Icon;
    }

    private void OnChangeWeapon(PlayerWeapon weapon)
    {
        weaponNameText.text = weapon.Preset.Name;
        weaponIconImage.sprite = weapon.Preset.Icon;
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