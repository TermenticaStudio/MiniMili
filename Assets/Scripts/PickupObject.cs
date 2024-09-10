using Feature.Audio;
using Mirror;
using System;
using UnityEngine;
[Serializable]
public class PickupObject : NetworkBehaviour
{
    [SerializeField] private AudioClip pickSFX;

    public virtual void Pickup()
    {
        AudioManager.Instance.PlaySFX(pickSFX);
        if (isServer)
        {
            NetworkServer.UnSpawn(gameObject);
           // Destroy(gameObject);
        }
    }
    public NetworkIdentity GetNetworkIdentity()
    {
        return netIdentity;
    }
}