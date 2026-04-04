# Extraction: Dead Isles — Design Pack Index

## purpose
This file is the quick index and handoff guide for the current design pack.
It identifies the most important documents, the recommended reading order, and the best reduced packet to hand to Roxy before implementation.

## current design state
The project now has a strong early-game design foundation covering:
- core premise
- story direction
- island progression
- home base growth
- specialist rescue progression
- combat direction
- zombie and human enemy roles
- loot and crafting progression
- staged Mining Island progression
- UI and home-loop flow

The pack is now strong enough to hand to a Unity specialist as a serious prototype packet.

## recommended top-level reading order
If someone is new to the project, they should read in this order:

1. `README.md`
2. `roxy-implementation-brief.md`
3. `first-playable-scope.md`
4. `build-roadmap.md`
5. `full-progression-flow-intro-to-mining-boss.md`
6. `home-island-progression-map.md`
7. `mining-island-multi-run-progression.md`
8. `npc-specialist-progression-tree.md`
9. `ui-inventory-and-crafting-screen-flow.md`

After that, move into the supporting detail docs as needed.

## core handoff set for Roxy
If the goal is to give Roxy the smallest strong packet, use this set first:

- `README.md`
- `roxy-implementation-brief.md`
- `first-playable-scope.md`
- `build-roadmap.md`
- `full-progression-flow-intro-to-mining-boss.md`
- `home-island-progression-map.md`
- `mining-island-multi-run-progression.md`
- `family-house-layout-and-encounter-flow.md`
- `npc-specialist-progression-tree.md`
- `ui-inventory-and-crafting-screen-flow.md`
- `unity-architecture-notes.md`
- `prototype-task-breakdown.md`

That set is the best practical starting packet.

## supporting design docs
These deepen specific systems and should be read as needed:

### game structure and progression
- `concept-notes.md`
- `gameplay-loop-and-progression.md`
- `story-and-factions.md`
- `island-progression.md`
- `npc-and-island-mapping.md`
- `progression-systems.md`
- `core-run-loop.md`
- `raid-economy-and-quest-systems.md`
- `full-progression-flow-intro-to-mining-boss.md`

### combat and enemies
- `player-feel-and-combat-spec.md`
- `zombie-roster-spec.md`
- `human-enemy-and-gang-roster-spec.md`
- `family-house-encounter-spec.md`
- `family-house-layout-and-encounter-flow.md`

### crafting, loot, and inventory
- `first-playable-items-and-resources.md`
- `crafting-unlock-structure.md`
- `first-playable-loot-table-spec.md`
- `detailed-loot-table-spec.md`
- `ui-inventory-and-crafting-screen-flow.md`

### home base and settlement growth
- `base-building-progression-spec.md`
- `raid-event-spec.md`
- `quest-structure-spec.md`
- `npc-specialist-progression-tree.md`
- `home-island-progression-map.md`

### Mining Island progression
- `mining-island-first-visit-mission-flow.md`
- `mining-island-multi-run-progression.md`

### implementation and production guidance
- `first-playable-scope.md`
- `build-roadmap.md`
- `prototype-task-breakdown.md`
- `unity-architecture-notes.md`
- `roxy-implementation-brief.md`

## current locked early-game truths
These are the most important locked decisions to preserve:

### game identity
- first-person, low-poly, single-player first
- survival + extraction structure
- home base and rescued community are central
- searching for family is the long-term story spine

### early progression
- passive animals appear before zombies
- first zombie appears only after first camp and cooked food
- family house is the first major human-threat landmark
- engineer is the first rescued specialist
- engineer must be housed and supported before major unlocks
- Mining Island is unlocked through engineer progression

### Mining Island staging
- first run only reaches the camp/living area
- the mine is locked on the first run
- player must recover documents and return them to engineer
- engineer then gives the key or access solution
- this teaches that islands are not cleared in one raid

### home and progression loop
- extraction is required to keep carried progress
- death loses carried items
- Home Island remains the long-term operational hub
- base building matters early and is part of first playable

## cleanup notes
The pack is now coherent enough for handoff, but future cleanup should continue to:
- keep terminology consistent
- avoid duplicate partial summaries when a newer stronger doc exists
- prefer implementation-ready documents over older brainstorming drafts when conflict appears

## practical guidance
If Roxy ever finds contradictions, the safest priority should be:
1. newest implementation-ready docs
2. early-game flow docs
3. first-playable scope and roadmap
4. older brainstorming docs last

## summary
This design pack is now strong enough to support a serious prototype-first Unity handoff.
The core handoff set listed above should be treated as the main packet.
