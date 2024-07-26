using UnityEngine;

namespace Feature.OverlapDetector
{
    public class OverlapDetectorController : MonoBehaviour
    {
        [SerializeField] private Transform checker;
        [SerializeField] private LayerMask layer;
        [SerializeField] private float checkRadius = 0.3f;

        public bool IsOverlapped { get; private set; }

        private void Update()
        {
            IsOverlapped = Check();
        }

        private bool Check()
        {
            if (Physics2D.OverlapCircleAll(new Vector2(checker.position.x, checker.position.y), checkRadius, layer).Length > 0)
                return true;

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (checker)
                Gizmos.DrawWireSphere(checker.position, checkRadius);
        }
    }
}