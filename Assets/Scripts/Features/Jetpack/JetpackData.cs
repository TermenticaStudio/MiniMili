using UnityEngine;

namespace Feature.Jetpack
{
    [CreateAssetMenu(menuName = "Features/Jetpack/Data")]
    public class JetpackData : ScriptableObject
    {
        [Header("Config")]
        public float jetPackForce;
        public float jetPackLaunchForce;
        public float launchDelay = 0.65f;
        public float flyingMaxVelocity;
        public float fallingMaxVelocity;

        [Header("Fuel")]
        public float jetPackFuel = 10;
        public float fuelUsageMultiplier = 2f;
        public float fuelChargeMutliplier = 1f;
        public float refuelDelay = 1;
    }
}