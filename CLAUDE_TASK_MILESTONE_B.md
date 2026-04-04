# Milestone B — Home Island Early Loop

Project root:
`C:\Users\Ian\.openclaw\workspace\extraction-dead-isles`

Unity version:
- 6000.4.1f1

Git remote:
- https://github.com/iwilkinson457/island-survivor

Current branch:
- main

Base commit:
- 49c749c feat: Milestone A passing — combat sandbox verified

## Goal
Build the smallest practical vertical slice after the combat sandbox:
- player spawns on Home Island shoreline
- can gather a few basic resources
- can carry them in a minimal inventory
- can craft/place a campfire
- can cook a first food item stub
- can stabilise a first camp
- zombies exist only in a limited, controlled way

This is still greybox. Do not jump ahead into hotel intro, human AI, family house combat, or full save systems.

## Scope rules
- Keep namespaces tidy under `ExtractionDeadIsles.*`
- Reuse Milestone A architecture where sensible
- Add only the minimum systems required for this slice to feel like an early game loop
- Avoid overbuilding UI or data complexity
- Keep ScriptableObject data patterns sensible for later expansion
- Do not break Milestone A combat sandbox scene
- Leave clear stubs/hooks for later milestones where needed

## Assumptions to implement unless the repo already forces a better choice
- Inventory = 20-slot backpack + 6-slot hotbar
- Primitive crafting = available from a simple inventory crafting panel / debug UI layer
- Campfire placement = simple surface-valid preview placement, no full building grid yet
- Food source = pickup/gatherable items + optional placed food pickups; passive animals are NOT required in this milestone
- Survival pressure = hunger/thirst drain is very light, just enough to justify campfire/food loop
- Corpse cleanup from Milestone A = keep as-is for now unless a tiny non-disruptive fade helper is easy to add; do not spend milestone time polishing corpses

## Deliverables
Implement code, docs, and scene setup instructions so Wilko can assemble and test a playable Home Island early loop.

### Must produce
1. New scripts for items, inventory, crafting, simple building/placement, world gathering, simple cooking, and a lightweight HUD/debug layer
2. Update any existing scripts that need clean hooks to integrate Milestone B
3. A new `SCENE_SETUP_MILESTONE_B.md`
4. A new `README_MILESTONE_B.md`
5. Commit and push all work to `main`

## Folder/file targets
Use these folders:
- `Assets/_Project/Scripts/Items/`
- `Assets/_Project/Scripts/Inventory/`
- `Assets/_Project/Scripts/Crafting/`
- `Assets/_Project/Scripts/Building/`
- `Assets/_Project/Scripts/World/`
- `Assets/_Project/Scripts/UI/`
- `Assets/_Project/ScriptableObjects/`
- `Assets/_Project/Scenes/` (docs only; do not generate .unity scene files)

Do not create binary Unity assets. Create docs and C# only.

---

# Systems to implement

## 1. Item data foundation
Create the following scripts:

### `Assets/_Project/Scripts/Items/ItemCategory.cs`
Enum with:
- Resource
- Food
- Tool
- Weapon
- Placeable
- Consumable
- Material

### `Assets/_Project/Scripts/Items/ItemDefinition.cs`
ScriptableObject for item data with serialized fields:
- `string itemId`
- `string displayName`
- `ItemCategory category`
- `bool stackable`
- `int maxStack`
- `bool isCookable`
- `string cookedResultItemId`
- `bool isPlaceable`
- `string placeablePrefabId` (string hook only for now)
- `int hungerRestore`
- `int thirstRestore`
- `float gatherToolMultiplier` (default 1)

Include public getters and validation in `OnValidate()`:
- stackable false => maxStack forced to 1
- maxStack minimum 1

### `Assets/_Project/Scripts/Items/ItemStack.cs`
Serializable class:
- `ItemDefinition item`
- `int quantity`
Methods:
- `IsEmpty`
- `CanStackWith(ItemDefinition other)`
- `SpaceRemaining`
- `AddAmount(int amount)` returns leftover
- `RemoveAmount(int amount)` returns removed

## 2. Inventory foundation

### `Assets/_Project/Scripts/Inventory/InventorySlot.cs`
Serializable class wrapping an `ItemStack`
Methods/properties:
- `HasItem`
- `CanAccept(ItemDefinition item)`
- `TryAdd(ItemDefinition item, int amount)` returns leftover
- `Remove(int amount)`
- `Clear()`

