# Gameplay Loop and Progression - Draft 1

Based on Wilko's current direction.

## Core game identity
A low-poly first-person solo survival + extraction game with possible co-op later.

The strongest design priorities are:
1. Exploration and discovery
2. Tense extraction runs

The tone should be grim, but with bits of relief and humour.
The visual style should stay low poly and readable, but the experience should still feel tense and rewarding rather than silly.

## Core player fantasy
The player begins as a vulnerable castaway survivor and gradually becomes a capable expedition runner who can:
- survive hostile islands
- rescue useful NPCs
- unlock specialist crafting
- improve the home camp
- extract increasingly valuable items from increasingly dangerous locations

Long-term, the player is not just surviving. They are building a functioning refuge and pushing outward to recover critical people, resources, knowledge, and equipment from nearby islands.

## High-level gameplay loop
### Home phase
At the home island, the player:
- stores loot
- crafts basic gear
- uses benches for more advanced items
- manages hunger and thirst prep
- upgrades shelter and camp facilities
- interacts with rescued NPCs
- chooses the next destination

### Expedition phase
The player:
- selects an island to visit
- travels there by menu / deployment rather than manual sailing
- spawns at one of several possible insertion points
- explores the island
- fights zombies / humans / wildlife depending on the island
- gathers island-specific resources
- searches for key items, unlocks, maps, clues, or specialists
- decides how long to stay versus how much risk to take

### Extraction phase
The player:
- returns to the boat to leave
- only keeps what they successfully bring home
- loses all carried items if they die before extraction
- is pushed toward meaningful risk-vs-reward decisions every run

## Core failure rule
If the player dies on an island:
- they respawn at home
- they lose everything they were carrying
- progression depends on getting back alive

This is a defining rule and should stay clear to the player from the beginning.

## Inventory rule
Inventory should stay controlled and readable.
Current rule:
- no bag-inside-bag exploit stacking
- an empty bag can be placed in another bag
- otherwise container nesting should be restricted

This supports extraction tension and prevents the game from being broken by infinite carry tricks.

## Home base design direction
The home island is both:
- the player's safe-ish recovery zone
- and a growing risk magnet

### Raider pressure system
The player wants a home-base raider system where visible signs of progress increase danger.
Examples:
- campfire smoke attracts attention
- larger structures increase chance of being noticed
- more advanced items or activity attract stronger raiders
- richer camp progression leads to more severe raids

This is a strong mechanic because it creates a meaningful tradeoff:
- building more helps progression
- building more also makes the base more noticeable and valuable to attack

This means home is not just passive storage. It becomes a dynamic strategic layer.

## Island structure
Recommended structure:
- small handcrafted islands
- each island has a clear identity, reward type, enemy profile, and reason to revisit
- each island should feel different in both purpose and pressure

Current island set:
- Home island
- Luxury hotel island
- Research base island
- Military base island
- Industrial storage island

Likely future rule:
Each island should answer one or more of these questions:
- what unique resource exists here?
- what specialist knowledge or recipe unlocks here?
- what threat type dominates here?
- what boss can spawn here?
- why would the player return later?

## Enemy structure
Threat balance should vary by island.
Current direction:
- most islands mainly feature zombies
- some islands, especially military-style ones, lean more toward hostile humans
- animals exist and support survival gameplay
- every island should have a boss with a chance to spawn

### Threat categories
- Zombies: baseline pressure, quantity, attrition, surprise
- Human enemies: smarter gun threats, guarding high-value areas
- Animals: hunting, nuisance, or occasional threat depending on species
- Environment: hunger, thirst, limited carry space, and extraction pressure

## Survival systems
Keep survival systems present but not oppressive.
Current direction:
- simple hunger system
- simple thirst system
- when either runs out, the player loses health

This keeps survival relevant without turning the game into spreadsheet management.

## Crafting depth
Crafting should be medium depth.

### Crafting tiers
#### Immediate / field craftable
Crafted directly from inventory:
- spear
- axe
- pickaxe
- other simple primitive items

#### Bench-based crafting
Requires camp stations / benches:
- better tools
- processed materials
- some ranged weapons
- more advanced survival equipment

#### NPC-specialist crafting / unlocking
Requires rescued NPCs or specialist knowledge:
- advanced gear
- specialised tools
- higher-tier weapons
- attachments
- island-specific technologies

This matches the island-progression goal well: the player can survive alone, but cannot reach the game's higher tiers without travelling, extracting, and rescuing the right people.

## Progression principle
The player should not be able to fully industrialise everything from the home island.

Instead:
- basic survival is local
- advanced progress depends on going to the right island
- specific places unlock specific capabilities
- extraction success is how progress is banked

Examples already implied:
- military island = better guns, attachments, ammo systems, tactical gear
- mining / industrial island = metals, industrial materials, upgraded tools, machinery parts
- research island = knowledge, medicine, peptide / science angle if desired, specialised crafting unlocks
- hotel island = story origin, supplies, keys, records, luxury materials, staff areas, maintenance routes

## Story and motivation - open design problem
Two unresolved narrative questions remain:
1. Who / what is the main enemy force in the wider game?
2. What is the player's ultimate goal?

Current possible directions mentioned:
- searching for family
- escaping the island region

Both can work, but they drive different structures:
- family search = more personal and emotional
- escape = more practical and survival-focused

A hybrid may work best:
- immediate goal: survive and build a refuge
- mid-game goal: search for clues / survivors / family / cause of outbreak
- long-game goal: either escape the archipelago or confront the source / controlling faction / final extraction route

## Boss idea direction
Each island should have a boss with a chance to spawn.
This is a good replayability mechanic if handled carefully.

Potential boss roles:
- apex infected
- heavily armed gang leader
- unique island-specific mutant
- armoured ex-military commander
- industrial brute / infected worker / experimental subject

These should not feel random arcade spawns. Ideally they are tied to island fantasy and high-value extraction rewards.

## Strong design pillars now visible
1. Extraction is the real progression gate.
2. Home is useful but increasingly dangerous.
3. Islands should be role-based, not generic.
4. Basic survival is local; advanced progress is distributed.
5. Death must matter.
6. The game should feel tense, readable, and replayable.

## Next design questions to answer
The next pass should define:
- the main long-term story goal
- who the main enemy force really is
- island-by-island roles and unlocks
- NPC rescue roster and what each NPC contributes
- exact structure of raids at home base
- what step-by-step prototype / first playable should include
