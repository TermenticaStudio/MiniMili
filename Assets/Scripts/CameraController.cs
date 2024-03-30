using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetTarget(Transform t)
    {
        cam.LookAt = t;
        cam.Follow = t;
    }
}