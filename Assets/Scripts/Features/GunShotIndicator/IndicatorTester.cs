using UnityEngine;
using UnityEngine.Events;

namespace Feature.GunShotIndicator
{
    public class IndicatorTester : MonoBehaviour
    {
        public UnityEvent OnFire;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                OnFire?.Invoke();
            }
        }
    }
}