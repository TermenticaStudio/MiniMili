using Logic.Player;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject gO = Instantiate(playerPrefab);
        Player player = gO.GetComponent<Player>();
        PlayerInfo playerInfo = gO.GetComponent<PlayerInfo>();

        gO.transform.position = GetStartPosition().position;

        NetworkServer.AddPlayerForConnection(conn, gO);
       // PlayerSpawnHandler.Instance.SpawnPickup(GetStartPosition().position);
    }
}
