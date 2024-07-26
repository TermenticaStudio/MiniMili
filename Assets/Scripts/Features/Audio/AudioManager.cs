using UnityEngine;

namespace Feature.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource sfxSource;

        private Transform _currentCamera;

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

            AudioSource.PlayClipAtPoint(clip, new Vector3(pos.x, pos.y, _currentCamera.transform.position.z + 2), volume);
        }

        public void SetCamera(Transform cam)
        {
            _currentCamera = cam;
        }
    }
}