using Mirror;
using System;
using System.Linq;
using UnityEngine;

public class PlayerWeaponsManager : NetworkBehaviour
{
    [SerializeField] private PlayerWeapon[] weapons;
    private PlayerWeapon activeWeapon;

    public event Action<int> OnChangeClipCount;
    public event Action<int> OnChangeAmmoCount;
    public event Action<PlayerWeapon> OnChangeWeapon;

    private void Start()
    {
        foreach (var weapon in weapons)
            DeactivateWeapon(weapon);

        ActivateWeapon(0);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (activeWeapon == null)
            return;

        if (PlayerInput.Instance.IsShooting)
            activeWeapon.Fire();
        else
            activeWeapon.ResetFire();

        if (PlayerInput.Instance.IsReloading)
            activeWeapon.Reload();

        if (PlayerInput.Instance.IsSwitching)
            SwitchWeapon();

        if(PlayerInput.Instance.IsChangingZoom)
            activeWeapon.ChangeZoom();
    }

    public void ActivateWeapon(int index)
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        activeWeapon = weapons[index];
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.ResetZoom();
        OnChangeWeapon?.Invoke(activeWeapon);

        UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount);
        UpdateClipCountUI(activeWeapon.CurrrentClipsCount);
    }

    public void SwitchWeapon()
    {
        var currentWeaponIndex = weapons.ToList().FindIndex(x => x == activeWeapon);

        if (currentWeaponIndex == weapons.Length - 1)
            currentWeaponIndex = 0;
        else
            currentWeaponIndex++;

        ActivateWeapon(currentWeaponIndex);
    }

    public void DeactivateWeapon(PlayerWeapon weapon)
    {
        weapon.gameObject.SetActive(false);
    }

    public void UpdateClipCountUI(int value)
    {
        OnChangeClipCount?.Invoke(value);
    }

    public void UpdateAmmoCountUI(int value)
    {
        OnChangeAmmoCount?.Invoke(value);
    }
}