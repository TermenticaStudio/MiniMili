using UnityEngine;

public interface IInput
{
    public Vector2 MovementJoystickDirection { get; }

    public Vector2 AimJoystickDirection { get; }

    public bool IsShooting { get; }

    public bool IsReloading { get; }

    public bool IsSwitching { get; }

    public bool IsChangingZoom { get; }

    public bool IsReplacing { get; }

    public bool IsMeleeing { get; }

    public bool IsSwitchingThrowables { get; }

    public bool IsThrowing { get; }
}