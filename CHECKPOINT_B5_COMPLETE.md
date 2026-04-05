# Milestone B.5 Checkpoint — Spatial Inventory Complete

Steps 2-12 implemented and committed.

## What was implemented

### Core inventory system
- **PlacedItem.cs** — updated to add `quantity` field and public mutable fields (`item`, `quantity`, `x`, `y`, `rotated`). Legacy read-only property aliases retained for compatibility.
- **BackpackGrid.cs** — fully rewritten as a quantity-aware spatial grid with:
  - `TryPlace(item, x, y, rotated, quantity)` returning `bool`
  - `TryMergeAmount` / `TryPlaceFirstFit` for automatic placement
  - `RemovePlaced`, `RemoveItemAt`, `RemoveAmount`
  - `CountItem`, `CanFitAmount`
  - `SimulateResize` / `ResizeWithItems` for backpack swap
  - `GetAllPlaced()` / `GetItemAt()`
  - Legacy `PlacedItems`, `GetAt`, `TryFindFirstFit` shims retained
- **PlayerInventory.cs** — migrated from flat `_backpackGrid` list to `BackpackGrid _spatialGrid`:
  - `SpatialGrid` property (replaces `Backpack` alias)
  - `starterBackpackItem` serialized field — auto-equips on Awake and sizes grid from item storage dims
  - `EquipBackpack(ItemDefinition)` — validates fit before resizing
  - `TryAddItem` — pocket merge → grid merge → pocket fill → grid place
  - `CanAddItem` — pocket capacity + `CanFitAmount` on grid
  - `CountItem` — pockets + grid
  - `RemoveItems` — pockets first, then grid
  - `BackpackGridCapacity` legacy property returns `Width * Height`
  - `Hotbar` alias retained; `Backpack` alias removed

### Craft-to-placement plumbing
- **PlayerCrafter.cs** — added `OnCraftedPlaceable` event; placeable crafts skip `CanAddItem` check and fire event instead of adding to inventory
- **SimplePlacementController.cs** — added `BeginPlacementWithItem(ItemDefinition)` for craft-triggered placement; `CancelPlacement` returns item to inventory when craft-triggered

### New components
- **WorldItemPickup.cs** (`Assets/_Project/Scripts/Interaction/`) — `IInteractable` that adds item to player inventory on E press; destroys self on pickup
- **InventoryPanel.cs** (`Assets/_Project/Scripts/UI/`) — full IMGUI inventory UI:
  - Tab key toggles open/close
  - Storage tab: spatial grid with drag-drop, pocket slots
  - Crafting tab: recipe list with ingredient counts and Craft buttons
  - Equipment panel: all equipment slots with drag-drop
  - Drag-drop supports rotation (R key), swap, and drop-to-world
  - Right-click context menu: Drop, Consume, Equip, Unequip
- **InventoryDebugPanel.cs** — removed `Backpack` slot rendering; added note label; kept Hotbar and crafting sections

---

## Scene wiring (Wilko)

### Player object (same GameObject as PlayerInventory, PlayerCrafter, etc.)

1. **Add `InventoryPanel` component** to the Player object.
   - Set `Inventory` → PlayerInventory component
   - Set `Crafter` → PlayerCrafter component
   - Set `Stats` → PlayerStats component
   - Set `Campfire Tracker` → CampfireProximityTracker component
   - Set `Placement Controller` → SimplePlacementController component
   - Set `Player Transform` → the Player transform
   - `World Drop Distance` default 1.5 is fine
   - `Interact Layer` → assign the "Resource" layer mask

2. **PlayerInventory `Starter Backpack Item` field** — point to the StarterSack ItemDefinition asset (create if not yet made, see below).

3. **Remove or disable `InventoryDebugPanel`** if using InventoryPanel — both toggle on Tab key, so only one should be active at a time. Keeping InventoryDebugPanel disabled by default is recommended.

### StarterSack ItemDefinition asset

Create at `Assets/_Project/Data/Items/StarterSack.asset`:
- Display Name: `Starter Sack`
- Category: `Material`
- Stackable: false, Max Stack: 1
- Grid Width: 1, Grid Height: 1
- Backpack Storage Width: **3**, Backpack Storage Height: **3**
- Compatible Equipment Slots: `[Backpack]`
- Is Placeable: false

### WorldItemPickup pickups

The `WorldItemPickup` component spawned by drop uses the `"Resource"` layer so `PlayerInteractor` raycasts will hit it. Ensure `PlayerInteractor`'s `interactLayer` mask includes the `"Resource"` layer.

### No prefab needed for WorldItemPickup

Dropped items are spawned as primitive spheres at runtime. No scene setup required beyond the layer mask.
