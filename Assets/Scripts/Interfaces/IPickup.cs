using UnityEngine;

public interface IPickup
{
    public void Pickup();

    public Vector2 GetPosition();
    void Init(int clipLeft, int ammoLeft);
}