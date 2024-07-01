using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [SerializeField] private bool resetOnPointerUp = true;

    public bool IsPointerDown { get; private set; }

    public override void OnPointerUp(PointerEventData eventData)
    {
        IsPointerDown = false;

        if (!resetOnPointerUp)
            return;

        base.OnPointerUp(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        IsPointerDown = true;
    }
}