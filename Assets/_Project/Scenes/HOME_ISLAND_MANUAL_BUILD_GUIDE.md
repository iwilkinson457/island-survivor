# Home Island Manual Build Guide

Project: **Extraction: Dead Isles**  
Target scene: `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`

This guide is for **Wilko building the scene manually in the Unity Editor**.
It assumes:
- Unity version: **6000.4.1f1**
- URP project
- imported packs already present locally:
  - `Assets/Tropical Foliage Collection/Tropical Foliage Collection - HDRP/Assets/Prefabs`
  - `Assets/Tropical Island/Prefabs`
- little or no prior Terrain-tool experience

The goal is **not** to make a final art scene in one pass.
The goal is to create a **good first playable art/blockout scene** that:
- feels like a real island
- reads clearly in gameplay
- has a beach, path, camp clearing, stone area, and pond/water-access zone
- uses Terrain for the land shape
- uses the tropical packs to sell the environment

---

## 0. Before you start

### What you are building
You are building a **first-pass Home Island environment** with this rough structure:
- one main island made from Terrain
- a readable **beach landing area**
- a visible **main path** inland
- a usable **camp clearing**
- a rough **stone/mining area**
- a small **pond / water access area**
- broad rock, tree, and foliage placement

### What "good enough" means for this pass
Good enough means:
- the island shape exists
- terrain is sculpted and painted in broad zones
- rocks and vegetation are placed in sensible passes
- the player can visually understand where to go
- the scene looks like a believable prototype island

Good enough does **not** mean:
- perfect sculpting
- perfect material blending
- finished set dressing
- polished biome design
- every empty area filled

---

## 1. Create or open the target scene

### If the scene does not exist yet
1. Open the project in Unity.
2. In the **Project** window, go to:
   - `Assets/_Project/Scenes`
3. In the top menu, choose:
   - **File > New Scene**
4. Choose a basic/empty URP-compatible scene if Unity asks.
5. Save it immediately:
   - **File > Save As...**
   - Save as: `HomeIsland_ArtBlockout.unity`
   - Path: `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`

### If the scene already exists
1. In the **Project** window, open:
   - `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity`
2. Double-click it to load it.

### Save immediately again
After it opens, save once more so you know you are editing the right scene.

---

## 2. Basic Unity navigation for a Terrain beginner

If you have not used Terrain before, this bit matters.

### Scene view controls
In the **Scene** view:
- **Right mouse button + move mouse** = look around
- **WASD while holding right mouse** = fly around
- **Mouse wheel** = zoom in/out
- **Middle mouse drag** = pan
- **Alt + left mouse** = orbit around selection
- **F** = focus selected object

### Good habit while working
- keep switching between **high overview view** and **ground-level player view**
- do not build the whole scene from one zoom level

### Recommended first camera rhythm
Use these three views repeatedly:
1. **High overview**: see island silhouette
2. **Mid-height**: judge routes and spacing
3. **Ground-level**: judge actual gameplay readability

---

## 3. Create the Terrain object

### Create terrain
1. In the top menu, choose:
   - **GameObject > 3D Object > Terrain**
2. A Terrain object should appear in the scene and hierarchy.
3. In the **Hierarchy**, rename it to:
   - `Terrain_HomeIsland`

### Set beginner-friendly terrain size
With the Terrain selected, go to the **Inspector**.

Set the Terrain size to this first-pass default:
- **Width**: `300`
- **Length**: `300`
- **Height**: `60`

If there is a Terrain Settings section/button, use that to set the size.
If the current inspector layout is unclear, look for the Terrain component settings with width/length/height controls.

### Why these values
These numbers are a good starting point because they are:
- large enough to feel like a small island
- small enough to manage by hand
- tall enough for modest ridges/cliffs without going silly

### Beginner caution
Do **not** start with a giant terrain like 1000 x 1000.
That makes blockout harder, not better.

---

## 4. Understand the Terrain tools before sculpting

With the Terrain selected, the Inspector should show Terrain editing tools.

