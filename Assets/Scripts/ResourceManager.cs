using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager
{

    private static PickupObject[] cachedPickupObjects;

    public static PickupObject[] PickupPrefabs
    {
        get
        {
            // Return cached objects if already loaded
            if (cachedPickupObjects != null)
            {
                return cachedPickupObjects;
            }

            List<PickupObject> pickupObjects = new List<PickupObject>();

            // Load all prefabs from the Resources folder
            var allPrefabs = LoadSpawnItemsDatabase();

            // Check if the prefab has a component that derives from PickupObject
            foreach (var prefab in allPrefabs.spawnObjects)
            {
                if (prefab != null)
                {
                    pickupObjects.Add(prefab);
                }
            }

            // Cache the results
            cachedPickupObjects = pickupObjects.ToArray();

            return cachedPickupObjects;
        }
    }
     private static SpawnItemsDB LoadSpawnItemsDatabase()
    {

        // Load the PrefabDatabase from the Resources folder
        var prefabDatabase = Resources.Load<SpawnItemsDB>("SpawnItemsDB");

        if (prefabDatabase == null)
        {
            Debug.LogError("PrefabDatabase.asset could not be loaded from Resources!");
            return null;
        }else{
            return prefabDatabase;
        }
    }

    public static void ClearCache()
    {
        cachedPickupObjects = null;
    }
}
