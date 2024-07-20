using UnityEngine;

public class SinMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 maxMovement;

    private Vector3 initPos;

    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        var sinX = Mathf.Sin((Time.time + Random.value) * speed) * maxMovement.x;
        var sinY = Mathf.Sin((Time.time + Random.value) * speed) * maxMovement.y;

        transform.position = initPos + new Vector3(sinX, sinY, 0);
    }
}