# Extraction: Dead Isles — Engineer Quest Chain Spec

## role of the engineer
The Mining Engineer is the first rescued specialist NPC. He is the first proof that:
- rescued NPCs matter
- home progression matters
- helping people expands the game world
- survivors are tied directly to future island progression

He should feel like a practical survivor, not a fantasy quest dispenser.

## personality direction
The engineer should feel:
- practical
- exhausted
- grateful, but not overly sentimental
- observant
- useful in industrial and expedition contexts, but not a fighter

## chain overview
The engineer arc should run like this:
1. rescue the engineer from the family house
2. return him to Home Island camp
3. build him a hut
4. stabilise him with basic supplies
5. provide a few practical materials
6. earn his trust
7. unlock Mining Island
8. receive first mining-related follow-up tasks

## rescue completion
Once the player frees the engineer, the first-pass implementation should keep things simple.

### recommended first implementation
- the rescue is resolved by interaction and quest state
- the engineer safely returns to camp without needing complex escort AI
- after the encounter resolves, he is available at home for the next step

## quest 1 — build the engineer a hut
This quest teaches that rescued NPCs need housing.

### first-pass hut requirement
The easiest first playable version is a simple NPC hut blueprint or a forgiving hut definition.

The hut should require something functionally equivalent to:
- floor
- walls
- roof
- door
- sleeping space

## quest 2 — basic recovery supplies
Once the hut is complete, the engineer needs basic recovery help.

### recommended first-pass supplies
- 2 cooked meat
- 1 water
- 1 bandage

This reinforces that quests use real survival items rather than arbitrary collectibles.

## quest 3 — practical working materials
Once stabilised, the engineer asks for simple building and working materials.

### recommended first-pass materials
- 20 wood
- 10 stone
- 6 scrap

These represent the minimum needed for him to get settled, think clearly, and start helping.

## quest 4 — the mining lead
After the player has rescued, housed, fed, and supported him, the engineer reveals what he knows.

### reveal content
- there are two mining operations: coal and iron
- the mining site may still contain useful materials, equipment, records, and survivors
- the site is dangerous, especially deeper sections
- the player is now ready to travel there

### reward
- Mining Island is unlocked
- the player receives the first practical mining objective
- the island becomes part of the main progression loop

## follow-up role after unlock
The engineer should continue to matter after Mining Island unlocks.

Future follow-up tasks can include:
- bring back coal or iron samples
- recover site tools or records
- recover maps or documents
- search for other survivors
- unlock better industrial crafting options over time

## emotional arc
The player should feel:
- I saved someone
- he lives here now
- this camp is becoming real
- helping people expands the world

## avoid
- too many fetch steps
- large material grind before the first island unlock
- overly strict hut rules too early
- instant island unlock with no support phase
