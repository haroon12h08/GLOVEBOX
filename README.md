# GLOVEBOX — Microgravity Glovebox Simulation

A Unity-based 3D simulation of a microgravity glovebox environment for future scientific experimentation.

---

## Project Overview

This project simulates the interior of a space-station glovebox where objects behave under microgravity conditions. The goal is to create an interactive, physically accurate environment that can support scientific experiment workflows.

---

## Roadmap

| Step | Goal |
|------|------|
| 1 | Build a 3D glovebox enclosure |
| 2 | Add floating objects inside the glovebox |
| 3 | Simulate microgravity physics |
| 4 | Add boundary collisions |
| 5 | Add object selection |
| 6 | Add grabbing mechanics |
| 7 | Add momentum-based release |
| 8 | Support multiple simultaneous objects |
| 9 | Add camera controls |
| 10 | Prepare for future scientific experiment integration |

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Bootstrap, game state, scene management
│   ├── Physics/        # Microgravity simulation, forces, collisions
│   ├── Interaction/    # Object selection, grabbing, releasing
│   ├── Objects/        # Individual object behaviours and definitions
│   └── UI/             # HUD, menus, overlays
├── Prefabs/            # Reusable Unity prefabs
├── Materials/          # Shaders and materials
├── Models/             # 3D mesh assets
├── ScriptableObjects/  # Data-driven configuration assets
└── Settings/           # Input, render pipeline, and other settings
```

---

## Status

| Step | Status |
|------|--------|
| 1 — Project structure | ✅ Complete |
| 2 — 3D glovebox environment | ✅ Complete |
| 3 — Floating objects | ⬜ Not started |
| 4 — Boundary collisions | ⬜ Not started |
| 5 — Object selection | ⬜ Not started |
| 6 — Grabbing | ⬜ Not started |
| 7 — Momentum release | ⬜ Not started |
| 8 — Multiple objects | ⬜ Not started |
| 9 — Camera controls | ⬜ Not started |
| 10 — Scientific experiment prep | ⬜ Not started |

## How to Build the Glovebox Scene (Step 2)

1. Open Unity Hub and open the **GLOVEBOX** project.
2. Open the scene: `Assets/Scenes/SampleScene.unity`.
3. In the Unity menu bar, click **Tools → Glovebox → Build Glovebox Scene**.
4. The glovebox enclosure will appear in the Scene view and Hierarchy.
5. Press **Ctrl+S** to save the scene.
6. Press **Play** to verify the camera view.

### Glovebox dimensions (adjustable in `GloveboxConfig.cs`)

| Dimension | Value |
|-----------|-------|
| Interior width | 4.0 m |
| Interior height | 2.5 m |
| Interior depth | 3.0 m |
| Wall thickness | 0.1 m |
