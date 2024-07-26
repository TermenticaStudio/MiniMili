using QFSW.QC;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private PickupObject[] items;

    public static ObjectSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

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
}