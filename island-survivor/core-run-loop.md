# Core Run Loop - Draft 1

Based on Wilko's current direction.

## Core moment-to-moment identity
The game should feel like:
- tense exploration
- stealth-driven early survival
- melee-heavy early progression
- gradual transition into bows, then later firearms
- extraction-focused decision making

The player should regularly feel:
- under-equipped
- tempted to push one building further
- relieved when they make it back alive
- stronger over time, but never completely safe

## Core loop of a run
### 1. Preparation at home
The player:
- stores previous loot
- crafts or repairs basic equipment
- gathers food / water / healing supplies
- chooses a destination island
- decides how much risk to take based on current gear and needs

### 2. Deployment
The player travels by boat through a menu / destination system.
There is no manual sailing in the first design pass.
The player arrives at one of several insertion points on the island.

### 3. Infiltration / scouting
On arrival, the player should ideally:
- observe the area
- identify threats
- choose whether to sneak, fight, or avoid
- decide what high-value points are worth hitting first

This is where stealth is important.

### 4. Engagement / scavenging
The player:
- searches buildings and resource nodes
- fights zombies, humans, or animals depending on the island
- gathers island-specific materials
- evaluates how much inventory space remains
- decides whether to keep pushing or head back

### 5. Risk escalation
The run becomes more dangerous as the player:
- uses more noise
- spends more time on the island
- takes damage
- fills inventory
- risks entering one more building or zone for high-value reward

This temptation is a key emotional part of the game.

### 6. Extraction
The player returns to the boat and leaves.
Only items successfully extracted back to home are kept.
If the player dies before extraction, carried loot is lost.

## Combat direction
### Early game combat
- stealth matters heavily
- melee matters heavily
- ranged combat is limited at first
- bows become available reasonably early
- firearms come later and should feel more valuable

### Desired feel
Combat should support:
- aimed shots and precision
- dangerous close fights
- tactical positioning
- enemy weak points

Important combat notes already defined by Wilko:
- headshots matter
- armour should matter on armoured enemies
- dismemberment-style outcomes can affect behaviour, e.g. blowing off a leg causes a zombie to crawl

This is a strong fit for a readable, satisfying low-poly game if handled carefully.

## Stealth system direction
Stealth is important early and should remain relevant later.

### Core enemy senses
Enemies should have:
- hearing
- vision

### Hearing system
Enemies react to sounds such as:
- footsteps
- gunfire
- other noisy actions if added later

If an enemy hears something, they should move toward the sound to investigate.

### Vision system
If an enemy sees the player:
- they should chase
- or fully enter combat / alert mode depending on the enemy type

This gives the player a natural stealth-combat loop:
- avoid
- distract
- isolate
- attack
- relocate

## Stamina system
Current preferred stamina scope:
- sprinting uses stamina
- jumping uses stamina

This keeps movement meaningful without turning the game into a stamina-management chore.

## Healing direction
Healing should exist, but stay simpler than hardcore survival sims.

Current direction:
- yes to healing items / systems
- no to broken limbs and complex infection simulation

Best-fit implementation likely includes:
- simple healing items
- maybe food helping slightly or indirectly
- stronger medical items unlocked through the medic

## Enemy behaviour direction
### Zombies
- common early enemy
- react to sight and sound
- can be disabled in useful ways
- may include special variants later

### Humans
- stronger tactical threat later
- especially important on military or organised hostile locations

### Bosses
Current rule:
- bosses only have a chance to spawn
- each island should have a few possible boss spawn locations

This is a good balance between replayability and predictability.

## Run tension drivers
The main sources of tension in a run should be:
- limited carrying capacity
- meaningful death penalty
- sound attracting enemies
- uncertain boss presence
- limited healing / gear durability if included
- the temptation to search one more building

## Day / night and weather
For the first pass:
- daylight only
- no weather needed yet

Night and weather can be added later once the base loop works.

## Strong design pillars now locked in
1. Stealth matters early.
2. Melee dominates early progression.
3. Bows arrive early enough to change pacing.
4. Guns come later and matter.
5. Extraction success is what counts.
6. Enemy hearing and vision are central to gameplay feel.
7. Precision damage and body-part effects can make combat more satisfying.

## Open implementation questions
- Should crouching, leaning, and stealth takedowns exist in the first playable?
- How loud should melee combat be versus ranged combat?
- Should bows allow partial recovery of arrows?
- How quickly should enemies lose track of the player after line of sight is broken?
- How readable can armour and weak points be in low-poly art style?
