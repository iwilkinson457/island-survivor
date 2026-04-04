# Unity Architecture Notes - Draft 1

Practical project-structure guidance for building Extraction: Dead Isles in Unity.

## Goal of this document
Define a clean Unity project structure before implementation begins.

This is important because the project contains several interacting systems:
- first-person controller
- zombie and human AI
- stealth sensing
- gathering and inventory
- crafting and NPC services
- base building
- extraction flow
- islands and progression
- save/load

If these systems are not structured clearly from the start, the project will become hard to maintain very quickly.

## High-level architecture principle
Build the project as a set of clear gameplay systems with data-driven content where possible.

### Good rule
- code handles behaviour
- ScriptableObjects hold tunable game data
- prefabs represent world objects
- scenes represent major locations
- save data tracks player and world progression

## Recommended top-level folder structure
A clean starting structure could look like this:

- `Assets/_Project/Scenes`
- `Assets/_Project/Scripts`
- `Assets/_Project/Prefabs`
- `Assets/_Project/Materials`
- `Assets/_Project/Models`
- `Assets/_Project/Animations`
- `Assets/_Project/Audio`
- `Assets/_Project/UI`
- `Assets/_Project/ScriptableObjects`
- `Assets/_Project/Art`
- `Assets/_Project/Resources` (only if truly needed)
- `Assets/_Project/Addressables` (later if needed)

## Recommended script structure
Within `Scripts`, split by system rather than by scene.

Suggested structure:
- `Scripts/Core`
- `Scripts/Player`
- `Scripts/AI`
- `Scripts/Combat`
- `Scripts/Interaction`
- `Scripts/Items`
- `Scripts/Inventory`
- `Scripts/Crafting`
- `Scripts/Building`
- `Scripts/NPCs`
- `Scripts/World`
- `Scripts/Progression`
- `Scripts/Save`
- `Scripts/UI`
- `Scripts/Utilities`

This is much easier to scale than scene-specific script piles.

## Scene structure recommendation
Do not create one massive scene for the whole game.

### Recommended scene strategy
Use separate scenes for:
- bootstrap / startup
- main menu
- Home Island
- Mining Island
- Hotel Island
- Industrial Storage Island
- Military Base Island
- Research Base Island
- test / sandbox scenes

### Early-stage minimum scenes
Start with:
- `Bootstrap`
- `MainMenu`
- `PrototypeSandbox`
- `HomeIsland`
- `MiningIsland`

## Bootstrap scene role
The bootstrap scene should be tiny and responsible for:
- loading core managers
- loading save state
- determining which scene to enter next
- handling persistent systems if needed

Avoid putting gameplay directly in the bootstrap scene.

## Core systems that should be separated early
### 1. Player controller
Should be its own system, not mixed into combat or interaction logic.

Suggested responsibilities:
- movement
- jump
- sprint
- stamina use
- camera look
- grounded logic

### 2. Interaction system
Should handle:
- what can be interacted with
- raycast / focus target detection
- prompt display hooks
- use / pickup action

This should not live inside every individual object.
Use an interface-based approach if possible.

### 3. Inventory and item data
Inventory should be separate from world pickup behaviour.

Recommended split:
- item definitions = ScriptableObjects
- world pickups = prefabs that reference item definitions
- player inventory = runtime container / inventory model

### 4. Crafting system
Crafting should not hardcode every item relation in scripts.

Recommended split:
- recipe data = ScriptableObjects
- crafting logic = service / manager or station logic
- UI = separate layer

### 5. AI sensing and state logic
Zombie and human AI should share some common structure where possible.

Recommended separation:
- sensing system (vision + hearing)
- state machine / state logic
- combat / attack behaviour
- movement / navigation

This makes it easier to build:
- regular zombies
- fast zombies
- armoured zombies
- crawlers
- humans later

## ScriptableObject usage recommendation
Use ScriptableObjects heavily for content and balance data, but not for runtime mutable state.

Good candidates for ScriptableObjects:
- item definitions
- recipe definitions
- enemy stats templates
- NPC definitions
- quest definitions
- loot table definitions
- island loot tables
- build piece definitions
- weapon stats
- zombie variant data

Do not store active save-game world state directly in ScriptableObjects.

## Prefab structure recommendation
Use prefabs for any repeated world object.

