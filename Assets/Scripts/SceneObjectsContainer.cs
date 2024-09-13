using Logic.Player;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectsContainer : MonoBehaviour
{
    public Player localPlayer;
    public bool IsServer { 
        get
        {
            return NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly || NetworkManager.singleton.mode == NetworkManagerMode.Host;
        } 
    }
}
