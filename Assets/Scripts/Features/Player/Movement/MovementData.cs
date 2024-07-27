using UnityEngine;

namespace Feature.Player.Movement
{
    [CreateAssetMenu(menuName = "Features/Player/Movement/Data")]
    public class MovementData : ScriptableObject
    {
        [Header("Movement Setting")]
        public float walkForce = 0.8f;
        public float walkMaxSpeed = 3f;
        public float crawlForce = 0.4f;
        public float crawlMaxSpeed = 1.5f;
        public float standHeight = 1.9f;
        public float crawlHeight = 1.4f;
    }
}