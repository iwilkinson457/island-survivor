# Prototype Task Breakdown

Practical implementation breakdown for the Extraction: Dead Isles Unity prototype.

## Goal of this document
Turn the current design packet into a realistic sequence of implementation tasks.

This is not the full game backlog.
This is the practical first build sequence for proving the game works in Unity.

## Prototype objective
The prototype must prove three things first:
1. first-person movement feels good
2. melee combat against zombies feels good
3. stealth / hearing / vision works

If those three things do not work, the rest of the game should not be expanded yet.

## Recommended implementation order

# Phase 1 - Project setup and core controller
## 1. Create Unity project foundation
- Create new Unity project
- Set up folders for:
  - Scenes
  - Scripts
  - Prefabs
  - Art
  - Audio
  - Materials
  - ScriptableObjects
  - UI
- Set coding conventions and naming rules
- Create bootstrap scene

## 2. First-person player controller
Build a simple first-person controller with:
- walk
- sprint
- jump
- mouse look
- grounded check
- basic stamina for sprinting and jumping

### Done when
- movement feels responsive
- no major camera jank
- stamina drains and recovers correctly

## 3. Interaction framework
Create a simple interaction system for:
- picking up items
- opening simple containers
- interacting with buildables / benches later
- rescuing NPCs later

### Done when
- player can look at an object and interact reliably
- interaction prompt is readable

# Phase 2 - Basic zombie combat sandbox
## 4. Basic zombie enemy controller
Create a zombie with:
- idle / wander
- movement toward target
- simple attack
- health
- death

### Done when
- zombie can detect player in a basic way and attack
- zombie can be killed reliably

## 5. Zombie sensing system
Implement:
- hearing radius / sound investigation
- vision cone / line-of-sight check
- state changes: idle -> investigate -> chase -> attack

### Done when
- footsteps / sound events can attract zombies
- zombie chases when player is seen
- zombie loses player after line of sight breaks long enough

## 6. Melee combat system
Create melee combat with:
- weapon swing
- hit detection
- hit reaction
- damage application
- head/body hit distinction if feasible

### Done when
- melee combat feels readable
- hits feel responsive
- fighting one or two zombies is tense but fair

## 7. Zombie damage reactions
Add first-pass reaction rules:
- normal hit reaction
- headshot bonus / lethal head damage if supported
- crawler behaviour if leg destruction is feasible in prototype
- armoured zombie damage reduction placeholder if supported

### Done when
- zombies feel different enough when hit
- combat has some tactical readability

# Phase 3 - Prototype sandbox scene
## 8. Greybox combat test level
Build a simple greybox test area using capsules / primitives.
Include:
- open area
- narrow corridor / interior area
- obstacles for line of sight
- simple loot placements

### Done when
- player can test stealth, chase, and melee in varied spaces

## 9. Sound event hooks
Hook sound generation into:
- footsteps if sprinting or moving loudly
- melee impacts if needed
- future ranged combat placeholder

### Done when
- stealth feels understandable rather than arbitrary

# Phase 4 - Basic survival and inventory foundation
## 10. Item system foundation
Create item data for key prototype items:
- wood
- fibre
- stone
- food
- water
- scrap
- infected residue
- simple tools / weapons

### Done when
- items can exist as pickups and inventory entries

## 11. Inventory system
Create a simple inventory with:
- slot-based or simple weight/slot hybrid system
- stack rules where appropriate
- limited capacity

### Done when
- inventory supports the tension of limited carry space
- adding/removing items works reliably

## 12. Pickup and loot flow
Create item pickups in the world.
For infected residue:
- obvious pickup presentation
- auto collect if close enough

### Done when
- gathering and combat rewards feel smooth
- no annoying corpse-search loop is required

## 13. Basic hunger / thirst / health loop
Implement:
- hunger value
- thirst value
- health loss when empty
- simple consumable use

### Done when
- survival matters without dominating the experience

