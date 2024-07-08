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

        SetCamera(Camera.main.transform);
        transform.SetParent(null);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private AudioSource sfxSource;

    private Transform currentCamera;

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!clip)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void Play2DSFX(AudioClip clip, Vector3 pos, float volume = 1f)
    {
        if (!clip)
            return;

        AudioSource.PlayClipAtPoint(clip, new Vector3(pos.x, pos.y, currentCamera.transform.position.z + 2), volume);
    }

    public void SetCamera(Transform cam)
    {
        currentCamera = cam;
    }
}