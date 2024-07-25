using Feature.OverlapDetector;
using UnityEngine;

namespace Feature.Player.Stabilizer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StabilizerController : MonoBehaviour
    {
        [SerializeField] private float minAngle = -30;
        [SerializeField] private float maxAngle = 30;

        private Rigidbody2D _rigid;
        private OverlapDetectorController _groundDetector;
        private Vector2 _directionInput;
        private Quaternion _targetRotation;
        private bool _isGrounded;

        private void Start()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _rigid.angularVelocity = 0;

            CheckGrounded();

            if (!_isGrounded && _directionInput.magnitude != 0 && _directionInput.y > 0)
            {
                var angle = Mathf.Atan2(_directionInput.y, _directionInput.x) * Mathf.Rad2Deg;
                angle = Mathf.Clamp(angle, 90 + minAngle, 90 + maxAngle);
                _targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
            }
            else
            {
                _targetRotation = Quaternion.identity;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * 5f);
        }

        public void InjectGroundDetector(OverlapDetectorController detector)
        {
            _groundDetector = detector;
        }

        public void FeedDirectionInput(Vector2 input)
        {
            _directionInput = input;
        }

        private void CheckGrounded()
        {
            if (_groundDetector == null)
            {
                _isGrounded = true;
                return;
            }

            _isGrounded = _groundDetector.IsOverlapped;
        }
    }
}