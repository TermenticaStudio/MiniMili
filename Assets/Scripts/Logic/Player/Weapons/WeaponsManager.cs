using Feature.Audio;
using System;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.Assertions;

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
            /*   if (isLocalPlayer)
               {
                   Init();
                   OnStartPlayer();
               }
               else
               {*/
 /*           if (!isInited)
            {
                Init();  // Ensure this runs on both client and server
            }*/
           /* if (activeWeaponIndex != -1)
                {
                    SelectWeapon(weapons[activeWeaponIndex].ID);
                }*/
            
            //  Init();
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
            if (!isInited)
            {
                Init();
            }
            activeWeaponIndex = GetNextOwnedWeaponIndex();
            if (activeWeaponIndex == -1)
            {
                Debug.Log("No weapon to select");
                return;
            }
            //    UpdateActiveWeapon(0, activeWeaponIndex);
        }
        [Command(requiresAuthority = false)]
        private void CmdUpdateActiveWeaponServer()
        {
            activeWeaponIndex = GetNextOwnedWeaponIndex();

            if (activeWeaponIndex == -1)
            {
                Debug.Log("No weapon to select");
                return;
            }
            if (isServerOnly)
            {
                SelectWeapon(weapons[activeWeaponIndex].ID);
            }
        }
        /*    [ClientRpc]
            private void UpdateActiveWeapon(int newIndex)
            {
                SelectWeapon(weapons[newIndex]);
            }*/
        void OnWeaponChange(int oldIndex, int newIndex)
        {
            Assert.IsNotNull(weapons[newIndex]);
            if (newIndex != oldIndex && weapons[newIndex] != null)
            {
                SelectWeapon(weapons[newIndex]);
            }else if (newIndex == oldIndex)
            {
                Debug.Log("this new weapon is same as old no need to change anything");
            }
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
                CmdShoot(Quaternion.Euler(activeWeapon.projectileSpawnPoint.eulerAngles));
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
            Debug.Log("switching weapon...");
            if (IsLastOwnedWeapon())
            {
                activeWeaponIndex = 0;
                activeWeaponIndex = GetNextOwnedWeaponIndex();
            }
            else
            {
                activeWeaponIndex = GetNextOwnedWeaponIndex();
            }
            if(isServer)
            {
                SelectWeapon(weapons[activeWeaponIndex].ID);
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
            if (existWeapon == null)
            {
                Debug.LogError("Weapon with ID " + weaponID + " not found or not owned!");
                return;
            }

            Debug.Log("Selecting " + existWeapon.name);
            if (!existWeapon)
            {
                Debug.Log("no weapon found");
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
            {
                AudioManager.Instance.Play2DSFX(selectWeaponSFX, transform.position);

                UpdateUI();
            }
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
        private void RpcSelectWeapon(string weaponID)
        {
            SelectWeapon(weaponID);
        }
        [Command]
        private void CmdShoot(Quaternion rot)
        {
            activeWeapon?.Fire(rot);
            if (isServerOnly)
            {
            }
            RpcShoot(rot);
        }
        [ClientRpc]
        private void RpcShoot(Quaternion rot)
        {
            activeWeapon?.Fire(rot);
        }
        [Command]
        private void CmdCancelShoot()
        {
            if (isServerOnly)
            {
                activeWeapon?.CancelFire();
            }
            RpcCancelShoot();
        }
        [ClientRpc]
        private void RpcCancelShoot()
        {
            activeWeapon?.CancelFire();
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
