using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraZoomController : MonoBehaviour
{
    public static CameraZoomController Instance;

    private CinemachineVirtualCamera cam;
    private CinemachineFramingTransposer framingTransposer;
    private Vector3 initOffset;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        initOffset = framingTransposer.m_TrackedObjectOffset;
    }

    public void SetLensSize(float size)
    {
        cam.m_Lens.OrthographicSize = size;
    }
}
