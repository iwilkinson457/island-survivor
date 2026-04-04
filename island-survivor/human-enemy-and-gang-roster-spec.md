# Extraction: Dead Isles — Human Enemy and Gang Roster Spec

## purpose
This document defines the first playable human enemy roster, their combat identity, their role in the family house encounter, and how they differ from zombies.

## roster design goals
Human enemies should:
1. feel different from zombies
2. create more deliberate combat pressure
3. make cover and approach matter more
4. support family house and raid-style location identity
5. scale cleanly into later raider and faction threats

For the first playable, human enemies should stay grounded and readable rather than highly tactical.

## overall human combat identity
### zombies vs humans
Zombies pressure through:
- numbers
- relentless movement
- attrition
- body damage and persistence

Humans should pressure through:
- awareness
- reaction speed
- positioning
- weapon choice
- intent

Humans should feel like rough survivors, not soldiers.

## first-pass human archetypes
The first playable should use four core archetypes:
1. scavenger melee
2. scavenger ranged
3. bruiser
4. gang leader

## scavenger melee
### role
The basic human enemy. Used to pressure the player in close spaces and support the rest of the gang.

### feel
- scrappy
- aggressive
- under-equipped but dangerous
- faster and more intentional than zombies

### weapon examples
- pipe
- club
- machete
- axe
- improvised blade

### behaviour
- patrols or loiters
- investigates sound
- rushes when alerted
- uses doors, corners, and interior space better than zombies
- can reposition in short bursts

## scavenger ranged
### role
Adds line-of-sight pressure and makes cover matter more.

### feel
- less durable than bruisers
- dangerous if ignored
- more cautious than melee archetypes

### weapon examples
- bow
- improvised ranged weapon
- one low-tier firearm user at most in a key early encounter if desired

### behaviour
- hangs back more than melee enemies
- uses loose cover
- pressures from angle
- repositions if the player closes distance

### first-playable rule
Use sparingly. One ranged human can change the fight enough.

## bruiser
### role
Heavy close-range pressure unit. Used to anchor a group and punish reckless rushing.

### feel
- heavier
- tougher
- slower than scavenger melee
- intimidating at close range

### weapon examples
- large axe
- sledgehammer
- heavy improvised weapon
- reinforced club

### behaviour
- advances more directly
- less subtle and more committed
- harder to stagger
- dangerous if allowed to close distance

## gang leader
### role
Strongest early human threat and encounter anchor for the family house.

### feel
- confident
- territorial
- dangerous
- better equipped than the rest, but not elite

### weapon examples
- stronger melee weapon
- limited sidearm possibility later if intentionally designed
- improvised armor or tougher clothing

### behaviour
- holds an important interior or hostage-adjacent space
- reacts quickly when combat begins
- may push more aggressively once gang numbers fall
- should feel like the main human obstacle of the encounter

## family house gang composition
### recommended first-pass structure
- 2 scavenger melee
- 1 scavenger ranged or support threat
- 1 bruiser
- 1 gang leader

Optional:
- 1 extra basic melee enemy if balancing requires it

## human awareness model
Humans should feel more deliberate than zombies.

### first-pass states
- idle
- suspicious
- investigating
- alerted
- combat engaged
- searching

### intended behaviours
Humans should:
- react to noises
- react to bodies
- call attention through animation or voice
- move toward likely threat areas
- use rooms and entry points more intelligently than zombies

## human combat behaviour rules
### movement and positioning
Humans should:
- use cover loosely
- hold angles more than zombies
- push through doors and corners with more intent
- reposition in short bursts
- punish open exposure better than infected

### pressure style
Humans should pressure the player by:
- holding positions
- forcing movement
- punishing line-of-sight mistakes
- rushing when they believe they have an advantage

## weapon rules in first playable
### common human gear
- pipes
- clubs
- machetes
- axes
- improvised melee tools

### rare ranged gear
- bow
- one low-tier firearm user at most in a major early encounter if explicitly wanted

### avoid
- large early firearm presence
- military loadouts
- large ammo drops
- tactical gear overload

## loot relationship
Human enemies should not break the game economy.

### acceptable rewards
- melee weapon chance
- access to gang stash areas
- small practical supplies
- clue items
- very limited ammo or firearm component support only if intentionally placed

### do not allow
- easy firearm farming
- large ammo drops
- early gun snowballing

## readability rules
Human roles should be readable through:
- silhouette
- stance
- weapon held
- aggression level
- encounter position

## implementation order recommendation
1. scavenger melee
2. scavenger ranged
3. bruiser
4. gang leader variant

## summary
### scavenger melee
- basic human pressure
- fast close threat

### scavenger ranged
- line-of-sight pressure
- should be used sparingly

### bruiser
- heavy close threat
- harder to stagger

### gang leader
- strongest early human enemy
- family house encounter anchor

## avoid
- making humans feel like soldiers too early
- making them zombies with guns
- overcomplicating first-pass human AI
- letting human loot undermine engineer and pistol progression
