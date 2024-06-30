using Logic.Player;
using System;
using System.Linq;
using UnityEngine;

public class PlayerWeaponsManager : MonoBehaviour
{
    [SerializeField] private PlayerWeapon[] weapons;
    private PlayerWeapon activeWeapon;

    //[SyncVar(hook = nameof(UpdateActiveWeapon))]
    private int activeWeaponIndex;
    private int lastActiveWeapon;

    public event Action<int> OnChangeClipCount;
    public event Action<int> OnChangeAmmoCount;
    public event Action<PlayerWeapon> OnChangeWeapon;
    public event Action<PlayerWeapon, PlayerWeapon> OnWeaponNearby;

    private Player player;

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
        //if (!isLocalPlayer)
        //  return;

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
    }

    private void UpdateActiveWeapon(int oldIndex, int newIndex)
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        lastActiveWeapon = newIndex;
        activeWeapon = weapons[newIndex];
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.ResetZoom();
        activeWeapon.SetAsActive();

        UpdateUI();
    }

    public void SelectNoWeapon()
    {
        if (activeWeapon != null)
            DeactivateWeapon(activeWeapon);

        activeWeapon = null;
    }

    public void SelectLastWeapon()
    {
        UpdateActiveWeapon(0, lastActiveWeapon);
    }

    //[Command]
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
        weapon.gameObject.SetActive(false);
        weapon.SetAsDeactive();
    }

    public void UpdateClipCountUI(int value)
    {
        OnChangeClipCount?.Invoke(value);
    }

    public void UpdateAmmoCountUI(int value)
    {
        OnChangeAmmoCount?.Invoke(value);
    }

    public void UpdateWeaponUI(PlayerWeapon weapon)
    {
        OnChangeWeapon?.Invoke(weapon);
    }

    public void UpdateUI()
    {
        UpdateWeaponUI(activeWeapon);
        UpdateClipCountUI(activeWeapon.CurrrentClipsCount);
        UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount);
    }

    public void NotifyWeaponNearby(PickupWeapon weapon)
    {
        var newWeapon = weapons.SingleOrDefault(x => x.ID == weapon.ID);

        if (newWeapon == null)
            return;

        OnWeaponNearby?.Invoke(activeWeapon, newWeapon);
    }
}