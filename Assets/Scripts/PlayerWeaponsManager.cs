using Logic.Player;
using System;
using System.Linq;
using UnityEngine;

public class PlayerWeaponsManager : MonoBehaviour
{
    [SerializeField] private PlayerWeapon[] weapons;
    [SerializeField] private int maxOwnedWeapons = 2;
    [SerializeField] private AudioClip selectWeaponSFX;

    private PlayerWeapon activeWeapon;

    private int activeWeaponIndex;
    private int lastActiveWeapon;

    public event Action<int> OnChangeClipCount;
    public event Action<int, int> OnChangeAmmoCount; // <CurrentAmmo, TotalAmmoPerClip>
    public event Action<PlayerWeapon> OnChangeWeapon;
    public event Action<PlayerWeapon, PlayerWeapon> OnWeaponNearby;
    public event Action<float> OnReloadWeapon; // <ReloadTime>

    private Player player;
    private PickupWeapon availableWeaponToReplace;

    private void Start()
    {
        player = GetComponent<Player>();

        foreach (var weapon in weapons)
        {
            weapon.Init();
            DeactivateWeapon(weapon);
        }
    }

    public void OnStartPlayer()
    {
        activeWeaponIndex = -1;
        activeWeaponIndex = GetNextOwnedWeaponIndex();

        if (activeWeaponIndex == -1)
        {
            Debug.Log("No weapon to select");
            return;
        }

        UpdateActiveWeapon(0, activeWeaponIndex);
    }

    private void Update()
    {
        if (player.Health.IsDead)
            return;

        if (PlayerInput.Instance.IsSwitching)
            CmdSwitchWeapon();

        if (activeWeapon == null)
            return;

        if (PlayerInput.Instance.IsShooting)
            activeWeapon.CmdFire();
        else
            activeWeapon.ResetFire();

        if (PlayerInput.Instance.IsReloading)
            activeWeapon.CmdReload();

        if (PlayerInput.Instance.IsChangingZoom)
            activeWeapon.ChangeZoom();

        if (PlayerInput.Instance.IsReplacing)
            PickupWeapon(availableWeaponToReplace);
    }

    private void UpdateActiveWeapon(int oldIndex, int newIndex)
    {
        SelectWeapon(weapons[newIndex]);
    }

    public void SelectNoWeapon()
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        activeWeapon = null;
    }

    private void SelectWeapon(PlayerWeapon weapon)
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        activeWeapon = weapon;
        activeWeapon.SelectLastZoom();
        activeWeapon.SetAsActive();
        activeWeapon.Recoil(100);

        AudioManager.Instance.Play2DSFX(selectWeaponSFX, transform.position, player.MainCamera.transform.position);
        UpdateUI();
    }

    public void SelectLastWeapon()
    {
        UpdateActiveWeapon(0, lastActiveWeapon);
    }

    public void CmdSwitchWeapon()
    {
        if (activeWeaponIndex == weapons.Length - 1)
        {
            activeWeaponIndex = 0;
            activeWeaponIndex = GetNextOwnedWeaponIndex();
        }
        else
        {
            activeWeaponIndex = GetNextOwnedWeaponIndex();
        }

        if (activeWeaponIndex == -1)
        {
            Debug.Log("No weapon to select");
            return;
        }

        UpdateActiveWeapon(0, activeWeaponIndex);
    }

    public int GetNextOwnedWeaponIndex()
    {
        for (int i = Convert.ToInt32(Mathf.Clamp(activeWeaponIndex, 0, Mathf.Infinity)); i < weapons.Length; i++)
        {
            if (!weapons[i].IsActive && weapons[i].IsOwned)
                return i;
        }

        return -1;
    }

    public void DeactivateWeapon(PlayerWeapon weapon)
    {
        weapon.SetAsDeactive();
    }

    public void UpdateClipCountUI(int value)
    {
        OnChangeClipCount?.Invoke(value);
    }

    public void UpdateAmmoCountUI(int current, int total)
    {
        OnChangeAmmoCount?.Invoke(current, total);
    }

    public void UpdateWeaponUI(PlayerWeapon weapon)
    {
        OnChangeWeapon?.Invoke(weapon);
    }

    public void StartReload(float time)
    {
        OnReloadWeapon?.Invoke(time);
    }

    public void UpdateUI()
    {
        UpdateWeaponUI(activeWeapon);
        UpdateClipCountUI(activeWeapon.CurrrentClipsCount);
        UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount, activeWeapon.ClipSize);
    }

    // REFACTOR: it shouldnt called everyframe!
    public void NotifyWeaponNearby(PickupWeapon weapon)
    {
        if (weapon == null)
        {
            availableWeaponToReplace = null;
            OnWeaponNearby?.Invoke(null, null);
            return;
        }

        var newWeapon = weapons.SingleOrDefault(x => x.ID == weapon.ID);

        if (newWeapon == null)
            return;

        if (newWeapon.ID == weapon.ID && newWeapon.IsOwned)
        {
            PickupWeapon(weapon);
            return;
        }

        availableWeaponToReplace = weapon;

        if (!CanOwnMore())
        {
            OnWeaponNearby?.Invoke(activeWeapon, newWeapon);
        }
        else
        {
            OnWeaponNearby?.Invoke(null, newWeapon);
        }
    }

    public void PickupWeapon(PickupWeapon weapon)
    {
        var existWeapon = weapons.SingleOrDefault(x => x.ID == weapon.ID && x.IsOwned);

        if (existWeapon)
        {
            existWeapon.IncreaseClips(weapon.GetAmmo().ClipLeft);
        }
        else
        {
            if (!CanOwnMore())
                activeWeapon.Drop();

            var newWeapon = weapons.SingleOrDefault(x => x.ID == weapon.ID);
            newWeapon.OwnWeapon();
            newWeapon.SetAmmo(weapon.GetAmmo());
            SelectWeapon(newWeapon);
        }

        weapon.Pickup();
    }

    private bool CanOwnMore()
    {
        return weapons.Count(x => x.IsOwned) < maxOwnedWeapons;
    }
}