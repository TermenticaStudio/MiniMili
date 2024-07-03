using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private PrefabPool.ObjectToPool pool;

    public void RegisterPool(PrefabPool.ObjectToPool pool)
    {
        this.pool = pool;
    }

    public virtual void OnDisable()
    {
        Release();
    }

    public void Release()
    {
        pool.pool.Release(this);
    }
}