### `Assets/_Project/Scripts/Inventory/PlayerInventory.cs`
MonoBehaviour on Player.
Serialized fields:
- `int backpackSize = 20`
- `int hotbarSize = 6`
Create arrays/lists of slots at runtime.
Methods:
- `bool TryAddItem(ItemDefinition item, int amount)`
- `bool HasItems(ItemDefinition item, int amount)`
- `bool RemoveItems(ItemDefinition item, int amount)`
- `ItemDefinition GetHotbarItem(int index)`
- `bool TryMoveToHotbar(int backpackIndex, int hotbarIndex)` simple swap/move helper
- `string GetInventoryDebugSummary()`
Events:
- `OnInventoryChanged`
Requirements:
- Fill existing stacks first, then empty slots
- No nested inventories, no equipment complexity

### `Assets/_Project/Scripts/UI/InventoryDebugPanel.cs`
Simple `OnGUI` debug panel toggled by `Tab`
Displays:
- backpack contents
- hotbar contents
- simple crafting recipe list if linked to crafter
- current selected placeable/tool info if available
This is debug/prototype UI, not polished game UI.

## 3. Crafting foundation

### `Assets/_Project/Scripts/Crafting/CraftingIngredient.cs`
Serializable pair:
- `ItemDefinition item`
- `int amount`

### `Assets/_Project/Scripts/Crafting/CraftingRecipe.cs`
ScriptableObject:
- `string recipeId`
- `string displayName`
- `ItemDefinition outputItem`
- `int outputAmount`
- `List<CraftingIngredient> ingredients`
- `bool requiresCampfire`
- `bool unlockedByDefault`

### `Assets/_Project/Scripts/Crafting/PlayerCrafter.cs`
MonoBehaviour on Player.
Serialized:
- `List<CraftingRecipe> defaultRecipes`
- `PlayerInventory inventory`
Methods:
- `bool CanCraft(CraftingRecipe recipe, bool nearCampfire)`
- `bool TryCraft(CraftingRecipe recipe, bool nearCampfire)`
- `IReadOnlyList<CraftingRecipe> GetRecipes()`
Requirements:
- remove ingredients only if output can be added
- if output add fails, do not consume ingredients
- `requiresCampfire` gate respected

## 4. World gathering and pickups

### `Assets/_Project/Scripts/World/GatherableResource.cs`
Implements `IInteractable`.
Serialized:
- `ItemDefinition itemToGrant`
- `int amount = 1`
- `int usesRemaining = 1`
- `string promptOverride`
- optional respawn flag and respawn time stub fields, but no respawn implementation required
On interact:
- attempts add to player inventory
- if success, decrements uses
- destroys self when depleted
- logs clear messages
Prompt examples:
- Pick up Sticks
- Gather Stone
- Harvest Fibre

### `Assets/_Project/Scripts/World/FoodPickup.cs`
Simple pickup variant for edible item pickups, same inventory add flow.

### `Assets/_Project/Scripts/World/ShorelineSpawnMarker.cs`
Tiny marker component only for editor clarity / future spawn systems.

## 5. Survival pressure and consumption
Update or add:

### Update `Assets/_Project/Scripts/Player/PlayerStats.cs`
Add very light hunger/thirst drain:
- hunger drain 0.4 per second
- thirst drain 0.7 per second
- clamp 0..100
- if hunger or thirst are at 0, take 2 damage per second after a short grace timer (5 sec)
- methods `RestoreHunger(int amount)`, `RestoreThirst(int amount)`
- method `Consume(ItemDefinition item)` that restores hunger/thirst if item category is Food/Consumable
Do not create complicated status systems.

### `Assets/_Project/Scripts/Items/QuickConsumeHelper.cs`
Static helper or small utility to consume an item from a hotbar slot if edible.
Keep it minimal.

## 6. Campfire placement and cooking loop

### `Assets/_Project/Scripts/Building/PlaceableType.cs`
Enum with at least:
- Campfire
- StorageCrate
- Workbench
- HutPiece

### `Assets/_Project/Scripts/Building/PlacementGhost.cs`
MonoBehaviour for a very simple placement preview helper.
Requirements:
- positions a ghost object at raycast hit point from camera
- aligns upright only (no surface normal rotation complexity)
- tracks valid/invalid placement based on slope + collision overlap tests
- exposes `IsValidPlacement`
No fancy visuals required; logic only.

