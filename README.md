# 3D Isometric Player Controller Prototype

A Unity 3D Project prototype with Isometric camera view and player controller with a simple 3D Character model.
The player moves relative to a fixed isometric camera, turns toward movement,
and faces the cursor while the right mouse button is held.

## Features

- Camera-relative WASD movement and smooth character turning.
- Right-click cursor aiming.
- Shift to run; backward movement while aiming is limited to walking.
- Idle, walk, and run animation blending.
- Orthographic camera follow with smooth, bounded mouse-wheel zoom.

## Controls

| Input | Action |
| --- | --- |
| WASD | Move |
| Hold Shift | Run, except backward while aiming |
| Hold right mouse button | Face the cursor |
| Scroll up / down | Zoom in / out |

## Getting started

1. Open the Unity project through Unity Hub using the version recorded in
   `ProjectSettings/ProjectVersion.txt`.
2. Ensure the Input System package is installed and Active Input Handling is
   set to Input System Package (New) or Both.
3. Open the prototype scene, press Play, and click inside the Game view.

## Scene setup

- The `Player` root has a Character Controller and `IsometricPlayer` script.
- The character model is a child of Player, with an Animator and root motion off.
- The model faces the Player's local positive Z direction; apply any facing
  correction to the model child.
- The Animator uses a 1D Blend Tree driven by the float parameter `Speed`:
  Idle at 0, in-place Walk at 2, and in-place Run at 4.
- Main Camera has `IsometricCamera` with Target assigned to the Player root.
- Start with camera Orthographic Size 7, Min Zoom 3, Max Zoom 12,
  Zoom Sensitivity 1, and Zoom Smooth Time 0.15.

The camera retains a fixed angle and zooms around the player. Scroll sensitivity
may need adjustment for different mice and trackpads.

## Current limitations

Forward walk/run clips are reused for locomotion. Dedicated backward and strafe
animations are needed for accurate directional animation while aiming.
Cursor aiming uses a horizontal plane at the player's feet.

## Character credit

Character: Casual Male by manoeldarochadeoliveira on CGTrader.
Third-party character assets retain their own license; this README does not
grant redistribution rights.

## Author

Made by LiteDuck1986, this project is free to use, copy and do whatever you want with it.