Depending on Unity version, the wording/icons may vary slightly, but you are looking for tools similar to:
- **Raise / Lower Terrain**
- **Set Height**
- **Smooth Height**
- **Paint Texture**
- **Paint Trees**
- **Paint Details**
- **Settings**

### The 3 sculpt tools you will use first
For this task, focus mainly on:
1. **Raise/Lower Terrain**
2. **Smooth Height**
3. **Set Height** (optional but helpful for flat areas)

### Brush basics
Terrain sculpting uses a brush with:
- **Brush Size** = how big the brush is
- **Opacity / Strength** = how strongly it changes the terrain

### Beginner defaults
Start broad and gentle:
- **Brush Size**: medium-large
- **Opacity / Strength**: low to medium

If you start too strong, the terrain gets ugly fast.

### Rule of thumb
- big brush for island shape
- medium brush for ridges and route shaping
- smaller brush only for local cleanup

Do **not** noodle tiny bumps everywhere.
That is the fastest way to make bad terrain.

---

## 5. Shape the island

This is the most important phase.

### Target landform
Build a single readable island with these zones:
- **south / south-east beach** = player-friendly shore area
- **treeline behind beach** = transition into jungle
- **main inland route** = obvious way to move inward
- **camp clearing** = flatter safe-ish inland zone
- **stone/mining area** = rougher, more rocky zone
- **pond/water access area** = small quiet water feature or water-adjacent pocket

### Step 1: make the basic island mass
Use **Raise/Lower Terrain** with a **large brush** and **low strength**.

Goal:
- broad island shape
- generally rounded / oval mass
- land highest near middle/back side
- lower edges near shoreline

Do this first:
1. Raise a broad central island mass.
2. Leave one side lower and flatter for the beach.
3. Keep the opposite side slightly higher/rougher.

### Step 2: define the beach side
Pick one side as the main beach. Recommended:
- **south** or **south-east** side

Shape it so that:
- the shoreline is wider and gentler there
- the terrain slopes up softly from beach to inland area
- this becomes the easiest, most welcoming part of the island

### Step 3: create inland rise
Behind the beach, sculpt a gradual rise inland.
This helps the island feel layered.

You want:
- low beach
- mid inland path zone
- slightly higher ridge / camp side / rocky side

### Step 4: create the camp clearing
Use either:
- **Smooth Height**, or
- **Set Height** with a modest flat area

Make one flatter inland zone for the camp.
Recommended placement:
- not right on the beach
- a little inland
- slightly elevated over the beach
- still easy to reach

Target size:
- big enough for campfire + some early structures later
- not huge

### Step 5: create stone/mining zone
Choose one side of the island to feel more rugged.
This should be rougher than the camp area.

Add:
- steeper shoulders
- rocky edges
- a more broken silhouette

Do **not** make it full mountain chaos.
Just enough to clearly read as the rough materials side.

### Step 6: create pond / water-access area
Create a small depression or lower basin inland or off to one side.
This can become:
- a pond
- a water collection area
- a wet access area

Keep it modest.
This is a supporting feature, not the main attraction.

### Step 7: smooth the ugly bits
Now use **Smooth Height**.
Do a cleanup pass over:
- accidental spikes
- jagged brush marks
- lumpy route areas
- weirdly pinched slopes

### Beginner caution
If the terrain starts looking messy:
- stop adding detail
- zoom out
- smooth the broad forms again

Broad readable shapes beat detailed ugly terrain every time.

---

## 6. Create the playable flow

Before painting textures, make sure the layout reads.

### Recommended flow
Use this rough route logic:
1. **Beach landing zone**
2. **Treeline transition**
3. **Main path inland**
4. **Camp clearing**
5. Optional route branch to:
   - stone area
   - pond area

### How to check readability
Go to ground level in Scene view and ask:
- if I spawned here, where would I naturally walk?
- can I tell where the camp zone is?
- do I have one main route instead of five weak routes?
- can I see open playable spaces versus dense visual filler?

If not, fix layout now before decorating more.

---

## 7. Terrain texture / layer plan

Now paint the terrain so zones read properly.

