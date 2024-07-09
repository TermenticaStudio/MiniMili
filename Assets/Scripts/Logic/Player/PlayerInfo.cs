using TMPro;
using UnityEngine;

namespace Logic.Player
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshPro nameText;

        private Player player;
        private string currentName;

        public int RespawnsLeft { get; private set; }
        public bool IsLocal { get; set; }

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
            currentName = name;
            nameText.text = name;

            if (IsLocal)
                HideName();
            else
                ShowName();
        }

        public string GetPlayerName()
        {
            return IsLocal ? "You" : currentName;
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