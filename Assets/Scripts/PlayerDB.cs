using UnityEngine;

[CreateAssetMenu(menuName ="Project/PlayerDB")]
public class PlayerDB : ScriptableObject
{
    public string Name;

    public static PlayerDB instance
    {
        get => Resources.Load<PlayerDB>("PlayerDB");
    }
}