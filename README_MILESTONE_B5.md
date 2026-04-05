# README — Milestone B.5: Spatial Inventory Foundation

## Purpose

Milestone B.5 replaces the current debug/prototype inventory with a proper gameplay-facing inventory system for Dead Isles.

This milestone establishes the real inventory foundation for the game, built around:
- fixed quick-access pockets / hotbar
- fixed equipment slots
- backpack-equipped storage capacity
- spatial backpack inventory
- multi-cell item sizes
- item rotation
- drag-and-drop placement inside the backpack grid
- storage and crafting tabs
- direct crafting-to-placement flow for placeables
- item context actions such as drop and consume

This is not a temporary UI cleanup. It is a foundational gameplay system.

---

## Design intent

Dead Isles should treat inventory as part of survival pressure, not just bookkeeping.

The inventory should create:
- physical carrying constraints
- meaningful packing decisions
- preparation tension before leaving camp
- value in better backpacks and equipment later
- a clear difference between equipped gear, quick access items, and carried storage

The backpack grid should feel like a real part of survival gameplay.

---

## Milestone B.5 pass condition

A player can:
1. press `Tab` to open a proper inventory UI
2. see player equipment on the right half of the screen
3. see Storage and Crafting tabs on the left half
4. use the bottom 6 pocket slots as the always-on hotbar
5. use an equipped backpack to access a spatial storage grid
6. drag items between pockets, backpack grid, and equipment slots
7. place multi-cell items into the backpack grid if they fit
8. rotate rotatable items while moving them
9. equip weapons, clothing/armour, and backpack items into valid equipment slots
10. right-click items to access valid item actions
11. drop items either from a context action or by dragging them outside the inventory panel
12. consume food/drink through item actions
13. craft items from the Crafting tab
14. automatically enter placement mode after crafting a placeable item
15. place crafted placeables with left click and cancel with right click
16. complete the full item loop with no item loss, duplication, false full states, or control conflicts

---

## Screen layout

## Inventory screen opens with `Tab`

### Right half — Player equipment panel
Fixed equipment slots:
- Weapon 1
- Weapon 2
- Head
- Torso
- Legs
- Backpack

This panel represents what the player is actively wearing or carrying as equipped gear.

### Left half — Tabbed content panel
Two tabs:
- Storage
- Crafting

---

## Storage tab

### Bottom row — Pockets / hotbar
- Always 6 fixed slots
- These are always the quick slots / hotbar
- These are simple slot-based slots, not part of the spatial grid
- No rotation in pocket slots
- These should remain visible and understandable as the player’s fast-access items

### Above pockets — Backpack storage grid
- Spatial 2D grid
- Grid size depends on equipped backpack
- Initial backpack is a **3 x 3 sack**
- Items may occupy multiple cells
- Items may be rotatable if flagged rotatable

This backpack grid is the primary carried-storage gameplay layer.

---

## Equipment slots

Fixed typed equipment slots:
- Weapon 1
- Weapon 2
- Head
- Torso
- Legs
- Backpack

### Equipment rules
- Weapon slots accept only weapon-compatible items
- Head accepts only head armour/clothing items
- Torso accepts only torso armour/clothing items
- Legs accepts only leg armour/clothing items
- Backpack accepts only backpack items

If a backpack is equipped, backpack storage capacity updates to match that backpack definition.

For Milestone B.5, the player starts with a starter sack equipped that provides **3 x 3** storage.

If a later backpack swap would cause overflow, the system should fail safely and reject the equip until the contents fit.

---

## Inventory domains

The inventory is divided into three distinct systems.

### 1. Equipment slots
Fixed typed slots for actively equipped gear.

### 2. Pocket slots
- 6 fixed hotbar slots
- simple slot-based storage
- intended for quick access items

### 3. Backpack storage grid
- 2D occupancy-based spatial grid
- variable size based on equipped backpack
- supports multi-cell items
- supports rotation

This separation should be explicit in the data model and UI.

---

## Spatial inventory rules

### Backpack grid
Each backpack defines:
- width
- height

Starter backpack:
- width: 3
- height: 3

### Item footprint
Items that can enter backpack storage should define at minimum:
- grid width
- grid height
- rotatable yes/no