You asked for support for these surfaces:
- path
- beach
- camp clearing area
- stone/mining area
- pond/lake/water access area

### Important note
Exact terrain layer workflow depends on which terrain-compatible materials/textures are already available in the project.
You may need to create Terrain Layers from appropriate ground textures already in the repo.

### Recommended terrain layer palette
Create or assign a small, controlled set of Terrain Layers:
1. **Beach Sand**
2. **Packed Dirt Path**
3. **Grass / Tropical Ground**
4. **Camp Clearing Dirt**
5. **Stone / Rocky Ground**
6. **Wet Mud / Damp Ground**

If you do not have perfect matching textures, use the closest available equivalents.
The zone readability matters more than perfection in this pass.

### Recommended zone usage
#### 1. Beach Sand
Use for:
- shoreline
- beach landing area
- immediate coastal edge

Keep this mostly on the flatter beach side.

#### 2. Packed Dirt Path
Use for:
- the main route from beach inland
- any branch path to stone zone or pond

Keep paths readable but not highway-wide.

#### 3. Grass / Tropical Ground
Use for:
- most general inland surface
- areas around trees and vegetation
- the broad default ground layer

This will likely be your main base layer.

#### 4. Camp Clearing Dirt
Use for:
- flattened camp area
- places where foot traffic would wear ground down

Blend with surrounding grass so it feels used, not painted as a hard circle.

#### 5. Stone / Rocky Ground
Use for:
- mining/stone area
- rough slopes near cliff support pieces
- rocky shoulders and material-rich areas

Do not paint half the island with rock. Keep it local and meaningful.

#### 6. Wet Mud / Damp Ground
Use for:
- around the pond
- water-access pocket
- low, damp-looking areas

Keep this subtle.

---

## 8. How to paint terrain textures for a beginner

### Add terrain layers
With the Terrain selected:
1. Switch to the Terrain **Paint Texture** tool.
2. Add/create Terrain Layers as needed.
3. Assign the chosen textures/material sources.

### Recommended painting order
Do it in this order:
1. paint most of the island with **Grass / Tropical Ground**
2. paint **Beach Sand** on the beach side
3. paint **Packed Dirt Path** for main route
4. paint **Camp Clearing Dirt** in the camp area
5. paint **Stone / Rocky Ground** in the mining zone
6. paint **Wet Mud / Damp Ground** around pond/water area

### Beginner painting rule
Use a **soft brush** and paint in broad zones.
Avoid drawing razor-sharp material borders everywhere.

### Specific zone plan
#### Beach
- broad sand patch along coast
- taper sand inward toward grass
- keep it visually welcoming and open

#### Path
- one main path from beach to camp
- slight width variation is good
- avoid perfectly straight ribbon shapes

#### Camp clearing
- mostly dirt/packed ground
- some grass around edges
- flatter and cleaner than wild jungle ground

#### Stone/mining area
- patchy rocky ground
- blend some dirt/grass at borders
- should read harsher and less comfortable

#### Pond/water-access area
- damp darker ground around edges
- some grass outside that
- not too large

### Beginner caution
If texture painting looks messy, it usually means:
- too many terrain layers
- brush too small
- too much fussing

Use fewer layers, broader zones, softer transitions.

---

## 9. Use the imported tropical asset packs correctly

### Big rule
Use the prefab packs to **support** the terrain, not replace it.

That means:
- Terrain = island body
- prefabs = cliffs, trees, vegetation, accents, framing, readability

### Tropical Island pack: what it is best for
Use mainly for:
- cliff and rock support
- palms and larger trees
- broad tropical silhouettes
- ferns, bushes, large plants

### Tropical Foliage Collection: what it is best for
Use mainly for:
- understory density
- filler between bigger pieces
- treeline richness
- jungle edge support

### Beginner caution
Do not dump dozens of random prefabs everywhere just because they look nice.
Each placement pass should have a job.

---

## 10. Rock placement pass

Do rocks before heavy foliage.
Rocks define structure.

