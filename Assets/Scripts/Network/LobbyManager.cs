using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Nakama.Helpers;
using Newtonsoft.Json;
using UnityEngine;


public class LobbyManager : MonoBehaviour
{
    #region FIELDS

    private NakamaManager nakamaManager = null;
    private MultiplayerManager multiplayerManager = null;
    private MatchManager matchManager = null;
    protected bool blockJoinsAndLeaves = false;

    #endregion

    #region EVENTS

    public event Action<List<PlayerData>> onPlayersReceived;
    public event Action<PlayerData> onPlayerJoined;
    public event Action<PlayerData> onPlayerLeft;
    public event Action<LobbyCountDownChangeStateMsg> onLobbyCountdownStateChange;
    public event Action<LobbyChatMessage> onChatMessage;
    public event Action<PlayerData, int> onLocalPlayerObtained;

    #endregion

    #region PROPERTIES

    public static LobbyManager Instance { get; private set; } = null;
    public List<PlayerData> Players { get; private set; } = new List<PlayerData>();
    public int PlayersCount { get => Players.Count(player => player != null); }
    public PlayerData CurrentPlayer { get; private set; } = null;
    public int CurrentPlayerNumber { get; private set; } = -1;
    public bool BlockJoinAndLeaves => blockJoinsAndLeaves;
    #endregion

    #region BEHAVIORS

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        multiplayerManager = MultiplayerManager.Instance;
        nakamaManager = NakamaManager.Instance;
        matchManager = MatchManager.Instance;
        matchManager.onMatchJoin += MatchJoined;
        matchManager.onMatchLeave += ResetLeaved;
        multiplayerManager.Subscribe(MessageCode.LobbyUpdatePlayers, SetPlayers);
        multiplayerManager.Subscribe(MessageCode.ChatMessage, OnGetChatMessage);
        multiplayerManager.Subscribe(MessageCode.LobbyPlayerJoined, PlayerJoined);
        multiplayerManager.Subscribe(MessageCode.lobbyCountdownChangeState, OnLobbyTimerStateChange);
        multiplayerManager.Subscribe(MessageCode.ChangeScene, MatchStarted);
    }


    private void OnDestroy()
    {
        matchManager.onMatchJoin -= MatchJoined;
        matchManager.onMatchLeave -= ResetLeaved;
        multiplayerManager.Unsubscribe(MessageCode.LobbyUpdatePlayers, SetPlayers);
        multiplayerManager.Unsubscribe(MessageCode.ChatMessage, OnGetChatMessage);
        multiplayerManager.Unsubscribe(MessageCode.lobbyCountdownChangeState, OnLobbyTimerStateChange);
        multiplayerManager.Unsubscribe(MessageCode.LobbyPlayerJoined, PlayerJoined);
        multiplayerManager.Unsubscribe(MessageCode.ChangeScene, MatchStarted);
    }

    private void OnLobbyTimerStateChange(MultiplayerMessage message)
    {
        LobbyCountDownChangeStateMsg msg = message.GetData<LobbyCountDownChangeStateMsg>();
        Debug.Log("recieved lobby timer state change :" + msg.ShouldStart);
        blockJoinsAndLeaves = msg.ShouldStart;
        onLobbyCountdownStateChange?.Invoke(msg);
    }
    private void OnGetChatMessage(MultiplayerMessage message)
    {
        LobbyChatMessage chtmsg = message.GetData<LobbyChatMessage>();
        onChatMessage?.Invoke(chtmsg);
    }

    private void SetPlayers(MultiplayerMessage message)
    {
        Players = message.GetData<List<PlayerData>>();
        Debug.Log("new players recieved in lobby manager");

        onPlayersReceived?.Invoke(Players);
        GetCurrentPlayer();
    }

    private void PlayerJoined(MultiplayerMessage message)
    {
        Debug.Log("new player joined recieved in lobby manager");

        PlayerData player = message.GetData<PlayerData>();
        int index = Players.IndexOf(null);
        if (index > -1)
            Players[index] = player;
        else
            Players.Add(player);

        onPlayerJoined?.Invoke(player);
    }
    public void SendChat(string message)
    {
        LobbyChatMessage payload = new LobbyChatMessage(NakamaUserManager.Instance.DisplayName, message);

        //string jsonPayload = JsonUtility.ToJson(payload);

        // Send chat message to the Nakama server (OperationCode.ChatMessage)
        MultiplayerManager.Instance.SendMatchState(MessageCode.ChatMessage, payload);
    }
    private void PlayersChanged(IMatchPresenceEvent matchPresenceEvent)
    {
        if (blockJoinsAndLeaves)
            return;

        foreach (IUserPresence userPresence in matchPresenceEvent.Leaves)
        {
            for (int i = 0; i < Players.Count(); i++)
            {
                if (Players[i] != null && Players[i].Presence.SessionId == userPresence.SessionId)
                {
                    onPlayerLeft?.Invoke(Players[i]);
                    Players[i] = null;
                }
            }
        }
    }
    public async Task LeaveLobby()
    {
        await matchManager.LeaveMatchAsync();
    }
    private void MatchJoined()
    {
        nakamaManager.Socket.ReceivedMatchPresence += PlayersChanged;
        GetCurrentPlayer();
    }

    private void GetCurrentPlayer()
    {
        if (Players == null)
            return;

        if (multiplayerManager.Self == null)
            return;

        if (CurrentPlayer != null)
            return;

        CurrentPlayer = Players.Find(player => player.Presence.SessionId == multiplayerManager.Self.SessionId);
        CurrentPlayerNumber = Players.IndexOf(CurrentPlayer);
        onLocalPlayerObtained?.Invoke(CurrentPlayer, CurrentPlayerNumber);
    }

    private void ResetLeaved()
    {
        nakamaManager.Socket.ReceivedMatchPresence -= PlayersChanged;
        blockJoinsAndLeaves = false;
        Players = null;
        CurrentPlayer = null;
        CurrentPlayerNumber = -1;
    }

    public void MatchStarted(MultiplayerMessage message)
    {
        blockJoinsAndLeaves = true;
    }

    #endregion
}
public class PlayerData
{
    #region FIELDS

