using Feature.ContentPassword;
using Feature.SceneLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
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
        var map = GetSelectedMapInfo();

        ContentPasswordController.Instance.Pass(map, () =>
        {
            SceneController.Instance.LoadGameScene(map.Title, map.SceneName);
        });
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