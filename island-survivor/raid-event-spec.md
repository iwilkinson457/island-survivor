# Extraction: Dead Isles — Raid Event Spec

## purpose
This document defines how raid events should work on Home Island. Raids should create pressure on settlement growth, give value to defenses, and make the home base feel like a visible foothold in a dangerous world.

## design goals
Raids should:
- make the home base feel vulnerable but worth defending
- reward smart settlement growth and preparation
- create tension between expansion and visibility
- punish neglect without making losses feel hopeless
- support human-enemy identity more than zombie identity

Raids should be painful but recoverable.

## what causes raids
Raid pressure should grow from visible signs of settlement life and settlement value.

### raid likelihood factors
- campfire smoke
- furnace or smelter smoke
- visible lights
- number of rescued NPCs
- number of structures built
- size of the camp

These elements should signal that the player is creating a settlement worth finding.

### raid strength factors
Raid size and quality should increase from:
- number of walls and defenses
- number of buildings
- number of stations
- general base value
- overall settlement development

This means a larger and more successful camp draws more serious attention.

## communication of raid risk
Raid risk should not feel invisible.

### warning styles
Use a mix of:
- subtle raid-risk meter or threat indicator
- NPC comments and concerns
- environmental signs
- nearby world tension signals

The game should communicate risk lightly, not with giant arcade warnings all the time.

## watchtower warning
A watchtower should provide a practical warning benefit.

### first-pass rule
If the player has a watchtower, they receive a 60-second warning before a raid properly begins.

### purpose
This gives the player time to:
- gear up
- reposition
- move to defenses
- prepare traps or angles

## raid structure
### raid event flow
1. raid pressure builds invisibly in the background
2. warning signs appear or a watchtower warning triggers
3. raiders approach and begin assault
4. player defends the base
5. raid ends when raiders are defeated or the player is defeated
6. consequences are applied and recovery begins

## enemy type in raids
Early raids should be human-driven, not zombie-driven.

### first-playable raid identity
- raiders
- scavenger-style attackers
- rough human enemies
- possible bruiser-style attacker in larger events

Zombies should not be the main source of home raids because boat travel would make that less believable in the current design.

## raid enemy behaviour
Raiders should:
- approach the home base from plausible directions
- break walls or doors if left unchecked
- enter through openings
- pressure the player in short, rough assault waves
- feel more like desperate hostile survivors than military squads

## raid objectives for the player
The player's goal during a raid is simple:
- survive
- protect important parts of the base
- stop raiders before they steal too much or destroy too much

## failure state and punishment
If the player dies during a raid:
- raiders steal random stored items
- parts of the base may be damaged
- some walls or structures may need repair

### punishment target
Losses should hurt, but they should not destroy the whole save or feel unrecoverable.

## success state
If the player defeats the raiders:
- the camp survives the event
- losses are reduced or prevented
- the player should feel that defenses and preparation mattered

## defenses and traps
Defenses should matter, but they should not trivialise raids.

### first-pass useful defenses
- walls
- doors
- early traps
- watchtower
- good base layout

### rule
Defenses should buy advantage, slow attackers, and improve survival. They should not make the player fully immune.

## raid damage rules
Raiders should be able to:
- break walls
- damage doors
- damage some structures
- steal stored resources if the player loses

### avoid
- full settlement wipe
- permanently deleting all major progress
- making one raid end the run unfairly

## scaling philosophy
Raids should scale with visible base success.

### low-tier raid
- small attacker group
- simple weapons
- pressure test for a growing camp

### mid-tier raid
- more attackers
- better mix of enemy types
- stronger structure pressure

### later scaling direction
Can eventually support better-armed raiders, but not needed in earliest implementation.

## role of rescued NPCs during raids
In the first playable, rescued NPCs should mostly be part of the stakes rather than fully simulated combatants.

### early rule
NPCs can:
- react
- shelter
- comment afterward
- reinforce the sense of loss or survival

Do not require advanced NPC combat participation in the first implementation.

## post-raid recovery
After a raid, the player should have a clear recovery loop:
- assess damage
- repair walls or structures
- replace stolen supplies
- reinforce base layout
- prepare for future attacks

This makes raids part of the larger survival loop rather than isolated gimmicks.

## relationship to base growth
Raids are meant to balance the benefits of building a bigger, more useful camp.

### intended trade-off
A larger settlement gives:
- more utility
- more storage
- more stations
- more NPC support

But it also gives:
- more visibility
- higher raid pressure
- stronger attacks over time

## intended player feeling
The player should feel:
- my camp matters
- growth has consequences
- defenses are worth building
- losing hurts, but I can recover

## avoid
- constant raid spam
- totally random unavoidable destruction
- giant tower-defense complexity in the first playable
- making raids more frequent than island runs
- making the player feel punished for building at all

## first-pass implementation summary
For the first playable, raids should:
- be event-based
- scale from camp visibility and value
- give a 60-second warning if a watchtower exists
- use human raiders rather than zombies
- allow raiders to break and steal
- end when raiders die or the player dies
- create painful but recoverable consequences
