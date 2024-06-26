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
        public GameObject Obj;
        internal ObjectPool<GameObject> pool;
    }

    private void Awake()
    {
        Instance = this;

        foreach (var poolObj in objects)
        {
            poolObj.pool = new ObjectPool<GameObject>(() =>
            {
                var next = Instantiate(poolObj.Obj, transform);
                next.SetActive(false);
                return next;

            }, (obj) =>
            {
                //obj.SetActive(true);
            }, (obj) =>
            {
                obj.SetActive(false);
            }, collectionCheck: false, defaultCapacity: 50);
        }
    }

    public GameObject Get(string id)
    {
        var pool = objects.SingleOrDefault(x => x.Id == id);

        if (pool == null)
            throw new Exception($"there is no pool with Id of {id}!");

        var obj = pool.pool.Get();
        return obj;
    }

    public void Return(string id, GameObject spawned)
    {
        var pool = objects.SingleOrDefault(x => x.Id == id);

        if (pool == null)
            throw new Exception($"there is no pool with Id of {id}!");

        pool.pool.Release(spawned);
    }
}