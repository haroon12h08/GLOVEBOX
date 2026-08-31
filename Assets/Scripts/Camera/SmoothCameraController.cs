using UnityEngine;

namespace Glovebox.CameraControl
{
    /// <summary>
    /// Provides smooth camera navigation including 2-finger touch gestures (orbit, pinch zoom, pan)
    /// and mouse/trackpad controls for desktop/Editor testing.
    /// Uses smooth damping for fluid, premium camera movement.
    /// </summary>
    public class SmoothCameraController : MonoBehaviour
    {
        [Header("Target & Positioning")]
        [SerializeField] private Vector3 pivotPoint = Vector3.zero;
        [SerializeField] private float distance = 13.0f;
        [SerializeField] private float minDistance = 3.0f;
        [SerializeField] private float maxDistance = 22.0f;

        [Header("Sensitivity Settings")]
        [SerializeField] private float orbitSensitivity = 0.3f;
        [SerializeField] private float zoomSensitivity = 0.5f;
        [SerializeField] private float panSensitivity = 0.015f;
        [SerializeField] private float smoothTime = 0.12f;

        [Header("Rotation Limits")]
        [SerializeField] private float minPitch = -75.0f;
        [SerializeField] private float maxPitch = 75.0f;

        // Internal target values
        private float _targetYaw = 0.0f;
        private float _targetPitch = 15.0f;
        private float _targetDistance = 13.0f;
        private Vector3 _targetPivot = Vector3.zero;

        // Current smoothed values
        private float _currentYaw;
        private float _currentPitch;
        private float _currentDistance;
        private Vector3 _currentPivot;

        // Velocity references for SmoothDamp
        private float _yawVelocity;
        private float _pitchVelocity;
        private float _distanceVelocity;
        private Vector3 _pivotVelocity;

        // Touch tracking
        private Vector2 _lastTouch0;
        private Vector2 _lastTouch1;
        private bool _isTwoFingerGesture;

        // Mouse tracking
        private Vector3 _lastMousePos;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            _targetYaw = _currentYaw = angles.y;
            _targetPitch = _currentPitch = angles.x;

            _targetDistance = _currentDistance = distance;
            _targetPivot = _currentPivot = pivotPoint;

            UpdateCameraPositionImmediately();
        }

        private void Update()
        {
            HandleTouchInput();
            HandleMouseInput();
            ApplySmoothMovement();
        }

        /// <summary>
        /// Handles 2-finger touch gestures for mobile/touchscreen devices.
        /// 2-finger drag -> Orbit camera
        /// 2-finger pinch -> Zoom in/out
        /// 2-finger move in sync -> Pan target pivot
        /// </summary>
        private void HandleTouchInput()
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                if (!_isTwoFingerGesture)
                {
                    _lastTouch0 = touch0.position;
                    _lastTouch1 = touch1.position;
                    _isTwoFingerGesture = true;
                    return;
                }

                Vector2 currentTouch0 = touch0.position;
                Vector2 currentTouch1 = touch1.position;

                // 1. Pinch Zoom calculation
                float previousDistance = Vector2.Distance(_lastTouch0, _lastTouch1);
                float currentDistance = Vector2.Distance(currentTouch0, currentTouch1);
                float deltaDistance = currentDistance - previousDistance;

                _targetDistance -= deltaDistance * zoomSensitivity * 0.05f;
                _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);

                // 2. Center movement for Orbit or Pan
                Vector2 prevCenter = (_lastTouch0 + _lastTouch1) * 0.5f;
                Vector2 currentCenter = (currentTouch0 + currentTouch1) * 0.5f;
                Vector2 deltaCenter = currentCenter - prevCenter;

                // Determine if fingers are moving in same direction (Pan) or relative (Orbit)
                Vector2 dir0 = touch0.deltaPosition;
                Vector2 dir1 = touch1.deltaPosition;
                float dot = Vector2.Dot(dir0.normalized, dir1.normalized);

                if (dot > 0.5f)
                {
                    // Parallel movement -> Pan camera target
                    Vector3 right = transform.right;
                    Vector3 up = transform.up;
                    _targetPivot -= (right * deltaCenter.x + up * deltaCenter.y) * panSensitivity;
                }
                else
                {
                    // Opposing/Rotational movement -> Orbit camera
                    _targetYaw += deltaCenter.x * orbitSensitivity;
                    _targetPitch -= deltaCenter.y * orbitSensitivity;
                    _targetPitch = Mathf.Clamp(_targetPitch, minPitch, maxPitch);
                }

                _lastTouch0 = currentTouch0;
                _lastTouch1 = currentTouch1;
            }
            else
            {
                _isTwoFingerGesture = false;
            }
        }

        /// <summary>
        /// Desktop & Trackpad mouse input fallback:
        /// Right-click drag -> Orbit
        /// Middle-click drag or Shift+Left-click drag -> Pan
        /// Scroll wheel -> Zoom
        /// </summary>
        private void HandleMouseInput()
        {
            // Skip mouse processing if touch is active
            if (Input.touchCount > 0) return;

            Vector3 mousePos = Input.mousePosition;

            // Scroll Wheel Zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                _targetDistance -= scroll * zoomSensitivity * 10.0f;
                _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);
            }

            // Right Click Drag -> Orbit
            if (Input.GetMouseButton(1))
            {
                Vector3 delta = mousePos - _lastMousePos;
                _targetYaw += delta.x * orbitSensitivity * 2.0f;
                _targetPitch -= delta.y * orbitSensitivity * 2.0f;
                _targetPitch = Mathf.Clamp(_targetPitch, minPitch, maxPitch);
            }
            // Middle Click or Shift + Left Click -> Pan
            else if (Input.GetMouseButton(2) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
            {
                Vector3 delta = mousePos - _lastMousePos;
                Vector3 right = transform.right;
                Vector3 up = transform.up;
                _targetPivot -= (right * delta.x + up * delta.y) * panSensitivity * 2.0f;
            }

            _lastMousePos = mousePos;
        }

        /// <summary>
        /// Smoothly interpolates yaw, pitch, distance, and pivot toward targets
        /// and applies to transform position & rotation.
        /// </summary>
        private void ApplySmoothMovement()
        {
            _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _targetYaw, ref _yawVelocity, smoothTime);
            _currentPitch = Mathf.SmoothDampAngle(_currentPitch, _targetPitch, ref _pitchVelocity, smoothTime);
            _currentDistance = Mathf.SmoothDamp(_currentDistance, _targetDistance, ref _distanceVelocity, smoothTime);
            _currentPivot = Vector3.SmoothDamp(_currentPivot, _targetPivot, ref _pivotVelocity, smoothTime);

            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0.0f);
            Vector3 position = _currentPivot - (rotation * Vector3.forward * _currentDistance);

            transform.rotation = rotation;
            transform.position = position;
        }

        private void UpdateCameraPositionImmediately()
        {
            Quaternion rotation = Quaternion.Euler(_targetPitch, _targetYaw, 0.0f);
            transform.rotation = rotation;
            transform.position = _targetPivot - (rotation * Vector3.forward * _targetDistance);
        }

        /// <summary>
        /// Public API to reset camera view to center.
        /// </summary>
        public void ResetView()
        {
            _targetPivot = Vector3.zero;
            _targetPitch = 15.0f;
            _targetYaw = 0.0f;
            _targetDistance = distance;
        }
    }
}
