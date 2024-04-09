using Cinemachine;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : NetworkBehaviour
{
    public static CameraController Instance;
    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
    }

    private void OnSpawnPlayer(PlayerInfo obj)
    {
        SetTarget(obj.transform);
    }

    public void SetTarget(Transform t)
    {
        cam.LookAt = t;
        cam.Follow = t;
    }
}