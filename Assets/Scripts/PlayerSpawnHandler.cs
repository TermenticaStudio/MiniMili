using Mirror;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnHandler : MonoBehaviour
{
    [SerializeField] private PlayerInfo player;
    [SerializeField] private float respawnTimer = 5;

    [Header("UI")]
    [SerializeField] private GameObject respawnUI;
    [SerializeField] private TextMeshProUGUI respawnTimerText;

    public static PlayerSpawnHandler Instance;

    public event Action<PlayerInfo> OnSpawnPlayer;

    private void Awake()
    {
        Instance = this;
        HideRespawnTimer();
    }

    private void Start()
    {
        SpawnPlayer();
    }

    // [ClientRpc]
    public void SpawnPlayerRpc(NetworkIdentity identity)
    {
        if (!identity.isLocalPlayer)
            return;

        OnSpawnPlayer?.Invoke(identity.GetComponent<PlayerInfo>());
    }

    public void SpawnPlayer()
    {
        var existingInstance = FindObjectOfType<PlayerInfo>();

        if (existingInstance != null)
        {
            OnSpawnPlayer?.Invoke(existingInstance);
            return;
        }

        var instance = Instantiate(player, GetStartPosition());
        OnSpawnPlayer?.Invoke(instance);
    }

    public void RequestForPlayerRespawn(PlayerInfo player)
    {
        RespawnPlayer(player);
    }

    private void RespawnPlayer(PlayerInfo player)
    {
        //if (!player.isLocalPlayer)
        //    return;

        StartCoroutine(RespawnPlayerCoroutine(player));
    }

    private IEnumerator RespawnPlayerCoroutine(PlayerInfo player)
    {
        var currTime = respawnTimer;

        while (currTime > 0)
        {
            currTime -= Time.deltaTime;
            ShowRespawnTimer(currTime);
            yield return null;
        }

        HideRespawnTimer();

        var spawnPoint = GetStartPosition(player);
        player.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);

        OnSpawnPlayer?.Invoke(player);
    }

    public Transform GetStartPosition(PlayerInfo player = null)
    {
        var spawnPoints = FindObjectsOfType<NetworkStartPosition>();

        var allPlayers = FindObjectsOfType<PlayerInfo>().ToList();

        if (player != null)
            allPlayers.Remove(player);

        NetworkStartPosition farest = null;
        var longestDistance = 0f;

        foreach (var spawnPoint in spawnPoints)
        {
            if (allPlayers.Count == 0)
            {
                farest = spawnPoints[Random.Range(0, spawnPoints.Length)];
                break;
            }

            for (int i = 0; i < allPlayers.Count; i++)
            {
                var distance = Vector2.Distance(spawnPoint.transform.position, allPlayers[i].transform.position);

                if (distance > longestDistance)
                {
                    longestDistance = distance;
                    farest = spawnPoint;
                }
            }
        }

        if (farest == null)
            throw new Exception("No respawn point found!");

        return farest.transform;
    }

    private void ShowRespawnTimer(float time)
    {
        respawnTimerText.text = $"{Convert.ToInt32(time)}s";
        respawnUI.SetActive(true);
    }

    private void HideRespawnTimer()
    {
        respawnUI.SetActive(false);
    }
}