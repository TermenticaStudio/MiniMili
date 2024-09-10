using Feature.Audio;
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    public class WeaponsManager : NetworkBehaviour
    {
        [SerializeField] private Weapon[] weapons;
        [SerializeField] private int maxOwnedWeapons = 2;
        [SerializeField] private AudioClip selectWeaponSFX;

        private Weapon activeWeapon;

        [SyncVar]private int activeWeaponIndex = -1;
        private int lastActiveWeapon;
        /// <summary>
        /// Maximum amount of passed time a projectile may have.
        /// This ensures really laggy players won't be able to disrupt
        /// other players by having the projectile speed up beyond
        /// reason on their screens.
        /// </summary>
        [SerializeField] private const float MAX_PASSED_TIME = 0.3f;
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

            /*            if (isLocalPlayer)
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
                        else
                        {
                            SelectWeapon(weapons[activeWeaponIndex]);
                        }
            */
        }

        private void Init()
        {
            if (isInited)
                return;

            player = GetComponent<Player>();

            foreach (var weapon in weapons)
                weapon.Init(isLocalPlayer);

            isInited = true;
        }

        public void OnStartPlayer()
        {
            Init();
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
            if (!isLocalPlayer) return;
            if (PlayerInput.Instance.IsSwitching)
                CmdSwitchWeapon();

            if (PlayerInput.Instance.IsReplacing)
                PickupWeapon(availableWeaponToReplace);

            if (PlayerInput.Instance.IsShooting)
                ClientFire();
            else
                ClientCancelFire();

            if (PlayerInput.Instance.IsReloading)
                activeWeapon?.Reload();

            if (PlayerInput.Instance.IsChangingZoom)
                activeWeapon?.ChangeZoom();

            if (PlayerInput.Instance.IsMeleeing)
                activeWeapon?.MeleeAttack();
        }

        private void UpdateActiveWeapon(int oldIndex, int newIndex)
        {
            Debug.Log("updating active weapon");
            // SelectWeapon(weapons[newIndex]);
            if (isServerOnly)
            {
                SelectWeapon(weapons[newIndex]);
                return;
            }
            if (isLocalPlayer)
            {
                UpdateActiveWeaponForOthers(newIndex);
            }
            else
            {
                SelectWeapon(weapons[newIndex]);
            }
        }
        [Command(requiresAuthority = false)]
        private void UpdateActiveWeaponForOthers(int newIndex)
        {
            RpcUpdateActiveWeaponForOthers(newIndex);
        }
        [ClientRpc]
        private void RpcUpdateActiveWeaponForOthers(int newIndex)
        {
            Debug.Log("RpcUpdateActiveWeaponForOthers");
            SelectWeapon(weapons[newIndex]);
        }
        public void SelectNoWeapon()
        {
            if (activeWeapon != null)
                DeactivateWeapon(activeWeapon);

            activeWeapon = null;
        }

        private void SelectWeapon(Weapon weapon)
        {
            if (activeWeapon != null)
                DeactivateWeapon(activeWeapon);

            Debug.Log("SelectWeapon");
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
        private void ClientFire()
        {
            activeWeapon?.Fire(true, 0, isServer);
            if (isServer) {
                ObserversFire(0);
            }
            else {
                ServerFire((uint)NetworkTime.time);
            }

        }
        private void ClientCancelFire()
        {
            activeWeapon?.CancelFire();
            ServerCancelFire();
        }
        [Command]
        private void ServerFire(uint tick)
        {
            /* You may want to validate position and direction here.
             * How this is done depends largely upon your game so it
             * won't be covered in this guide. */

            //Get passed time. Note the false for allow negative values.
            float passedTime = (float)NetworkTime.time - tick;
            /* Cap passed time at half of constant value for the server.
             * In our example max passed time is 300ms, so server value
             * would be max 150ms. This means if it took a client longer
             * than 150ms to send the rpc to the server, the time would
             * be capped to 150ms. This might sound restrictive, but that would
             * mean the client would have roughly a 300ms ping; we do not want
             * to punish other players because a laggy client is firing. */
            passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

            //Spawn on the server.
            activeWeapon?.Fire(false, passedTime, true);
            //Tell other clients to spawn the projectile.
            ObserversFire(tick);
        }
        /// <summary>
        /// Fires on all clients but owner.
        /// </summary>
        [ClientRpc(includeOwner = false)]
        private void ObserversFire(uint tick)
        {
            //Like on server get the time passed and cap it. Note the false for allow negative values.
            float passedTime = (float)NetworkTime.time - tick;
            passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

            activeWeapon?.Fire(false, passedTime, false);
        }
        [Command]
        private void ServerCancelFire()
        {
            ObserversCancelFire();
        }
        [ClientRpc(includeOwner =false)]
        private void ObserversCancelFire()
        {
            activeWeapon?.CancelFire();
        }

        public void SelectLastWeapon()
        {
            UpdateActiveWeapon(0, lastActiveWeapon);
        }

        public void RefillWeapon()
        {
            if (activeWeapon == null)
                return;

            activeWeapon.SetAmmo(new PickupWeapon.Ammo(activeWeapon.Preset.clipsCount, activeWeapon.Preset.clipSize));
            UpdateUI();
        }
        [Command]
        public void CmdSwitchWeapon()
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

          //  UpdateActiveWeapon(0, activeWeaponIndex);
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
        public int FindWeaponIndex(string weaponID)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if(weaponID == weapons[i].ID) return i;
            }
            throw new Exception("No weapon found for this id");
        }
        [Command]
        private void UpdateActiveWeaponIndexOnServer(int newIndex)
        {
            activeWeaponIndex = newIndex;
        }
        private bool IsLastOwnedWeapon()
        {
            var lastWeapon = weapons.Last(x => x == activeWeapon);

            if (lastWeapon == null || activeWeapon == null)
                return false;

            return lastWeapon == activeWeapon;
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
            if (activeWeapon == null) return;
            UpdateWeaponUI(activeWeapon);
            UpdateClipCountUI(activeWeapon.GetTotalLeftAmmo());
            UpdateAmmoCountUI(activeWeapon.CurrentAmmoCount, activeWeapon.ClipSize);
        }

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
                if (newWeapon.Preset.isFirearm)
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
                UpdateActiveWeaponIndexOnServer(FindWeaponIndex(newWeapon.ID));
                SelectWeapon(newWeapon);
                CmdPickUpWeapon(weapon.netIdentity);
            }
            //   weapon.Pickup();
        }
        [Command]
        private void CmdPickUpWeapon(NetworkIdentity weaponPickupIdentity)
        {
            PickupWeapon weapon = weaponPickupIdentity.GetComponent<PickupWeapon>();
            RpcPickUpWeapon(weapon.ID, weapon.GetAmmo());
            weapon.Pickup();

        }
        [ClientRpc(includeOwner = false)]
        private void RpcPickUpWeapon(string weaponID, PickupWeapon.Ammo ammoRemaining)
        {
            if (!CanOwnMore())
                activeWeapon.Drop();
            var newWeapon = weapons.SingleOrDefault(x => x.ID == weaponID);
            newWeapon.OwnWeapon();
            newWeapon.SetAmmo(ammoRemaining);
            SelectWeapon(newWeapon);
        }
        private bool CanOwnMore()
        {
            return weapons.Count(x => x.IsOwned) < maxOwnedWeapons;
        }
    }
}