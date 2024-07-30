using System.Collections.Generic;
using UnityEngine;

namespace Feature.GunShotIndicator
{
    public class IndicatorController : MonoBehaviour
    {
        [SerializeField] private Indicator prefab;
        [SerializeField] private Transform indicatorsHolder;
        [SerializeField] private int instancesCount = 10;

        private List<Indicator> _currentInstances = new();
        private Camera _cam;

        private void Start()
        {
            CreateInstances();
            FindCamera();
        }

        private void CreateInstances()
        {
            foreach (Transform obj in indicatorsHolder.transform)
                Destroy(obj.gameObject);

            for (int i = 0; i < instancesCount; i++)
            {
                var instance = Instantiate(prefab, indicatorsHolder);
                _currentInstances.Add(instance);
            }
        }

        public void ShowIndicator(Vector3 lookAt)
        {
            var origin = _cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

            foreach (var indicator in _currentInstances)
            {
                if (indicator.IsActive)
                    continue;

                indicator.Display(origin, lookAt);
                return;
            }

            _currentInstances[Random.Range(0, _currentInstances.Count)].Display(origin, lookAt);
        }

        private void FindCamera()
        {
            _cam = Camera.main;
        }
    }
}