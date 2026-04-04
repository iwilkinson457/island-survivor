# Raid, Economy, and Quest Systems - Draft 1

Based on Wilko's current direction.

## Overview
These systems tie the whole game together.
They answer four important questions:
1. What attracts raiders to the home base?
2. What do zombie kills contribute to progression?
3. How do NPC crafting services unlock and function?
4. How do quests and maps open the path to later islands?

## Home raid system
### Core principle
The home base becomes more useful over time, but that same growth makes it more visible and attractive to human attackers.

This is a major design strength because progress creates pressure.
The player is never just decorating. Every improvement changes risk.

## What increases raid likelihood
These factors increase the chance that a raid happens:
- campfire smoke
- smelter smoke
- lights
- rescued NPC count

These are all visibility / life-sign indicators.
They tell hostile survivors that someone is living here and likely storing resources.

## What increases raid size / quality
These factors do not just increase the chance of attack. They increase the quality, size, or strength of the attacking group:
- walls and defences
- generators
- number of buildings

This is a strong rule because it implies attackers react to what they think the camp is worth.
A larger, more developed camp attracts more serious enemies.

## How the player learns raid risk is rising
Current direction:
- all warning methods should exist lightly

### Recommended warning methods
- subtle background risk meter
- NPC dialogue comments
- environmental signs such as tracks, noises, or distant signs of watchers
- visual camp cues that imply increased attention

This should feel more like rising unease than constant UI nagging.

## Raid event structure
### Core raid outcomes
During a raid:
- the player kills raiders and defends the base
- raiders break walls and doors to get in
- traps and walls can slow them down, but not make the base invulnerable

### Raid end conditions
A raid ends when:
- the player kills all raiders
- or the player dies

If the player dies:
- raiders steal random stored items

This is a good penalty because it is painful and memorable without fully resetting the base.

## Watchtower bonus
If the player has built a watchtower:
- they receive a 60 second warning before the raid begins
- this gives time to prepare armour, weapons, and position

This is an excellent upgrade because it is practical, readable, and easy for players to value.

## Raid punishment level
Target punishment:
- painful but recoverable

This is the correct level.
Raids should matter, but they should not make the player feel the whole save is ruined.

## Zombie reward economy
### Reward name
Current preferred name:
- Infected Residue

This works well because it sounds useful, a little grim, and tied to the outbreak without forcing the player into repetitive gross body-harvesting actions.

## How residue is collected
Current preferred method:
- obvious small pickup drop
- auto-collected if the player gets close enough

This is a strong implementation choice because it keeps combat rewards visible and satisfying while avoiding corpse-search tedium.

## What infected residue is used for
Current preferred use:
- NPC crafting services

This is a very good system because it links combat, extraction, and camp progression together.
The player does not kill zombies just to clear an area. They are also earning access to specialist work.

## NPC crafting service rule
Current preferred rule:
An NPC crafting service requires:
- quest-line unlock
- ingredients
- infected residue currency

This means specialist crafting has three gates:
1. rescue / unlock the NPC through progression
2. complete enough quest steps to gain access to their service
3. bring the physical materials plus combat-earned residue

This is a strong progression stack.
It prevents advanced crafting from feeling too cheap or automatic.

## Quest system
### Quest type support
Current desired quest categories:
- bring me items
- rescue someone
- clear a place
- find a map or clue
- kill a boss
- build me a hut or station
- collect samples
- deliver something between islands

This is a good quest spread because it supports:
- combat
- exploration
- base growth
- extraction loops
- story progression

## Quest guidance style
Current preferred rule:
Quest presentation depends on the quest type.

### Good examples
- full quest markers for clear, direct objectives
- area markers for boss hunts or search zones
- location markers for clearing a place
- little or no exact guidance for generic requested items that must be found through ordinary looting

This is a smart mixed approach.
Not every quest should feel like a GPS objective.

## Island unlock system
Current preferred method:
- NPC quest chain
- find a map

This is now one of the clearest progression rules in the whole design.

### Why it works
- exploration is rewarded
- NPCs matter
- progression feels earned
- maps become meaningful objects instead of flavour text
- the player gets a mix of structure and discovery

## Mining Engineer first quest chain
Current agreed rough chain:
1. rescue him
2. build his hut
3. bring food / water
4. gather wood / stone / tools
5. he reveals the mining location
6. later quests reveal the hotel clue / map

This is an excellent early progression chain because it naturally teaches:
- rescue
- build
- supply
- NPC usefulness
- island unlock progression

## Civilian valuables and scavenged goods
Items like:
- money
- phones
- laptops
- skincare products
- toiletries
- other civilian valuables

should be used for a mix of:
- NPC quest materials
- trade-like crafting requirements
- story clue support
- occasional crafting ingredients where appropriate

### Important rule
They should not become a second generic sellable-currency economy.
That is the correct choice.
The main progression economy already has a strong route through ingredients plus infected residue.
A second broad sell economy would muddy the loop.

## Recommended design interpretation for civilian goods
Rather than being 'sold', these items should be:
- specifically wanted by certain NPCs
- used in recipes or upgrades
- tied to camp comfort / requests / side objectives
- occasionally useful for story or clue reconstruction

This keeps scavenged goods meaningful without turning everything into shop trash.

## Strong system principles now locked in
1. Visibility drives raid likelihood.
2. Base value / sophistication drives raid strength.
3. Raids are event-based and painful but recoverable.
4. Watchtowers give a meaningful pre-raid warning advantage.
5. Zombie kills produce infected residue through low-friction pickup.
6. Specialist crafting requires quest unlock + ingredients + residue.
7. Island progression is driven by quest chains and discovered maps.
8. Civilian valuables should stay useful without creating a second broad currency system.

## Open questions still remaining
- How often should raid checks happen?
- How many residue units should typical crafts cost?
- Should special infected drop more residue than regular zombies?
- Which NPC wants which civilian goods most?
- Should residue be shared across all NPC services, or partly specialised later?
- What is the first boss-linked quest chain and reward?
