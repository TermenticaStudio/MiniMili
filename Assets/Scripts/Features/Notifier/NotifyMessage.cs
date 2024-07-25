using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Feature.Notifier
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class NotifyMessage : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;
        [SerializeField] private float fadeDuration = 0.5f;

        private TextMeshProUGUI _txt;

        public void Init(string text, Color textColor)
        {
            _txt = GetComponent<TextMeshProUGUI>();

            _txt.text = text;
            _txt.color = textColor;

            _txt.DOFade(0, fadeDuration).SetDelay(duration).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}