using Logic.Player;
using Logic.Player.ThrowablesSystem;
using Logic.Player.WeaponsSystem;
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

    [Header("Throwable")]
    [SerializeField] private Button switchThrowableBtn;
    [SerializeField] private Image currentThrowableImage;
    [SerializeField] private Button throwBtn;
    [SerializeField] private TextMeshProUGUI throwablesCountTxt;

    private Player player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameEvents.OnLocalPlayerSpawn += OnSpawnPlayer;

        OnWeaponNearby(null, null);
        reloadIndicator.SetActive(false);
    }

    private void OnDisable()
    {
        if (GameEvents.Instance == null)
            return;

        GameEvents.OnLocalPlayerSpawn -= OnSpawnPlayer;

        UnsubscribeThrowablesEvents();
        UnsubscribeWeaponsEvents();
    }

    private void OnSpawnPlayer()
    {
        player = FindObjectOfType<SceneObjectsContainer>().localPlayer;

        SubscribeThrowablesEvents();
        SubscribeWeaponsEvents();

        SetRespawnsLeftText(player.GetComponent<PlayerInfo>().RespawnsLeft);
    }

    private void SubscribeWeaponsEvents()
    {
        if (player == null)
            return;

        player.WeaponsManager.OnChangeClipCount += OnChangeClipsCount;
        player.WeaponsManager.OnChangeAmmoCount += OnChangeAmmoCount;
        player.WeaponsManager.OnChangeWeapon += OnChangeWeapon;
        player.WeaponsManager.OnWeaponNearby += OnWeaponNearby;
        player.WeaponsManager.OnReloadWeapon += OnReloadWeapon;
        player.WeaponsManager.UpdateUI();
    }

    private void UnsubscribeWeaponsEvents()
    {
        if (player == null)
            return;

        player.WeaponsManager.OnChangeClipCount -= OnChangeClipsCount;
        player.WeaponsManager.OnChangeAmmoCount -= OnChangeAmmoCount;
        player.WeaponsManager.OnChangeWeapon -= OnChangeWeapon;
        player.WeaponsManager.OnWeaponNearby -= OnWeaponNearby;
        player.WeaponsManager.OnReloadWeapon -= OnReloadWeapon;
    }

    private void SubscribeThrowablesEvents()
    {
        if (player == null)
            return;

        player.Throwables.OnChangeThrowable += OnChangeThrowables;
        player.Throwables.OnUpdateThrowableCount += OnUpdateThrowableCount;
        player.Throwables.UpdateUI();
    }

    private void UnsubscribeThrowablesEvents()
    {
        if (player == null)
            return;

        player.Throwables.OnChangeThrowable -= OnChangeThrowables;
        player.Throwables.OnUpdateThrowableCount -= OnUpdateThrowableCount;
    }

    private void OnUpdateThrowableCount(int count)
    {
        throwablesCountTxt.text = count.ToString();
    }

    private void OnChangeThrowables(Throwable throwable)
    {
        var isEquiped = throwable != null;

        currentThrowableImage.gameObject.SetActive(isEquiped);
        throwBtn.gameObject.SetActive(isEquiped);

        if (isEquiped)
            currentThrowableImage.sprite = throwable.Preset.Icon;
    }

    private void OnWeaponNearby(Weapon current, Weapon newWeapon)
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

    private void OnChangeWeapon(Weapon weapon)
    {
        weaponNameText.text = weapon.Preset.Name;
        weaponIconImage.sprite = weapon.Preset.Icon;

        meleeBtn.SetActive(weapon.HaveMelee());

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