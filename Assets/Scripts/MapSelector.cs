using Feature.ContentPassword;
using Mirror;
using Nakama;
using Nakama.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [SerializeField, Scene] private string menuScene;
    [SerializeField, Scene] private string lobbyScene;
    [SerializeField] private MapInfoSO[] maps;
    [SerializeField] private MapPreview mapPreviewPrefab;
    [SerializeField] private Transform mapsHolder;
    [SerializeField] private Sprite randomMapSprite;
    [SerializeField] private Button startGameBtn;

    private List<MapPreview> currentPreviews = new();

    private void Start()
    {
        LoadMaps();

        startGameBtn.onClick.AddListener(LoadGame);
        MatchManager.Instance.onMatchJoin += onMatchJoin;
        MatchManager.Instance.onMatchLeave += onMatchLeave;
    }

    private void onMatchLeave()
    {
        SceneManager.LoadScene(menuScene);
    }

    private void onMatchJoin()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    private void LoadMaps()
    {
        foreach (Transform obj in mapsHolder.transform)
            Destroy(obj.gameObject);

        var randomMap = Instantiate(mapPreviewPrefab, mapsHolder.transform);

        var randomMapSO = ScriptableObject.CreateInstance<MapInfoSO>();
        randomMapSO.Setup("Random", randomMapSprite, null);

        randomMap.Init(randomMapSO, OnChangeMap);
        currentPreviews.Add(randomMap);
        randomMap.Select();

        foreach (var map in maps)
        {
            var instance = Instantiate(mapPreviewPrefab, mapsHolder);
            instance.Init(map, OnChangeMap);
            currentPreviews.Add(instance);
        }
    }

    private void OnChangeMap()
    {
        foreach (var item in currentPreviews)
            item.Deselect();
    }

    private void LoadGame()
    {
        MatchManager.Instance.JoinMatchAsync();
        /*var map = GetSelectedMapInfo();

        ContentPasswordController.Instance.Pass(map, () =>
        {
            SceneManager.LoadScene(map.SceneName);
        });*/
    }

    private MapInfoSO GetSelectedMapInfo()
    {
        foreach (var item in currentPreviews)
        {
            if (item.IsSelected && !string.IsNullOrEmpty(item.Info.SceneName))
                return item.Info;
        }

        if (maps.Length == 0)
            throw new System.Exception("No map is available to select!");

        var randMap = maps[Random.Range(0, maps.Length)];
        return randMap;
    }
}