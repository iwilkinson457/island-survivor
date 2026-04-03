# Scene Setup Guide — Extraction: Dead Isles Milestone A

Open the project in Unity 6000.4.1f1 before following these steps.

## 1. URP Pipeline Asset

1. In Project window, go to Assets/_Project/
2. Right-click → Create → Rendering → URP Asset (with Universal Renderer)
   - Name it: `URP_PipelineAsset`
   - This also creates `URP_PipelineAsset_Renderer`
3. Edit → Project Settings → Graphics
   - Set Default Render Pipeline to `URP_PipelineAsset`
4. Edit → Project Settings → Quality
   - For each quality level, set Render Pipeline Asset to `URP_PipelineAsset`

## 2. NavMesh Package

If not already imported:
- Window → Package Manager → Unity Registry → AI Navigation → Install

## 3. Bootstrap Scene

1. File → New Scene (Empty)
2. Save as: `Assets/_Project/Scenes/Bootstrap.unity`
3. No GameObjects needed — this scene is a placeholder for Milestone B+

## 4. PrototypeSandbox Scene (Milestone A greybox)

### 4a. Create the scene
1. File → New Scene (Basic URP)
2. Save as: `Assets/_Project/Scenes/PrototypeSandbox.unity`

### 4b. Ground plane
1. GameObject → 3D Object → Plane
2. Name: `Ground`
3. Scale: (5, 1, 5) — this makes it 50×50 units
4. Position: (0, 0, 0)
5. Layer: `Default` (or create a `Ground` layer)

### 4c. Obstacles (cover for greybox)
Create 6 cubes for cover:
- GameObject → 3D Object → Cube
- Name each: Obstacle_1 through Obstacle_6
- Suggested transforms:
  - Obstacle_1: Position (8, 1, 5), Scale (2, 2, 2)
  - Obstacle_2: Position (-7, 1, 8), Scale (2, 2, 3)
  - Obstacle_3: Position (3, 1, -10), Scale (3, 2, 2)
  - Obstacle_4: Position (-10, 1, -6), Scale (2, 2, 2)
  - Obstacle_5: Position (12, 1, -3), Scale (1, 2, 4)
  - Obstacle_6: Position (-4, 1, 12), Scale (4, 2, 1)

### 4d. NavMesh bake
1. Window → AI → Navigation (Bake)
2. Make sure Ground and Obstacles have Navigation Static checked
   - Select each → Inspector → top-right dropdown → Navigation Static ✓
3. Agents tab: confirm default agent radius 0.35, height 1.8, step height 0.4
4. Click Bake

### 4e. Directional Light
1. The default Directional Light from Basic URP scene is fine
2. Rotation: (50, -30, 0) — gives readable shadows for greybox

### 4f. Player Setup

1. Create empty GameObject → Name: `Player`
   - Position: (0, 0.9, -18)
   - Tag: `Player`
2. Add component: `CharacterController`
   - Height: 1.8, Radius: 0.35, Center Y: 0.9
3. Add component: `PlayerController` (Scripts/Player/PlayerController)
4. Add component: `PlayerStats`
5. Add component: `PlayerInteractor`

6. Create child GameObject under Player → Name: `CameraHolder`
   - Local Position: (0, 1.6, 0)
7. Create child of CameraHolder → Name: `MainCamera`
   - Add component: Camera (set Tag to MainCamera)
   - Add component: Audio Listener
   - Local Position: (0, 0, 0)

8. In `PlayerController` Inspector:
   - Camera Transform: drag `CameraHolder` into Camera Transform field
   - Ground Mask: set to `Default` layer (or your Ground layer)
   - Confirm all serialised values are visible

9. In `PlayerInteractor` Inspector:
   - Camera Transform: drag `CameraHolder`
   - Interact Mask: `Default` (everything for now)

10. Create child of CameraHolder → Name: `WeaponRoot`
    - Local Position: (0.3, -0.3, 0.5)
    - Add component: `WeaponHolder`
    - Add component: `MeleeWeapon`
      - Attack Origin: drag `WeaponRoot` itself into Attack Origin field
    - In `WeaponHolder` Inspector: drag `WeaponRoot` into Active Melee Weapon

### 4g. Zombie Setup (repeat x3)

For each zombie (name them Zombie_1, Zombie_2, Zombie_3):

1. Create empty GameObject → Name: `Zombie_1`
   - Suggested positions: (5, 0.9, 5), (-8, 0.9, 7), (0, 0.9, 10)
   - Tag: `Enemy` (create tag if needed)

2. Add component: `NavMeshAgent`
   - Radius: 0.35, Height: 1.8, Stopping Distance: 1.5, Speed: 1.5 (will be overridden by state machine)

3. Add component: `ZombieController`
4. Add component: `ZombieStateMachine`
5. Add component: `EnemySenses`

6. For `EnemySenses`:
   - Vision Block Mask: set to `Default` (so obstacles block vision)
   - Create child empty GameObject → Name: `EyePosition`
     - Local Position: (0, 1.6, 0.1)
     - Drag into `EnemySenses` Eye Position field

7. Create three child GameObjects for hit zones:

   **HitZone_Head:**
   - Add CapsuleCollider: Height 0.4, Radius 0.18
   - Local Position: (0, 1.7, 0)
   - Add component: `HitZoneCollider`
     - Zone: Head

   **HitZone_Torso:**
   - Add CapsuleCollider: Height 0.8, Radius 0.25
   - Local Position: (0, 1.1, 0)
   - Add component: `HitZoneCollider`
     - Zone: Torso

   **HitZone_Leg:**
   - Add CapsuleCollider: Height 0.7, Radius 0.22
   - Local Position: (0, 0.5, 0)
   - Add component: `HitZoneCollider`
     - Zone: Leg

8. Add a CapsuleCollider to the root Zombie object (not a child):
   - Height: 1.8, Radius: 0.35, Is Trigger: false
   - This is the main physics collider for NavMesh agent

### 4h. Debug Overlay

1. GameObject → UI → Canvas
   - Canvas Scaler: Scale With Screen Size, 1920×1080
2. Create empty child: `DebugOverlayObject`
   - Add component: `DebugOverlay`

### 4i. Final checks
- Ensure the Player GameObject has Tag: `Player`
- Ensure NavMesh is baked (blue overlay visible on ground/obstacles in Scene view)
- Hit Play and verify:
  - Player moves with WASD
  - Mouse look works
  - LMB swings (check Console for hit log)
  - RMB kickback fires
  - Zombies wander
  - Sprinting near a zombie triggers Suspicious state (check Console)

## 5. Controls Reference

| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look |
| Left Shift | Sprint (drains stamina) |
| Left Ctrl | Crouch |
| Space | Jump (costs stamina) |
| LMB | Light attack |
| RMB | Kickback / shove |
| E | Interact |

## 6. Git Setup

Create `.gitignore` at the project root (already created by scaffold).

Initial commit:
```
git add .
git commit -m "feat: Milestone A scaffold — FPS controller, zombie AI, melee combat"
git push -u origin main
```
