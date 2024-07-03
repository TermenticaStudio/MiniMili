using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PrefabPool : MonoBehaviour
{
    public static PrefabPool Instance;

    [SerializeField] private List<ObjectToPool> objects = new();

    [Serializable]
    public class ObjectToPool
    {
        public string Id;
        public PoolObject Obj;
        internal ObjectPool<PoolObject> pool;
    }

    private void Awake()
    {
        Instance = this;

        foreach (var poolObj in objects)
        {
            poolObj.pool = new ObjectPool<PoolObject>(() =>
            {
                return OnCreate(poolObj);
            }, (obj) =>
            {
                OnGet(obj, poolObj);
            }, (obj) =>
            {
                OnRelease(obj);
            }, (obj) =>
            {
                OnDestroy(obj);
            }, collectionCheck: false, defaultCapacity: 50, maxSize: 50);
        }
    }

    private static void OnDestroy(PoolObject obj)
    {
        Destroy(obj.gameObject);
    }

    private void OnRelease(PoolObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnGet(PoolObject obj, ObjectToPool poolObj)
    {
        obj.RegisterPool(poolObj);
        obj.gameObject.SetActive(true);
    }

    private PoolObject OnCreate(ObjectToPool poolObj)
    {
        var next = Instantiate(poolObj.Obj, transform);
        next.gameObject.SetActive(true);
        return next;
    }

    public PoolObject Get(string id)
    {
        var pool = objects.SingleOrDefault(x => x.Id == id);

        if (pool == null)
            throw new Exception($"there is no pool with Id of {id}!");

        var obj = pool.pool.Get();

        return obj;
    }
}