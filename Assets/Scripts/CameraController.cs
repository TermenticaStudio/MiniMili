using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        Instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();

        AudioManager.Instance.SetCamera(transform);
    }

    public void SetTarget(Transform t)
    {
        cam.LookAt = t;
        cam.Follow = t;

        if (t == null)
            cam.enabled = false;
        else
            cam.enabled = true;
    }
}