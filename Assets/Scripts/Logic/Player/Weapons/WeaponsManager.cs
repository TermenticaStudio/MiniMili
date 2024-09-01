using Feature.Audio;
using System;
using System.Linq;
using UnityEngine;
using Mirror;

namespace Logic.Player.WeaponsSystem
{
    public class WeaponsManager : NetworkBehaviour
    {
        [SerializeField] private Weapon[] weapons;
        [SerializeField] private int maxOwnedWeapons = 2;
        [SerializeField] private AudioClip selectWeaponSFX;

        [SyncVar(hook = nameof(OnWeaponChange))] private int activeWeaponIndex = -1;



        private Weapon activeWeapon;
        public event Action<int> OnChangeClipCount;
        public event Action<int, int> OnChangeAmmoCount; // <CurrentAmmo, TotalAmmoPerClip>
        public event Action<Weapon> OnChangeWeapon;
        public event Action<Weapon, Weapon> OnWeaponNearby;
        public event Action<float> OnReloadWeapon; // <ReloadTime>

        private Player player;
        private PickupWeapon availableWeaponToReplace;
        private bool isInited;

        private void Start()
        {
            /* if (isLocalPlayer)
             {
                 Init();
                 OnStartPlayer();
             }*/
            Init();
            if (activeWeaponIndex != -1)
            {
                SelectWeapon(weapons[activeWeaponIndex].ID);
            }
        }

        private void Init()
        {
            if (isInited)
                return;
            Debug.Log("initing");
            player = GetComponent<Player>();

            foreach (var weapon in weapons)
                weapon.Init();

            isInited = true;
            Debug.Log("Done initing");
        }

        public void OnStartPlayer()
        {
            Debug.Log("starting player");
            Init();
            CmdUpdateActiveWeaponServer();
            if (isServerOnly)
            {
                Debug.Log("this is player in server");
            }
            //    UpdateActiveWeapon(0, activeWeaponIndex);
        }
        [Command]
        private void CmdUpdateActiveWeaponServer()
        {
            activeWeaponIndex = GetNextOwnedWeaponIndex();
            if (activeWeaponIndex == -1)
            {
                Debug.Log("No weapon to select");
                return;
            }

        }
        /*    [ClientRpc]
            private void UpdateActiveWeapon(int newIndex)
            {
                SelectWeapon(weapons[newIndex]);
            }*/
        void OnWeaponChange(int oldIndex, int newIndex)
        {
            Debug.Log("synced weapon is this " + weapons[newIndex]);
                SelectWeapon(weapons[newIndex].ID);   
        }
        private void Update()
        {
            if (!isLocalPlayer || player.Health.IsDead)
                return;

            if (PlayerInput.Instance.IsSwitching)
                CmdSwitchWeapon();

            if (PlayerInput.Instance.IsReplacing)
                CmdPickupWeapon(availableWeaponToReplace.GetNetworkIdentity());

            if (PlayerInput.Instance.IsShooting)
                activeWeapon?.Fire();
            else
                activeWeapon?.CancelFire();

            if (PlayerInput.Instance.IsReloading)
                activeWeapon?.Reload();

            if (PlayerInput.Instance.IsChangingZoom)
                activeWeapon?.ChangeZoom();

            if (PlayerInput.Instance.IsMeleeing)
                activeWeapon?.MeleeAttack();
        }

        [Command]
        private void CmdSwitchWeapon()
        {
            if (IsLastOwnedWeapon())
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
        }

