using UnityEngine;

namespace Glovebox.Physics
{
    /// <summary>
    /// Configures a Rigidbody to behave as if it is floating in microgravity.
    ///
    /// How it works (MVP):
    ///   - Unity gravity is disabled on this Rigidbody (useGravity = false).
    ///   - Very small linear and angular drag is applied so the object eventually
    ///     comes to rest after being disturbed, mimicking the slight air resistance
    ///     present in a sealed glovebox — without implementing fluid simulation.
    ///   - The object is NOT kinematic: it remains a full physics participant and
    ///     will collide with glovebox walls and other Rigidbody objects normally.
    ///
    /// This component is intentionally minimal. Do not add grabbing, selection,
    /// state machines, or force management here — those belong in later steps.
    ///
    /// Usage:
    ///   1. Add this component to any GameObject that should float.
    ///   2. Make sure the GameObject also has a Rigidbody and a Collider.
    ///   3. Set Mass and Drag via Inspector if per-object tuning is needed.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MicrogravityObject : MonoBehaviour
    {
        // ── Inspector-exposed settings ────────────────────────────────────────

        [Header("Mass")]
        [Tooltip("Mass of the object in kilograms. 1 kg is a reasonable default for a small lab sample.")]
        [Min(0.001f)]
        public float Mass = 1f;

        [Header("Drag (simulates sealed-enclosure air resistance)")]
        [Tooltip("Linear drag coefficient. Small value so the object drifts slowly to rest after a push.")]
        [Range(0f, 2f)]
        public float LinearDrag = 0.05f;

        [Tooltip("Angular drag coefficient. Keeps slow tumbling from building up indefinitely.")]
        [Range(0f, 2f)]
        public float AngularDrag = 0.05f;

        // ── Private state ────────────────────────────────────────────────────

        private Rigidbody _rb;

        // ── Unity messages ───────────────────────────────────────────────────

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            ApplyMicrogravitySettings();
        }

        // ── Public API (used by future interaction / force systems) ───────────

        /// <summary>
        /// Re-applies microgravity settings. Call this if Inspector values are
        /// changed at runtime and need to take effect immediately.
        /// </summary>
        public void ApplyMicrogravitySettings()
        {
            _rb.useGravity    = false;           // No Earth-gravity pull
            _rb.isKinematic   = false;           // Full physics participation
            _rb.mass          = Mass;
            _rb.linearDamping = LinearDrag;      // Unity 6+ property name
            _rb.angularDamping = AngularDrag;    // Unity 6+ property name
        }

#if UNITY_EDITOR
        // Validate in the Editor so the Inspector shows a warning for bad values.
        private void OnValidate()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody>();
            if (_rb != null) ApplyMicrogravitySettings();
        }
#endif
    }
}
