using Mirror;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    public static PrefabPool singleton;

    [Header("Settings")]
    public GameObject prefab;

    [Header("Debug")]
    public int currentCount;
    public Pool<GameObject> pool;

    void Start()
    {
        InitializePool();
        singleton = this;
    }

    void InitializePool()
    {
        pool = new Pool<GameObject>(CreateNew, 50);
    }

    GameObject CreateNew()
    {
        GameObject next = Instantiate(prefab, transform);
        next.name = $"{prefab.name}_pooled_{currentCount}";
        next.SetActive(false);
        currentCount++;
        return next;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject next = pool.Get();

        next.transform.position = position;
        next.transform.rotation = rotation;
        next.SetActive(true);
        return next;
    }

    public void Return(GameObject spawned)
    {
        spawned.SetActive(false);
        pool.Return(spawned);
    }
}
