using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Message : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private TextMeshProUGUI txt;

    public void Init(string text, Color textColor)
    {
        txt = GetComponent<TextMeshProUGUI>();

        txt.text = text;
        txt.color = textColor;

        txt.DOFade(0, fadeDuration).SetDelay(duration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}