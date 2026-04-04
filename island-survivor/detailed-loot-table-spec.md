# Extraction: Dead Isles — Detailed Loot Table Spec

## purpose
This document turns the first-pass loot direction into a more implementation-ready structure for the first playable. It defines area identity, container identity, rarity usage, strict firearm rules, and special placement logic.

## rarity model
Use a simple three-tier rarity structure in the first playable:
- common
- uncommon
- rare

### rarity meaning
- Common = expected baseline finds for that location
- Uncommon = useful finds that feel good but are not guaranteed
- Rare = exciting finds, standout rewards, or progression-linked items

## global loot rules
- Loot should match location identity.
- Important progression items should not be fully random.
- Firearm progression must remain protected.
- Different areas should feel different to loot, not just look different.
- Semi-random placement should be used for important clues by selecting from a small valid pool of locations.

## Home Island loot identity
### role
Home Island supports:
- early survival
- light scavenging
- hunting and gathering
- starting the home camp
- lead-up to the family house

It should not feel like a jackpot island.

### container and loot point types
- abandoned luggage near the family house
- small sheds
- beach debris
- family house kitchen cabinets
- family house wardrobes
- berry gather points
- pond water refill point

### common loot
#### beach debris
- cloth
- rope
- scrap
- bottles
- low-value survival junk

#### small sheds
- wood
- scrap
- basic tools
- cloth
- rope

#### berry gather points
- berries
- nearby natural survival pickups if needed

#### pond
- water refill only

### uncommon loot
#### abandoned luggage
- cloth
- food tins
- bottles
- bandage
- small personal items
- low-tier carry utility chance

#### small sheds
- axe
- rope bundle
- better scrap bundle
- simple utility supplies

### rare loot
- family photo
- better backpack
- rare small medikit chance
- clue items tied to early story or family traces

## family house loot identity
### role
The family house is a dangerous progression landmark. It should feel more rewarding than normal Home Island spaces because the player earns it through risk.

### strongest reward identity
- backpack or carry gear
- meds
- gang stash items

### common loot
#### kitchen cabinets
- food tins
- bottled water
- domestic supplies

#### wardrobes and bedrooms
- cloth
- small personal items
- low-tier carry support
- occasional simple med item

#### gang stash common loot
- scrap
- rope
- cloth
- improvised melee gear
- scavenged junk

### uncommon loot
- bandages
- small medikit
- better melee weapon chance
- stronger food/water stack
- gang utility stash items
- clues tied to the engineer or local collapse

### rare loot
- better backpack
- special gang stash item
- family photo
- key progression clue
- rare useful supply cache

### rule
The family house should feel rewarding, but it must never become a random firearm jackpot.

## Mining Island worker camp loot identity
### role
The worker camp is the first profitable expedition reward zone. It should reward the player for scouting and surviving the first mining run without replacing deeper mine progression.

### strongest reward identity
- notes and logs
- food
- scrap

### common loot
- food tins
- bottles or stored water
- scrap
- cloth
- rope
- worker junk
- low-value utility items

### uncommon loot
- bandages
- small medikit
- worker notes
- site logs
- simple tools
- better scrap bundles

### rare loot
- important logs
- stronger progression clue
- rare practical item
- better industrial salvage in small quantity

## Mining Island coal zone loot identity
### role
The coal zone should reward fuel progression, worksite identity, and controlled firearm-support materials.

### strongest reward identity
- coal
- tools
- gunpowder

### common loot
- coal
- scrap
- rough tools
- dirty worksite salvage

### uncommon loot
- tool upgrades
- better work tools
- machine-useful junk
- small gunpowder amounts

### rare loot
- larger coal haul
- good tool find
- limited hidden gunpowder cache
- strong industrial clue or log

### balancing rule
Gunpowder should exist here in controlled quantity only. It should stay valuable.

## Mining Island iron zone / deeper mine loot identity
### role
The iron zone should feel riskier, tighter, and more valuable than outer areas.

### strongest reward identity
- iron ore
- dangerous clue items
- industrial materials

### common loot
- iron ore
- scrap
- heavy industrial materials
- damaged worksite items

### uncommon loot
- better industrial materials
- rarer processing materials
- valuable site records
- deeper warning logs

### rare loot
- dangerous clue items
- key progression documents
- mine keycard
- standout industrial reward
- boss-adjacent reward support

## clue placement rules
Important clues should be semi-random from a shortlist of valid locations.

### valid examples
- worker office desk
- locker room file box
- mess hall office shelf
- family house drawer or bedroom storage
- gang stash corner
- deep mine foreman station
- maintenance shelf or cabinet

This keeps replay variation without making progression frustrating.

## special rare finds
The first playable should support a few standout rare finds.

### better backpack
- practical reward
- best placed in the family house or another controlled high-risk reward point

### mine keycard
- progression-linked reward
- best placed in a deeper mine administrative or secured work area

### family photo
- emotional story item
- best placed in a domestic space such as the family house

## food and meds tone
Use a moderate scarcity model.

### meaning
- the player usually finds something useful if they search properly
- hunger and healing still matter
- bad planning is still punished
- runs still need preparation

## firearm and ammo rules
### strict firearm rule
- pistol only through engineer progression
- bullets only craftable
- bullets require gunpowder and iron ingots
- firearms should not appear as common random loot

### practical implication
This protects the intended loop:
- rescue engineer
- unlock mine
- gather coal and iron
- process materials
- unlock pistol path
- craft bullets

## summary by area
### Home Island
Best for:
- basic survival resources
- natural gatherables
- low-tier scavenging
- early emotional/story finds

### family house
Best for:
- backpack or carry upgrade
- meds
- gang stash items
- domestic story finds

### worker camp
Best for:
- food
- scrap
- notes and logs

### coal zone
Best for:
- coal
- tools
- limited gunpowder

### iron zone / deeper mine
Best for:
- iron ore
- dangerous clue items
- stronger industrial materials

## avoid
- making all containers interchangeable
- over-randomising progression items
- flooding the player with ammo early
- placing military-level rewards in non-military spaces
- making outer Mining Island zones better than deeper areas in every way
