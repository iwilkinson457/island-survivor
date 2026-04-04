# Extraction: Dead Isles — Asset Requirements Checklist

## Purpose
This document tracks the art, audio, animation, UI, and presentation assets required for Dead Isles.

Use it as a living checklist.
As assets are acquired, chosen, created, or rejected, update the status for each line item.

---

## Status key
- [ ] needed
- [x] acquired / completed
- [~] placeholder exists / temporary version in use
- [M] should be made in-house
- [B] likely best to buy / source externally
- [F] likely available free / low-cost
- [S] should stay simple / greybox for now

---

## Current art direction
**Atmospheric stylised realism**

This means:
- grounded tropical island ambience
- believable foliage, shoreline, decay, and weathering
- readable silhouettes
- mood and atmosphere over raw fidelity
- not obvious low poly
- not ultra-photoreal AAA

---

# 1. Environment — Home Island

## 1.1 Terrain and shoreline
- [ ] [B] Home Island terrain foundation
  - coastline shapes
  - beach transitions
  - inland elevation variation
  - safe campable clearing areas
  - Notes: should support tropical survival mood without forcing a full open-world terrain solution immediately.

- [ ] [B] Sand / beach ground materials
  - dry sand
  - damp shoreline sand
  - worn transition areas
  - Notes: should read clearly under URP lighting.

- [ ] [B] Rock / cliff materials and meshes
  - coastal rocks
  - cliff edges
  - scattered stone forms
  - Notes: should match stylised realism, not cartoon low poly.

- [ ] [M] Prototype terrain composition pass
  - gameplay-driven greybox converted into usable island layout
  - Notes: can be built in-house first using primitives / terrain tools before final art replacement.

## 1.2 Foliage and natural dressing
- [ ] [B] Tropical trees / palms set
  - silhouette variety
  - readable trunk shapes
  - canopy variation
  - Notes: avoid overly bright/cartoon trees.

- [ ] [B] Bushes / scrub / undergrowth
  - low bushes
  - medium dense scrub
  - trail-edge vegetation
  - Notes: should support stealth tension and visibility control.

- [ ] [B] Ground foliage
  - grass tufts
  - ferns
  - weeds
  - scattered leaf clusters

- [ ] [B] Vines / overgrowth set
  - for later family house / ruins / abandoned structures
  - Notes: useful beyond Home Island.

- [ ] [M] Foliage placement ruleset
  - density zones
  - safe-zone versus danger-zone readability
  - Notes: better made in-house as level-design logic rather than bought.

## 1.3 Coastal clutter and natural props
- [ ] [B] Driftwood / washed-up timber props
- [ ] [B] Small stones / pebbles / shoreline scatter props
- [ ] [B] Broken crates / debris / wreckage pieces
- [ ] [B] Dead brush / dried plant scatter
- [ ] [M] Placement composition for shoreline storytelling

---

# 2. Survival props and camp setup

## 2.1 First-camp props
- [ ] [M] Campfire visual prefab
  - firepit stones
  - wood arrangement
  - optional ember/fire states later
  - Notes: likely better made/customised in-house to match gameplay needs.

- [ ] [M] Placeable campfire upgraded art mesh
  - keep gameplay root separate from visual child mesh

- [ ] [B] Simple bedroll / tarp / starter camp props
- [ ] [B] Crate / storage box props
- [ ] [B] Basic scavenged shelter clutter
  - bucket
  - rope
  - old cloth
  - scrap timber

## 2.2 Crafting and survival station props
- [ ] [M] Primitive crafting station placeholders
- [ ] [B] Workbench / survivor table references for later milestones
- [ ] [B] Containers / barrels / buckets / practical survivor clutter

---

# 3. Resource and pickup visuals

## 3.1 Core resource pickups
- [ ] [M] Stick pickup visual
- [ ] [M] Stone pickup visual
- [ ] [M] Fibre pickup visual
- [ ] [M] Raw fish pickup visual
- [ ] [M] Cooked fish visual/icon pairing
- [ ] [M] Campfire kit world item visual
- [ ] [M] Crude spear world item visual placeholder

