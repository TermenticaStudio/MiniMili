using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [SerializeField] private bool resetOnPointerUp = true;
    [SerializeField] private float resetDuration = 0.5f;

    public bool IsPointerDown { get; private set; }

    public override void OnPointerUp(PointerEventData eventData)
    {
        IsPointerDown = false;

        if (!resetOnPointerUp)
            return;

        SetInput(Vector2.zero);

        DOVirtual.Vector3(GetHandlePosition(), Vector2.zero, resetDuration, (value) =>
        {
            SetHandlePosition(value);
        });
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        IsPointerDown = true;
    }
}