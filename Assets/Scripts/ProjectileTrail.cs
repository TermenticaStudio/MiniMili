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

    public void StartTimer()
    {
        trailRenderer?.Clear();
        DOVirtual.Float(minTime, maxTime, duration, value =>
        {
            trailRenderer.time = value;
        });
    }
}