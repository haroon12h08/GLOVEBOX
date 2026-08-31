using UnityEngine;

namespace Glovebox.Core
{
    /// <summary>
    /// Central configuration for the glovebox enclosure dimensions and appearance.
    /// These values are the single source of truth used by GloveboxBuilder.
    /// </summary>
    public static class GloveboxConfig
    {
        // ── Interior dimensions ──────────────────────────────────────────────
        // Width  = X axis  (left ↔ right)
        // Height = Y axis  (floor ↔ ceiling)
        // Depth  = Z axis  (front ↔ back)

        public const float InteriorWidth  = 12.0f;  // metres (enlarged)
        public const float InteriorHeight = 6.0f;   // metres (enlarged)
        public const float InteriorDepth  = 8.0f;   // metres (enlarged)

        // ── Wall thickness ───────────────────────────────────────────────────
        public const float WallThickness = 0.15f;

        // ── Camera ──────────────────────────────────────────────────────────
        public const float CameraDistance = 13.0f;
        public const float CameraHeight   = 3.0f;   // slightly above centre

        // ── Colours (used at build time to assign materials) ─────────────────
        public static readonly Color WallColor       = new Color(0.82f, 0.86f, 0.90f); // light steel blue-grey
        public static readonly Color FloorColor      = new Color(0.70f, 0.74f, 0.78f); // slightly darker
        public static readonly Color FrontPanelColor = new Color(0.55f, 0.80f, 1.00f, 0.20f); // pale blue, transparent
    }
}