Examples of intended support:
- sticks: 1x1
- fish: 1x2
- spear: 1x3
- med item: 2x2
- armour piece: 2x2 or 2x3

Exact footprints can be tuned later, but the system must support them now.

### Rotation
If an item is rotatable:
- it can rotate 90 degrees while being moved
- width/height swap
- fit checks update immediately

Recommended prototype control:
- press `R` while dragging to rotate

---

## Drag-and-drop behavior

The player should be able to drag and drop items between valid inventory areas.

### Supported moves
- pocket to pocket
- pocket to backpack
- backpack to pocket
- backpack to backpack
- storage to equipment
- equipment to storage

### Pocket behavior
Pocket slots support:
- move
- swap
- stack merge where valid

### Backpack behavior
Backpack grid supports:
- drag item to new grid position
- rotate during drag if item allows it
- place only if fit check succeeds
- otherwise item returns safely to origin

### Equipment behavior
Equipment slots support:
- move weapon to weapon slot
- move clothing/armour to correct body slot
- move backpack item to backpack slot
- unequip back to storage if room exists

### Invalid drops
If a target is incompatible or there is no space:
- reject placement
- return item to original source
- no item loss
- no duplication
- no hidden destruction

---

## Item context actions

Items should support contextual actions, not just movement.

### Required B.5 actions
- Drop
- Consume (food/drink)
- Equip
- Unequip

### Future actions to support later
- unload ammo from magazine
- split stack
- inspect
- repair
- combine
- reload
- quick move

The system should be designed so additional item actions can be added later without rewriting the inventory model.

### Right-click context menu
Right clicking an item should open a small context menu.

The menu should show only actions valid for that item in its current state.

Examples:
- any item: Drop
- food/drink: Consume
- equippable item in storage: Equip
- equipped item: Unequip

Clicking outside the menu closes it.

---

## Dropping items

Dropping is required for Milestone B.5.

### Drop methods
1. Right-click context menu → Drop
2. Drag item outside the inventory panel and release → Drop to world

### Drop rules
When dropping:
- remove item from inventory
- spawn world drop near player
- preserve quantity
- preserve item identity
- avoid duplication
- avoid silent deletion

For B.5, simplest acceptable rule:
- dropping a stack drops the whole stack

Later we can add split-stack/drop-one behaviors.

---

## Consuming items

Food and drink should be consumable from the inventory.

### Consume rules
If item is food or drink:
- right-click item
- choose Consume
- apply hunger/thirst effect
- remove one item from stack
- refresh inventory/UI state cleanly

If consumption cannot occur for any reason, fail safely with no item loss.

---

## Crafting tab

The Crafting tab should show craftable items in a clear grid or list.

Each craftable entry should clearly communicate:
- item name
- craftable or not craftable state
- missing ingredients if not craftable
- whether item is placeable

### Craft action
- Left click crafts immediately if requirements are met

---

## Crafting result behavior

### Non-placeable crafted items
After crafting:
- consume ingredients
- place crafted item into a valid destination

Recommended destination priority:
1. merge into existing valid stack
2. place into backpack if it fits
3. place into valid pocket if appropriate
4. fail safely if no room

### Placeable crafted items
After crafting:
- consume ingredients
- create or reserve crafted placeable item
- close inventory
- begin placement mode immediately
- left click places
- right click cancels

### On placement cancel
Preferred behavior:
- crafted placeable remains as an inventory item or returns to inventory if space exists
- placement cancel should not silently destroy the item

---

## Input and UI behavior

### When inventory is open
- cursor visible
- cursor unlocked
- mouse look disabled
- gameplay interaction suppressed
- placement input suppressed unless inventory intentionally closes into placement mode
- drag/drop works cleanly

### When inventory is closed
- cursor hidden
- cursor locked
- gameplay controls restored
- gameplay interaction restored

### Visual clarity requirements
The UI should clearly distinguish:
- equipment slots
- pocket/hotbar slots
- backpack grid
- storage tab
- crafting tab
- valid item drop targets
- invalid item drop targets
- dragged item preview
- valid / invalid grid fit feedback
- craftable / uncraftable recipe state
- selected hotbar slot

