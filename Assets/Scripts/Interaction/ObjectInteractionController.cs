using UnityEngine;

namespace Glovebox.Interaction
{
    /// <summary>
    /// Enables direct touch/mouse raycast interaction with microgravity objects.
    /// Supports tap/click impulse pushing and smooth drag-and-throw mechanics in zero-gravity.
    /// </summary>
    public class ObjectInteractionController : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask interactableLayer = ~0;
        [SerializeField] private float impulseForce = 3.5f;
        [SerializeField] private float torqueForce = 2.0f;
        [SerializeField] private float dragSpeed = 10.0f;
        [SerializeField] private float maxThrowSpeed = 8.0f;

        private Camera _mainCamera;
        private Rigidbody _selectedRigidbody;
        private float _grabDistance;
        private Vector3 _dragVelocity;

        private Renderer _hoveredRenderer;
        private Color _originalColor;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            HandleHover();
            HandleDragInput();
        }

        private void FixedUpdate()
        {
            if (_selectedRigidbody != null)
            {
                Vector3 currentWorldPos = GetPointerWorldPosition(_grabDistance);
                Vector3 delta = currentWorldPos - _selectedRigidbody.position;
                _dragVelocity = delta / Time.fixedDeltaTime;
                _dragVelocity = Vector3.ClampMagnitude(_dragVelocity, maxThrowSpeed);

                #if UNITY_6000_0_OR_NEWER
                _selectedRigidbody.linearVelocity = Vector3.Lerp(_selectedRigidbody.linearVelocity, _dragVelocity, Time.fixedDeltaTime * dragSpeed);
                #else
                _selectedRigidbody.velocity = Vector3.Lerp(_selectedRigidbody.velocity, _dragVelocity, Time.fixedDeltaTime * dragSpeed);
                #endif
            }
        }

        private void HandleHover()
        {
            if (_selectedRigidbody != null) return;

            Ray ray = _mainCamera.ScreenPointToRay(GetPointerPosition());
            if (Physics.Raycast(ray, out RaycastHit hit, 50.0f, interactableLayer))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null && hit.collider.GetComponent<Rigidbody>() != null)
                {
                    if (_hoveredRenderer != rend)
                    {
                        ClearHover();
                        _hoveredRenderer = rend;
                        if (_hoveredRenderer.material.HasProperty("_Color"))
                        {
                            _originalColor = _hoveredRenderer.material.color;
                            _hoveredRenderer.material.color = Color.Lerp(_originalColor, Color.white, 0.35f);
                        }
                    }
                    return;
                }
            }

            ClearHover();
        }

        private void ClearHover()
        {
            if (_hoveredRenderer != null)
            {
                if (_hoveredRenderer.material.HasProperty("_Color"))
                {
                    _hoveredRenderer.material.color = _originalColor;
                }
                _hoveredRenderer = null;
            }
        }

        private void HandleDragInput()
        {
            if (Input.GetMouseButtonDown(0) && Input.touchCount <= 1)
            {
                Ray ray = _mainCamera.ScreenPointToRay(GetPointerPosition());
                if (Physics.Raycast(ray, out RaycastHit hit, 50.0f, interactableLayer))
                {
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic)
                    {
                        _selectedRigidbody = rb;
                        _grabDistance = Vector3.Distance(_mainCamera.transform.position, hit.point);

                        Vector3 pushDirection = (hit.point - _mainCamera.transform.position).normalized;
                        rb.AddForceAtPosition(pushDirection * impulseForce, hit.point, ForceMode.Impulse);
                        rb.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && _selectedRigidbody != null)
            {
                #if UNITY_6000_0_OR_NEWER
                _selectedRigidbody.linearVelocity = _dragVelocity;
                #else
                _selectedRigidbody.velocity = _dragVelocity;
                #endif
                _selectedRigidbody.AddTorque(Random.insideUnitSphere * torqueForce * 0.5f, ForceMode.Impulse);
                _selectedRigidbody = null;
            }
        }

        private Vector3 GetPointerPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
        }

        private Vector3 GetPointerWorldPosition(float distance)
        {
            Ray ray = _mainCamera.ScreenPointToRay(GetPointerPosition());
            return ray.GetPoint(distance);
        }
    }
}