High-value prefab groups:
- player prefab
- zombie prefabs
- human enemy prefabs later
- pickups
- containers
- crafting stations
- building pieces
- NPC prefabs
- resource node prefabs
- extraction boat prefab

Keep prefab variants clean and intentional.
Do not create prefab chaos with unclear inheritance if it can be avoided.

## Building system architecture
Because base building is important, structure it carefully.

### Prototype stage
Keep it very limited.
Use simple placeable prefabs such as:
- campfire
- storage box
- workbench
- hut placeholder

### First playable stage
Expand into simple modular player-house pieces:
- floor
- wall
- roof
- door
- window

### Recommended data split
- build piece definition = ScriptableObject
- placeable world piece = prefab
- building placement logic = building system scripts

## NPC architecture recommendation
NPCs should be split into:
- NPC identity / data
- NPC world presence
- NPC quest progression state
- NPC crafting/service offerings

This is important because the same NPC may move through states such as:
- not found
- rescued
- hut not built
- present at camp
- services partially unlocked
- services fully unlocked

Do not hardwire all this into one monolithic NPC script.

## Quest system recommendation
Quests should be data-driven enough to scale.

### Recommended structure
- quest definition data
- quest objective data
- runtime quest progress state
- UI tracking layer
- reward / unlock handlers

Quest types already expected:
- bring item
- rescue NPC
- clear location
- find clue/map
- kill boss
- build hut/station
- collect sample
- deliver between islands

This means quest logic should be modular from the start.

## Loot system recommendation
Island loot should be thematic and table-driven.

### Recommended split
- island loot table definitions
- container / spawn-point categories
- rare / fixed special loot placements
- enemy-specific drops

This supports the design rule:
- you do not find military weapons in the hotel

## Extraction flow architecture
Extraction is core to the whole game and should not be treated like an afterthought.

Recommended systems:
- boat interaction trigger
- destination selection UI
- extraction validation
- carried loot banking logic
- scene transition / load flow
- respawn-at-home logic on death

## Save system recommendation
Because saving is home-based, save design should reflect progression milestones.

### Save should track
- player inventory stored at home
- carried loadout / current run state if needed
- rescued NPCs
- huts built
- unlocked islands
- completed quest flags
- camp structures
- progression state
- world-state flags for major unlocks

### Important rule
Separate save data model from gameplay MonoBehaviours.
Use serializable data containers and reconstruction where possible.

## Suggested manager / service layer
Avoid using giant god-objects, but a few well-bounded systems are reasonable.

Possible services / managers:
- GameStateManager
- SaveManager
- SceneFlowManager
- InventoryService
- QuestService
- BuildingService
- CraftingService
- LootService
- AudioEventService (later)

Only add these when they solve a real coordination problem.
Do not create unnecessary manager spam.

## UI architecture recommendation
Keep UI separate from gameplay logic.

High-priority UI groups:
- interaction prompts
- player HUD
- inventory UI
- crafting UI
- quest log
- destination / boat UI
- simple raid warning UI later

The UI should observe gameplay state, not own gameplay state.

## Recommended enemy architecture for early implementation
For zombies, a simple shared architecture could be:
- `EnemyBase`
- `EnemySenses`
- `EnemyStateMachine`
- `ZombieCombat`
- variant-specific config data

This allows variation without rewriting the whole system for each zombie type.

## Recommended first implementation data objects
Likely early ScriptableObjects:
- `ItemDefinition`
- `RecipeDefinition`
- `BuildPieceDefinition`
- `EnemyVariantDefinition`
- `NpcDefinition`
- `QuestDefinition`
- `LootTableDefinition`

These would give the project a strong scalable core.

## Testing strategy recommendation
Do not only test inside the main islands.
Create dedicated test scenes for:
- movement
- combat
- AI sensing
- building placement
- inventory / crafting

This will save a lot of time.

## Scope control warning
The biggest danger is mixing:
- prototype code
- one-off hacks
- story-specific logic
- save logic
- UI logic
into the same scripts.

That is how Unity projects become brittle.

The project should always prefer:
- small focused scripts
- data-driven content definitions
- clear ownership of systems
- scenes that load cleanly

## Strong recommendation before Roxy starts
Roxy should treat the first build as:
- systems-first
- greybox-first
- data-structured early
- no art polish priority until the loop works

## Best next document after this
The next useful planning document would be:
- `roxy-implementation-brief.md`

That should translate the design packet plus architecture notes into a concise specialist handoff for Unity implementation.
