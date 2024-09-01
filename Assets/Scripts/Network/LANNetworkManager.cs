using Logic.Player;
using Mirror;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LANNetworkManager : NetworkManager
{
    [SerializeField] private LanDiscovery lanDiscovery;
    public static new LANNetworkManager singleton => NetworkManager.singleton as LANNetworkManager;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var spawnPoint = GetStartPosition();
        var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

      //  PlayerSpawnHandler.Instance.SpawnPlayerRpc(conn.identity);
    }

    public override Transform GetStartPosition()
    {
        var spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        var allPlayers = FindObjectsOfType<PlayerInfo>().ToList();

        NetworkStartPosition farest = null;
        var longestDistance = 0f;

        if (allPlayers.Count == 0)
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;

        foreach (var spawnPoint in spawnPoints)
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                var distance = Vector2.Distance(spawnPoint.transform.position, allPlayers[i].transform.position);

                if (distance > longestDistance)
                {
                    longestDistance = distance;
                    farest = spawnPoint;
                }
            }
        }

        return farest.transform;
    }
    public bool HostLAN(string serverName)
    {
        try
        {
            StartHost();
            lanDiscovery.AdvertiseServer(serverName);
            return true;
        }
        catch
        {
            StopHost();
            lanDiscovery.StopDiscovery();
            return false;
        }
    }
    public void ConnectToHost(Uri uri)
    {
        try
        {
            StartClient(uri);
        }
        catch
        {
            StopClient();
        }
    }
    public async Task<bool> FindNearbyHost(float? timeout = null)
    {
        try
        {
            lanDiscovery.StartDiscovery();
            float timeoutsec = timeout ?? 5;
            TimeSpan timeOutSpan = TimeSpan.FromSeconds(timeoutsec);
            while (!lanDiscovery.DidFindAny)
            {
                await Task.Delay(100);
                timeOutSpan -= TimeSpan.FromMilliseconds(100);
                if (timeOutSpan < TimeSpan.Zero)
                {
                    throw new Exception("No nearby host found");
                }
            }
            return true;

        }
        catch
        {
            lanDiscovery.StopDiscovery();
            return false;
        }

    }
}