### Recommended rock prefabs to start with
From `Assets/Tropical Island/Prefabs`, begin with:
- `SM_cliffs_tropical_01`
- `SM_cliffs_tropical_02`
- `SM_cliffs_tropical_03`
- `SM_cliffs_tropical_04`
- `SM_cliff_island_01`
- `SM_cliff_island_02 (1)`

### Where to place rocks
#### Beach edges
- a few rock groups near the sides of the beach
- not blocking the main open landing area

#### Island outer edges
- stronger rock presence on non-beach sides
- helps sell rough shoreline and island weight

#### Stone/mining area
- denser rock support here
- this area should visibly read as where stone/materials would come from

#### Path framing
- occasional rocks to guide direction and break up empty space

#### Camp clearing perimeter
- a few rocks around edges only
- do not clutter the usable center

### Rock placement rules
- place in clusters, not equal spacing
- rotate and scale slightly for variation
- use fewer bigger shapes before adding many small ones
- always check from player height

### What not to do
- do not make a rock wall around the whole island
- do not place every rock prefab you can find
- do not block traversal accidentally

---

## 11. Tree placement pass

Trees create the broad visual rhythm.

### Recommended tree prefabs to start with
From `Assets/Tropical Island/Prefabs`:
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

### Where to place trees
#### Beach
- sparse palms
- keep good sightlines
- preserve open landing area

#### Treeline
- denser tree band behind beach
- this should feel like a transition into jungle

#### Inland route edges
- trees should frame the route, not block it

#### Camp clearing perimeter
- trees around outside edge of the clearing
- keep center more open

#### Pond area
- a few trees nearby, but do not fully hide the water feature

### Tree placement rules
- think in groups and edges, not perfect spacing
- leave breathing room between major trunks
- do not cover every open patch with a tree

---

## 12. Bush / fern / foliage placement pass

This is where the scene starts to feel alive.
But it is also where readability can get wrecked.

### Start with Tropical Island understory
Good candidates:
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

### Then enrich with Tropical Foliage Collection
Use `Tropical_Plant_*` prefabs as support filler.

### Best places for dense foliage
- treeline band
- outside edges of path
- around rocks
- around tree bases
- edges of pond area
- transitions between major zones

### Places to keep lighter
- center of beach
- center of main path
- middle of camp clearing
- any area that needs obvious gameplay space

### Beginner caution
If a zone feels cool in screenshot view but confusing at player height, it is too dense.
Gameplay wins.

---

## 13. Suggested broad placement plan by area

### A. Beach landing zone
**Terrain:** sand  
**Props:** sparse palms, a few low bushes, a few rock accents  
**Readability goal:** clear and welcoming

Do:
- keep this open
- leave room for player movement and orientation
- use only enough vegetation to stop it feeling empty

Do not:
- make a jungle wall on the beach
- fill the whole beach with props

### B. Main path inland
**Terrain:** packed dirt path over grass  
**Props:** side rocks, bushes, some trees framing the route  
**Readability goal:** player naturally follows it

Do:
- keep path visible from beach
- curve it slightly for natural feel
- use vegetation to support the route

Do not:
- make the path vanish under foliage
- create too many equally strong path branches in first pass

### C. Camp clearing
**Terrain:** flatter dirt/grass mix  
**Props:** perimeter trees, a few rock anchors, lighter foliage  
**Readability goal:** obvious future home-base space

Do:
- keep usable middle space
- make it feel protected but not cramped

Do not:
- fill the center with trees or giant plants
- over-detail it yet

### D. Stone/mining area
**Terrain:** rocky ground + rougher slopes  
**Props:** cliff pieces, larger rocks, sparser but harsher vegetation  
**Readability goal:** obviously a rough resource zone

Do:
- make this feel less comfortable than camp
- use rock clustering and rough ground

Do not:
- turn it into an impossible mountain maze
- spam rock prefabs until movement is awkward

### E. Pond / water-access area
**Terrain:** damp/wet ground around low basin  
**Props:** limited trees, some reeds/foliage feeling, softer edges  
**Readability goal:** calm secondary feature

Do:
- keep it modest
- make it easy to identify from nearby ground

