# GLOVEBOX — Architecture Overview

This document describes the intended responsibilities of each script folder. Keep each layer focused; avoid letting responsibilities bleed across boundaries.

---

## Scripts/Core

**Responsibility:** Application lifecycle and global state.

- Scene loading and transitions
- Global settings and configuration access
- Any singleton managers (e.g. `GameManager`, `SceneLoader`)
- Entry point for initialising other systems

Core has no knowledge of specific objects or interaction logic.

---

## Scripts/Physics

**Responsibility:** Microgravity simulation and physical forces.

- Apply near-zero gravity to objects
- Simulate drag and damping in a sealed enclosure
- Handle boundary collision responses
- Provide force/velocity utilities used by Interaction

Physics operates on Rigidbody data; it does not decide *which* objects are selected.

---

## Scripts/Interaction

**Responsibility:** How the user selects, grabs, and releases objects.

- Raycasting or proximity-based selection
- Grab / hold mechanics
- Momentum-based release calculations
- Input handling for interaction events

Interaction relies on Physics for force application; it does not define what an object *is*.

---

## Scripts/Objects

**Responsibility:** Per-object behaviour and identity.

- `InteractableObject` component (mass, material type, special properties)
- Object state (floating, held, colliding)
- Object-specific reactions (e.g. fragile, magnetic)

Objects are passive; they expose an API that Physics and Interaction drive.

---

## Scripts/UI

**Responsibility:** All heads-up display and menu logic.

- Object information overlays
- Grab/release feedback indicators
- Pause / settings menus
- Debug readouts during development

UI reads state; it does not modify physics or object data directly.

---

## Dependency Direction

```
Core
 └── Physics
      └── Interaction
           └── Objects
                └── (UI reads from all layers)
```

Dependencies flow downward only. Upper layers must not import from lower layers.
