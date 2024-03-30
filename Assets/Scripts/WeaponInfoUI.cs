using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoUI : NetworkBehaviour
{
    public static WeaponInfoUI Instance;

    [SerializeField] private TextMeshProUGUI clipCountText;
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Image weaponIconImage;

    [SerializeField] private Slider jetpackFuelSlider;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private TextMeshProUGUI zoomText;

    private PlayerWeaponsManager weaponsManager;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        weaponsManager = FindObjectOfType<PlayerWeaponsManager>();

        weaponsManager.OnChangeClipCount += OnChangeClipsCount;
        weaponsManager.OnChangeAmmoCount += OnChangeAmmoCount;
        weaponsManager.OnChangeWeapon += OnChangeWeapon;
    }

    private void OnChangeWeapon(PlayerWeapon weapon)
    {
        weaponNameText.text = weapon.Name;
        weaponIconImage.sprite = weapon.Icon;
    }

    private void OnDisable()
    {
        if (weaponsManager == null)
            return;

        weaponsManager.OnChangeClipCount -= OnChangeClipsCount;
        weaponsManager.OnChangeAmmoCount -= OnChangeAmmoCount;
        weaponsManager.OnChangeWeapon -= OnChangeWeapon;
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

    public void SetZoomText(int  zoom)
    {
        zoomText.text = $"X{zoom}";
    }
}