using UnityEngine;

public class JetpackFlame : MonoBehaviour
{
    [Header("Flames")]
    [SerializeField] private ParticleSystem flame;
    [SerializeField] private Vector3 minPowerSize;
    [SerializeField] private Vector3 maxPowerSize;
    private bool isActive;

    [Header("Smoke")]
    [SerializeField] private ParticleSystem smoke;
    private Vector2 smokeSize;
    private Vector2 smokeTargetSize;
    private Vector2 currentSmokeSize;

    [Header("Trail")]
    [SerializeField] private TrailRenderer flameTrail;

    [Header("SFX")]
    [SerializeField] private AudioSource flameSFX;
    [SerializeField] private float minPowerVol;
    [SerializeField] private float maxPowerVol;
    private float targetVol;

    private void Start()
    {
        smokeSize = new Vector2(smoke.main.startSize.constantMin, smoke.main.startSize.constantMax);

        ActivateFlame(false);
        SetSmokeSize(Vector2.zero);
        flameSFX.volume = 0;
    }

    private void Update()
    {
        currentSmokeSize = Vector2.Lerp(currentSmokeSize, isActive ? smokeTargetSize : Vector2.zero, Time.deltaTime * (isActive ? 5f : 1f));
        flameSFX.volume = Mathf.Lerp(flameSFX.volume, isActive ? targetVol : 0, Time.deltaTime * 5f);

        SetSmokeSize(currentSmokeSize);
    }

    public void ActivateFlame(bool value)
    {
        if (isActive == value)
            return;

        var em = flame.emission;
        em.enabled = value;

        isActive = value;
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

    private void SetSmokePower(float amount)
    {
        smokeTargetSize = Vector2.Lerp(Vector2.zero, smokeSize, amount);
    }

    private void SetSmokeSize(Vector2 size)
    {
        var final = smoke.main;
        final.startSize = new ParticleSystem.MinMaxCurve(size.x, size.y);
    }

    public void SetSFXPower(float amount)
    {
        targetVol = Mathf.Lerp(minPowerVol, maxPowerVol, amount);
    }
}
