using Feature.Audio;
using Feature.Flip;
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace Logic.Player.ThrowablesSystem
{
    public class ThrowablesManager : NetworkBehaviour
    {
        [SerializeField] private Throwable[] throwables;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform handPivot;
        [SerializeField] private AnimationCurve throwCurve;
        [SerializeField] private float power;
        [SerializeField] private AudioClip selectThrowableSFX;

        public Throwable ActiveThrowable { get; private set; }
        public float Power { get => power; }
        public Transform SpawnPoint { get => spawnPoint; }
        public Transform HandPivot { get => handPivot; }
        public AnimationCurve ThrowCurve { get => throwCurve; }
        public Vector3 ThrowDirection { get => SpawnPoint.right * (IsFlipped() ? -1 : 1); }

        public event Action<int> OnUpdateThrowableCount;
        public event Action<Throwable> OnChangeThrowable;

        private Player player;
        private bool isInitiated;
        private FlipController flipController;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (isInitiated)
                return;

            player = GetComponent<Player>();

            foreach (var item in throwables)
                item.Init(player, this);

            isInitiated = true;
        }

        private void Update()
        {
            if (player.Health.IsDead)
                return;
            if (!isLocalPlayer) return;
            if (PlayerInput.Instance.IsThrowing)
                ThrowObject();

            if (PlayerInput.Instance.IsSwitchingThrowables)
                SwitchThrowables();
        }
        [Command]
        private void ThrowObject()
        {
            ActiveThrowable?.ThrowObject(isServer, false);
            ClientsThrowObject();
        }
        [ClientRpc]
        private void ClientsThrowObject()
        {
            ActiveThrowable?.ThrowObject(false, isLocalPlayer);
        }
        public void OnStartPlayer()
        {
            Init();

            var startThrowable = GetNextOwnedThrowable();

            if (startThrowable == null)
            {
                Debug.Log("No throwable to select");
                return;
            }

            SelectThrowable(startThrowable);
        }

        public void SelectNoWeapon()
        {
            if (ActiveThrowable != null)
                ActiveThrowable.SetAsDeactive();

            ActiveThrowable = null;

            UpdateUI();
        }

        private void SelectThrowable(Throwable weapon)
        {
            if (weapon.CurrentCount == 0)
            {
                SelectNoWeapon();
                return;
            }

            if (ActiveThrowable != null)
                ActiveThrowable.SetAsDeactive();

            ActiveThrowable = weapon;
            ActiveThrowable.SetAsActive();

            AudioManager.Instance.Play2DSFX(selectThrowableSFX, transform.position);
            UpdateUI();
        }

        public Throwable GetNextOwnedThrowable()
        {
            for (int i = Convert.ToInt32(Mathf.Clamp(throwables.ToList().IndexOf(ActiveThrowable), 0, Mathf.Infinity)); i < throwables.Length; i++)
            {
                if (!throwables[i].IsActive && throwables[i].IsOwned)
                    return throwables[i];
            }

            return null;
        }

        public void SwitchThrowables()
        {
            var nextOwnedOne = GetNextOwnedThrowable();

            if (nextOwnedOne == null)
            {
                Debug.Log("No throwable to select");

                if (ActiveThrowable)
                    if (ActiveThrowable.CurrentCount == 0)
                        SelectNoWeapon();

                return;
            }

            SelectThrowable(nextOwnedOne);
        }

        public void UpdateUI()
        {
            UpdateThrowableUI(ActiveThrowable);

            if (ActiveThrowable)
                UpdateCountUI(ActiveThrowable.CurrentCount);
        }

        public void UpdateCountUI(int current)
        {
            OnUpdateThrowableCount?.Invoke(current);
        }

        public void UpdateThrowableUI(Throwable throwable)
        {
            OnChangeThrowable?.Invoke(throwable);
        }

        public void NotifyThrowableNearby(ThrowablePickup throwable)
        {
            if (throwable == null)
                return;

            var newWeapon = throwables.SingleOrDefault(x => x.ID == throwable.ID);

            if (newWeapon == null)
                return;

            PickupThrowable(throwable);
        }

        public void PickupThrowable(ThrowablePickup throwable)
        {
            var existThrowable = throwables.SingleOrDefault(x => x.ID == throwable.ID && x.IsOwned);

            if (existThrowable)
            {
                existThrowable.IncreaseCount(1);
            }
            else
            {
                var newThrowable = throwables.SingleOrDefault(x => x.ID == throwable.ID);
                newThrowable.Own();
                newThrowable.SetCount();
                SelectThrowable(newThrowable);
            }

            throwable.Pickup();
        }

        public void InjectFlipController(FlipController controller)
        {
            flipController = controller;
        }

        private bool IsFlipped()
        {
            return flipController && flipController.IsFlipped;
        }
    }
}