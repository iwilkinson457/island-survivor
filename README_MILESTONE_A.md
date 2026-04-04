# Extraction: Dead Isles — Milestone A: Combat Sandbox

## Art direction
- Target style: **atmospheric stylised realism**
- Not obvious low poly, and not ultra-real AAA realism
- Aim for grounded tropical mood, believable decay, readable silhouettes, and atmosphere-first presentation
- Current milestone remains greybox/system-first; final art replacement should follow this direction

## Project path
`C:\Users\Ian\.openclaw\workspace\extraction-dead-isles\`

## What was built

### Core systems
- **GameEvents** — static event bus: sound emission, player died, enemy died, item picked up
- **SoundType / HitZone / IDamageable** — shared enums and interface

### Player
- **PlayerController** — CharacterController-based FPS: walk (4 m/s), sprint (7 m/s), crouch (2 m/s), jump, camera look, stamina system
- **PlayerStats** — health (100), hunger/thirst stubs
- **PlayerInteractor** — raycast interaction, E key, IInteractable interface

### AI
- **EnemyBase** — abstract IDamageable base with death event
- **EnemySenses** — hearing (subscribes to sound events), vision cone (110°, 15m range, LOS raycast, crouch reduction)
- **ZombieStateMachine** — Idle → Wander → Suspicious → Chase → Attack → Dead, knockback coroutine
- **ZombieController** — hit zone multipliers (Head ×2.5, Torso ×1.0, Leg ×0.7 + slow on threshold)

### Combat
- **MeleeWeapon** — light attack (25 dmg), kickback (5 dmg + knockback), SphereCast hit detection, zone-aware
- **HitZoneCollider** — marker component for head/torso/leg colliders on zombies
- **WeaponHolder** — LMB/RMB input → MeleeWeapon calls

### Interaction
- **IInteractable** — interface
- **PickupInteractable** / **TestInteractable** — example implementations

### Utilities
- **DebugOverlay** — OnGUI display of health, stamina, speed, zombie count, last sound event

## How to open
1. Open Unity Hub → Add Project → select this folder
2. Select Unity 6000.4.1f1
3. Follow SCENE_SETUP.md to configure URP, create scenes, and assemble the greybox sandbox

## Milestone A success test

| Test | Expected behaviour |
|---|---|
| WASD movement | Responsive, no stuttering, correct speed tiers |
| Sprint + stamina | Sprinting drains stamina; can't sprint at 0 |
| Jump | Normal FPS jump; costs stamina |
| Crouch | Reduces capsule height, slows movement, reduces sound radius |
| Zombie wanders | Idle and wander states without player presence |
| Sprint near zombie | Zombie enters Suspicious, moves toward sound origin |
| Walk near zombie in LOS | Zombie enters Chase |
| LMB attack zombie | Hit registered, zone logged in Console |
| Head hit | ~2.5× base damage (one-shot possible with base weapon) |
| Leg hit | Reduced damage + zombie slows after leg threshold |
| RMB kickback | Zombie shoved backward, brief stun |
| LOS lost | Zombie returns to Suspicious, then Idle if no new trigger |
| Crouch approach | Zombie vision range reduced to 7m |
| Zombie attacks player | Player health decreases (logged in Console) |

## What is stubbed (Milestone B+)
- Hunger and thirst drain
- Inventory and item data
- Crafting
- Building placement
- NPC system
- Save/load
- Bow, firearms
- Crawler locomotion (leg state exists, animation not implemented)
- Sound effects and animation clips
- Art assets (greybox only)

## Known compile-time notes
- `FindObjectsByType` used in DebugOverlay — Unity 6 compatible
- Player and Enemy must use Tags: `Player` and `Enemy` respectively for some lookups
- NavMesh must be baked in the Editor before zombies can pathfind
