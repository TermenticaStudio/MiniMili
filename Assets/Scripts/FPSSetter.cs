using UnityEngine;

public class FPSSetter : MonoBehaviour
{
    [SerializeField] private int fps = 60;

    private void Start()
    {
        Application.targetFrameRate = fps;
    }
}