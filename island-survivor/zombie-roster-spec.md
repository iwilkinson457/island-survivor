# Extraction: Dead Isles — Zombie Roster Spec

## roster design goals
The zombie roster should:
1. keep early combat readable
2. reward different aiming choices
3. create pressure variety without giant complexity
4. make different spaces feel different
5. support the head and leg damage model

Each type should have a clear combat role, not just different health values.

## regular zombie
### role
Baseline infected pressure. This enemy teaches the core combat loop.

### feel
- slow to medium speed
- manageable alone
- more threatening in pairs or groups

### combat behaviour
- walks or shambles toward the player
- reacts to sight and sound
- searches for a while after losing track
- simple chase behaviour

### damage role
- head hits are the cleanest kill route
- leg damage slows it
- enough leg damage can potentially create a crawler state

### where it appears
- Home Island
- general baseline encounters
- Mining Island camp and common spaces

## fast zombie
### role
Panic disruptor. Its job is to break player comfort and punish tunnel vision.

### feel
- quick and urgent
- less tanky than regular zombies
- dangerous because of speed rather than durability

### combat behaviour
- spots and closes faster
- rushes harder once alerted
- changes direction more sharply

### damage role
- headshots are very valuable
- leg hits should strongly reduce its threat

### where it appears
- later Home Island encounters in small numbers
- selected Mining Island spaces
- never overused in the first playable

## armoured zombie
### role
Target-priority test. This enemy forces the player to rethink simple headshot autopilot.

### feel
- heavy
- stubborn
- slower than regular zombies
- dangerous because it keeps advancing

### armour concept
Armour can be represented by:
- mining gear
- helmets
- face shields
- industrial chest protection
- thick work clothing
- improvised protection

### combat behaviour
- steady advance
- harder to interrupt
- weak frontal attacks feel less effective

### damage role
- covered head and torso should resist simple kills
- legs remain a valid tactical answer
- precision and positioning matter more

### where it appears
- especially Mining Island
- occasionally in industrial-themed spaces

## crawler
### role
Low-profile lingering threat. It extends danger after the player thinks the fight is over.

### feel
- damaged and nasty
- slow in movement space but still threatening nearby
- easy to underestimate

### existence model
The crawler can appear as:
- a naturally spawned type in some spaces
- a damaged state created by heavy leg damage

### combat behaviour
- drags itself toward the player
- attacks low and close
- is easy to miss in cluttered spaces

### where it appears
- indoors
- in cluttered corners
- later in deeper mining spaces
- as a combat outcome after leg destruction

## mining boss
### role
The first true apex infected and the main milestone threat of Mining Island.

### identity
A large mutated miner-type zombie rooted in the mining site’s identity.

### feel
- larger than normal infected
- physically imposing
- harder to stop
- dangerous in confined or uneven terrain

### combat style
The boss should not just be a big health sponge. It should focus on:
- heavy forward pressure
- punishing close-range hits
- occasional burst pressure or charge
- readable weak points or timing windows

### damage role
The boss should still follow the game’s core combat language:
- head damage matters
- leg damage matters in some form
- movement disruption matters
- spacing and environment matter

### likely arena style
- lower mine section
- pit machinery area
- shaft chamber
- underground work zone

### reward role
Killing the boss should feel like a major island milestone and should support later progression.

## encounter interaction design
Good mixes include:
- regular + fast
- regular + crawler
- armoured + regular

The first playable boss fight should remain readable and not rely on too many support infected at once.

## pacing by area
### early Home Island
- mostly regular zombies
- occasional damaged or crawler situations

### later Home Island
- some fast zombie pressure in small amounts

### family house
- humans are the main threat
- zombies are secondary or environmental

### Mining Island worker areas
- regular zombies
- some fast zombies
- occasional armoured mining zombies

### deeper Mining Island
- more armoured presence
- more crawler risk
- higher pressure and boss escalation

## readability rules
Each zombie type should be readable through:
- silhouette
- movement style
- sound
- posture
- gear or damage state

## avoid too early
- giant mutation catalogue
- special ranged infected
- poison or gas gimmicks
- screamers
- wall-climbers
- multi-phase cinematic boss logic
- too many special rules per enemy type

## implementation order recommendation
1. regular zombie
2. crawler state via leg damage
3. fast zombie
4. armoured zombie
5. mining boss