Notes:
- These are small, gameplay-specific, and may be best made in-house or assembled from simple meshes first.
- They need to remain readable and lightweight.

## 3.2 Gatherable node visuals
- [ ] [M] Stone node variant(s)
- [ ] [M] Stick cluster variant(s)
- [ ] [M] Fibre harvest node variant(s)
- [ ] [M] Simple fishing/shore food source placeholder

---

# 4. Zombies and enemies

## 4.1 Core zombie models
- [ ] [B] Base zombie character set
  - at least 2–4 readable variants for prototype variety
  - should fit atmospheric stylised realism
  - should not feel cartoonish or hyper-photoreal against environment

- [ ] [B] Zombie clothing variation set
  - torn civilian / island survivor / worker style options

- [ ] [B] Zombie material pass compatibility
  - skin
  - dirty clothing
  - readable silhouette under fog and shadow

## 4.2 Zombie animation needs
- [ ] [B/F] Idle animations
- [ ] [B/F] Walk / shuffle animations
- [ ] [B/F] Chase / aggressive locomotion
- [ ] [B/F] Attack animations
- [ ] [B/F] Hit reaction animations
- [ ] [B/F] Death animations
- [ ] [M] Any gameplay-specific tuning / animation state hookups

## 4.3 Later enemy types
- [ ] [S] Special infected variants
- [ ] [S] Human gang enemies
- [ ] [S] Mining island infected variants
- [ ] [S] Boss art direction references

Notes:
- Keep later enemies deferred until Home Island visual tone is proven.

---

# 5. Player presentation

## 5.1 First-person presentation
- [ ] [M] First-person weapon presentation pass
- [ ] [B/F] First-person hands / arms if required
- [ ] [B/F] Melee weapon model(s)
  - crude spear
  - improvised club / pipe / machete later

## 5.2 Player animation support
- [ ] [B/F] Melee swing animation set
- [ ] [B/F] Kick / shove animation set
- [ ] [B/F] Idle / movement layering references if first-person rig is expanded

Notes:
- Because gameplay code already exists, these are presentation upgrades, not system blockers.

---

# 6. NPCs and survivors

## 6.1 Engineer
- [ ] [S] Engineer concept direction
- [ ] [B] Engineer model or compatible survivor character base
- [ ] [B/F] Basic idle / injured / rescued pose support
- [ ] [M] Final engineer look integration into camp systems

## 6.2 Future survivors
- [ ] [S] Specialist survivor roster visual planning
- [ ] [S] Shared survivor base packs if needed later

---

# 7. Structures and explorable locations

## 7.1 Family house / early structures
- [ ] [S] Family house exterior kit
- [ ] [S] Family house interior props
- [ ] [S] Gang occupation dressing props
- [ ] [S] House decay / overgrowth set

## 7.2 Home Island points of interest
- [ ] [S] Sheds / beach structures / utility huts
- [ ] [S] Wreckage or abandoned outpost props
- [ ] [S] Settlement-growth placeholder structures

## 7.3 Mining Island future needs
- [ ] [S] Worker camp kit
- [ ] [S] Mine gate set
- [ ] [S] Industrial mining props
- [ ] [S] Coal / iron resource environment props

Notes:
- Important, but not immediate blockers for Home Island tone pass.

---

# 8. Materials and shaders

## 8.1 Master material library
- [ ] [M] Foliage master material
- [ ] [M] Wood master material
- [ ] [M] Rock / ground master material
- [ ] [M] Metal / decay master material
- [ ] [M] Fabric / rope / cloth master material

## 8.2 Rendering baseline
- [ ] [M] URP Lit material baseline confirmed
- [ ] [M] Material naming conventions
- [ ] [M] Imported-asset material cleanup workflow

Notes:
- These should be standardised in-house.
- Avoid shader pack sprawl unless a real need appears.

---

# 9. Lighting, post, and atmosphere

## 9.1 Lighting setup
- [ ] [M] Home Island baseline daylight mood
- [ ] [M] Fog tuning for distance mood and readability
- [ ] [M] Contrast tuning for shoreline vs inland tension
- [ ] [M] Time-of-day visual target selection

