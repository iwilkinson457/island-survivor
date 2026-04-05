# Home Island Art/Blockout Handoff

Project: **Extraction: Dead Isles**  
Target scene to create in Unity: `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`

## Intent
This pass is an **art/blockout** scene pass only.

Goals:
- terrain-created island silhouette
- readable beach landing zone
- clear treeline transition
- at least one inland route
- one plausible camp clearing
- broad rock placement from Tropical Island prefabs
- broad vegetation placement from Tropical Island + Tropical Foliage prefabs
- stylised semi-realistic atmosphere
- gameplay readability over density/polish

Non-goals for this pass:
- final biome polish
- full worldbuilding
- detailed prop storytelling everywhere
- replacing existing gameplay systems
- full terrain painting/vegetation polish pass

---

## Source asset folders confirmed
### Tropical foliage
- `Assets/Tropical Foliage Collection/Tropical Foliage Collection - HDRP/Assets/Prefabs`

### Tropical island prefabs
- `Assets/Tropical Island/Prefabs`

---

## Recommended scene layout
Use a **single medium-sized island** built from Unity Terrain, not a premade island mesh.

### Readable layout beats
1. **South / south-east beach landing zone**
   - broad, forgiving playable arrival area
   - sparse palms and low scrub only
   - good visibility for orientation

2. **Treeline transition band**
   - denser shrubs, ferns, and small palms just beyond the sand
   - enough occlusion to feel sheltered, but not maze-like

3. **Primary inland route**
   - one obvious route that curves uphill/inward from the beach
   - keep route width generous enough for early traversal and future encounter readability

4. **Camp clearing**
   - slightly elevated, flatter space inland
   - plausible first home-base vibe
   - close enough to beach to feel connected, far enough to feel protected

5. **Rock/cliff framing**
   - heavier cliff/rock presence on island edges and one side of the route
   - use rocks to shape readable borders and sightlines, not to form the whole island body

---

## Terrain setup recommendation
Create a fresh scene:
- `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`

### Terrain baseline
Suggested starting values:
- Terrain Width: **300**
- Terrain Length: **300**
- Terrain Height: **60**

### Sculpting target
Shape the terrain as:
- soft oval island mass
- lower, flatter beach on one side
- rising midground ridge behind beach
- one gentle inland route climbing toward camp clearing
- outer edge broken with a few steeper shoulders where rock prefabs can sell cliff character

### Terrain readability rules
- avoid noisy micro-bumps everywhere
- keep the beach intentionally simple
- keep route slope walkable and visually legible
- clearing should be one of the flattest zones in the scene

---

## Prefab palette recommendation
Use a restrained set so the blockout stays readable.

### Rock / cliff set from Tropical Island
Recommended first-pass pieces:
- `SM_cliffs_tropical_01`
- `SM_cliffs_tropical_02`
- `SM_cliffs_tropical_03`
- `SM_cliffs_tropical_04`
- `SM_cliff_island_01`
- `SM_cliff_island_02 (1)`

Use them for:
- shoreline edge breakup
- framing the inland route
- backing the camp clearing
- creating silhouette interest on non-beach sides

Do **not** use them as the actual island landmass.
They are support pieces around the terrain silhouette.

### Tree / canopy set from Tropical Island
Recommended first-pass pieces:
- `SM_palm_tree_tropical_01`
- `SM_palm_tree_tropical_02`
- `SM_palm_tree_tropical_03`
- `SM_palm_tree_tropical_04`
- `SM_palm_tree_tropical_05`
- `SM_palm_tree_short_01`
- `SM_palm_tree_short_02`
- `SM_tree_jungle_01`
- `SM_tree_jungle_02`
- `SM_tree_jungle_03`
- `SM_tree_jungle_04`
- `SM_tree_great_jungle`

Placement intent:
- beach = lighter, more spaced palms
- treeline = mixed palms + jungle trees
- inland = denser canopy but still path-readable
- camp clearing = trees around the perimeter, not in the usable center

### Mid/low vegetation from Tropical Island
Recommended support pieces:
- `SM_bush_jungle_01`
- `SM_fern_tropical_01`
- `SM_fern_tropical_02`
- `SM_fern_tropical_03`
- `SM_plant_tropical_01`
- `SM_plant_tropical_02`
- `SM_plant_tropical_03`
- `SM_plant_tropical_04`
- `SM_plant_tropical_05`
- `SM_plant_tropical_large_01`
- `SM_plant_tropical_large_02`
- `SM_plant_tropical_large_03`
- `SM_plant_tropical_large_04`
- `SM_plant_tropical_small_01`
- `SM_plant_tropical_small_02`
- `SM_grass_01`

