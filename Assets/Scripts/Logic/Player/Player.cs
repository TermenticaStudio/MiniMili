using UnityEngine;

namespace Logic.Player
{
    [RequireComponent(typeof(Health), typeof(PlayerInfo), typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerWeaponsManager))]
    public class Player : MonoBehaviour
    {
        public string Id { get; set; }

        public Health Health { get; private set; }
        public PlayerInfo Info { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public PlayerWeaponsManager WeaponsManager { get; private set; }

        private void Start()
        {
            Health = GetComponent<Health>();
            Info = GetComponent<PlayerInfo>();
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponsManager = GetComponent<PlayerWeaponsManager>();

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

        private void OnSpawn(PlayerInfo obj)
        {
            Health.Revive();
            Info.SetPlayerName(PlayerPrefs.GetString("PLAYER_NAME"));
            CameraController.Instance.SetTarget(obj.transform);
            WeaponsManager.OnStartPlayer();
        }

        private void OnUpdateHealth(float arg1, float arg2)
        {
            WeaponInfoUI.Instance.SetHealth(arg1 / arg2);
        }

        private void OnDie()
        {
            PlayerSpawnHandler.Instance.RequestForPlayerRespawn(Info);
            CameraController.Instance.SetTarget(null);
        }
    }
}