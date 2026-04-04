# Extraction: Dead Isles — Player Feel and Combat Spec

## player feel direction

The game should feel quick, responsive, and readable rather than slow or simulation-heavy. The intended feel is closer to a fast mainstream FPS than a heavy milsim. Movement should support quick repositioning, short bursts of aggression, and fast decision-making under pressure.

### movement rules
- Movement should feel quick and responsive.
- The target feel is closer to Call of Duty pace than Gray Zone Warfare.
- The player can crouch and prone.
- Jumping should be a normal FPS-style jump.
- Fall damage should exist, but be light.

### stance roles
- Standing is the default exploration and combat stance.
- Crouch is used for stealth, lower visibility, and tighter movement through cover.
- Prone is used mainly for concealment, ambush, and line-of-sight reduction rather than fast movement.

## combat feel direction

Combat should be readable, fast, and practical. The player should feel capable of surviving by moving well, opening well, and creating space when pressured.

### melee feel
- Early melee should feel fast and responsive.
- The early melee kit should include:
  - Light attack
  - Kickback
- Kickback is the core spacing tool. It should stagger enemies and create breathing room.

### stagger rules
- Zombies should mainly stagger on kickback.
- Basic hits should not endlessly stunlock enemies.
- Kickback should be a reliable panic-management and spacing mechanic.

### damage model
The first-pass damage model should emphasize readable tactical choices.

- Head damage matters.
- Leg damage matters.
- Head hits should reward precision and faster kills.
- Leg hits should slow or partially disable enemies.
- The first playable can support simple leg-state degradation, such as normal, slowed, and crawler states.

### stealth combat
Stealth should be part of the game from the start, but the first implementation should be simple.

- Stealth attacks should do extra damage.
- The player does not need a full takedown animation system in the first pass.
- If the player attacks while unseen, the opening hit should apply bonus damage and stronger initial advantage.

## enemy awareness model

Enemy awareness should create tension without turning every small mistake into instant swarm aggro.

### first-pass awareness states
- Idle
- Suspicious
- Searching
- Alerted

### intended behaviour
- When a zombie hears something, it should investigate and look around for a while.
- Enemies should not immediately become full hive-mind aggro from every small sound.
- Suspicion and search states should make stealth feel readable and useful.

## combat experience goals
The player should feel able to:
- open carefully
- use stealth damage
- manage space with kickback
- choose between head damage and leg damage
- retreat and reposition when pressure gets too high

The game should avoid feeling sluggish, animation-locked, or overly realistic in the early playable.

## avoid too early
- full takedown animation systems
- heavy stamina tax on every action
- large melee combo trees
- parry-heavy melee combat
- ultra-smart swarm behaviour
- milsim handling complexity
