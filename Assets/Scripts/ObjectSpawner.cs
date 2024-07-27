using QFSW.QC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private PickupObject[] items;
    [SerializeField] private float spawnItemDelay = 10;
    [SerializeField] private int spawnCount = 3;

    private ObjectSpawnPoint[] spawnPoints;
    public static ObjectSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnPoints = FindObjectsOfType<ObjectSpawnPoint>();
        InvokeRepeating(nameof(SpawnRandomItem), 0, spawnItemDelay);
    }

    public void Spawn(string id, Vector3? pos = null, Quaternion? rot = null)
    {
        var searchId = id.Replace(" ", null);
        var item = items.SingleOrDefault(x => x.name.Replace(" ", null) == searchId);
        Vector3 SpawnPos = Vector3.zero;
        Quaternion SpawnRot = Quaternion.identity;

        if (item == null)
        {
            Debug.Log($"Item with {id} didn't found to spawn!");
            return;
        }

        if (pos.HasValue)
            SpawnPos = pos.Value;

        if (rot.HasValue)
            SpawnRot = rot.Value;

        Instantiate(item, SpawnPos, SpawnRot, null);
    }

    [Command("List-Items")]
    public static void PrintAvailableItems()
    {
        Debug.Log("Available Items: ");
        foreach (var item in Instance.items)
        {
            Debug.Log(item.name.Replace(" ", null));
        }
    }

    [Command("Spawn")]
    public static void Spawn(string id)
    {
        if (Instance == null)
        {
            Debug.Log("Object spawner is not available.");
            return;
        }

        Instance.Spawn(id, PlayerSpawnHandler.Instance.LocalPlayer.transform.position);
    }

    private void SpawnRandomItem()
    {
        var availablePoints = new List<ObjectSpawnPoint>(spawnPoints);
        var randomSpawnPoints = new List<ObjectSpawnPoint>();
        var rand = new System.Random();

        for (int i = 0; i < Mathf.Clamp(spawnCount, 0, spawnPoints.Length); i++)
        {
            var point = availablePoints[rand.Next(availablePoints.Count)];

            availablePoints.Remove(point);
            randomSpawnPoints.Add(point);
        }

        foreach (var spawnPoint in randomSpawnPoints)
        {
            spawnPoint.DestroyItemsInArea();
            Spawn(items[rand.Next(items.Length)].name, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
}