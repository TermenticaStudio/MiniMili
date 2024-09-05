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
using Mirror;
using Mirror.Examples.MultipleMatch;
using Mirror.Examples.Shooter;
using System;
using UnityEngine;

namespace Logic.Player
{
    [RequireComponent(typeof(HealthController), typeof(PlayerInfo), typeof(Rigidbody2D))]
    [RequireComponent(typeof(WeaponsManager))]
    public class Player : NetworkBehaviour
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
        public FlipController Flip { get => flipController; }
        public AimController Aim { get; private set; }
        public SceneObjectsContainer sceneObjectsContainer;
        private Vector3 _targetAim;
        private Vector3 _currentAim;


        void Awake()
        {
             sceneObjectsContainer = FindObjectOfType<SceneObjectsContainer>();
        }
        private void Start()
        {
            SetupComponents();
            if(!isLocalPlayer)
            {
                MultiSetup();
            }
        }

        private void SetupEvents()
        {
            Health.OnDie += OnDie;
            Health.OnUpdateHealth += OnUpdateHealth;
        }

        private void OnDisable()
        {
            Health.OnDie -= OnDie;
            Health.OnUpdateHealth -= OnUpdateHealth;
        }

        private void Update()
        {
            if (!Health.IsDead && isOwned)
            {
                jetpack.FeedDirectionInput(PlayerInput.Instance.GetMovement());
                movement.FeedDirectionInput(PlayerInput.Instance.GetMovement());
                Aim.FeedDirectionInput(PlayerInput.Instance.GetAim());
                if (cameraLook)
                {
                    SetCameraLookPos(transform.position);
                }
                stabilizerController.FeedDirectionInput(PlayerInput.Instance.GetMovement());
                _currentAim = Vector3.Lerp(_currentAim, _targetAim, Time.deltaTime * cameraLerpSpeedMul);
            }

        }
        private void SetupComponents(){
            Health = GetComponent<HealthController>();
            Info = GetComponent<PlayerInfo>();
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponsManager = GetComponent<WeaponsManager>();
            Throwables = GetComponent<ThrowablesManager>();
            Aim = GetComponent<AimController>();
            jetpack.EnableJetpack();
            jetpack.ResetFuel();
            Info.SetPlayerName();
        }
        private void LocalSetup()
        {
            if (Info == null)
            {
                SetupComponents();
            }
            Health.Revive();
            Aim.ResetAim();
            SetupEvents();
            movement.InjectGroundDetector(groundDetector);
            movement.InjectCeilDetector(ceilDetector);
            jetpack.InjectGroundDetector(groundDetector);
            Aim.InjectFlipController(flipController);
            movement.InjectFlipController(flipController);
            Throwables.InjectFlipController(flipController);
            stabilizerController.InjectGroundDetector(groundDetector);
            Throwables.OnStartPlayer();
            WeaponsManager.OnStartPlayer();
            SetCameraLookPos(transform.position);
            CameraController.Instance.SetTarget(cameraLook);
            PlayerInput.Instance.ResetInput();
            Health.OnUpdateHealth += OnUpdateHealth;
            Info.IsLocal = true;
            GameEvents.OnLocalPlayerSpawn();
        }
        private void MultiSetup()
        {
            movement.InjectGroundDetector(groundDetector);
            movement.InjectCeilDetector(ceilDetector);
            jetpack.InjectGroundDetector(groundDetector);
            Aim.InjectFlipController(flipController);
            movement.InjectFlipController(flipController);
            Throwables.InjectFlipController(flipController);
            stabilizerController.InjectGroundDetector(groundDetector);
            Throwables.OnStartPlayer();
            WeaponsManager.OnStartPlayer();

        }
        public override void OnStartLocalPlayer()
        {
            if (sceneObjectsContainer) { sceneObjectsContainer.localPlayer = this; }
            LocalSetup();
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
         public void SetAimLookPos(Vector3 pos)
        {
            _targetAim = pos - transform.position;
        }
    }
}