# Phase 5 - Primitive crafting and base stub
## 14. Basic crafting system
Create recipe system for first primitive items:
- stone knife
- spear
- axe
- pickaxe
- bow
- arrows
- simple healing item

### Done when
- player can gather materials and craft the minimum survival kit

## 15. Very limited building placement
For prototype stage, only allow simple placeables:
- campfire
- storage box
- workbench
- simple hut marker or predefined hut shell

### Done when
- player can place essential base objects without major bugs

## 16. Home base greybox area
Create a simple home island prototype zone with:
- resource nodes
- safe-ish camp area
- a few zombie patrols
- placeable base area
- route toward family house / gang area later

### Done when
- player can gather, craft, fight, and build in one loop space

# Phase 6 - First progression proof
## 17. Mining Engineer rescue flow
Prototype a simple rescue flow:
- locate engineer
- clear area if needed
- interact / rescue
- unlock ability to build his hut

### Done when
- player has a clear first progression milestone

## 18. NPC hut unlock logic
Implement:
- build hut for rescued NPC
- once built, NPC becomes active at base
- later service hook placeholder

### Done when
- rescue -> build hut -> permanent camp upgrade loop works

## 19. Mining island travel stub
Create simple island selection / travel flow:
- interact with boat
- choose destination
- load mining island scene
- arrive at spawn point

### Done when
- the game proves island-to-island structure

## 20. Mining island greybox
Build a mining island test slice with:
- ore nodes
- tighter combat spaces
- higher danger than home
- extraction point / boat return

### Done when
- mining island feels distinct from home
- player can perform a real out-and-back run

## 21. Extraction and loss loop
Implement:
- return to boat to extract
- carried loot banks only on successful return
- death loses carried items
- respawn at home
- save updated at home / after successful return

### Done when
- the extraction rule is real and meaningful

# Phase 7 - First playable polish layer
## 22. Zombie variants
Add first playable variants:
- regular zombie
- fast zombie
- armoured zombie
- crawler

### Done when
- enemy variety changes player behaviour

## 23. One gang encounter on home island
Add one small human encounter / encounter zone.
Keep scope narrow.

### Done when
- player gets an early hint that humans matter later

## 24. Mining island boss
Add one real boss encounter on the mining island.
This can be:
- always present for first playable, or
- chance-based later once the core works

### Done when
- the mining island has a memorable high-threat objective

## 25. Hotel clue setup
Add first clue chain placeholder:
- engineer quests
- hidden map toward hotel progression

### Done when
- the player sees the next major destination forming

# Phase 8 - Validation and scope control
## 26. Playtest the loop repeatedly
Test these questions:
- does movement feel good?
- does melee feel good?
- does stealth feel understandable?
- does building feel useful?
- does dying matter?
- does returning from the mine feel rewarding?
- do players want one more run?

## 27. Fix before expanding
Before adding more islands or more systems:
- fix frustrating combat
- fix bad AI readability
- fix inventory friction
- fix boring gathering pacing
- fix bad extraction pacing

Do not expand content while the core loop still feels weak.

## Recommended backlog after prototype success
Only after the prototype and first playable are working should the project expand into:
- proper raid system
- infected residue service economy
- more NPC services
- luxury hotel island
- better base building
- improved UI
- low-poly art pass
- audio pass
- stronger story/clue structure

## Important scope rule for Roxy / implementation
If a task does not directly support:
- feel
- loop
- progression proof
then it probably should not be in the prototype.

## Suggested milestone summary
### Milestone A
- movement
- zombie AI
- melee
- stealth sandbox

### Milestone B
- inventory
- gathering
- crafting
- limited building
- home island loop

### Milestone C
- engineer rescue
- mining island travel
- extraction loop
- death / respawn / save rule

### Milestone D
- zombie variants
- gang teaser
- mining boss
- hotel clue hook

## Final prototype success statement
The prototype is successful when the game can honestly demonstrate:
- satisfying first-person movement
- tense stealth and melee survival
- meaningful home base setup
- one complete extraction run from home to mine and back
- a clear reason to keep developing the project
