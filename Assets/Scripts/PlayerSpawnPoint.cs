using Logic.Player;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private float playerCheckRadius = 2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - Vector3.up, transform.position + Vector3.up);
    }

    public bool PlayerInArea()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, playerCheckRadius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out var result))
                return true;
        }

        return false;
    }

    public bool IsPointInCameraSight()
    {
        var cam = Camera.main;

        var directionToObject = transform.position - cam.transform.position;

        if (Vector3.Angle(cam.transform.forward, directionToObject) > cam.fieldOfView)
            return false;

        return true;
    }
}