using Feature.Audio;
using Mirror;
using System;
using UnityEngine;
[Serializable]
public class PickupObject : MonoBehaviour
{
    [SerializeField] private AudioClip pickSFX;

    public virtual void Pickup()
    {
        AudioManager.Instance.PlaySFX(pickSFX);
        Destroy(gameObject);
    }
    public NetworkIdentity GetNetworkIdentity()
    {
        return GetComponent<NetworkIdentity>();
    }
}