Do not:
- make it the whole scene’s focus
- hide it too deeply in clutter

---

## 14. How to keep the scene readable for gameplay

This matters a lot.

### Readability rules
1. **One clear primary route** from beach inland.
2. **One obvious safe-ish clearing** for camp.
3. **One obviously rougher area** for stone/mining.
4. **Visual density at edges**, not everywhere.
5. **Major silhouettes first**, fine detail second.

### Ground-level checks
Regularly lower the Scene camera to player height and check:
- can I tell where I can walk?
- can I still see the path?
- is the camp clearing obvious?
- am I getting visually trapped by plant spam?
- do important areas feel different from each other?

### If the scene feels confusing
Usually fix it by:
- removing foliage
- widening spaces
- simplifying path shape
- reducing rock clutter
- strengthening terrain material contrast

Not by adding more stuff.

---

## 15. What not to overdo

### Do not overdo terrain detail
Bad first-pass habit:
- tiny bumps everywhere
- noisy ridges everywhere
- constant sculpt fussing

Better:
- broad forms
- readable slopes
- a few clear landmarks

### Do not overdo foliage density
Bad first-pass habit:
- filling every empty patch
- placing too many plant types in one area
- hiding routes and spaces

Better:
- leave breathing room
- cluster plants in masses
- let open ground exist

### Do not overdo rock count
Bad first-pass habit:
- trying to make every edge dramatic
- overbuilding cliffs

Better:
- use rocks to support composition and zone identity

### Do not overdo polish before layout works
If layout/readability is weak, more art will not save it.

---

## 16. Save and checkpoint advice

### Save often
Use:
- **File > Save**
frequently

### Recommended checkpoints
Save mentally in these phases:
1. after creating scene + terrain
2. after basic island sculpt
3. after terrain painting
4. after rock placement
5. after tree placement
6. after foliage pass

### Practical habit
After each major phase:
- save
- walk the scene from ground level
- decide what is still unclear
- fix only the biggest readability issue next

### If you make a mess
Do not panic and keep layering more edits on top.
Instead:
- save a copy if needed
- remove the worst clutter
- smooth the terrain back down
- simplify the area

---

## 17. Suggested first-pass working order

If you want the short version, do this exact order:

1. Open or create `HomeIsland_ArtBlockout.unity`
2. Create `Terrain_HomeIsland`
3. Set size to `300 x 300 x 60`
4. Sculpt broad island mass
5. Define beach side
6. Define inland rise
7. Flatten camp clearing
8. Rough in stone area
9. Rough in pond/water-access area
10. Smooth ugly terrain
11. Paint broad grass layer
12. Paint beach sand
13. Paint path
14. Paint camp clearing dirt
15. Paint stone/rock zone
16. Paint pond damp ground
17. Place major rocks/cliffs
18. Place major trees/palms
19. Place bushes/ferns/plants
20. Add foliage-pack density where needed
21. Check from player height
22. Remove clutter that harms readability
23. Save

---

## 18. Minimum done checklist

Stop the first pass when all of these are true:

- [ ] `Assets/_Project/Scenes/HomeIsland_ArtBlockout.unity` exists and saves cleanly
- [ ] Terrain exists and clearly forms an island
- [ ] Beach zone is obvious
- [ ] Main inland path is obvious
- [ ] Camp clearing is obvious and usable
- [ ] Stone/mining area is visibly rougher/rockier
- [ ] Pond/water-access area exists and is readable
- [ ] Terrain textures/layers broadly separate the zones
- [ ] Rocks are placed in broad structural groups
- [ ] Trees establish beach / treeline / inland rhythm
- [ ] Bushes and foliage add life without swallowing readability
- [ ] From player height, the scene is understandable
- [ ] You have resisted over-detailing the first pass

If all of those are true, the first pass is **done enough**.

---

## 19. Final advice

Do not chase perfection on pass one.
Make the island:
- readable
- believable
- usable
- easy to iterate

A clean, simple blockout with good terrain and sensible placement is far better than an overstuffed “pretty” scene that plays badly.
