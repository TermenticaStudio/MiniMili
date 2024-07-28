using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SceneBackgroundCameraSetter : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }
}