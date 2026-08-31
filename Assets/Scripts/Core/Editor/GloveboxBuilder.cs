using UnityEngine;
using UnityEditor;
using Glovebox.Core;

namespace Glovebox.Editor
{
    /// <summary>
    /// Menu: Tools > Glovebox > Build Glovebox Scene
    ///
    /// Creates (or re-creates) all glovebox GameObjects in the currently open scene.
    /// Running it a second time removes the old Glovebox root first, so the result
    /// is always clean and deterministic.
    ///
    /// The glovebox consists of six solid sides:
    ///   Floor, Ceiling, Left Wall, Right Wall, Back Wall, Front Panel (transparent).
    ///
    /// Every surface uses a BoxCollider so future objects can collide with it.
    /// The camera is repositioned to give a clear view of the interior.
    /// </summary>
    public static class GloveboxBuilder
    {
        private const string RootName = "Glovebox";

        // ── Menu entry ───────────────────────────────────────────────────────

        [MenuItem("Tools/Glovebox/Build Glovebox Scene")]
        public static void BuildGloveboxScene()
        {
            // Remove any previous build so this is idempotent.
            DestroyExistingGlovebox();

            // Create the root container.
            GameObject root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Build Glovebox");

            // Shorthand dimensions from config.
            float w = GloveboxConfig.InteriorWidth;
            float h = GloveboxConfig.InteriorHeight;
            float d = GloveboxConfig.InteriorDepth;
            float t = GloveboxConfig.WallThickness;

            // Half-extents of the interior (useful for positioning panels).
            float hw = w / 2f;
            float hh = h / 2f;
            float hd = d / 2f;

            // ── Build each surface ─────────────────────────────────────────

            // Floor — sits at y = 0, interior starts just above it.
            CreatePanel(root, "Floor",
                position  : new Vector3(0f, -(hh + t / 2f), 0f),
                scale     : new Vector3(w + t * 2f, t, d + t * 2f),
                color     : GloveboxConfig.FloorColor,
                opaque    : true);

            // Ceiling — mirrors the floor.
            CreatePanel(root, "Ceiling",
                position  : new Vector3(0f, hh + t / 2f, 0f),
                scale     : new Vector3(w + t * 2f, t, d + t * 2f),
                color     : GloveboxConfig.WallColor,
                opaque    : true);

            // Left wall.
            CreatePanel(root, "Wall Left",
                position  : new Vector3(-(hw + t / 2f), 0f, 0f),
                scale     : new Vector3(t, h + t * 2f, d + t * 2f),
                color     : GloveboxConfig.WallColor,
                opaque    : true);

            // Right wall.
            CreatePanel(root, "Wall Right",
                position  : new Vector3(hw + t / 2f, 0f, 0f),
                scale     : new Vector3(t, h + t * 2f, d + t * 2f),
                color     : GloveboxConfig.WallColor,
                opaque    : true);

            // Back wall (far end, positive Z).
            CreatePanel(root, "Wall Back",
                position  : new Vector3(0f, 0f, hd + t / 2f),
                scale     : new Vector3(w + t * 2f, h + t * 2f, t),
                color     : GloveboxConfig.WallColor,
                opaque    : true);

            // Front panel (facing the camera, negative Z) — transparent so the
            // interior is always visible. Still has a collider for boundary use.
            CreatePanel(root, "Front Panel",
                position  : new Vector3(0f, 0f, -(hd + t / 2f)),
                scale     : new Vector3(w + t * 2f, h + t * 2f, t),
                color     : GloveboxConfig.FrontPanelColor,
                opaque    : false);

            // ── Reposition the camera ──────────────────────────────────────
            PositionCamera();

            // ── Mark the scene dirty so Unity knows to save ───────────────
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("[GloveboxBuilder] Glovebox built successfully. " +
                      $"Interior: {w}m × {h}m × {d}m. " +
                      "Save the scene with Ctrl+S.");
        }

        // ── Validation: only enable the menu item when a scene is open ──────

        [MenuItem("Tools/Glovebox/Build Glovebox Scene", validate = true)]
        private static bool ValidateBuildGloveboxScene()
        {
            return !string.IsNullOrEmpty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        // ── Private helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Creates a single rectangular panel (cube primitive scaled to panel shape).
        /// Assigns an unlit-compatible material with the given colour.
        /// Keeps the BoxCollider that Unity adds automatically to cube primitives.
        /// </summary>
        private static void CreatePanel(
            GameObject parent,
            string     panelName,
            Vector3    position,
            Vector3    scale,
            Color      color,
            bool       opaque)
        {
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = panelName;
            panel.transform.SetParent(parent.transform, worldPositionStays: false);
            panel.transform.localPosition = position;
            panel.transform.localScale    = scale;

            // Assign a new material with the requested colour.
            Material mat = new Material(GetShader(opaque));
            mat.name = panelName + " Mat";
            mat.color = color;

            if (!opaque)
            {
                // Standard shader transparent mode setup.
                mat.SetFloat("_Mode", 3);                         // Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }

            panel.GetComponent<Renderer>().material = mat;

            // The BoxCollider is already present from CreatePrimitive — nothing extra needed.

            Undo.RegisterCreatedObjectUndo(panel, "Build Glovebox Panel");
        }

        /// <summary>Returns the appropriate built-in shader for the panel type.</summary>
        private static Shader GetShader(bool opaque)
        {
            // "Standard" works for both opaque and transparent (mode is set via properties).
            Shader s = Shader.Find("Standard");
            if (s == null)
            {
                // Fallback — should not happen in a standard Unity project.
                s = Shader.Find("Diffuse");
            }
            return s;
        }

        /// <summary>
        /// Repositions the Main Camera so it looks straight into the glovebox
        /// through the front panel. Tilts it slightly downward for a natural
        /// laboratory viewing angle.
        /// </summary>
        private static void PositionCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogWarning("[GloveboxBuilder] No Main Camera found. Camera was not repositioned.");
                return;
            }

            float hd = GloveboxConfig.InteriorDepth / 2f;

            // Place the camera in front of the front panel.
            mainCam.transform.position = new Vector3(
                0f,
                GloveboxConfig.CameraHeight,
                -(hd + GloveboxConfig.CameraDistance));

            // Look at the centre of the glovebox interior.
            mainCam.transform.LookAt(Vector3.zero);

            Undo.RecordObject(mainCam.transform, "Reposition Camera");
        }

        /// <summary>
        /// Removes the Glovebox root GameObject from the scene if it already exists.
        /// This makes the builder idempotent — safe to run multiple times.
        /// </summary>
        private static void DestroyExistingGlovebox()
        {
            GameObject existing = GameObject.Find(RootName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing);
                Debug.Log("[GloveboxBuilder] Removed existing Glovebox — rebuilding.");
            }
        }
    }
}
