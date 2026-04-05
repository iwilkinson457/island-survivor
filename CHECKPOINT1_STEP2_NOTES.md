# Checkpoint 1 — Step 2 Notes

## What was done

Implemented the spatial backpack grid data model for Milestone B.5.

### New files

- `Assets/_Project/Scripts/Inventory/PlacedItem.cs`
  Class representing a single item occupying one or more cells in the spatial grid.
  Fields: `Item`, `X` (top-left col), `Y` (top-left row), `Rotated`.
  Derived properties: `EffectiveWidth`, `EffectiveHeight` (auto-swap when rotated).
  Constructor is `internal` — instances are created and owned by `BackpackGrid` only.

- `Assets/_Project/Scripts/Inventory/BackpackGrid.cs`
  Self-contained 2D spatial occupancy grid.
  Internal state: `PlacedItem[,] _cells` (null = empty) + `List<PlacedItem> _placedItems`.

  **Fit check:**
  - `CanPlace(item, col, row, rotated, exclude)` — bounds + overlap check.
    `exclude` parameter allows checking a move for an item already on the grid.

  **Placement (atomic):**
  - `TryPlace(item, col, row, rotated)` — runs fit check first; on failure returns null
    and leaves the grid unchanged (safe-reject guarantee).

  **Removal:**
  - `TryRemove(PlacedItem)` — erases all cells the item occupied; returns false if not found.

  **Move (atomic):**
  - `TryMove(PlacedItem, newCol, newRow, newRotated)` — fit check with exclude, then
    erase-old / write-new. On failure item stays at original position.

  **Query helpers:**
  - `GetAt(col, row)` — occupant at a cell or null.
  - `Find(ItemDefinition)` — first placed item matching a definition.
  - `TryFindFirstFit(item, rotated, out col, out row)` — left-to-right top-to-bottom scan.

  **Resize (backpack swap support):**
  - `TryResize(newWidth, newHeight)` — verifies all placed items fit in new bounds before
    committing. Returns false (no change) if overflow would occur.

  **Clear:**
  - `Clear()` — removes all items.

### Modified files

- `Assets/_Project/Scripts/Inventory/PlayerInventory.cs`
  - Added `[Header("Spatial Backpack Grid")]` Inspector fields:
    `backpackSpatialWidth` (default 3), `backpackSpatialHeight` (default 3).
  - Added `private BackpackGrid _spatialBackpack` field.
  - Initialised `_spatialBackpack` in `Awake` using the Inspector dimensions.
  - Exposed via `public BackpackGrid SpatialBackpack` property.
  - Added spatial backpack methods (all call `NotifyChanged` on mutation):
    - `TryPlaceInBackpack(item, col, row, rotated)`
    - `TryRemoveFromBackpack(PlacedItem)`
    - `TryMoveInBackpack(PlacedItem, newCol, newRow, newRotated)`
    - `TryAutoPlaceInBackpack(item, rotated)` — finds first fit, tries opposite rotation if item is rotatable
    - `CanFitInBackpack(item, rotated)` — fit check without placement
    - `TryResizeSpatialBackpack(newWidth, newHeight)` — safe resize for backpack equip/unequip

## What was NOT changed

- All existing flat-list backpack/hotbar/equipment logic is unchanged.
- `InventoryDebugPanel` and all other callers continue to compile.
- No UI work.
- No starter sack auto-equip.
- No drag/drop.
- No crafting changes.

## Design decisions

- `PlacedItem` is a reference type (class) so both the list and the cell array share the same
  object — mutations (X, Y, Rotated during TryMove) are immediately visible everywhere.
- `CanPlace` takes an optional `exclude` parameter rather than requiring callers to remove-then-check,
  keeping move operations atomic at the model level.
- The flat `_backpackGrid` list (legacy) and the new `_spatialBackpack` (spatial) coexist for now.
  The legacy list will be retired when the UI is rebuilt in Steps 6–7.
- `TryResize` fails-safe: no partial state if items would overflow.
- Rotation is purely a model concept at this step — UI binding comes in Step 7.

## Build assumptions

No new assembly definitions needed.
`BackpackGrid` and `PlacedItem` are plain C# classes (no MonoBehaviour / ScriptableObject).
`PlayerInventory` references both via the same `ExtractionDeadIsles.Inventory` namespace.
`out _` discard syntax requires C# 7 — supported by all modern Unity versions.