## 9.2 Atmosphere support
- [ ] [M] Post-processing baseline
  - colour grading
  - vignette if used lightly
  - bloom only if justified
  - exposure kept controlled

- [ ] [M] Visual benchmark scene for Home Island
  - Notes: this should be selected in-house and used as reference for consistency.

---

# 10. UI and icons

## 10.1 Item and survival UI
- [ ] [B/F] Resource icons
- [ ] [B/F] Food icons
- [ ] [B/F] Crafting icons
- [ ] [B/F] Status icons
  - health
  - hunger
  - thirst
  - stamina

## 10.2 Interface presentation
- [ ] [M] UI style direction
  - survival readability
  - not overly glossy
  - not fantasy/RPG ornate

- [ ] [M] Inventory panel visual pass later
- [ ] [M] Crafting panel visual pass later

---

# 11. Audio

## 11.1 Ambient audio
- [ ] [B/F] Tropical ambient loops
  - wind
  - surf
  - distant birds/insects
  - humid island bed

- [ ] [B/F] Tension ambience layers
  - eerie drones
  - subtle horror support
  - low-intensity threat tone

## 11.2 Gameplay audio
- [ ] [B/F] Footsteps
  - sand
  - dirt
  - foliage
  - wood later

- [ ] [B/F] Melee impacts
- [ ] [B/F] Zombie vocal set
- [ ] [B/F] Zombie attack sounds
- [ ] [B/F] Resource gather sounds
- [ ] [B/F] Campfire loop sounds
- [ ] [B/F] UI feedback sounds

## 11.3 Music
- [ ] [S] Main menu / identity music
- [ ] [S] Exploration music policy
- [ ] [S] Combat sting policy

Notes:
- Keep music restrained early. Ambience matters more than soundtrack at prototype stage.

---

# 12. VFX

## 12.1 Core prototype VFX
- [ ] [M] Simple hit impact feedback
- [ ] [M] Basic zombie hit feedback
- [ ] [M] Campfire flame / ember placeholder
- [ ] [M] Gather feedback pop / flash / dust if needed lightly

## 12.2 Later VFX
- [ ] [S] Blood variation pass
- [ ] [S] Weather effects
- [ ] [S] Mining island dust / sparks / industrial VFX

---

# 13. Technical art / pipeline requirements

## 13.1 Prefab structure rules
- [ ] [M] Gameplay prefabs folder structure
- [ ] [M] Environment prefabs folder structure
- [ ] [M] Interactable prefabs folder structure
- [ ] [M] Visual-child / collider separation rules
- [ ] [M] Interact anchor / VFX anchor conventions

## 13.2 Import workflow
- [ ] [M] Scale normalisation checklist
- [ ] [M] Pivot sanity checklist
- [ ] [M] Collider cleanup checklist
- [ ] [M] URP material conversion checklist
- [ ] [M] Asset folder cleanup rules

---

# 14. Immediate priority list

## Buy / source soon
- [ ] Base tropical environment set
- [ ] Base zombie character set
- [ ] Basic foliage support set if environment pack is incomplete
- [ ] Survival icon set
- [ ] Ambient + zombie SFX starter packs

## Make in-house soon
- [ ] Campfire visual prefab
- [ ] Core resource pickup visuals
- [ ] Gatherable node visuals
- [ ] Master material set
- [ ] Home Island lighting baseline
- [ ] Prefab/folder conventions

## Keep greybox for now
- [ ] Advanced structures
- [ ] Family house final art
- [ ] Mining Island art
- [ ] specialist NPC final-quality character art
- [ ] advanced building visuals

---

# 15. Decision log

Use this section as assets are evaluated.

## Approved assets
- _(none yet)_

## Rejected assets
- _(none yet)_

## Assets to build ourselves
- Campfire visual prefab
- Core resource pickup visuals
- Gatherable node visuals
- Master material set
- Lighting/post baseline
- Prefab structure conventions

---

# 16. Notes
- Milestones A and B remain valid and should not be rebuilt.
- Art direction changed from earlier low-poly thinking to **atmospheric stylised realism**.
- Future asset choices must stay visually coherent.
- Avoid mixing low-poly, cartoon, and photoreal packs.
- Keep gameplay roots separate from decorative meshes.