### Density support from Tropical Foliage Collection
Use these mainly to enrich the treeline and inland edge zones:
- `Tropical_Plant_001_a/b`
- `Tropical_Plant_002_a/b`
- `Tropical_Plant_003_a/b`
- `Tropical_Plant_004_a/b`
- `Tropical_Plant_005_a/b`
- `Tropical_Plant_006_a`
- `Tropical_Plant_007_a/b`
- `Tropical_Plant_008_a`
- `Tropical_Plant_009_a`
- `Tropical_Plant_010_a/b`
- `Tropical_Plant_011_a/b`
- `Tropical_Plant_012_a/b`
- `Tropical_Plant_013_a/b`
- `Tropical_Plant_014_a/b`
- `Tropical_Plant_015_a/b`
- `Tropical_Plant_016_a/b`
- `Tropical_Plant_017_a/b/c/d`
- `Tropical_Plant_018_a/b`
- `Tropical_Plant_019_a/b`
- `Tropical_Plant_020_a/b`

Use the foliage pack as **supporting density**, not wall-to-wall clutter.

---

## Manual Unity editor steps Wilko should do
These are the parts that are realistically editor-driven.

### 1. Create the new scene
1. Open Unity 6000.4.1f1 project.
2. Create a new Basic URP scene.
3. Save as:
   - `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`

### 2. Create terrain island
1. GameObject -> 3D Object -> Terrain
2. Set terrain size approximately:
   - Width 300
   - Length 300
   - Height 60
3. Sculpt a soft island silhouette:
   - flatter beach on south/south-east side
   - rising inland spine behind beach
   - one readable inland path direction
   - flatter inland clearing for first camp
4. Keep the outer silhouette simple and readable first.

### 3. Lighting / atmosphere baseline
1. Keep a single Directional Light.
2. Aim for late-afternoon warm light rather than harsh noon.
3. Add light fog / atmospheric depth if needed through existing URP/global volume setup.
4. Bias the scene toward:
   - warm sand highlights
   - richer green shadow zones
   - readable silhouettes

### 4. Beach zone pass
1. Keep vegetation sparse.
2. Place a few short palms and low plants.
3. Use one or two broad rock groupings to stop the beach edge feeling empty.
4. Preserve open space for player readability.

### 5. Treeline pass
1. Build a clear density step-up from beach to jungle edge.
2. Use palms + bushes + ferns first.
3. Add foliage-pack plants as fillers between major pieces.
4. Keep openings where the main inland route should visually pull the player.

### 6. Inland route pass
1. Define one obvious route from beach to clearing.
2. Use rocks on one side and vegetation masses on the other to guide flow.
3. Avoid tight choke points unless intentionally readable.
4. Keep route broad enough for future traversal/combat.

### 7. Camp clearing pass
1. Flatten a practical clearing inland.
2. Ring it with trees, shrubs, and a few rock anchors.
3. Do not overfill the middle.
4. Make it feel like a future base spot.

### 8. Broad rock placement
Use cliff pieces mainly:
- on non-beach shoreline edges
- as backdrop shapes
- to frame route and clearing
- to create silhouette breaks

Keep them broad and intentional, not spammed.

### 9. Broad vegetation placement
1. First pass: major trees and palms.
2. Second pass: shrubs/ferns.
3. Third pass: foliage-pack density fill.
4. Stop before readability collapses.

### 10. Gameplay sanity check
Before finishing the pass:
- confirm beach is readable
- confirm route is obvious
- confirm camp clearing is navigable
- confirm vegetation is not swallowing the playable space
- confirm existing player/gameplay hooks are not unnecessarily broken

---

## Recommended object grouping in scene
Suggested top-level scene hierarchy:
- `Environment`
  - `Terrain_HomeIsland`
  - `RockClusters`
  - `Trees_Palms`
  - `Understory`
  - `CampClearing_Markers`
- `Lighting`
- `Gameplay`
- `Debug`

---

## What was completed directly in-repo
Completed in repo-side planning/handoff:
- verified the requested imported asset folders exist locally
- audited available prefab palette from both packs
- defined a restrained prefab shortlist for this scene pass
- defined the terrain-first composition plan and layout beats
- defined explicit Unity editor steps for Wilko to execute safely

Not completed directly in repo:
- actual Terrain sculpting
- painted/detail vegetation placement in the scene
- final saved `HomeIsland_ArtBlockout.unity` scene asset

Reason:
- Terrain sculpting and broad art placement are editor-authored Unity scene work and are not safe to fabricate via raw text editing without opening Unity.

---

## Handoff summary
This pass should produce:
- a terrain-shaped island, not a premade island mesh
- a readable beach -> treeline -> inland route -> camp clearing flow
- cliff/rock support from Tropical Island prefabs
- vegetation density from both packs
- a stylised semi-realistic Home Island blockout that stays gameplay-readable
