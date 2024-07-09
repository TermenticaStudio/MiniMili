using UnityEngine;

public static class ImpactCreator
{
    public static void CreateImpact(Collider2D collider, Vector2 position, Vector2 normal)
    {
        var surface = collider.gameObject.GetComponent<Surface>();

        if (surface == null)
            return;

        var id = Impacts.IMPACT_POOLING[surface.surfaceImpact];

        if (string.IsNullOrEmpty(id))
            throw new System.Exception("ImpactCreator:: CreateImpact:: id is null");

        var instance = PrefabPool.Instance.Get(id);
        instance.transform.SetPositionAndRotation(position, Quaternion.FromToRotation(Vector2.up, normal) * Quaternion.Euler(0, 0, Random.Range(-30, 30)));
    }
}