        [ClientRpc]
        private void RpcUpdateActiveWeapon(int newIndex)
        {
            SelectWeapon(weapons[newIndex]);
        }

        
/*        private void SelectWeapon(string weaponID)
        {
            if (activeWeapon != null)
                DeactivateWeapon(activeWeapon);

            var existWeapon = weapons.SingleOrDefault(x => x.ID == weaponID && x.IsOwned);
            if (!existWeapon)
            {
                throw new Exception("no weapon found to activate");
            }
            activeWeapon = existWeapon;
            activeWeapon.SelectLastZoom();
            activeWeapon.SetAsActive();
            activeWeapon.Recoil(100);

            if (isLocalPlayer)
                AudioManager.Instance.Play2DSFX(selectWeaponSFX, transform.position);

            UpdateUI();
        }
*/        
        private void SelectWeapon(string weaponID)
        {
            if (activeWeapon != null)
                DeactivateWeapon(activeWeapon);

            var existWeapon = weapons.SingleOrDefault(x => x.ID == weaponID && x.IsOwned);
            Debug.Log("Selecting " + existWeapon.name);
            if (!existWeapon)
            {
                throw new Exception("no weapon found to activate");
            }
            activeWeapon = existWeapon;
            activeWeapon.SelectLastZoom();
            activeWeapon.SetAsActive();
            activeWeapon.Recoil(100);

            if (isLocalPlayer)
            {
                AudioManager.Instance.Play2DSFX(selectWeaponSFX, transform.position);
                UpdateUI();
            }
        }
        private void SelectWeapon(Weapon weapon)
        {
            if (activeWeapon != null)
                DeactivateWeapon(activeWeapon);

            activeWeapon = weapon;
            activeWeapon.SelectLastZoom();
            activeWeapon.SetAsActive();
            activeWeapon.Recoil(100);

            if (isLocalPlayer)
                AudioManager.Instance.Play2DSFX(selectWeaponSFX, transform.position);

            UpdateUI();
        }
        [Command]
        public void CmdPickupWeapon(NetworkIdentity pickupWeaponObject)
        {
            Debug.Log(pickupWeaponObject.gameObject);
            var pickupWeapon = pickupWeaponObject.gameObject.GetComponent<PickupWeapon>();
            if (pickupWeapon == null) return;

            var existWeapon = weapons.SingleOrDefault(x => x.ID == pickupWeapon.ID && x.IsOwned);

            if (existWeapon != null)
            {
                existWeapon.IncreaseClips(pickupWeapon.GetAmmo().ClipLeft);
            }
            else
            {
                if (!CanOwnMore() && activeWeapon != null)
                    activeWeapon.Drop();

                var newWeapon = weapons.SingleOrDefault(x => x.ID == pickupWeapon.ID);
                if (newWeapon != null)
                {
                    newWeapon.OwnWeapon();
                    newWeapon.SetAmmo(pickupWeapon.GetAmmo());
                    RpcSelectWeapon(newWeapon.ID);
                }
            }

            pickupWeapon.Pickup();
        }

        [ClientRpc]
        private void RpcSelectWeapon(NetworkIdentity weaponObject)
        {
            SelectWeapon(weaponObject.GetComponent<Weapon>());
        }
        [ClientRpc]
        private void RpcSelectWeapon(string weaponID)
        {
            SelectWeapon(weaponID);
        }
        private int GetNextOwnedWeaponIndex()
        {
            Debug.Log("Getting Next Owned Weapon Index ");
            for (int i = Convert.ToInt32(Mathf.Clamp(activeWeaponIndex, 0, Mathf.Infinity)); i < weapons.Length; i++)
            {
                if (!weapons[i].IsActive && weapons[i].IsOwned)
                    Debug.Log("found " + weapons[i].name);
                return i;
            }
            Debug.Log("didnt find any weapon");

            return -1;
        }

        private bool IsLastOwnedWeapon()
        {
            return weapons.LastOrDefault(x => x.IsOwned) == activeWeapon;
        }

        public void DeactivateWeapon(Weapon weapon)
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

        public void UpdateWeaponUI(Weapon weapon)
        {
            if (weapon == null) return;
            OnChangeWeapon?.Invoke(weapon);
        }

        public void RefreshWeapon()
        {
            UpdateWeaponUI(activeWeapon);
        }

        public void StartReload(float time)
        {
            OnReloadWeapon?.Invoke(time);
        }

        public void UpdateUI()
        {
            if(activeWeapon == null) return;
            UpdateWeaponUI(activeWeapon);
            UpdateClipCountUI(activeWeapon.GetTotalLeftAmmo());
            UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount, activeWeapon.ClipSize);
        }

        public void NotifyWeaponNearby(NetworkIdentity weaponIdentity)
        {
            if (weaponIdentity == null)
            {
                availableWeaponToReplace = null;
                OnWeaponNearby?.Invoke(null, null);
                return;
            }
            var weapon = weaponIdentity.gameObject.GetComponent<PickupWeapon>();
            var newWeapon = weapons.SingleOrDefault(x => x.ID == weapon.ID);

            if (newWeapon == null)
                return;

            if (newWeapon.ID == weapon.ID && newWeapon.IsOwned)
            {
                if (newWeapon.Preset.isFirearm)
                    CmdPickupWeapon(weapon.GetNetworkIdentity());
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

        private bool CanOwnMore()
        {
            return weapons.Count(x => x.IsOwned) < maxOwnedWeapons;
        }

        public void RefillWeapon()
        {
            if (activeWeapon == null)
                return;

            activeWeapon.SetAmmo(new PickupWeapon.Ammo(activeWeapon.Preset.clipsCount, activeWeapon.Preset.clipSize));
            UpdateUI();
        }
    }
}