    private const string PresenceKey = "presence";
    private const string DisplayNameKey = "displayName";

    #endregion

    #region PROPERTIES

    [JsonProperty(PresenceKey)] public UserPresence Presence { get; private set; }
    [JsonProperty(DisplayNameKey)] public string DisplayName { get; private set; }

    #endregion

    #region CONSTRUCTORS

    public PlayerData(UserPresence presence, string displayName)
    {
        Presence = presence;
        DisplayName = displayName;
    }

    #endregion
}
public class LobbyCountDownChangeStateMsg
{
    private const string durationKey = "duration";
    private const string startKey = "start";

    [JsonProperty(durationKey)]public int Duration { get; private set; }
    [JsonProperty(startKey)] public bool ShouldStart { get; private set; }
    public LobbyCountDownChangeStateMsg(int duration, bool shouldStart)
    {
        Duration = duration;
        ShouldStart = shouldStart;
    }
}
public class PresenceData
{
    #region FIELDS

    private const string SessionIdKey = "sessionId";

    #endregion

    #region PROPERTIES

    [JsonProperty(SessionIdKey)] public string SessionId { get; private set; }

    #endregion

    #region CONSTRUCTORS

    public PresenceData(string sessionId)
    {
        SessionId = sessionId;
    }

    #endregion
}
[Serializable]
public class LobbyChatMessage
{
    private const string senderKey = "sender";
    private const string messageKey = "message";

    [JsonProperty(senderKey)]public string sender;
    [JsonProperty(messageKey)]public string message;
    //public long timestamp;
    public LobbyChatMessage(string sender, string message)
    {
        this.sender = sender;
        this.message = message;
    }
}
