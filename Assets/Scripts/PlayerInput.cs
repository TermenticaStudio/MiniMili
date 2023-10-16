using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private FixedJoystick movementJoystick;

    public Vector2 MovementJoystickDirection { get => movementJoystick.Direction; }
}