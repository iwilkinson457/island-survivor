# Scene Setup Guide — Extraction: Dead Isles Milestone B

Open the project in **Unity 6000.4.1f1**.

This milestone does **not** generate scene files automatically. Wilko should assemble the greybox scene manually in the Editor.

---

## 1. Create the Home Island prototype scene

Create a new scene:
- `Assets/_Project/Scenes/HomeIsland_Prototype.unity`

Use the Milestone A player rig as the starting point.

### Required player components
On the Player root, ensure these are present:
- `CharacterController`
- `PlayerController`
- `PlayerStats`
- `PlayerInteractor`
- `WeaponHolder`
- `PlayerInventory`
- `PlayerCrafter`
- `CampfireProximityTracker`
- `SimplePlacementController`
- `InventoryDebugPanel`
- `HomeIslandDebugHUD`

Wire these references:
- `PlayerController.cameraTransform` = player camera
- `PlayerInteractor.cameraTransform` = player camera
- `SimplePlacementController.cameraTransform` = player camera
- `SimplePlacementController.inventory` = Player `PlayerInventory`
- `PlayerCrafter.inventory` = Player `PlayerInventory`
- `InventoryDebugPanel.inventory` = Player `PlayerInventory`
- `InventoryDebugPanel.crafter` = Player `PlayerCrafter`
- `InventoryDebugPanel.campfireTracker` = Player `CampfireProximityTracker`
- `InventoryDebugPanel.placementController` = Player `SimplePlacementController`
- `HomeIslandDebugHUD.playerController` = Player `PlayerController`
- `HomeIslandDebugHUD.playerStats` = Player `PlayerStats`
- `HomeIslandDebugHUD.campfireTracker` = Player `CampfireProximityTracker`
- `HomeIslandDebugHUD.placementController` = Player `SimplePlacementController`

Spawn the player on the shoreline.

---

## 2. Greybox the island slice

Build only a small safe slice:

### Shoreline
- large Plane for beach
- slightly raised inland cubes/terrain blocks behind it
- add a `ShorelineSpawnMarker` empty object near the beach spawn point

### Camp clearing
- 20–40m inland, make a flat clearing where the first campfire can be placed safely
- leave enough open ground for placement testing

### Resource distribution
Place gatherables in a sensible loop:
- beach edge: `Stone`
- scrub/grass patches: `Fibre`
- driftwood / stick clusters: `Sticks`
- optional inland stump/logs: `Wood` source stub if you want an extra resource node

### Controlled danger
Place only **1–2 zombie patrols** far enough away that early gathering is possible.
This milestone is not about constant combat pressure.

---

## 3. Create item ScriptableObjects

Create folder if needed:
- `Assets/_Project/ScriptableObjects/Items/`

Create these `ItemDefinition` assets via:
- **Create → Extraction Dead Isles → Items → Item Definition**

### Required items

#### 1. Sticks
- itemId: `sticks`
- displayName: `Sticks`
- category: `Resource`
- stackable: true
- maxStack: 20

#### 2. Stone
- itemId: `stone`
- displayName: `Stone`
- category: `Resource`
- stackable: true
- maxStack: 20

#### 3. Fibre
- itemId: `fibre`
- displayName: `Fibre`
- category: `Resource`
- stackable: true
- maxStack: 20

#### 4. Raw Fish
- itemId: `raw_fish`
- displayName: `Raw Fish`
- category: `Food`
- stackable: true
- maxStack: 10
- isCookable: true
- cookedResultItemId: `cooked_fish`
- hungerRestore: 5
- thirstRestore: 0

#### 5. Cooked Fish
- itemId: `cooked_fish`
- displayName: `Cooked Fish`
- category: `Food`
- stackable: true
- maxStack: 10
- hungerRestore: 20
- thirstRestore: 0

#### 6. Campfire Kit
- itemId: `campfire_kit`
- displayName: `Campfire Kit`
- category: `Placeable`
- stackable: true
- maxStack: 5
- isPlaceable: true
- placeablePrefabId: `campfire_runtime`

