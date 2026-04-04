# Build Roadmap - Draft 1

Based on Wilko's current direction.

## One-sentence vision
This game is about a survivor in the apocalypse trying to build a community and find their family.

That sentence should be treated as the anchor for the whole project.
If a system does not support survival, community-building, extraction tension, or the family search, it should be questioned.

## Core project identity
- First-person low-poly survival + extraction game
- Single-player first, possible co-op later
- Grim tone with bits of relief and humour
- Home base progression with rescued NPCs
- Island-to-island progression through quests, maps, and extraction
- Early game focused on stealth and melee
- Mid/late game escalates into humans, firearms, and mutants

## Development philosophy
This is a first serious game project, so development should follow one rule above all:

### Build the game in layers
Do not try to make the full dream at once.
Prove the feel first, then prove the loop, then prove the first slice, then expand.

The correct order is:
1. prototype the feel
2. build a first playable loop
3. create a polished vertical slice
4. expand content carefully

## Stage 1 - Core Prototype
### Goal
Prove that the game feels good at the most basic level.

### The prototype must prove
Top priorities confirmed by Wilko:
- first-person movement feels good
- melee against zombies feels good
- stealth / hearing / vision works

### Prototype content
#### Player
- first-person controller
- walk
- sprint
- jump
- basic camera look
- simple stamina for sprinting and jumping

#### Combat
- melee weapon swing
- zombie hit reaction
- zombie death
- basic head hit / body hit distinction if feasible
- simple bow later only if prototype scope allows

#### Enemy AI
- basic idle / patrol / wander
- hearing trigger
- vision trigger
- investigate sound source
- chase on sight
- lose player after some time if line of sight is broken

#### Building
- very limited placement only
- start with predefined simple placeables such as:
  - campfire
  - storage
  - hut marker / simple hut
  - workbench

#### World
- greybox only
- capsules / primitive shapes are fine
- no art polish priority

### Prototype success test
The prototype is successful if:
- moving around feels responsive
- zombies are tense and readable to fight
- stealth feels understandable
- sound and sight reactions are obvious enough to learn

## Stage 2 - First Playable
### Goal
Prove the actual game loop.

### First playable content
#### Islands
- Home Island
- Mining Island

#### Systems included
- basic gathering
- primitive crafting
- extraction loop
- home base return
- limited inventory
- lose carried items on death
- save only at home / on successful return
- Mining Engineer rescue chain
- mining island unlock
- early base building

#### Building scope
Base building must be in the first playable.

Include:
- player shelter pieces
  - floor
  - wall
  - roof
  - door
  - window
- storage
- campfire
- simple workbench
- mining engineer hut

#### Crafting scope
Must-have craftables currently confirmed:
- axe
- pickaxe
- spear
- bow
- arrows
- stone knife
- campfire
- storage box
- workbench
- hut parts
- simple healing item

#### Resources
Must-have resources currently confirmed:
- wood
- fibre
- stone
- food
- water
- scrap
- ore
- infected residue
- coal
- iron

#### Enemies
Zombie types for first playable:
- regular zombie
- fast zombie
- armoured zombie
- crawler

Human enemies:
- one gang encounter on home island
- do not attempt full human-combat systems yet beyond what is needed for this encounter

Boss:
- one real boss on Mining Island

### First playable success test
The first playable is successful if the player can:
1. survive at home
2. gather resources
3. craft primitive gear
4. rescue the mining engineer
5. build a basic camp
6. unlock the mining island
7. extract resources from the mining island
8. return home alive with meaningful loot
9. feel tempted to go back out again

## Stage 3 - Vertical Slice
### Goal
Create a polished, convincing slice of the real game.

### Recommended vertical slice scope
- Home Island polished
- Mining Island polished
- first pass of Luxury Hotel Island
- clearer NPC quest presentation
- better UI / inventory readability
- one proper raid event
- infected residue economy working
- Mining Engineer fully useful
- one or two additional NPC systems started
- map/clue leading toward the hotel

### Purpose of this stage
This stage should prove:
- the art direction works
- the progression structure works
- the game is emotionally and mechanically distinct
- the project is worth expanding further

## Stage 4 - Expansion to full game structure
### Main content additions
- Luxury Hotel Island full implementation
- Industrial Storage Island
- Military Base Island
- Research Base Island
- more NPC rescues
- broader crafting and service economy
- proper raids and defence systems
- family clue trail
- your child rescue arc
- partner endgame reveal
- mutant systems
- more human enemy depth
- boat upgrade meta-progression

## Recommended production order for major systems
### Order of implementation
1. player movement and camera
2. zombie AI with hearing / vision
3. melee combat
4. simple gathering and interaction
5. inventory and item pickup
6. basic crafting
7. limited building placement
8. home island loop
9. extraction and save/death loop
10. mining island
11. mining engineer quest chain
12. boss on mining island
13. NPC hut and service unlock flow
14. hotel map clue progression
15. raid prototype
16. larger content expansion

## What to deliberately avoid too early
Do not build these too early:
- co-op networking
- full weather system
- full day/night cycle
- advanced mutant systems
- complex human squad AI
- advanced boat simulation
- giant open world
- huge building part catalogue
- too many NPCs at once
- complicated cinematic story scenes

These can destroy momentum if attempted too early.

## Save/load recommendation
Current preferred direction:
- save only at home
- save after death / respawn state updates
- no free save-anywhere system in the main loop

This supports the extraction structure well.
A later backup or safe quit system can still be added if needed for usability.

## Art recommendation
Confirmed direction:
- start with greybox / capsules
- focus on play feel first
- move into simple coherent low-poly art later

This is the correct approach.

## Why this roadmap is realistic
It protects the project from the most common first-game mistake:
trying to build too many systems before proving the fun.

This roadmap keeps the project centred on:
- feel first
- loop second
- scope control always

## Best next handoff package
Before handing this to Roxy, the design packet should include:
- README
- concept-notes
- gameplay-loop-and-progression
- story-and-factions
- island-progression
- npc-and-island-mapping
- progression-systems
- raid-economy-and-quest-systems
- core-run-loop
- first-playable-scope
- build-roadmap

## Recommended next document
The next most useful document would be:
- a concrete feature backlog / task breakdown

Suggested filename:
- `prototype-task-breakdown.md`

That document should convert the roadmap into practical implementation chunks for Unity.
