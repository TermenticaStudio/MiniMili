using Logic.Player;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject gO = Instantiate(playerPrefab);
        gO.name = conn.connectionId.ToString();
        Player player = gO.GetComponent<Player>();
        player.MultiSetup();
        PlayerInfo playerInfo = gO.GetComponent<PlayerInfo>();
        gO.transform.position = GetStartPosition().position;

        NetworkServer.AddPlayerForConnection(conn, gO);
    }
    public override void OnStartServer()
    {
        PlayerSpawnHandler.Instance.SpawnPickup(GetStartPosition().position);
    }
}
