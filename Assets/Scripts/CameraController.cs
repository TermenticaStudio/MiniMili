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

        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnDisable()
    {
        if (PlayerSpawnHandler.Instance == null)
            return;

        PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    //}

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();
    //    PlayerSpawnHandler.Instance.OnSpawnPlayer -= OnSpawnPlayer;
    //}

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