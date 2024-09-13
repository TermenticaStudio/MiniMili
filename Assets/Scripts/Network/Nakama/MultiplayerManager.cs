using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nakama.Helpers
{
    public partial class MultiplayerManager : MonoBehaviour
    {
        #region FIELDS

        private const int TickRate = 5;
        private const float SendRate = 1f / (float)TickRate;
        private const string LogFormat = "{0} with code {1}:\n{2}";
        private const string SendingDataLog = "Sending data";
        private const string ReceivedDataLog = "Received data";

        [SerializeField] private bool enableLog = false;

        private Dictionary<MessageCode, Action<MultiplayerMessage>> onReceiveData = new Dictionary<MessageCode, Action<MultiplayerMessage>>();

        #endregion

        #region EVENTS

        public event Action onLocalTick = null;

        #endregion

        #region PROPERTIES

        public static MultiplayerManager Instance { get; private set; } = null;
        public IUserPresence Self { get => MatchManager.Instance.match == null ? null : MatchManager.Instance.match.Self; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InvokeRepeating(nameof(LocalTickPassed), SendRate, SendRate);
        }

        private void LocalTickPassed()
        {
            onLocalTick?.Invoke();
        }

        public void SendMatchState(MessageCode code, object data = null)
        {
            if (MatchManager.Instance.match == null)
                return;

            string json = data != null ? data.Serialize() : string.Empty;
            if (enableLog)
                LogData(SendingDataLog, (long)code, json);

            NakamaManager.Instance.Socket.SendMatchStateAsync(MatchManager.Instance.match.Id, (long)code, json);
        }

        public void ShouldReceiveMatchState(bool should)
        {
            if (should)
            {
                NakamaManager.Instance.Socket.ReceivedMatchState -= OnReceivedMatchState;
                NakamaManager.Instance.Socket.ReceivedMatchState += OnReceivedMatchState;
            }
            else
            {
                NakamaManager.Instance.Socket.ReceivedMatchState -= OnReceivedMatchState;
            }
        }
       
        public void SendMatchState(MessageCode code, byte[] bytes)
        {
            if (MatchManager.Instance.match == null)
                return;

            if (enableLog)
                LogData(SendingDataLog, (long)code, String.Empty);

            NakamaManager.Instance.Socket.SendMatchStateAsync(MatchManager.Instance.match.Id, (long)code, bytes);
        }

        private void OnReceivedMatchState(IMatchState newState)
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
        public void Subscribe(MessageCode code, Action<MultiplayerMessage> action)
        {
            if (!onReceiveData.ContainsKey(code))
                onReceiveData.Add(code, null);

            onReceiveData[code] += action;
        }

        public void Unsubscribe(MessageCode code, Action<MultiplayerMessage> action)
        {
            if (onReceiveData.ContainsKey(code))
                onReceiveData[code] -= action;
        }

        private void LogData(string description, long dataCode, string json)
        {
            Debug.Log(string.Format(LogFormat, description, (MessageCode)dataCode, json));
        }

        #endregion
    }
}
