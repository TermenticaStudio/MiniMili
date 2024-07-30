using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.GunShotIndicator
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float fadeOutDelay = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private bool _isActive;

        public bool IsActive { get => _isActive; }

        private void Start()
        {
            FadeOutInstant();
        }

        public void Display(Vector3 origin, Vector3 lookAt)
        {
            var moveDirection = origin - lookAt;
            var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            _isActive = true;
            FadeIn();
            FadeOut(() =>
            {
                _isActive = false;
            });
        }

        private void FadeIn()
        {
            image.DOFade(1, 0);
        }

        private void FadeOut(Action onCompletion)
        {
            image.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay).OnComplete(() =>
            {
                onCompletion?.Invoke();
            });
        }

        private void FadeOutInstant()
        {
            image.DOFade(0, 0);
        }
    }
}