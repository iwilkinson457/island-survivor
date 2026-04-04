# Extraction: Dead Isles — UI, Inventory, and Crafting Screen Flow

## purpose
This document defines the first-pass user interface structure for the game, with emphasis on player HUD, inventory flow, home storage flow, crafting access, station interaction, and how the player should move between expedition play and home-base management.

## design goals
The UI should:
- feel clean and readable
- support fast survival decisions
- avoid clutter and over-simulation
- make extraction and home management easy to understand
- support the game's practical survival-extraction identity

The interface should feel functional and grounded, not overloaded with systems noise.

## overall UI philosophy
The UI should support two different gameplay modes:
1. field mode
2. home management mode

### field mode
The player is:
- moving
- fighting
- looting
- surviving
- making quick decisions

The UI should stay light and readable.

### home management mode
The player is:
- unloading loot
- sorting storage
- crafting
- building
- talking to NPCs
- preparing for next run

The UI can become more detailed here because the pressure is lower.

## first-pass HUD
The on-screen HUD during active play should stay simple.

### recommended HUD elements
- health
- hunger
- thirst
- stamina
- current equipped weapon or tool
- ammo info when relevant
- small interaction prompt
- optional stealth awareness feedback later if needed

### HUD rules
- health should always be readable
- hunger and thirst should be visible but not overwhelming
- stamina should matter mainly for sprinting and jumping
- weapon and ammo display should be compact
- avoid heavy screen clutter

## damage and status feedback
Damage feedback should be readable, but not noisy.

### first-pass needs
- hit response
- low health feedback
- healing feedback
- simple hunger and thirst warning states

### avoid
- too many flashing alerts
- complex medical-body diagrams in early versions
- too many tiny survival meters

## interaction prompt design
The game will have many interactables, so prompts must stay clear.

### interaction prompt should communicate
- what the object is
- what action can be taken
- any requirement if blocked

### examples
- open crate
- collect scrap
- refill bottle
- requires key
- talk to engineer
- use furnace

## inventory philosophy
Inventory should be simple first.

### locked decisions already respected
- simple slot-grid inventory first
- no heavy early weight simulation
- dedicated weapon slots exist
- extracted loot stays on the player until manually unloaded at home

### first-pass inventory structure
- backpack grid
- primary weapon slot
- secondary weapon slot
- melee slot
- quick-use healing or consumable access later if needed

## inventory screen design
The inventory screen should feel practical and fast.

### left-side content
- player equipment slots
- weapon slots
- current armor or clothing later if added

### main content
- backpack slot grid

### lower or side detail panel
- item details
- stack amount
- action options

### common actions
- move
- drop
- equip
- use
- split stack
- combine if relevant
- repack ammo magazines

## ammo handling UI
The player wants magazines plus reserve ammo, with repacking done in the inventory UI without animation.

### first-pass ammo flow
- equipped gun shows loaded mag status
- reserve ammo exists separately
- inventory lets player repack mags

### design goal
The UI should make this readable without becoming an extraction-sim spreadsheet.

## loot flow in the field
Looting should be quick and legible.

### first-pass loot container flow
- open container or loot source
- see simple item list or slots
- transfer items to backpack if space allows
- close quickly and move on

### design goal
The player should be tempted to risk staying longer, but the UI itself should never be the friction point.

## extraction relationship to inventory
One of the game's key rules is:
loot is only truly safe once the player extracts and returns home.

### important UI implication
The inventory should help the player feel:
- this is what I currently have on me
- if I die, I lose what I am carrying
- if I make it home, I can unload and bank it

This is one of the most important loops in the game.

## home return inventory flow
When the player returns home, the game should support a clear unload-and-sort routine.

### intended player rhythm
1. arrive home
2. open inventory
3. move items from carried inventory into storage
4. route materials to stations or future crafting
5. decide what to keep on body for next run

### design goal
Returning home should feel satisfying and organised, not messy or confusing.

## storage UI
Storage should be finite and physical because the player stores progress in actual crates and camp structures.

### first-pass storage flow
- interact with a placed storage crate
- open crate inventory panel
- transfer between player inventory and crate

### later possible support
- different storage categories
- better box capacity
- specialty storage

But first pass should stay simple.

## crafting philosophy
Crafting should exist in two layers:
1. personal crafting
2. station crafting

This matches the progression structure already locked.

## personal crafting UI
Personal crafting should support primitive and practical items.

### good first-pass personal recipes
- stone knife
- spear
- axe
- bow
- arrows
- bandage
- bedroll
- simple placeables

### UI rule
Personal crafting can live in the inventory screen as a separate craft tab or side panel.

### player expectation
If it feels like a simple survival recipe, the player should expect to craft it personally.

## station crafting UI
Station crafting should feel more grounded and specific.

### key stations in first playable
- campfire
- crafting bench
- furnace
- ammo bench

### station rule
A station should clearly communicate:
- what it makes
- what ingredients are needed
- whether another requirement is missing
- whether an NPC unlock is required

## campfire UI
### role
Simple food conversion and basic survival support.

### first-pass flow
- interact with campfire
- see cookable items
- cook using available raw food and simple requirements

### design goal
Fast and readable.

## crafting bench UI
### role
Mid-tier practical crafting.

### expected recipes
- machete
- improved arrows or utility conversions
- some hut improvements
- some repair or support recipes later

### UI design
Can be list-based with recipe details on selection.

## furnace UI
### role
Processing station.

### expected outputs
- charcoal
- iron ingots

### design goal
This should feel like material conversion, not generic crafting.

## ammo bench UI
### role
Specialist-gated firearm support.

### expected first output
- small pistol bullets

### important rule
The ammo bench should clearly show that it is part of deeper progression, not a default early crafting tool.

## NPC service UI
Specialists should feel distinct from stations.

### NPC interaction should support
- dialogue
- quest turn-in
- unlock checks
- service or recipe access
- progression hints

### design rule
NPC service UI should feel more personal and contextual than a bench screen.

## blocked recipe communication
A recipe may be unavailable because of:
- missing materials
- missing station
- missing NPC unlock
- missing island progression

### UI rule
The game should explain blocked recipes in plain language.

### examples
- requires furnace
- requires engineer unlock
- requires iron ingots
- requires ammo bench

This is important because the progression system is intentionally layered.

## map and objective UI relationship
The game uses a mix of full markers, area markers, and clue-based guidance.

### UI implication
Quest and map screens should support:
- clear active objective summary
- rough area guidance where appropriate
- discovered clues and notes when relevant

The UI should help the player remember what they are doing without over-automating discovery.

## home management loop as UI flow
The player's home loop should feel like this:
1. return from run
2. unload items into storage
3. check specialist dialogue and quest progress
4. process materials at stations
5. craft what is needed
6. rebuild, repair, or place structures
7. gear up for next run
8. depart again

If the UI supports this loop cleanly, the whole game will feel much stronger.

## build mode relationship
Building should feel separate enough from inventory clutter.

### first-pass rule
The player should enter a simple build placement mode when constructing pieces.

### building UI should communicate
- selected piece
- required materials
- valid placement or invalid placement

### design goal
Readable and lightweight, not a complex architecture editor.

## avoid
- giant overloaded inventory screens
- too many tabs too early
- unclear reasons why recipes are locked
- forcing too many clicks for basic loot transfer
- cluttered HUD during combat
- making home storage and crafting feel like spreadsheet work

## summary
The UI should support two core rhythms:

### field rhythm
move -> fight -> loot -> survive -> extract

### home rhythm
return -> unload -> sort -> craft -> talk -> build -> prepare -> leave

If the UI makes both rhythms clean and readable, the whole progression loop will feel much better.
