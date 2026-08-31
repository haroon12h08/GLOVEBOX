using UnityEngine;

namespace Glovebox.Core
{
    /// <summary>
    /// Central configuration for the glovebox enclosure dimensions and appearance.
    /// These values are the single source of truth used by GloveboxBuilder.
    /// Change dimensions here and re-run Tools > Glovebox > Build Glovebox Scene.
    /// </summary>
    public static class GloveboxConfig
    {
        // ── Interior dimensions ──────────────────────────────────────────────
        // Width  = X axis  (left ↔ right)
        // Height = Y axis  (floor ↔ ceiling)
        // Depth  = Z axis  (front ↔ back)

        public const float InteriorWidth  = 6.0f;   // metres
        public const float InteriorHeight = 3.0f;   // metres
        public const float InteriorDepth  = 4.0f;   // metres

        // ── Wall thickness ───────────────────────────────────────────────────
        public const float WallThickness = 0.1f;

        // ── Camera ──────────────────────────────────────────────────────────
        // Placed in front of the glovebox, looking inward through the front panel.
        public const float CameraDistance = 6.5f;
        public const float CameraHeight   = 1.5f;   // slightly above centre

        // ── Colours (used at build time to assign materials) ─────────────────
        public static readonly Color WallColor       = new Color(0.82f, 0.86f, 0.90f); // light steel blue-grey
        public static readonly Color FloorColor      = new Color(0.70f, 0.74f, 0.78f); // slightly darker
        public static readonly Color FrontPanelColor = new Color(0.55f, 0.80f, 1.00f, 0.25f); // pale blue, transparent
    }
}
