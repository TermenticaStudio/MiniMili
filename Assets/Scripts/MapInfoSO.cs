using UnityEngine;

[CreateAssetMenu(menuName = "Presets/Map Info")]
public class MapInfoSO : ScriptableObject
{
    public MapInfoSO(string title, Sprite preview, string sceneName)
    {
        Title = title;
        Preview = preview;
        SceneName = sceneName;
    }

    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public Sprite Preview { get; private set; }
    [field: SerializeField] public string SceneName { get; private set; }
}