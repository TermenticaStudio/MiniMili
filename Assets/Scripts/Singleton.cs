using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(null);
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}