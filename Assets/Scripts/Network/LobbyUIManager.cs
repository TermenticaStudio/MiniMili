using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
[DefaultExecutionOrder(-500)]
public class LobbyUIManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private Transform playersContainer;
    [SerializeField] private PlayerLobbyButton playerButton;
    private LobbyManager lobbyManager = null;

    [SerializeField] private GameObject waitingText = null;
    [SerializeField] private Timer timer = null;
    [SerializeField] private Button backButton;
    [SerializeField] private ChatUI chatUI;


    private bool initialPlayersLoad;
    #endregion

    #region BEHAVIORS

    private void Awake()
    {
        initialPlayersLoad = false;
        lobbyManager = LobbyManager.Instance;
        lobbyManager.onPlayerJoined += PlayerJoined;
        lobbyManager.onPlayerLeft += PlayerLeft;
        lobbyManager.onPlayersReceived += PlayersReceived;
        lobbyManager.onChatMessage += OnChatMessage;
        chatUI.onSendChat += SendChatMessage;
        lobbyManager.onLobbyCountdownStateChange += OnLobbyCountdownStateChange;
        backButton.onClick.AddListener(TryLeaveLobby);
        UpdatePreviousInfos(LobbyManager.Instance.Players);
        initialPlayersLoad = true;
    }


    private void OnDestroy()
    {
        lobbyManager.onPlayerJoined -= PlayerJoined;
        lobbyManager.onPlayerLeft -= PlayerLeft;
        lobbyManager.onPlayersReceived -= PlayersReceived;
        chatUI.onSendChat += SendChatMessage;
        lobbyManager.onLobbyCountdownStateChange -= OnLobbyCountdownStateChange;
        lobbyManager.onChatMessage -= OnChatMessage;
    }
    private void OnLobbyCountdownStateChange(LobbyCountDownChangeStateMsg msg)
    {
        bool gameStarting = msg.ShouldStart;
        waitingText.SetActive(!gameStarting);
        timer.gameObject.SetActive(gameStarting);
        if (gameStarting)
            timer.Begin();
    }
    private void OnChatMessage(LobbyChatMessage chat)
    {
        /* foreach (var chat in chats)
         {
             chatUI.WriteMessageToChat(chat.message, chat.sender);
         }*/
        chatUI.WriteMessageToChat(chat.message, chat.sender);
    }
    private async void TryLeaveLobby()
    {
        try
        {
            backButton.interactable = false;
            if (lobbyManager.BlockJoinAndLeaves)
            {
                throw new Exception("You cant leave when game is starting");
            }

            await lobbyManager.LeaveLobby();
            SceneManager.LoadScene(0);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

        }
        finally
        {
            if (backButton != null)
                backButton.interactable = true;
        }

    }
    private void PlayersReceived(List<PlayerData> players)
    {
        if (initialPlayersLoad) return;
        Debug.Log("new players recieved");
        UpdateStatus(players);
    }
    public void SendChatMessage(string message)
    {
        lobbyManager.SendChat(message);
    }
    private void PlayerLeft(PlayerData player)
    {
        DeletePlayerButton(player);
       // UpdateStatus();
    }

    private void PlayerJoined(PlayerData player)
    {
        CreatePlayerButton(player);
        Debug.Log("new player joined recieved");

        // UpdateStatus();
    }
    private void UpdatePreviousInfos(List<PlayerData> players)
    {
        UpdateStatus(players);
        bool gameStarting = lobbyManager.BlockJoinAndLeaves;
        waitingText.SetActive(!gameStarting);
        timer.gameObject.SetActive(gameStarting);
        if (gameStarting)
            timer.Begin();

    }
    private void UpdateStatus(List<PlayerData> players)
    {
        playersContainer.DestroyAllChildren();
        foreach (var player in players)
        {
            CreatePlayerButton(player);
        }

    }
    private void CreatePlayerButton(PlayerData playerData)
    {
        GameObject obj = Instantiate(playerButton.gameObject,playersContainer);
        obj.GetComponentOrThrow<PlayerLobbyButton>().Setup(playerData);
    }
    private void DeletePlayerButton(PlayerData playerData)
    {
        for (int i = 0; i < playersContainer.childCount; i++)
        {

            var playerId = playersContainer.GetChild(i).GetComponent<PlayerLobbyButton>().playerData.Presence.UserId;
            if(playerId == playerData.Presence.UserId)
            {
                Destroy(playersContainer.GetChild(i).gameObject);
            }
        }
    }

    #endregion
}
