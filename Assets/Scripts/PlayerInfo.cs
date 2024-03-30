using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar()]
    [SerializeField] private string Name;

    public void SetName(string name)
    {
        Name = name;
    }
}