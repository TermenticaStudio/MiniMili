using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private Transform target;
    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (target == null)
        {
            SearchForTarget();
            return;
        }

        if (cam.LookAt == null)
            SetTarget(target);
    }

    private void SearchForTarget()
    {
        foreach (var t in FindObjectsOfType<PlayerInfo>())
        {
            if (t.isLocalPlayer)
                target = t.transform;
        }
    }

    private void SetTarget(Transform t)
    {
        cam.LookAt = t;
        cam.Follow = t;
    }
}