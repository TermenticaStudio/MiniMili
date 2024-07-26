using Feature.Audio;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] private AudioClip pickSFX;

    public virtual void Pickup()
    {
        AudioManager.Instance.PlaySFX(pickSFX);
        Destroy(gameObject);
    }
}