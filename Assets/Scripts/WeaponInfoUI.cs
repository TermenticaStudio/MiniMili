using Logic.Player;
using System.Collections;
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
    [SerializeField] private GameObject reloadBtn;
    [SerializeField] private GameObject reloadIndicator;

    [SerializeField] private TextMeshProUGUI respawnCountText;

    [SerializeField] private GameObject meleeBtn;

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
        reloadIndicator.SetActive(false);
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
        weaponsManager.OnReloadWeapon -= OnReloadWeapon;
    }

    private void OnSpawnPlayer(PlayerInfo obj)
    {
        weaponsManager = obj.GetComponent<PlayerWeaponsManager>();

        weaponsManager.OnChangeClipCount += OnChangeClipsCount;
        weaponsManager.OnChangeAmmoCount += OnChangeAmmoCount;
        weaponsManager.OnChangeWeapon += OnChangeWeapon;
        weaponsManager.OnWeaponNearby += OnWeaponNearby;
        weaponsManager.OnReloadWeapon += OnReloadWeapon;

        weaponsManager.UpdateUI();

        SetRespawnsLeftText(obj.RespawnsLeft);
    }

    private void OnWeaponNearby(PlayerWeapon current, PlayerWeapon newWeapon)
    {
        if (current == null && newWeapon == null)
        {
            replaceWeapon.SetActive(false);
            return;
        }

        replaceWeapon.SetActive(true);

        if (current == null)
        {
            currentWeaponImage.gameObject.SetActive(false);
        }
        else
        {
            currentWeaponImage.gameObject.SetActive(true);
            currentWeaponImage.sprite = current.Preset.Icon;
        }

        newWeaponImage.sprite = newWeapon.Preset.Icon;
    }

    private void OnChangeWeapon(PlayerWeapon weapon)
    {
        weaponNameText.text = weapon.Preset.Name;
        weaponIconImage.sprite = weapon.Preset.Icon;

        meleeBtn.SetActive(weapon.CanMelee());

        clipCountText.gameObject.SetActive(weapon.Preset.isFirearm);
        ammoCountText.gameObject.SetActive(weapon.Preset.isFirearm);
    }

    private void OnChangeClipsCount(int count)
    {
        clipCountText.text = count.ToString("D3");
    }

    private void OnChangeAmmoCount(int count, int totalAmmo)
    {
        ammoCountText.text = count.ToString("D3");

        if (count == totalAmmo)
        {
            SetReloadBtnActivation(false);
        }
        else
        {
            SetReloadBtnActivation(true);
        }
    }

    private void OnReloadWeapon(float obj)
    {
        StartCoroutine(ReloadIndicatorCoroutine(obj));
    }

    private IEnumerator ReloadIndicatorCoroutine(float time)
    {
        reloadIndicator.SetActive(true);

        yield return new WaitForSeconds(time);

        reloadIndicator.SetActive(false);
    }

    public void SetJetpackFuel(float amount)
    {
        jetpackFuelSlider.value = amount;
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }

    public void SetReloadBtnActivation(bool activate)
    {
        reloadBtn.SetActive(activate);
    }

    public void SetZoomText(int zoom)
    {
        zoomText.text = $"{zoom}X";
    }

    public void SetRespawnsLeftText(int count)
    {
        respawnCountText.text = $"x{count}";
    }
}