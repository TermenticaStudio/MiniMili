using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Feature.SceneLoader
{
    public class SceneControllerView : MonoBehaviour
    {
        [SerializeField] private GameObject loading;
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private TextMeshProUGUI loadingMapText;

        private float _FADE_TIME = 0.25f;

        public void Init()
        {
            FadeOut(true);
        }

        public void SetMapName(string mapName)
        {
            loadingMapText.text = mapName;
        }

        public IEnumerator FadeIn()
        {
            loading.SetActive(true);
            fadePanel.DOFade(0, 0);
            fadePanel.DOFade(1, _FADE_TIME);
            fadePanel.blocksRaycasts = true;

            yield return new WaitForSeconds(_FADE_TIME);
        }

        public IEnumerator FadeOut(bool force = false)
        {
            var fadeTime = force ? 0 : _FADE_TIME;

            fadePanel.blocksRaycasts = false;
            fadePanel.DOFade(0, fadeTime).OnComplete(() =>
            {
                loading.SetActive(false);
            });

            yield return new WaitForSeconds(fadeTime);
        }
    }
}