using Nakama;
using Nakama.Helpers;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public bool IsOnMatch { get => match != null; }
    [SerializeField] private bool enableLog = false;
    public IMatch match = null;
    public event Action onMatchJoin = null;
    public event Action onMatchLeave = null;
    private const string JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
    public static MatchManager Instance { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
    }

    public async void JoinMatchAsync()
    {
        MultiplayerManager.Instance.ShouldReceiveMatchState(true);
        NakamaManager.Instance.onDisconnected += OnDisconnected;
        IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinOrCreateMatchRpc);
        string matchId = rpcResult.Payload;
        match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId);
        onMatchJoin?.Invoke();
    }

    public async Task LeaveMatchAsync()
    {
        NakamaManager.Instance.onDisconnected -= OnDisconnected;
        MultiplayerManager.Instance.ShouldReceiveMatchState(false);
        await NakamaManager.Instance.Socket.LeaveMatchAsync(match);
        match = null;
        onMatchLeave?.Invoke();
    }

    private void OnDisconnected()
    {
        NakamaManager.Instance.onDisconnected -= OnDisconnected;
        MultiplayerManager.Instance.ShouldReceiveMatchState(false);
        match = null;
        onMatchLeave?.Invoke();
    }

}