#### 7. Crude Spear
- itemId: `crude_spear`
- displayName: `Crude Spear`
- category: `Weapon`
- stackable: false
- maxStack: 1

---

## 4. Create recipe ScriptableObjects

Create folder:
- `Assets/_Project/ScriptableObjects/Recipes/`

Create assets via:
- **Create → Extraction Dead Isles → Crafting → Recipe**

### Recipe A — Campfire Kit
- recipeId: `campfire_kit_recipe`
- displayName: `Campfire Kit`
- outputItem: `Campfire Kit`
- outputAmount: 1
- requiresCampfire: false
- unlockedByDefault: true
- ingredients:
  - Sticks x3
  - Stone x4
  - Fibre x2

### Recipe B — Crude Spear
- recipeId: `crude_spear_recipe`
- displayName: `Crude Spear`
- outputItem: `Crude Spear`
- outputAmount: 1
- requiresCampfire: false
- unlockedByDefault: true
- ingredients:
  - Sticks x2
  - Stone x1
  - Fibre x1

Assign both recipes to `PlayerCrafter.defaultRecipes`.
Assign these on `SimplePlacementController`:
- `campfireKitItem = Campfire Kit`
- `rawCookInput = Raw Fish`
- `cookedFoodOutput = Cooked Fish`

---

## 5. Create gatherables

For each pickup/gather node, use primitive meshes.

### Stone node
- GameObject with visible primitive
- Collider
- `GatherableResource`
- `itemToGrant = Stone`
- `amount = 1`
- `usesRemaining = 1`
- promptOverride = `Gather Stone`

### Stick node
- `itemToGrant = Sticks`
- promptOverride = `Pick up Sticks`

### Fibre node
- `itemToGrant = Fibre`
- promptOverride = `Harvest Fibre`

### Raw fish pickup
Use a small placeholder mesh near shoreline or shallow water:
- Collider
- `FoodPickup`
- `foodItem = Raw Fish`
- `amount = 1`

Make sure all interactables are on the layer included by `PlayerInteractor.interactMask`.

---

## 6. Campfire behaviour

You do **not** need a prefab.
The runtime placement system creates the campfire stand-in automatically.

Optional upgrade:
If you want cooked fish to work immediately from the placed campfire, after first placement do this once in Play Mode for testing assumptions:
- inspect runtime campfire object
- ensure `CampfireStation` has:
  - `rawItem = Raw Fish`
  - `cookedItem = Cooked Fish`

If you want this wired before play, duplicate the runtime logic later into a prefab in a future cleanup pass.
For Milestone B, runtime greybox is acceptable.

---

## 7. Controls for this milestone

- `Tab` = toggle Inventory Debug Panel
- `1–6` = select hotbar slot
- `F` = begin placement for selected `Campfire Kit`
- `Left Click` = confirm placement while in placement mode
- `Right Click` = cancel placement mode
- `E` = interact / gather / use campfire

Crafting currently happens through the debug panel's **Craft** buttons.

---

## 8. Exact smoke-test loop

Run this exact loop:

1. Spawn on the shoreline
2. Gather:
   - Sticks x3+
   - Stone x4+
   - Fibre x2+
3. Open `Tab` debug inventory
4. Craft `Campfire Kit`
5. If needed, move `Campfire Kit` into a hotbar slot
6. Press `1–6` to select the hotbar slot with the campfire kit
7. Press `F` to enter placement mode
8. Place the campfire in the inland clearing
9. Gather one `Raw Fish`
10. Walk into campfire range
11. Interact with the campfire using `E` to cook
12. Consume the cooked food using your temporary debug/inventory flow or by manually validating inventory contents
13. Confirm hunger/thirst are draining slowly and the campfire loop stabilises the first camp
14. Confirm zombies exist but do not overwhelm the opening loop

---

## 9. Expected result

Milestone B is passing when Wilko can:
- land/spawn on Home Island
- gather basics
- craft a campfire kit
- place a campfire
- obtain raw food
- convert it into cooked food
- see a believable first-camp prototype loop forming
