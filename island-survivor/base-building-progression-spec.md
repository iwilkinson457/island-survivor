# Extraction: Dead Isles — Base Building Progression Spec

## purpose
This document defines the first playable base-building structure, how home growth should work, what counts as valid early housing, and how building ties into survival and NPC progression.

## design goals
Base building in the first playable should:
- feel practical rather than decorative
- directly support survival and progression
- make Home Island feel like a real operating base
- support NPC rescue and settlement growth
- stay limited enough to remain buildable in early development

## base-building scope in first playable
The first playable should support medium-scope base building rather than huge sandbox freedom.

### intended early structure
The player should be able to build:
- a simple personal shelter
- practical home utilities
- defensive basics
- at least one valid NPC hut
- a growing work area with stations

## first-playable buildable set
Confirmed early buildables:
- campfire
- storage crate
- bedroll
- hut pieces
- walls
- door
- crafting bench
- furnace
- farm plot
- engineer's hut
- ammo bench

## early building philosophy
The first buildables should solve practical problems.

### examples
- campfire solves cooking
- storage crate solves inventory overflow
- bedroll supports home identity and rest
- hut pieces create shelter and housing
- furnace enables ore progression
- ammo bench enables bullet progression

## player shelter spec
The player's own early home should be simple but meaningful.

### first-pass supported pieces
- floor
- wall
- roof
- door
- window openings later if needed, but not essential for first implementation

### first-pass rule
One-level structures are enough for the first playable. Do not support large multi-storey complexity early.

## engineer hut spec
The engineer needs a valid hut before major progression unlocks continue.

### design goal
This should teach that rescued NPCs need a place in the camp and that the home base is becoming a settlement.

### first-pass implementation options
The simplest version is either:
- a dedicated engineer hut blueprint
or
- a forgiving hut-validation rule using placed build pieces

### recommended first-pass hut rule
The engineer hut should count as valid if it has:
- floor
- walls
- roof
- door
- sleeping space

This keeps the requirement readable without becoming frustrating.

## walls and defenses
The first playable should support simple perimeter growth.

### early defensive pieces
- walls
- door or gate-equivalent entry control later if needed

### current rule
Keep advanced defense systems limited in first playable. The home should be protectable, but not turned into a giant fortress simulator immediately.

## storage progression
Storage should remain physical and finite.

### rule
Storage capacity should come from built and placed containers, not infinite menu stash space.

### first-pass storage support
- storage crate
- possible later upgraded storage variants, but not required in earliest playable

## station progression
Stations should visually and mechanically show camp growth.

### early station sequence
1. campfire
2. crafting bench
3. furnace
4. ammo bench

### purpose
This sequence mirrors overall progression:
- survival
- stability
- industrial processing
- firearm support

## farm plot role
Farm plots should exist in the first playable, but they should support stability rather than replace scavenging entirely.

### role
- support longer-term home sustainability
- reinforce the sense of a growing settlement
- provide practical food progression later in the first playable loop

## layout philosophy
The home base should grow in understandable layers.

### suggested order
1. temporary campfire and bedroll setup
2. simple shelter and storage
3. engineer hut
4. crafting bench and furnace area
5. defensive walls and base shaping
6. farm plot and ammo bench as progression deepens

## constraints to protect scope
### avoid too early
- multi-storey building complexity
- giant catalogue of decorative pieces
- electrical systems
- overly strict structural simulation
- highly detailed snapping rules that slow first implementation
- making building so freeform that NPC housing validation becomes hard to read

## intended player feeling
The player should feel:
- this camp keeps me alive
- this camp stores my progress
- this camp proves the world is changing
- this camp grows as I rescue people and return from islands

## summary
Base building in the first playable should be:
- practical
- medium in scope
- closely tied to progression
- readable to use
- limited enough to ship

The home base should begin as survival shelter and become a small working settlement.
