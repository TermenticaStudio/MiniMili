using UnityEngine;

public interface ISpawnable
{
    public string Id { get; set; }
    public GameObject Object { get; set; }
}