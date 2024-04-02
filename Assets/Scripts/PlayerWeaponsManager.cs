using Mirror;
using System;
using UnityEngine;

public class PlayerWeaponsManager : NetworkBehaviour
{
    [SerializeField] private PlayerWeapon[] weapons;
    private PlayerWeapon activeWeapon;

    [SyncVar(hook = nameof(UpdateActiveWeapon))]
    private int activeWeaponIndex;

    public event Action<int> OnChangeClipCount;
    public event Action<int> OnChangeAmmoCount;
    public event Action<PlayerWeapon> OnChangeWeapon;

    private void Start()
    {
        foreach (var weapon in weapons)
            DeactivateWeapon(weapon);

        activeWeaponIndex = 0;
        UpdateActiveWeapon(0, activeWeaponIndex);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (activeWeapon == null)
            return;

        if (PlayerInput.Instance.IsShooting)
            activeWeapon.CmdFire();
        else
            activeWeapon.ResetFire();

        if (PlayerInput.Instance.IsReloading)
            activeWeapon.CmdReload();

        if (PlayerInput.Instance.IsSwitching)
            CmdSwitchWeapon();

        if(PlayerInput.Instance.IsChangingZoom)
            activeWeapon.ChangeZoom();
    }

    private void UpdateActiveWeapon(int oldIndex, int newIndex)
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        activeWeapon = weapons[newIndex];
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.ResetZoom();
        OnChangeWeapon?.Invoke(activeWeapon);

        UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount);
        UpdateClipCountUI(activeWeapon.CurrrentClipsCount);
    }

    [Command]
    public void CmdSwitchWeapon()
    {
        if (activeWeaponIndex == weapons.Length - 1)
            activeWeaponIndex = 0;
        else
            activeWeaponIndex++;

        UpdateActiveWeapon(0, activeWeaponIndex);
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