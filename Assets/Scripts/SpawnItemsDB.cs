using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Project/SpawnItemsDB")]
public class SpawnItemsDB : ScriptableObject
{
    public PickupObject[] spawnObjects;
}
