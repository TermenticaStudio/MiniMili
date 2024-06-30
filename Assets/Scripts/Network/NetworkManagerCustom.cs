using Logic.Player;
using Mirror;
using System.Linq;
using UnityEngine;

public class NetworkManagerCustom : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var spawnPoint = GetStartPosition();
        var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        PlayerSpawnHandler.Instance.SpawnPlayerRpc(conn.identity);
    }

    public override Transform GetStartPosition()
    {
        var spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        var allPlayers = FindObjectsOfType<PlayerInfo>().ToList();

        NetworkStartPosition farest = null;
        var longestDistance = 0f;

        if (allPlayers.Count == 0)
            return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;

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
}