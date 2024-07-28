using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Feature.SceneLoader
{
    [RequireComponent(typeof(SceneControllerView))]
    public class SceneController : Singleton<SceneController>
    {
        [SerializeField] private string gameBaseScene;

        private SceneControllerView _view;

        public event Action OnSceneLoaded;

        private void Start()
        {
            _view = GetComponent<SceneControllerView>();
            _view.Init();
        }

        public void LoadGameScene(string envSceneTitle, string envSceneName)
        {
            StartCoroutine(LoadGameSceneCoroutine(envSceneTitle, envSceneName));
        }

        private IEnumerator LoadGameSceneCoroutine(string envSceneTitle, string envSceneName)
        {
            _view.SetMapName(envSceneTitle);
            yield return _view.FadeIn();

            var baseSceneAsc = SceneManager.LoadSceneAsync(gameBaseScene);

            while (!baseSceneAsc.isDone)
                yield return null;

            var envSceneAsc = SceneManager.LoadSceneAsync(envSceneName, LoadSceneMode.Additive);

            while (!envSceneAsc.isDone)
                yield return null;

            OnSceneLoaded?.Invoke();

            yield return _view.FadeOut();
        }

        public void LoadScene(string title, string sceneName)
        {
            StartCoroutine(LoadSceneCoroutine(title, sceneName));
        }

        private IEnumerator LoadSceneCoroutine(string title, string sceneName)
        {
            _view.SetMapName(title);
            yield return _view.FadeIn();

            var baseSceneAsc = SceneManager.LoadSceneAsync(sceneName);

            while (!baseSceneAsc.isDone)
                yield return null;

            OnSceneLoaded?.Invoke();

            yield return _view.FadeOut();
        }
    }
}