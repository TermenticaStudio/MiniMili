using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(null);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private AudioSource sfxSource;

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!clip)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void Play2DSFX(AudioClip clip, Vector3 pos, Vector3 camPos, float volume = 1f)
    {
        if (!clip)
            return;

        AudioSource.PlayClipAtPoint(clip, new Vector3(pos.x, pos.y, camPos.z + 2), volume);
    }
}