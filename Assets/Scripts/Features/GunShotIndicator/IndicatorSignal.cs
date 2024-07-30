using UnityEngine;

namespace Feature.GunShotIndicator
{
    public class IndicatorSignal : MonoBehaviour
    {
        private IndicatorController controller;

        private void Start()
        {
            controller = FindObjectOfType<IndicatorController>();
        }

        public void Signal()
        {
            var isInSight = CameraSightChecker.IsObjectInCameraSight(gameObject);

            if (isInSight)
                return;

            controller.ShowIndicator(transform.position);
        }
    }
}