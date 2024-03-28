using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraZoomController : MonoBehaviour
{
    public static CameraZoomController Instance;

    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetLensSize(float size)
    {
        cam.m_Lens.OrthographicSize = size;
    }
}
