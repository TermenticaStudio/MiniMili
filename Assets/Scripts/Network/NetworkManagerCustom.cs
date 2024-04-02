using Mirror;

public class NetworkManagerCustom : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var spawnPoint = GetStartPosition();
        var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        PlayerSpawnHandler.Instance.SpawnPlayerRpc(conn.identity);
    }
}