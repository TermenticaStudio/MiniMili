using Nakama;
using Nakama.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingManager : MonoBehaviour
{
    #region FIELDS

    private const string JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
    private const string LogFormat = "{0} with code {1}:\n{2}";
    private const string ReceivedDataLog = "Received data";
    private Dictionary<MessageCode, Action<MultiplayerMessage>> onReceiveData = new Dictionary<MessageCode, Action<MultiplayerMessage>>();
    private IMatch match = null;
    [SerializeField] private bool enableLog = false;
    #endregion

    #region EVENTS

    public event Action onMatchJoin = null;
    public event Action onMatchLeave = null;
    public event Action onLocalTick = null;

    #endregion
    public async void JoinMatchAsync()
    {
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
        NakamaManager.Instance.onDisconnected += Disconnected;
        IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinOrCreateMatchRpc);
        string matchId = rpcResult.Payload;
        match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId);
        onMatchJoin?.Invoke();
    }
    private void Receive(IMatchState newState)
    {
        if (enableLog)
        {
            var encoding = System.Text.Encoding.UTF8;
            var json = encoding.GetString(newState.State);
            LogData(ReceivedDataLog, newState.OpCode, json);
        }

        MultiplayerMessage multiplayerMessage = new MultiplayerMessage(newState);
        if (onReceiveData.ContainsKey(multiplayerMessage.DataCode))
            onReceiveData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
    }
    private void Disconnected()
    {
        NakamaManager.Instance.onDisconnected -= Disconnected;
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        match = null;
        onMatchLeave?.Invoke();
    }
    private void LogData(string description, long dataCode, string json)
    {
        Debug.Log(string.Format(LogFormat, description, (MessageCode)dataCode, json));
    }
}
