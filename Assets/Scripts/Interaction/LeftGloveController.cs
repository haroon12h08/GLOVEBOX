using UnityEngine;

namespace Glovebox.Interaction
{
    /// <summary>
    /// Controls movement of the Left Glove prototype within the microgravity glovebox.
    /// Uses Rigidbody.MovePosition to ensure physics-based collisions and momentum transfer
    /// when pushing floating microgravity objects.
    /// Default Controls:
    ///   - J / L : Move Left (-X) / Right (+X)
    ///   - I / K : Move Forward (+Z) / Backward (-Z)
    ///   - U / O : Move Down (-Y) / Up (+Y)
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LeftGloveController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 1.5f;

        [Header("Boundary Limits (Glovebox Interior)")]
        [SerializeField] private Vector3 minBounds = new Vector3(-2.8f, -1.3f, -1.8f);
        [SerializeField] private Vector3 maxBounds = new Vector3(2.8f, 1.3f, 1.8f);

        private Rigidbody _rb;
        private Vector3 _inputDirection;

        public float MovementSpeed => movementSpeed;
        public Vector3 MinBounds => minBounds;
        public Vector3 MaxBounds => maxBounds;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.useGravity = false;
            }
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            MoveGlove();
        }

        private void ReadInput()
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            // X-axis: J (left / -X), L (right / +X)
            if (Input.GetKey(KeyCode.J)) x -= 1f;
            if (Input.GetKey(KeyCode.L)) x += 1f;

            // Y-axis: U (down / -Y), O (up / +Y)
            if (Input.GetKey(KeyCode.U)) y -= 1f;
            if (Input.GetKey(KeyCode.O)) y += 1f;

            // Z-axis: K (backward / -Z), I (forward / +Z)
            if (Input.GetKey(KeyCode.K)) z -= 1f;
            if (Input.GetKey(KeyCode.I)) z += 1f;

            _inputDirection = new Vector3(x, y, z).normalized;
        }

        private void MoveGlove()
        {
            if (_rb == null) return;

            Vector3 deltaPosition = _inputDirection * (movementSpeed * Time.fixedDeltaTime);
            Vector3 targetPosition = _rb.position + deltaPosition;

            // Clamp position within configured glovebox interior boundaries
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);

            _rb.MovePosition(targetPosition);
        }
    }
}
