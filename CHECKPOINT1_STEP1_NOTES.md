# Checkpoint 1 — Step 1 Notes

## What was done

Implemented the inventory-domain foundation for Milestone B.5.

### New files

- `Assets/_Project/Scripts/Inventory/InventoryDomain.cs`
  Enum: `Equipment`, `PocketsHotbar`, `BackpackGrid`.
  Makes the three inventory domains explicit in the type system.

- `Assets/_Project/Scripts/Inventory/EquipmentSlotType.cs`
  Enum: `Weapon1`, `Weapon2`, `Head`, `Torso`, `Legs`, `Backpack`.
  Typed slot identifiers for the equipment panel.

- `Assets/_Project/Scripts/Inventory/EquipmentSlot.cs`
  Class for a single typed equipment slot.
  Holds one `ItemStack` (non-stacked, quantity = 1).
  `CanAccept` delegates to `ItemDefinition.IsCompatibleWithEquipmentSlot`.
  Methods: `TryEquip`, `Unequip`, `Clear`.

### Modified files

- `Assets/_Project/Scripts/Items/ItemDefinition.cs`
  Added three Inspector header groups:
  - **Equipment**: `compatibleEquipmentSlots` (`EquipmentSlotType[]`) — which slots this item can occupy.
  - **Backpack Storage**: `backpackStorageWidth`, `backpackStorageHeight` — capacity this item provides when equipped in the Backpack slot.
  - **BackpackGrid Footprint**: `gridWidth`, `gridHeight`, `rotatable` — spatial footprint for Step 2.
  Added `IsCompatibleWithEquipmentSlot(EquipmentSlotType)` method.
  Added public properties for all new fields.

- `Assets/_Project/Scripts/Inventory/PlayerInventory.cs`
  - Renamed backing field `_backpack` → `_backpackGrid` (domain-explicit).
  - Renamed backing field `_hotbar` → `_pocketsHotbar` (domain-explicit).
  - Renamed serialized field `backpackSize` → `backpackGridCapacity`.
  - Added `private EquipmentSlot[] _equipmentSlots` (one per `EquipmentSlotType` value).
  - Added domain-explicit properties: `BackpackGrid`, `PocketsHotbar`, `EquipmentSlots`.
  - Kept legacy-compatible aliases `Backpack` and `Hotbar` so `InventoryDebugPanel` compiles unchanged.
  - Added `GetEquipmentSlot(EquipmentSlotType)`, `TryEquipItem`, `UnequipItem`.
  - `GetInventoryDebugSummary` now includes equipment slot listing.

## What was NOT done (reserved for later steps)

- No spatial grid logic (Step 2)
- No rotation support (Step 3)
- No starter sack auto-equip (Step 5)
- No UI work (Step 6)
- No drag/drop (Step 7)
- No transfer/swap overhaul
- No crafting changes

## Build assumptions

Project is Unity. No assembly definition changes were needed.
`ItemDefinition` now references `ExtractionDeadIsles.Inventory` for `EquipmentSlotType`.
No circular dependencies: `EquipmentSlotType` is a plain enum with no upward references.
