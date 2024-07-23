using UnityEngine;

[CreateAssetMenu(menuName = "Presets/Map Info")]
public class MapInfoSO : ScriptableObject, IEncryptedContent
{
    public void Setup(string title, Sprite preview, string sceneName)
    {
        Title = title;
        Preview = preview;
        SceneName = sceneName;
    }

    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public Sprite Preview { get; private set; }
    [field: SerializeField] public string SceneName { get; private set; }

    [field: SerializeField] public bool IsLocked { get; private set; }
    [field: SerializeField] public string Password { get; private set; }
}