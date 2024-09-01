using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ProjectileTrail : MonoBehaviour
{
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private float duration;

    private TrailRenderer trailRenderer;

    private void OnEnable()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void StartTimer(float speed, float length)
    {
        trailRenderer?.Clear();
        var finalTime = length / speed;

        DOVirtual.Float(finalTime / 1.5f, finalTime, duration, value =>
        {
            trailRenderer.time = value;
        });
    }
}