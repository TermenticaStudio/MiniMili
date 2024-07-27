using Feature.Flip;
using Feature.Health;
using Feature.Jetpack;
using Feature.Notifier;
using Feature.OverlapDetector;
using Feature.Player.Aim;
using Feature.Player.Movement;
using Feature.Player.Stabilizer;
using Logic.Player.ThrowablesSystem;
using Logic.Player.WeaponsSystem;
using UnityEngine;

namespace Logic.Player
{
    [RequireComponent(typeof(HealthController), typeof(PlayerInfo), typeof(Rigidbody2D))]
    [RequireComponent(typeof(WeaponsManager))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform cameraLook;
        [SerializeField] private float cameraLerpSpeedMul = 5f;
        [SerializeField] private JetpackController jetpack;
        [SerializeField] private MovementController movement;
        [SerializeField] private OverlapDetectorController groundDetector;
        [SerializeField] private OverlapDetectorController ceilDetector;
        [SerializeField] private FlipController flipController;
        [SerializeField] private StabilizerController stabilizerController;
        private Vector3 _targetCamLook;

        public string Id { get; set; }

        public HealthController Health { get; private set; }
        public PlayerInfo Info { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public WeaponsManager WeaponsManager { get; private set; }
        public ThrowablesManager Throwables { get; private set; }
        public AimController Aim { get; private set; }
        public JetpackController Jetpack { get => jetpack; }
        public MovementController Movement { get => movement; }
        public FlipController Flip { get => flipController; }

        private void Start()
        {
            Health = GetComponent<HealthController>();
            Info = GetComponent<PlayerInfo>();
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponsManager = GetComponent<WeaponsManager>();
            Throwables = GetComponent<ThrowablesManager>();
            Aim = GetComponent<AimController>();

            movement.InjectGroundDetector(groundDetector);
            movement.InjectCeilDetector(ceilDetector);
            jetpack.InjectGroundDetector(groundDetector);
            Aim.InjectFlipController(flipController);
            movement.InjectFlipController(flipController);
            Throwables.InjectFlipController(flipController);
            stabilizerController.InjectGroundDetector(groundDetector);

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
            if (!Health.IsDead)
            {
                jetpack.FeedDirectionInput(PlayerInput.Instance.GetMovement());
                movement.FeedDirectionInput(PlayerInput.Instance.GetMovement());
                Aim.FeedDirectionInput(PlayerInput.Instance.GetAim());
                stabilizerController.FeedDirectionInput(PlayerInput.Instance.GetMovement());
            }

            if (!cameraLook)
                return;

            cameraLook.transform.position = Vector3.Lerp(cameraLook.transform.position, _targetCamLook, Time.deltaTime * cameraLerpSpeedMul);
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
            jetpack.EnableJetpack();
            jetpack.ResetFuel();
        }

        private void OnUpdateHealth(float arg1, float arg2)
        {
            WeaponInfoUI.Instance.SetHealth(arg1 / arg2);
        }

        private void OnDie()
        {
            if (Health.LastDamageBy == null || Health.LastDamageBy == this)
            {
                NotifyManager.Instance.Notify(MessageTexts.GetMessageContent(MessageTexts.MessageType.Suicide), Info.GetPlayerName());
            }
            else if (Health.LastDamageBy != null)
            {
                NotifyManager.Instance.Notify(MessageTexts.GetMessageContent(MessageTexts.MessageType.Kill), Info.GetPlayerName(), Health.LastDamageBy.Info.GetPlayerName());
            }

            PlayerSpawnHandler.Instance.RequestForPlayerRespawn(Info);
            jetpack.PurgeFuel();
            jetpack.DisableJetpack();
            //CameraController.Instance.SetTarget(null);
        }

        public void SetCameraLookPos(Vector3 pos, bool force = false)
        {
            _targetCamLook = pos;

            if (force)
                cameraLook.position = _targetCamLook;
        }
    }
}