using UnityEngine;

public class ObjectSpawnPoint : MonoBehaviour
{
    [SerializeField] private float areaRadius = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - Vector3.up / 2f, transform.position + Vector3.up / 2f);
    }

    public void DestroyItemsInArea()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, areaRadius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<PickupObject>(out var result))
            {
                Destroy(collider.gameObject);
            }
        }
    }
}