This does not need final art polish, but it must stop feeling like a debug panel.

---

## Data model requirements

Each item definition should support at minimum:
- display name
- stackable yes/no
- max stack
- general category
- equipment compatibility if equippable
- placeable yes/no
- grid width
- grid height
- rotatable yes/no
- quick-slot allowed yes/no
- backpack-storage allowed yes/no
- consumable yes/no or equivalent behavior flags

The model should clearly separate:
- item category
- valid equipment slot(s)
- inventory footprint
- behavior flags

Avoid baking all behavior into one brittle enum.

---

## Backpack requirements

Backpack items should define:
- storage width
- storage height
- compatibility with backpack equipment slot

### Starter state
Player begins with:
- starter sack equipped in Backpack slot
- starter sack providing **3 x 3** storage

The system should be extensible for larger backpacks later.

---

## Scope boundaries for B.5

### In scope
- fixed equipment slots
- fixed 6-slot pockets / hotbar
- backpack equipment slot
- starter 3x3 sack
- spatial backpack inventory
- multi-cell items
- rotation support
- drag-and-drop between storage zones
- storage and crafting tabs
- item context actions
- dropping items
- consuming food/drink
- direct craft-to-placement flow
- reliable input state handling

### Out of scope for now
- final visual polish
- advanced tooltips
- durability systems
- weight / encumbrance
- advanced loot container UIs
- save/load persistence unless needed to avoid architecture rework
- magazine unload, reload, split stack, repair, inspect, and other advanced actions beyond the foundational hooks

---

## Recommended implementation order for Roxy

### Step 1 — Refactor inventory architecture
- separate equipment, pockets, and backpack grid into explicit systems

### Step 2 — Add spatial backpack grid model
- occupancy map
- footprint placement
- fit tests
- place/remove operations

### Step 3 — Add rotation support
- width/height swap
- rotate while dragging
- live fit validation

### Step 4 — Add equipment slots
- weapon 1
- weapon 2
- head
- torso
- legs
- backpack

### Step 5 — Add starter backpack item
- starter sack
- auto-equipped
- 3x3 storage

### Step 6 — Replace debug inventory UI
- right equipment panel
- left storage/crafting tabs
- bottom 6 pocket slots
- backpack grid above pockets

### Step 7 — Add drag-and-drop behavior
- pockets
- backpack grid
- equipment slots
- swap / merge / reject logic

### Step 8 — Add right-click context actions
- Drop
- Consume
- Equip
- Unequip

### Step 9 — Add drag-outside-panel to drop
- release outside valid UI target spawns world drop

### Step 10 — Add crafting tab integration
- recipe display
- craftability state
- recipe click to craft

### Step 11 — Add placeable crafting integration
- close inventory after crafting placeable
- begin placement immediately

### Step 12 — Smoke test and hardening
- verify full item loop works cleanly

---

## Required smoke test for pass

Milestone B.5 passes only if all of the following work:

1. player picks up several items of different types
2. player opens inventory with `Tab`
3. player sees equipment panel on the right
4. player sees Storage and Crafting tabs on the left
5. player sees 6 fixed pocket slots at the bottom
6. player sees starter 3x3 backpack grid
7. player drags items between pockets and backpack
8. player places at least one multi-cell item into backpack
9. player rotates a rotatable item with `R`
10. player equips a weapon into Weapon 1 or Weapon 2
11. player equips backpack item into Backpack slot
12. invalid drops fail safely
13. player right-clicks an item and sees valid context actions
14. player drops an item via context action
15. player drops an item by dragging it outside inventory panel
16. player consumes a food/drink item
17. craftable items show correctly in Crafting tab
18. crafting a Campfire Kit closes inventory and starts placement
19. left click places
20. right click cancels
21. no disappearing items
22. no duplicated items
23. no false inventory-full states
24. no cursor/look/input conflicts while inventory is open

---

## Relationship to Milestone C

Milestone B.5 should happen before expanding Home Island progression into Milestone C.

Reason:
- the inventory, crafting, and placement loop is currently the main friction point
- Milestone C depends on a stable player-item loop
- this milestone turns the inventory from debug scaffolding into a reliable gameplay foundation

Milestone C should build on top of this, not around it.
