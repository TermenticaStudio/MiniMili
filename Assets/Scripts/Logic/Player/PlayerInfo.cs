using TMPro;
using UnityEngine;

namespace Logic.Player
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshPro nameText;

        private Player player;
        public string CurrentName { get; private set; }
        public int RespawnsLeft { get; private set; }

        private void Start()
        {
            player = GetComponent<Player>();

            player.Health.OnDie += HideName;
            player.Health.OnRevive += ShowName;
        }

        private void OnDisable()
        {
            player.Health.OnDie -= HideName;
            player.Health.OnRevive -= ShowName;
        }

        private void Update()
        {
            nameText.transform.rotation = Quaternion.identity;
        }

        public void SetPlayerName(string name)
        {
            CurrentName = name;
            nameText.text = name;
        }

        public void ShowName()
        {
            nameText.gameObject.SetActive(true);
        }

        public void HideName()
        {
            nameText.gameObject.SetActive(false);
        }

        public void SetRespawnCount(int count)
        {
            RespawnsLeft = count;
        }

        public void UseRespawn()
        {
            RespawnsLeft--;
        }

        public bool CanRespawn()
        {
            return RespawnsLeft > 0;
        }
    }
}