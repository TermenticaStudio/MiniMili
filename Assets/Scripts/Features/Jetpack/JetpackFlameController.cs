using UnityEngine;

namespace Feature.Jetpack
{
    public class JetpackFlameController : MonoBehaviour
    {
        [Header("Flames")]
        [SerializeField] private ParticleSystem flame;
        [SerializeField] private Vector3 minPowerSize;
        [SerializeField] private Vector3 maxPowerSize;
        private bool _isActive = true;
        private bool _isPreLaunch;

        [Header("Smoke")]
        [SerializeField] private ParticleSystem smoke;
        private Vector2 _smokeSize;
        private Vector2 _smokeTargetSize;
        private Vector2 _currentSmokeSize;

        [Header("Trail")]
        [SerializeField] private TrailRenderer flameTrail;

        [Header("SFX")]
        [SerializeField] private AudioSource flameSFX;
        [SerializeField] private float minPowerVol;
        [SerializeField] private float maxPowerVol;
        private float _targetVol;

        private void Start()
        {
            _smokeSize = new Vector2(smoke.main.startSize.constantMin, smoke.main.startSize.constantMax);

            ActivateFlame(false);
            SetSmokeSize(Vector2.zero);
            flameSFX.volume = 0;
        }

        private void Update()
        {
            _currentSmokeSize = Vector2.Lerp(_currentSmokeSize, (_isActive || _isPreLaunch) ? _smokeTargetSize : Vector2.zero, Time.deltaTime * (_isActive ? 5f : 1f));
            flameSFX.volume = Mathf.Lerp(flameSFX.volume, _isActive ? _targetVol : 0, Time.deltaTime * 5f);

            SetSmokeSize(_currentSmokeSize);
        }

        public void ActivateFlame(bool value)
        {
            if (_isActive == value)
                return;

            var em = flame.emission;
            em.enabled = value;

            _isActive = value;
            _isPreLaunch = false;
            flameTrail.emitting = value;
        }

        public void SetPower(float amount)
        {
            var finalValue = Vector3.Lerp(minPowerSize, maxPowerSize, amount);

            var size = flame.main;
            size.startSizeX = finalValue.x;
            size.startSizeY = finalValue.y;
            size.startSizeZ = finalValue.z;

            SetSmokePower(amount);
            SetSFXPower(amount);
        }

        public void ActivatePreFire()
        {
            SetSmokePower(1f, true);
            flameTrail.emitting = true;
            _isPreLaunch = true;
        }

        private void SetSmokePower(float amount, bool force = false)
        {
            _smokeTargetSize = Vector2.Lerp(Vector2.zero, _smokeSize, amount);

            if (force)
                _currentSmokeSize = _smokeTargetSize;
        }

        private void SetSmokeSize(Vector2 size)
        {
            var final = smoke.main;
            final.startSize = new ParticleSystem.MinMaxCurve(size.x, size.y);
        }

        public void SetSFXPower(float amount)
        {
            _targetVol = Mathf.Lerp(minPowerVol, maxPowerVol, amount);
        }
    }
}