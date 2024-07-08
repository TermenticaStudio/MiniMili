using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Logic.Player.ThrowablesSystem
{
    public class Throwable : MonoBehaviour
    {
        const string SETTINGS_GROUP = "Settings";
        const string HOLSTER_GROUP = "Holster";

        [FoldoutGroup(SETTINGS_GROUP)]
        [SerializeField] private ThrowablePreset preset;

        [FoldoutGroup(HOLSTER_GROUP)]
        [SerializeField] private Transform holsterPos;
        [FoldoutGroup(HOLSTER_GROUP)]
        [SerializeField] private string defaultLayer;
        [FoldoutGroup(HOLSTER_GROUP)]
        [SerializeField] private string holsterLayer;

        private Transform defaultParent;
        private Vector3 defaultPos;

        public ThrowablePreset Preset { get => preset; }
        public int CurrentCount { get; private set; }
        public bool IsOwned { get; private set; }
        public string ID { get => preset.Id; }
        public bool IsActive { get; private set; }

        private Player player;
        private ThrowablesManager playerThrowables;

        public void Init(Player player, ThrowablesManager playerThrowables)
        {
            this.player = player;
            this.playerThrowables = playerThrowables;

            defaultParent = transform.parent;
            defaultPos = transform.localPosition;

            player.Health.OnRevive += OnRevivePlayer;
            player.Health.OnDie += OnPlayerDie;

            OnRevivePlayer();
            SetAsDeactive();
        }

        private void OnDestroy()
        {
            if (player)
            {
                player.Health.OnRevive -= OnRevivePlayer;
                player.Health.OnDie -= OnPlayerDie;
            }
        }

        public void ThrowObject()
        {
            if (CurrentCount <= 0)
                return;

            CurrentCount--;
            playerThrowables.UpdateCountUI(CurrentCount);

            var instance = Instantiate(preset.ThrowObject, playerThrowables.SpawnPoint.position, Quaternion.identity, null);
            instance.Throw(playerThrowables.ThrowDirection, playerThrowables.Power);

            DOVirtual.Float(0, 1, 1, value =>
            {
                playerThrowables.HandPivot.DOLocalRotate(Vector3.forward * playerThrowables.ThrowCurve.Evaluate(value), 0);
            });

            if (CurrentCount == 0)
                playerThrowables.SwitchThrowables();
        }

        private void OnRevivePlayer()
        {
            CurrentCount = preset.startCount;
            IsOwned = preset.isOwnedByDefault;
        }

        private void OnPlayerDie()
        {
            if (!IsActive)
                return;

            //Drop();
        }

        public void Drop()
        {
            Instantiate(preset.pickup, transform.position, transform.rotation, null);
            Disown();
        }

        public void SetAsDeactive()
        {
            IsActive = false;

            if (IsOwned)
                Holster();
            else
                gameObject.SetActive(false);
        }

        public void SetAsActive()
        {
            IsActive = true;
            gameObject.SetActive(true);
            GetInHand();
        }

        public void Own()
        {
            IsOwned = true;
        }

        public void Disown()
        {
            IsOwned = false;
            SetAsDeactive();
        }

        public void Holster()
        {
            transform.SetParent(holsterPos);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (var rend in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                rend.sortingLayerName = holsterLayer;
            }
        }

        public void GetInHand()
        {
            transform.SetParent(defaultParent);
            transform.SetLocalPositionAndRotation(defaultPos, Quaternion.identity);

            foreach (var rend in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                rend.sortingLayerName = defaultLayer;
            }
        }

        public void IncreaseCount(int count = 1)
        {
            CurrentCount += count;
            playerThrowables.UpdateCountUI(CurrentCount);
        }

        public void SetCount()
        {
            CurrentCount = preset.startCount;
        }
    }
}