### `Assets/_Project/Scripts/Building/SimplePlacementController.cs`
MonoBehaviour on Player.
Responsibilities:
- start placement for a campfire item
- raycast from camera to place on ground within max distance
- left click confirm if valid
- right click cancel
- consumes item from inventory only on successful placement
- spawns a primitive stand-in via runtime GameObject composition if no prefab exists
For this milestone, runtime-composed campfire is acceptable.

### `Assets/_Project/Scripts/Building/CampfireStation.cs`
MonoBehaviour implementing `IInteractable`.
Responsibilities:
- reports whether player is near a campfire
- accepts one simple cook action via interact or public method
- if player has a cookable raw item and recipe/output mapping exists, convert it to cooked result
- log the result clearly
- no timed progress bar needed; instant conversion is fine for prototype

### `Assets/_Project/Scripts/World/CampfireProximityTracker.cs`
MonoBehaviour on Player or campfire detector helper.
Tracks whether player is within campfire usage radius.
Used by `PlayerCrafter` for recipes that require campfire.

## 7. Basic tool/weapon loop hook

### `Assets/_Project/Scripts/World/StarterLoadoutBootstrap.cs`
Optional tiny helper to seed one or two safe starter items in testing if needed.
Use only if necessary and keep it disabled by default or clearly documented.

### Update `Assets/_Project/Scripts/Combat/WeaponHolder.cs`
Allow a crafted primitive melee tool/weapon to be logically represented later, but keep Milestone A behaviour intact.
Small clean hook only, do not overbuild weapon systems.

## 8. Home Island debug/prototype HUD support

### `Assets/_Project/Scripts/UI/HomeIslandDebugHUD.cs`
Simple `OnGUI` debug HUD showing:
- health
- hunger
- thirst
- nearby campfire true/false
- selected hotbar slot index
- short instructions:
  - Tab inventory
  - 1-6 select hotbar
  - Left click place when in placement mode
  - Right click cancel placement
  - E interact
This can replace or complement the existing DebugOverlay; do not create duplicate noisy overlays if avoidable.

### Update `Assets/_Project/Scripts/Utilities/DebugOverlay.cs`
If necessary, keep it useful but not redundant.

## 9. Scene assembly instructions
Create `SCENE_SETUP_MILESTONE_B.md` with precise instructions for assembling a new scene:
- `HomeIsland_Prototype`
- shoreline spawn point
- a simple beach area, treeline, and inland camp clearing built from primitive cubes/planes/terrain if easy
- gatherable nodes placed around the shoreline and near the camp area
- one or two controlled zombie patrols far enough away that early gathering is possible
- a place for the player to make the first campfire safely
- instructions for creating ScriptableObjects for at least these items:
  - Sticks
  - Stone
  - Fibre
  - RawMeat OR RawFish (choose one)
  - CookedMeat OR CookedFish (matching the raw item)
  - Campfire Kit
  - Crude Spear or Stone Axe (choose one sensible primitive craft)
- instructions for creating at least these recipes:
  - Campfire Kit recipe
  - Primitive weapon/tool recipe
  - Cooked food conversion if needed
- explain what components go on the Player for Milestone B:
  - PlayerInventory
  - PlayerCrafter
  - SimplePlacementController
  - HomeIslandDebugHUD / InventoryDebugPanel
- include the exact test loop:
  1. spawn on shore
  2. gather resources
  3. craft campfire kit
  4. place campfire
  5. gather raw food pickup
  6. cook it
  7. consume cooked food
  8. stabilise hunger/thirst and confirm first camp works

## 10. README for milestone
Create `README_MILESTONE_B.md` covering:
- what was built
- how it fits after Milestone A
- controls added in Milestone B
- what remains stubbed
- exact pass criteria for this milestone
- recommended next milestone focus after B

## 11. Code quality rules
- Use `[CreateAssetMenu]` on ScriptableObjects where appropriate
- Use `[RequireComponent]` where sensible
- Keep public API clean
- No giant god-class inventory/building manager
- No `FindObjectOfType` spam in Update loops
- Be defensive around null item definitions
- Ensure everything compiles in Unity 6000.4.1f1
- Do not remove or break Milestone A combat flow

## 12. Git
When done:
1. commit with message:
   `feat: Milestone B home island early loop scaffold`
2. push to main

If push needs pull first, reconcile cleanly.

## 13. Completion notification
When completely finished, run:
`openclaw system event --text "Milestone B scaffold complete — home island early loop ready for Roxy review" --mode now`
