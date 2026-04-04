# Roxy Implementation Brief

Project: Extraction: Dead Isles (working title)
Primary engine: Unity
Primary goal: Build the first prototype and first playable in a disciplined way.

## Purpose of this brief
This document is the specialist handoff brief for Roxy.
It is intended to translate the design packet into a practical Unity implementation direction.

Roxy should not treat this as a request to build the full game immediately.
Roxy should treat this as a request to:
- establish a clean Unity project structure
- prove the core gameplay feel first
- build toward a first playable without uncontrolled scope growth

## Game summary
Extraction: Dead Isles is a first-person low-poly survival + extraction game.
The player is a survivor in an apocalypse trying to build a community and find their family.

The core identity is:
- exploration and discovery
- tense extraction runs
- early stealth and melee survival
- growing home base with rescued NPCs
- island-to-island progression through quests and maps
- meaningful death through loss of carried loot

## What matters most right now
The prototype must prove three things before broader expansion:
1. first-person movement feels good
2. melee combat against zombies feels good
3. stealth / hearing / vision works

If those three things are weak, further feature expansion should pause until they are improved.

## Core player loop to preserve
At a high level:
- prepare at home
- choose island
- deploy by boat/menu travel
- explore and scavenge
- fight or avoid threats
- decide whether to risk one more building
- return to the boat to extract
- only successful return banks progress

## Prototype scope for Roxy
### Build first
- first-person controller
- stamina for sprinting and jumping
- zombie AI with hearing and vision
- melee combat
- basic interaction system
- simple item pickup and inventory foundation
- greybox combat sandbox

### Then build
- primitive crafting
- very limited base placement
- home island greybox
- mining engineer rescue flow
- mining island greybox
- extraction and death/respawn loop

### Then layer in
- zombie variants
- one gang encounter on home island
- one mining island boss
- hidden clue / hotel progression hook

## First playable scope
Roxy should target the first playable around:
- Home Island
- Mining Island

This first playable must include:
- basic gathering
- primitive tools and weapons
- home base setup
- mining engineer rescue
- boat travel to mining island
- extraction back to home
- lose carried items on death
- save at home / on successful return

## Core design rules not to break
### 1. Extraction matters
If the player dies on an island, carried items are lost.
Progress only banks on successful return.

### 2. Base building matters early
Base building is not optional flavour.
It is part of the game's identity and must exist in the first playable.

### 3. Stealth matters early
Enemies must react to sound and sight.
Early progression should not feel like a pure brawler.

### 4. Islands must have identity
Loot and progression must stay island-appropriate.
Example:
- do not put military weapons in hotel loot

### 5. Scope control matters
Do not attempt co-op, full weather, full day/night, advanced mutants, or broad human-combat systems in the early prototype.

## Desired combat feel
### Early game
- melee heavy
- stealth important
- bow unlocked reasonably early
- firearms later

### Important combat details
- aim down sights and precise shots should matter later
- headshots matter
- armour should matter on armoured enemies
- leg destruction causing crawler behaviour is desirable if implementation cost is reasonable

## Enemy direction
### First playable zombie set
- regular zombie
- fast zombie
- armoured zombie
- crawler

### Human enemies
- only one small gang encounter on home island in early scope
- full human combat depth comes later

### Bosses
- one real boss on mining island for first playable

## Stealth and sensing requirements
Enemies should have:
- hearing system
- vision system

Expected behaviour:
- hear sound -> investigate
- see player -> chase
- lose line of sight -> eventually drop back or search

The player should be able to understand why detection happened.
Readability matters as much as correctness.

## Building requirements
### Prototype stage
Very limited placement only:
- campfire
- storage box
- workbench
- simple hut placeholder

### First playable stage
Expand to simple freeform-but-limited building for player shelter:
- floor
- wall
- roof
- door
- window

Use wood first.
Stone upgrades can come later through builder progression.

## Required resources for first playable
- wood
- fibre
- stone
- food
- water
- scrap
- infected residue
- coal
- iron

## Required early craftables
- stone knife
- spear
- axe
- pickaxe
- bow
- arrows
- simple healing item
- campfire
- storage box
- workbench
- hut pieces / shelter pieces

## NPC progression direction
Current early critical NPC:
- Mining Engineer on Home Island

Core flow:
1. rescue engineer
2. build his hut
3. complete simple support quests
4. unlock mine destination
5. later unlock hotel clue/map through further questing

This rescue -> hut -> utility loop is a foundational progression pattern and should be preserved cleanly.

## Raid system guidance
Full raid system is not required for the earliest prototype.
However, the design direction should be remembered:
- base visibility affects raid likelihood
- base value/size affects raid strength
- watchtower later gives early warning
- failed raids steal random stored items
- raids should be painful but recoverable

For prototype / first playable, at most include:
- a raid-risk stub
- or a simple single-scripted raid event later

## Zombie reward economy guidance
Zombie kills should eventually contribute to:
- Infected Residue

Collection direction:
- visible small pickup
- auto collected if close enough

Use direction:
- NPC crafting services

This does not need to be fully complete in the earliest prototype, but systems should not be structured in a way that makes it awkward later.

## Save/load direction
Preferred early save rule:
- save at home
- save after successful return / progress banking
- no broad save-anywhere system in the main loop

Roxy should structure save data cleanly from the start, even if the earliest prototype only has a small amount of state.

## Recommended Unity architecture direction
Roxy should follow the architecture notes document and prefer:
- systems split by responsibility
- ScriptableObjects for game data
- prefabs for reusable world objects
- separate scenes for islands / test spaces
- clean save data models rather than state trapped in scene objects

High-priority systems should stay separate:
- player controller
- interaction
- inventory
- crafting
- AI senses / state logic
- building
- NPC progression
- extraction flow
- save system

## Preferred development order
1. project setup
2. first-person controller
3. interaction framework
4. zombie AI
5. melee combat
6. greybox combat test scene
7. inventory / pickup system
8. primitive crafting
9. limited build placement
10. home island greybox
11. mining engineer rescue loop
12. mining island greybox
13. extraction loop
14. death / respawn / save-at-home logic
15. first zombie variants
16. gang teaser
17. mining boss
18. hotel clue hook

## Acceptance criteria for the first useful milestone
The first milestone is successful if:
- movement feels responsive
- one or more zombies can be fought in a tense but fair way
- enemies detect by sound and sight in understandable ways
- the player can gather and craft at least one or two primitive tools
- a simple base area can be established

## Acceptance criteria for first playable
The first playable is successful if the player can:
- survive on home island
- rescue the mining engineer
- build basic shelter / utility
- unlock and travel to mining island
- gather valuable resources there
- return home and bank progress
- die and lose carried items in a meaningful way
- want to go back for another run

## What Roxy should avoid
Do not spend early time on:
- final art polish
- giant terrain/world size
- broad story scripting
- too many NPC systems at once
- complicated human enemy combat depth
- advanced boats
- co-op
- night/weather systems
- feature creep outside the prototype proof goals

## Reference documents
Roxy should read these project files before implementation:
- `README.md`
- `concept-notes.md`
- `gameplay-loop-and-progression.md`
- `story-and-factions.md`
- `island-progression.md`
- `npc-and-island-mapping.md`
- `progression-systems.md`
- `raid-economy-and-quest-systems.md`
- `core-run-loop.md`
- `first-playable-scope.md`
- `build-roadmap.md`
- `prototype-task-breakdown.md`
- `unity-architecture-notes.md`

## Final instruction to Roxy
Treat this as a serious prototype-first Unity project.
Do not try to impress by overbuilding.
Prove the feel, prove the loop, keep the structure clean, and make the game expandable.
