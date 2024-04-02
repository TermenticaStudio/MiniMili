using Mirror;
using System;

public class PlayerSpawnHandler : NetworkBehaviour
{
    public static PlayerSpawnHandler Instance;

    public event Action<PlayerInfo> OnSpawnPlayer;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void SpawnPlayerRpc(NetworkIdentity identity)
    {
        if (!identity.isLocalPlayer)
            return;

        OnSpawnPlayer?.Invoke(identity.GetComponent<PlayerInfo>());
    }
}