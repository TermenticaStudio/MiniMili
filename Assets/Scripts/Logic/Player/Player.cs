using Logic.Player.ThrowablesSystem;
using Logic.Player.WeaponsSystem;
using UnityEngine;

namespace Logic.Player
{
    [RequireComponent(typeof(Health), typeof(PlayerInfo), typeof(Rigidbody2D))]
    [RequireComponent(typeof(WeaponsManager))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform cameraLook;
        [SerializeField] private float cameraLerpSpeedMul = 5f;
        private Vector3 targetCamLook;

        public string Id { get; set; }

        public Health Health { get; private set; }
        public PlayerInfo Info { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public WeaponsManager WeaponsManager { get; private set; }
        public ThrowablesManager Throwables { get; private set; }
        public PlayerAim Aim { get; private set; }

        private void Start()
        {
            Health = GetComponent<Health>();
            Info = GetComponent<PlayerInfo>();
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponsManager = GetComponent<WeaponsManager>();
            Throwables = GetComponent<ThrowablesManager>();
            Aim = GetComponent<PlayerAim>();

            PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawn;

            Health.OnDie += OnDie;
            Health.OnUpdateHealth += OnUpdateHealth;
        }

        private void OnDisable()
        {
            PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawn;

            Health.OnDie -= OnDie;
            Health.OnUpdateHealth -= OnUpdateHealth;
        }

        private void Update()
        {
            if (!cameraLook)
                return;

            cameraLook.transform.position = Vector3.Lerp(cameraLook.transform.position, targetCamLook, Time.deltaTime * cameraLerpSpeedMul);
        }

        private void OnSpawn(PlayerInfo obj)
        {
            Health.Revive();
            Info.SetPlayerName(PlayerPrefs.GetString("PLAYER_NAME"));
            CameraController.Instance.SetTarget(cameraLook);
            SetCameraLookPos(transform.position);
            WeaponsManager.OnStartPlayer();
            Throwables.OnStartPlayer();
            PlayerInput.Instance.ResetInput();
            Aim.ResetAim();
        }

        private void OnUpdateHealth(float arg1, float arg2)
        {
            WeaponInfoUI.Instance.SetHealth(arg1 / arg2);
        }

        private void OnDie()
        {
            if (Health.LastDamageBy == null || Health.LastDamageBy == this)
            {
                InGameMessage.Instance.Notify(MessageTexts.GetMessageContent(MessageTexts.MessageType.Suicide), Info.GetPlayerName());
            }
            else if (Health.LastDamageBy != null)
            {
                InGameMessage.Instance.Notify(MessageTexts.GetMessageContent(MessageTexts.MessageType.Kill), Info.GetPlayerName(), Health.LastDamageBy.Info.GetPlayerName());
            }

            PlayerSpawnHandler.Instance.RequestForPlayerRespawn(Info);
            CameraController.Instance.SetTarget(null);
        }

        public void SetCameraLookPos(Vector3 pos, bool force = false)
        {
            targetCamLook = pos;

            if (force)
                cameraLook.position = targetCamLook;
        }
    }
}