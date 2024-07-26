using Feature.Flip;
using UnityEngine;

namespace Feature.Player.Aim
{
    public class AimController : MonoBehaviour
    {
        [SerializeField] private Transform rightHand;

        [Header("Left Hand")]
        [SerializeField] private Transform leftHand;
        [SerializeField] private Vector2 leftHandMinMaxRotation;

        [Header("Head")]
        [SerializeField] private Transform headPivot;
        [SerializeField] private Vector2 minMaxRotationLimit;

        private Vector2 _lastAimDirection;
        private Vector2 _directionInput;
        private FlipController _flipController;

        private void Update()
        {
            Aim();
        }

        private void Aim()
        {
            if (_directionInput.magnitude != 0)
                _lastAimDirection = _directionInput;

            var rot_z = Mathf.Atan2(_lastAimDirection.y, _lastAimDirection.x) * Mathf.Rad2Deg;
            rightHand.rotation = Quaternion.Euler(0, 0, rot_z);
            leftHand.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(rot_z, leftHandMinMaxRotation.x, leftHandMinMaxRotation.y));
            headPivot.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(rot_z, minMaxRotationLimit.x, minMaxRotationLimit.y));

            if (_flipController)
            {
                var dot = Vector2.Dot(rightHand.right, Vector2.right);

                if (dot < 0)
                    _flipController.Flip(-1);
                else
                    _flipController.Flip(1);

                // Fix rotation if player is flipped
                if (_flipController.IsFlipped)
                {
                    rightHand.rotation = Quaternion.Euler(0, 0, rot_z + 180);
                    //leftHand.localRotation = Quaternion.Euler(0, 0, rot_z + 180);
                    //headPivot.localRotation = Quaternion.Euler(0, 0, rot_z + 180);

                    rightHand.localScale = Vector3.one;
                    leftHand.localScale = Vector3.one;
                    headPivot.localScale = Vector3.one;
                }
            }
        }

        public void ResetAim()
        {
            _lastAimDirection = Vector2.right;
        }

        public void FeedDirectionInput(Vector2 input)
        {
            _directionInput = input;
        }

        public void InjectFlipController(FlipController controller)
        {
            _flipController = controller;
        }
    }
}