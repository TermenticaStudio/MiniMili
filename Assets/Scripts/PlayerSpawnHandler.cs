using Feature.Notifier;
using Logic.Player;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

internal class PlayerSpawnHandler
{
    [SerializeField] private float respawnTimer = 5;
    [SerializeField] private int respawnCount = 3;

    [Header("UI")]
    [SerializeField] private float displayRespawnDelay = 2;
    [SerializeField] private GameObject respawnUI;
    [SerializeField] private TextMeshProUGUI respawnTimerText;
   // public static PlayerSpawnHandler Instance;

    [SerializeField]
    private static PickupObject[] pickups;


    internal static void OnStart()
    {
        pickups = ResourceManager.PickupPrefabs;
       // Instance = this;
     //   HideRespawnTimer();

        //SpawnPlayer();
    }

    // [ClientRpc]
    /* public void SpawnPlayerRpc(NetworkIdentity identity)
     {
         if (!identity.isLocalPlayer)
             return;

         OnSpawnPlayer?.Invoke(identity.GetComponent<PlayerInfo>());
     }*/

    public void SpawnPlayer(PlayerInfo instance)
    {
   //     var instance = FindObjectOfType<PlayerInfo>();

      /*  if (instance == null)
            instance = Instantiate(player, GetStartPosition());*/

        instance.SetRespawnCount(respawnCount);
        instance.IsLocal = true;
      //  OnSpawnPlayer.Invoke(instance);
        NotifyManager.Instance.Notify(MessageTexts.GetMessageContent(MessageTexts.MessageType.Joined), instance.GetPlayerName());
    }
    [Server]
    internal static void SpawnPickup(Vector3 pos)
    {
        if(pickups == null)
        {
            pickups = ResourceManager.PickupPrefabs;
        }
        var go = UnityEngine.Object.Instantiate(pickups[Random.Range(0, pickups.Length)].gameObject, pos, Quaternion.identity);
        NetworkServer.Spawn(go);
    }
    public static void RequestForPlayerRespawn(PlayerInfo player)
    {
        RespawnPlayer(player);
    }

    private static void RespawnPlayer(PlayerInfo player)
    {
        //if (!player.isLocalPlayer)
        //    return;

        if (!player.CanRespawn())
            return;

       // StartCoroutine(RespawnPlayerCoroutine(player));
    }

    private IEnumerator RespawnPlayerCoroutine(PlayerInfo player)
    {
        yield return new WaitForSeconds(displayRespawnDelay);

       /* var currTime = respawnTimer;

        while (currTime > 0)
        {
            currTime -= Time.deltaTime;
            ShowRespawnTimer(currTime);
            yield return null;
        }

        HideRespawnTimer();

        var spawnPoint = GetStartPosition(player);
        player.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = Vector2.zero;

        player.UseRespawn();
*/
   //     GameEvents.OnLocalPlayerSpawn?.Invoke(player);
    }

    public Transform GetStartPosition(SpawnPoint[] spawnPoints, List<PlayerInfo> allPlayers, PlayerInfo player = null)
    {
/*        var spawnPoints = FindObjectsOfType<SpawnPoint>();

        var allPlayers = FindObjectsOfType<PlayerInfo>().ToList();
*/
        if (player != null)
            allPlayers.Remove(player);

        SpawnPoint farest = null;
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
        respawnTimerText.text = $"{Convert.ToInt32(time)}...";
        respawnUI.SetActive(true);
    }

    private void HideRespawnTimer()
    {
        respawnUI.SetActive(false);
    }
}