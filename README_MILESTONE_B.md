# README — Milestone B: Home Island Early Loop

## Art direction
- Target style: **atmospheric stylised realism**
- Prefer grounded tropical island ambience, believable foliage/shoreline/decay, and readable survival-horror silhouettes
- Avoid obvious low-poly/cartoon presentation and avoid hyper-real asset mismatch
- Milestone B remains prototype-greybox in implementation, but any art swaps from here should align to this direction

## What Milestone B adds

Milestone B turns the Milestone A combat sandbox into the first actual survival-game loop stub on Home Island.

Added foundations:
- item data via `ItemDefinition`
- stack-based backpack + hotbar inventory
- simple recipe-driven crafting
- gatherable resources and food pickups
- light hunger/thirst survival pressure
- runtime campfire placement
- campfire cooking interaction
- debug inventory/crafting panel
- Home Island setup instructions for a playable shoreline-to-first-camp loop

## How it fits after Milestone A

Milestone A proved:
- movement feel
- zombie AI
- melee combat
- stealth/hearing/vision

Milestone B proves:
- the player can arrive on Home Island and start building stability
- resources can feed into inventory + crafting
- camp setup has an immediate purpose
- survival pressure exists, but stays prototype-light

This is the first step from “combat prototype” toward “actual game loop.”

## Controls added in Milestone B

- `Tab` — toggle inventory / crafting debug panel
- `1–6` — select hotbar slot
- `F` — start placement for selected placeable item (currently campfire kit)
- `Left Click` — confirm placement while in placement mode
- `Right Click` — cancel placement mode
- `E` — interact / gather / use campfire

## What remains stubbed

Still intentionally stubbed or prototype-only:
- polished inventory UI
- drag/drop item movement UX
- proper world item drops
- prefab-driven placeable building set
- timed cooking progress
- passive animals / hunting loop
- save/load persistence
- extraction travel loop
- hotel intro
- human AI / family-house combat
- full home-building system

## Pass criteria for Milestone B

Milestone B passes when Wilko can:
1. spawn on Home Island shoreline
2. gather basic resources
3. craft a campfire kit
4. place a campfire in a safe clearing
5. collect raw food
6. cook it at the campfire
7. observe hunger/thirst pressure and confirm the first camp loop is working

## Recommended next milestone after B

**Milestone C — Extraction Loop Stub**

Recommended focus:
- move from Home Island comfort/safety into a first risk/reward excursion
- leave Home Island, loot a small hostile zone, and return with extracted resources
- minimal persistence/state assumptions only as needed
- keep scope tight: one outward trip, one return, one clear reward
