using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using Mirror;
using DG.Tweening.Core.Easing;
using System.IO;
using System.Text;
using System;
using Nakama;
using Newtonsoft.Json;
using Nakama.Helpers;

public class HttpServer : MonoBehaviour
{
    public CustomNetworkManager networkManager;

    private HttpListener listener;
    private string url = "http://localhost:8080/";

    public void StartHttpServer()
    {
        if (!HttpListener.IsSupported)
        {
            Debug.LogError("HTTP Listener is not supported on this platform.");
            return;
        }
        listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        ListenForRequests();
    }

    private async void ListenForRequests()
    {
        while (true)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/unity-api/match/start-game")
            {
                // Read and parse the request body
                string requestBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                // Optionally: Deserialize the request body to get matchId and players
                StartGameRequest requestData = JsonUtility.FromJson<StartGameRequest>(requestBody);

                // Log the parsed data
                Debug.Log($"Match ID: {requestData.matchId}");
                foreach (var player in requestData.players)
                {
                    Debug.Log($"Player ID: {player.presence.UserId}, Username: {player.presence.Username}");
                }

                // Call networkManager's method to start the game
                networkManager.StartNewGame(requestData);
                
                // Send an HTTP response
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.OK;
                byte[] buffer = Encoding.UTF8.GetBytes("Game started successfully");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.Close();
            }
        }
    }
    private void OnApplicationQuit()
    {
        listener.Close();
    }
}
[Serializable]
public class StartGameRequest
{
    private const string matchIdKey = "matchId";
    private const string playersKey = "players";

    [JsonProperty(matchIdKey)]public string matchId;
    [JsonProperty(playersKey)]public NakamaPlayer[] players;
    public StartGameRequest(string matchId, NakamaPlayer[] players)
    {
        this.matchId = matchId;
        this.players = players;
    }
}
[Serializable]
public class NakamaPlayer
{
    private const string displayNameKey = "displayName";
    public UserPresence presence;
    [JsonProperty(displayNameKey)] public string displayName;
    public NakamaPlayer(UserPresence presence, string displayName)
    {
        this.presence = presence;
        this.displayName = displayName;
    }
}
