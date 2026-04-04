# First Playable Scope - Draft 1

Based on Wilko's current direction.

## Purpose of the first playable
The first playable should prove that the core game idea works.
It does not need to prove the full story, all islands, all enemy types, or the entire crafting tree.

It only needs to prove that this is a fun survival + extraction loop with a believable home base and meaningful progression.

## Recommended first playable scope
### Included islands
- Home Island
- Mining Island

This is the right first slice because it proves:
- starting survival
- basic gathering and crafting
- home base progression
- first off-island travel
- extraction loop
- first major NPC rescue progression

## Included player systems
### Movement and camera
- first-person player controller
- walking
- sprinting
- jumping
- basic interaction

### Survival
- simple hunger
- simple thirst
- health
- healing items

### Inventory and extraction
- limited inventory
- simple container / bag rules
- return to boat to extract
- lose carried items on death

### Combat
- melee-heavy early combat
- simple aimed bow combat if included in first slice
- headshots / critical weak points if feasible
- simple enemy armour logic only if low complexity

### Stealth
- crouch / stealth movement if feasible
- enemy hearing
- enemy vision
- enemy investigation behaviour
- chase on sight

### Crafting
- simple inventory crafting for primitive tools
  - axe
  - pickaxe
  - spear
- basic camp crafting station / bench
- enough recipes to support home -> mine -> return loop

## Included building systems
Base building must be present in the first playable.
Current direction from Wilko is correct.

### First playable building scope
#### Player base essentials
- place simple foundations / floors
- walls
- roof
- door
- window
- storage
- campfire / cooking point
- one or two workstations

#### NPC building scope
- mining engineer hut
- maybe one additional hut type if needed for proof of concept

### Building material scope
- wood first
- no stone building required in the first playable unless scope allows

### Important building constraint
Keep first playable building simple, readable, and functional.
Do not aim for a full survival-builder feature set yet.

## Included NPCs
### Required
- Mining Engineer

### Optional if scope allows
- Builder or Medic teaser on Mining Island

Best recommendation:
- fully implement Mining Engineer first
- tease next NPC progression rather than fully implement multiple complete NPC systems immediately

## Included world structure
### Home Island should support
- primitive gathering
- small number of easy zombies
- safe-ish starting area
- family house / gang area if feasible
- rescue of mining engineer
- basic base setup

### Mining Island should support
- extraction run loop
- ore gathering / better resource acquisition
- higher danger than home island
- one or more important points of interest
- clue / progression toward the next island

## Included enemies
### Required
- standard zombie

### Optional if scope allows
- one tougher zombie variant
- very light gang / human teaser on home island

Do not include full mutant systems yet.
Do not include full military-human combat systems yet.

## Included progression proof
The first playable should let the player do this:
1. start on home island
2. gather primitive materials
3. craft basic tools / weapons
4. kill a few easy zombies
5. rescue the mining engineer
6. build at least a basic shelter / camp setup
7. unlock mining island
8. travel to mining island
9. gather valuable resources
10. return alive and bank progress

If that loop feels good, the project is real.

## Included raid system scope
Raids are important to the full game, but first playable should probably use only a stub or light version.

### Recommended first playable raid implementation
One of these only:
- a simple scripted first raid event
- or a visible raid-risk system without full complex raid behaviour

Do not build the full scaling raid system in version 1.

## Explicitly out of scope for first playable
- co-op
- research island
- mutants
- advanced story campaign
- full raid system scaling
- full gun progression system
- advanced boat upgrades
- day/night cycle
- weather
- deep NPC management
- multiple advanced NPC quest chains
- full industrial / military / research progression trees

## Nice-to-have but not required
- bow in first playable
- one random boss chance on mining island
- one hidden map clue toward hotel
- one special infected variant
- simple zombie reward / crafting service currency prototype

## Success criteria for first playable
The first playable is successful if the player can say:
- I understand the game loop.
- Home feels like a real base.
- Going to the mining island feels risky.
- Bringing loot back feels rewarding.
- Dying and losing carried gear matters.
- I want to go back out for another run.

## Development recommendation
Do not try to make the first playable pretty first.
Make it:
- readable
- stable
- replayable enough to test the loop
- easy to expand into hotel / industrial / military / research later

## Best next design step after this
After locking this scope, the next useful document should be a build roadmap divided into:
- Prototype
- First playable
- Vertical slice
- Expansion path
