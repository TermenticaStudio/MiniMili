using Mirror;
using System.Diagnostics;
using UnityEngine;

public class NetworkManagerCustom : NetworkManager
{
    public struct CreateMMOCharacterMessage : NetworkMessage
    {
        public string name;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        var characterMessage = new CreateMMOCharacterMessage
        {
            name = "Saleh",
        };

        NetworkClient.Send(characterMessage);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreateMMOCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(playerPrefab);

        var playerInfo = gameobject.GetComponent<PlayerInfo>();
        playerInfo.SetName(message.name);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);

        if (conn.identity.isLocalPlayer)
            CameraController.Instance.SetTarget(gameobject.transform);
    }
}