# Extraction: Dead Isles — Inventory, Death, and Home Loop Spec

## inventory direction

The inventory system should be easy to read and easy to use in the first playable. Complexity can be added later if needed.

### core inventory structure
- Use a simple slot grid.
- Do not introduce weight in the first playable.
- Weight can be added later if the game needs more carry tension.

### equipment slots
Separate weapon slots should exist outside the main inventory grid.

- Primary weapon slot
- Secondary weapon slot
- Melee weapon slot

This keeps the inventory readable and supports clear loadout choices.

### ammo handling
Ammo should use magazines plus reserve ammunition.

- The player reloads weapons using magazines.
- The player can repack magazines in the inventory screen.
- No repack animation is required.
- The system should feel more tactile than pooled ammo, but not drift into full realism simulation.

## extraction and death rules

The game should be harsh but clear.

### death rule
- On death, the player loses everything they are carrying.
- The player respawns back at home.
- Corpse recovery is not part of the first playable, but may be considered later.

This rule is important because it gives real weight to every island run.

### successful extraction
- When the player extracts successfully, all loot stays in their inventory.
- Loot does not magically auto-bank into safe storage.
- Once back home, the player can decide what to store, equip, craft with, or carry again.

This supports a grounded home-return ritual and makes the base feel more physical.

## home loop direction

Home should be a real operational base, not just a menu disguised as a place.

### main home activities
Between runs, the player should mainly:
- craft
- build
- talk to NPCs and complete quest steps
- sort loot into storage

### storage model
Storage should be physical and limited by placed objects.

- Storage is limited by crafted and placed storage boxes.
- There is no infinite stash.
- Building more storage is part of base growth.

## survival upkeep

### hunger and thirst
- Hunger and thirst should always exist.
- They should apply both on islands and at home.
- Home should be easier to manage than field runs, but not free of upkeep.

## NPC support model
NPCs should not become a passive drain on player resources.

- NPCs are self-sufficient in day-to-day survival.
- They should not quietly consume the player's stored food or materials.
- Resource demands should happen through quests and progression steps instead.

## intended loop
A healthy loop looks like this:
1. gear up at home
2. eat and drink if needed
3. choose a loadout
4. travel to an island
5. loot, fight, and make risk decisions
6. extract successfully or lose everything on death
7. return home with gear still on the player
8. unload, craft, build, and progress NPCs
9. prepare for the next run

## avoid too early
- weight systems
- corpse recovery loops
- infinite global stash
- NPC maintenance sim
- overcomplicated ammo handling beyond mags plus reserve
