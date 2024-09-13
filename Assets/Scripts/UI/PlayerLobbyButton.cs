using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLobbyButton : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI playerNameText;
    public string playerName;
    public PlayerData playerData;
    public void Setup(PlayerData player)
    {
        playerName = player.DisplayName;
        playerData = player;
        SetText();
    }
    private void SetText()
    {
        playerNameText.text = playerName;
    }
}
