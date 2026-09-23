# Content Inventory — Player Skills & Boss Abilities

**This file is meant to be kept up to date.** Whenever a skill or boss ability is added, rebalanced,
renamed, or its unlock condition changes, update the relevant row here in the same session.

Data below was pulled directly from the live Editor (instantiating each skill/enemy prefab and
invoking `Skill.Awake()` via reflection so `Skill.Description`, `SkillMana`, `SkillCooldown` etc.
resolve exactly as they would in-game), not guessed from raw prefab YAML. Last full refresh:
2026-09-22.

## Player Skills

All values: mana cost, cast time (0 = instant), channel time, cooldown (0 = none but still subject
to the global cooldown), and how the skill is currently obtained in a normal playthrough.

| Skill (internal name) | Ready? | Mana | Cast | Channel | Cooldown | Target | Unlock | Description |
|---|---|---|---|---|---|---|---|---|
| Greater Heal | ✅ | 5 | 2s | - | 0 | Ally | Starting skill | Mana-efficient, low-output heal for 40 health. |
| Renew | ✅ | 5 | instant | - | 4s | Ally | Starting skill | HoT: heals 10 every 3s for 15s (5 ticks, 50 total). |
| Shadow Word: Death | ✅ | 4 | 1.5s | - | 0 | Enemy | Unlocked on beating **Boss 3 Normal** *(fixed 2026-09-22 — used to also be a starting skill, which made this unlock condition dead code; see changelog.md)* | Deals 12 damage. |
| Circle of Healing | ⚠️ | 8 | instant | - | 7s | Up to 5 allies (AoE) | Unlocked on beating **Boss 1 Normal** | Heals up to 5 allies for 40 health each. **Live bug under investigation (2026-09-22):** mana/cooldown are consumed but no healing happens, intermittently — worked before, broke again. Prefab config itself checks out (verified live); root cause not yet found. Temporary debug logging added to `Skill.PerformSkill`, `SkillRangeCustom.GetPool`/`GetTargets`, `SkillPerformance.Perform`, `SkillEffectHeal.PerformEffect` (uncommitted, remove once diagnosed). |
| Power Word: Shield | ✅ | 5 | instant | - | 4s | Ally | Unlocked on beating **Boss 2 Normal** | Shields the target for 15s, absorbing 40 damage. *(Tooltip text fixed 2026-09-22 — used to show a leftover placeholder string.)* |
| Penance | ❌ | 4 | - | 1.5s (tick every 0.5s) | 5s | Ally (per its `SkillRange`, despite the description also mentioning an enemy-damage variant) | **Not obtainable yet** — meant to unlock on beating **Boss 4**, which doesn't exist yet | Channeled heal, ticks for 11 per 0.5s. *(Rebalanced 2026-09-22: mana 10→4, cooldown 0s→5s.)* |
| Despell | ❌ | 3 | instant | - | 5s | Ally | **Not obtainable** — intentionally left out of the unlock chain, may end up unused | Removes one random negative status effect from the target. |
| Quick Heal | ❌ | 10 | instant | - | 0 | Ally | **Not obtainable** — intentionally left out for now, possibly a future special/unusual unlock | **Tooltip is broken**: shows the literal string "ERROR" instead of real text (its `{{cast:0.0}}` template points at an empty `_effectsCastFinish` list — same bug class as the old Power Word Shield issue, not yet fixed). |
| *(DEBUG) Kill Enemy* | ✅ | 0 | 0.5s | - | 0 | Ally **and** Enemy | **Intentionally** part of every new save's starting loadout (per 2026-09-22 design decision) — kept in deliberately, dev-managed | Deals 10–20 damage. Its internal `Skill.Name` is also `"Shadow Word: Death"` — identical display name to the real damage skill, so it's indistinguishable in the UI. Not a bug — the dev wants it to stay for now. |

✅ = obtainable in a normal playthrough, working tooltip, no known issues. ⚠️ = obtainable but has a known, currently-uninvestigated/unfixed live bug. ❌ = not obtainable yet and/or broken.

**Also TODO (design idea, 2026-09-22):** Circle of Healing should never be "un-castable". Right now,
hovering over a dead ally and pressing Circle of Healing seems to just fail to activate. Intended
behavior: it should always be castable — hovering over a *living* ally guarantees that ally is one of
the (up to 5) targets, with the rest chosen randomly among other living allies; hovering over a dead
ally, an enemy, or nothing in particular should just pick 5 random living allies with no guaranteed
target. Not investigated yet — likely needs a real code change to how mouse-over targeting feeds into
`SkillRangeCustom`, not just prefab tuning.

Starting bar (`_initialSkillsChosenPrefabs`, 5 slots total): Greater Heal, Renew, [DEBUG] Kill Enemy
*(fixed 2026-09-22 — Shadow Word: Death removed from this list, see changelog.md)*, 2 free slots
(filled by Circle of Healing after Boss 1 Normal, then Power Word: Shield after Boss 2 Normal).

## Bosses / Encounters

Boss ability data pulled the same way (live-instantiated + `Awake()` invoked). `-` means the field
doesn't apply or the effect prefab didn't expose a value this way. ✅ = tuned and has a real tooltip.
❌ = missing a description, untuned/placeholder numbers, or doesn't exist yet.

### Boss 1 — Encounter 1

| Difficulty | Ability | Ready? | Cast/Channel | Cooldown | Damage/Effect |
|---|---|---|---|---|---|
| Normal | Auto Attack | ✅ | 2.5s cast | 0 | 20 dmg, prioritizes tanks, bonus vs healers/DDs |
| Normal | Toss Boulder | ✅ | 1s cast | 5s | 30 dmg, prioritizes healers/DDs |
| Normal | Enrage | ✅ | 3s cast | 25s | Self-buff: +40% haste & cooldown reduction for 12s |
| Heroic | Auto Attack | ✅ | 2s cast | 0 | 25 dmg |
| Heroic | Toss Boulder | ✅ | 1s cast | 4s | 35 dmg, prioritizes healers/DDs *(fixed 2026-09-22, also renamed from generic "Skill Damage")* |
| Heroic | Enrage | ✅ | 3s cast | 15s | Self-buff: +40% haste & cooldown reduction for 12s, same as Normal *(fixed 2026-09-22, also renamed from generic "Skill Damage")* |

### Boss 2 — Encounter 2

| Difficulty | Ability | Ready? | Cast/Channel | Cooldown | Damage/Effect |
|---|---|---|---|---|---|
| Normal | Auto Attack | ✅ | 1.5s cast | 0 | 10 dmg to **2** single targets *(fixed 2026-09-22 — tooltip used to say "a single target", but `SkillRangeCustom._numberTargets` is actually 2, matching the fight's 2 tanks)* |
| Normal | AoE | ✅ | 1.5s channel, 3 ticks (every 0.5s) | 10s | hits up to 5 players, 12 dmg per tick *(fixed 2026-09-22)* |
| Heroic | Auto Attack | ✅ | 1.5s cast | 0 | 12 dmg to **2** single targets *(fixed 2026-09-22, same reason as Normal)* |
| Heroic | AoE | ✅ | 1.5s channel, 3 ticks (every 0.5s) | 10s | hits up to 5 players, 12 dmg per tick, same as Normal *(fixed 2026-09-22)* |
| Heroic | Throw Rock *(HC-only extra ability)* | ✅ | 1s cast | 12s | 30 dmg, single target *(fixed 2026-09-22)* |

Boss 2 is the simplest boss overall — no dedicated "State Data" ability prefab exists for it (unlike
Boss 1's Rage and Boss 3's Dark Pact), consistent with it being the least fleshed-out of the three.

### Boss 3 — Encounter 3

Party size for this fight is **10** (2 Tank, 2 Heal, 6 DPS via `EncounterPartySettings._charactersForced`
on `Encounter 3 Normal.prefab`) — fixed 2026-09-22, it was accidentally set to 20 forced characters.

| Difficulty | Ability | Ready? | Cast/Channel | Cooldown | Damage/Effect |
|---|---|---|---|---|---|
| Normal | Auto Attack | ✅ | 2.5s cast | 0 | 20 dmg, prioritizes tanks (randomly picks one of the fight's 2 tanks — same priority mechanism as Boss 1/2, no special config needed) *(fixed 2026-09-22, was 0 dmg)* |
| Normal | Necrotic Curse *(renamed from generic "Skill Damage")* | ✅ | 1s cast | 8s | 40 direct damage, plus a debuff (new `State Data - Boss 3 Necrotic Curse Debuff.prefab`) dealing an additional 60 damage over 4s (15 dmg/tick every 1s) *(built 2026-09-22)* |
| Normal | Max Health | ✅ | - | - | 500 *(raised from 100 on 2026-09-22, dev balance call)* |
| Heroic | Auto Attack | ✅ | 2.5s cast | 0 | 25 dmg, prioritizes tanks *(built 2026-09-23)* |
| Heroic | Necrotic Curse | ✅ | 1s cast | 6s | 50 direct damage, plus a debuff (`State Data - Boss 3 HC Necrotic Curse Debuff.prefab`) dealing an additional 80 damage over 4s (20 dmg/tick) *(built 2026-09-23)* |
| Heroic | Max Health | ✅ | - | - | 500, unchanged from Normal *(matches the Boss 1 / Boss 2 convention of not scaling health for HC — only ability damage/cooldowns scale)* |

Boss 3 HC was built 2026-09-23 by duplicating Boss 3 Normal and scaling numbers using the same
convention already established by Boss 1 HC / Boss 2 HC (roughly +20-25% ability damage, shorter
cooldowns, unchanged health): Auto Attack 20→25 dmg, Necrotic Curse 40→50 direct / 60→80 curse (15→20
per tick) / cooldown 8s→6s. **Fully wired and playable** — registered in
`EncounterManager._encounterPrefabs` (index 6), and the level-select UI already had a
"Button - Encounter Chooser 3 Heroic" prepared in `MainMenu.unity` (gated behind beating Boss 3
Normal, per `MenuPanelChooseEncounter`'s existing logic), so no UI work was needed beyond building the
actual fight content.

The old "Dark Pact" state-data prefab (`State Data - Boss 3 Dark Pact.prefab`) is still unused/unwired
— the dev deliberately started with these 2 simpler abilities instead ("fangen mal gerade klein an").

**Gotcha found while building Necrotic Curse:** `Skill.GenerateDescription`'s regex
(`{{(cast|channel):.*\..*}}`) is greedy and does **not** support two `{{...}}` placeholders in the same
description string — it matches from the first `{{` to the *last* `}}` in the whole string, mangling
the middle text and throwing a `FormatException` on `int.Parse`. Worked around here by hardcoding the
description text instead of templating both effects. See gotchas.md.

**Correction vs. the 2026-09-22 scouting report:** that report said Boss 3 Normal's enemy prefab had
*zero* `Skill` components, based on a text grep for the `Skill.cs` script GUID. That grep was
wrong — `Enemy 3 Normal.prefab` is a **prefab variant**, so its inherited components don't show up
as literal GUID references in its own file. Querying the live Editor (as done for this inventory)
correctly showed it has 2 skills all along.
Also see [gotchas.md](gotchas.md) for the Encounter-3-was-never-finished background and
[changelog.md](changelog.md) for the 2026-09-22 fixes.

### Boss 4 — Encounter 4 (built 2026-09-22, shelved 2026-09-23 — not in the next release)

Built per the dev's spec (cyclical enrage buff, alternating-tank auto attack, a focused nuke on a
random DD/healer), then **deliberately shelved**: the dev decided the next release ships with only
the 3 existing bosses (Normal + HC each), and Boss 4 is not part of that. It is **not registered** in
`EncounterManager`'s encounter list, so it cannot be reached in-game — confirmed by the dev's own
playtest (killed Boss 1/2/3, Boss 4 correctly did not appear). The prefabs are committed and sitting
unused; nothing further needs to be done to "hide" it.

| Difficulty | Ability | Cast/Channel | Cooldown | Damage/Effect |
|---|---|---|---|---|
| Normal | Auto Attack | 4s cast | 0 (cast-time-gated only) | 60 dmg, strictly alternates between the fight's 2 tanks (`_numberDontHitSameInARow=1`) |
| Normal | Soul Brand *(placeholder name)* | 2s cast | 6s | 50 dmg to a random DD or healer (never a tank) |
| Normal | Enrage | 2s cast | 12s | Self-buff, 6s duration: +20% haste, +20% cooldown reduction |

Two mechanics from the original spec are **not implemented** — not a bug, a hard capability gap in
the current Skill/SkillEffect system, reported back to the dev at build time rather than faked:
- **"+20% damage dealt" while enraged**: `CharacterStats.AttackRateAdditive` exists as a field but is
  never read anywhere in the codebase (same dead stat Boss 1's own Enrage already leaves at 0). Left
  at 0 rather than showing a tooltip number that would do nothing.
- **"+20% damage taken" while enraged**: no field for this exists at all, not even an unused one —
  `CharacterHealth.TakeDamage` applies incoming damage with zero multiplier hooks. Omitted entirely.
- **Soul Brand's raid-wide 10 dmg splash** (hit one target hard, everyone else lightly, in one cast):
  not achievable with one `Skill` — `SkillPerformance` applies every effect in a skill to the same
  shared target list, there's no "primary vs. rest" concept, and a character has exactly one shared
  cast slot (`CastManager` is one-per-character, not one-per-skill) so a second skill can't truly run
  in parallel to fake it either. Only the single focused 50 dmg hit was built.

All three would need an actual code change (a real damage-dealt/damage-taken multiplier hook, and
either a "primary + splash" concept in `SkillPerformance` or a way to target two groups from one
skill). Not scheduled — Boss 4 itself is shelved.

Other placeholders on this content, still using dev-supplied filler until someone does a naming/balance
pass: Character name "Boss 4", max health 500 (matched to Boss 3's post-balance tier, not tuned), party
composition copied verbatim from Boss 3 (2 Tank/2 Heal/6 DPS).

### Mythic difficulty

No `Encounter` prefab currently has `Difficulty == Mythic` (or `MythicPlus`). The Mythic button in
`MenuPanelChooseEncounter` can become visible once its unlock condition is met (see
[gotchas.md](gotchas.md) for that logic), but selecting it will fail —
`EncounterManager.SetEncounterIndex` logs "no match was found" and doesn't start anything, since
there's no Mythic-tier boss content yet.

## Release Plan (as of 2026-09-23)

**Next release ships with exactly 3 bosses, each with a Normal and a Heroic version.** Boss 4 (and
Boss 5) are explicitly **out of scope** for it — Boss 4 exists as unused, unregistered content (see
above) and Boss 5 is still just an idea. **Boss 3 HC was the last missing piece for that target and is
now done (2026-09-23)** — the 3×2 release scope is complete content-wise, pending playtesting.

## Design Plan / Roadmap (as of 2026-09-22)

This is the dev's intended target state for early progression. Keep this section current — it's the
thing to check before assuming what "should" unlock where.

### Intended skill-unlock progression

| Step | Encounter | Unlocks | Status |
|---|---|---|---|
| Start | - | Greater Heal, Renew, [DEBUG] Kill Enemy (3 starting skills) | **Done (2026-09-22)** |
| Boss 1 (unchanged fight) | Encounter 1 Normal | Circle of Healing | Already correct, no change needed |
| Boss 2 (unchanged fight, has the AoE) | Encounter 2 Normal | Power Word: Shield | Already correct, no change needed |
| Boss 3 (redesign planned — see below) | Encounter 3 Normal | Shadow Word: Death | **Done (2026-09-22)** — unlock condition already existed and now actually fires, since Shadow Word: Death was removed from the starting loadout |
| Boss 4 (new boss, built but shelved 2026-09-23) | Encounter 4 Normal | Penance | **Shelved, not in the next release.** The boss itself is built (see the Boss 4 section above) but deliberately not registered in `EncounterManager`, and this unlock condition was never added. Penance's stats are already rebalanced (mana 4, cooldown 5s) and ready for whenever Boss 4 actually ships. |
| Boss 5 (idea only, not implemented) | Encounter 5 Normal | *(tbd)* | **TODO** — design idea only |

### Boss design ideas

- **Boss 3 rework idea:** hits regularly for heavy damage, specifically to pressure the player into
  using Power Word: Shield to survive. Boss 3 is currently unfinished content (see the correction
  note above and [gotchas.md](gotchas.md)) — this rework would happen as part of finishing it, not
  on top of the current placeholder abilities.
- **Boss 4 idea (new boss):** gets stronger the longer the fight goes on (e.g. a stacking
  enrage/haste effect over time, forcing the group to end the fight quickly rather than turtle).
  Unlocks Penance on first kill.
- **Boss 5 idea (new boss, rough concept):** 3 allies take near-lethal (~100% HP) hits in quick
  succession; the intended counterplay is Penance, since its channel performs multiple ticks that
  can each land on a *different* target — landing 3 fast ticks across the 3 endangered allies is
  meant to be the "solution" to the mechanic.
- **Mind Blast rename idea:** once Shadow Word: Death is no longer a guaranteed starting skill (see
  TODO above), consider renaming/repurposing the `Player Skill Cast Damage` skill from
  "Shadow Word: Death" to "Mind Blast". Not urgent, just noted so it isn't forgotten.

### Other content decisions

- **Despell:** intentionally left out of the unlock chain for now — may end up unused entirely.
- **Quick Heal:** intentionally left out of the unlock chain for now. Its broken "ERROR" tooltip
  (see table above) is *not yet fixed* — dev floated maybe using this skill as a special/unusual
  unlock later (not a normal boss-clear reward), so it's being kept but deprioritized rather than
  deleted.
- **Penance rebalance:** Mana cost 10 → 4, Cooldown 0s → 5s. **Done 2026-09-22.**

### TODOs & small plans

1. ~~**Remove Shadow Word: Death from the starting loadout.**~~ **Done 2026-09-22.** Removed the
   `Player Skill Cast Damage` entry from `SkillLearnSystem._initialSkillsChosenPrefabs`. The existing
   Boss 3 unlock condition is now live.
2. ~~**Apply the Penance rebalance.**~~ **Done 2026-09-22.** `Player Skill Channel Heal`'s
   `SkillMana._manaCost` 10 → 4, `SkillCooldown._localCooldownApply` 0 → 5.
3. **Fix Quick Heal's "ERROR" tooltip.** Plan: same root cause class as the old Power Word Shield
   bug — inspect `Player Skill Instant Heal`'s `SkillCast._effectsCastFinish` (currently empty, which
   is why `{{cast:0.0}}` fails) and either populate that list properly or replace the description
   with hardcoded text pulled from whatever effect the skill actually performs. Needs the same kind
   of investigation done for Power Word Shield. Not scheduled — Quick Heal isn't obtainable anyway.
4. ~~**Build Boss 4.**~~ **Built 2026-09-22, shelved 2026-09-23** — see the Boss 4 section above.
   Deliberately not registered in `EncounterManager` and not part of the next release. No unlock
   condition for Penance was added since the boss isn't reachable.
5. ~~**Build Boss 3 HC.**~~ **Done 2026-09-23.** Auto Attack and Necrotic Curse scaled up (see the
   Boss 3 HC row above), registered in `EncounterManager`, and reachable via the pre-existing
   "Button - Encounter Chooser 3 Heroic" once Boss 3 Normal is beaten. This completes the next
   release's 3-bosses-×-2-difficulties content scope. The unused "Dark Pact" state data is still
   intentionally unwired ("fangen mal gerade klein an").
6. **Design & build Boss 5** (idea stage only — mechanic above needs to be fleshed out before it's
   buildable). Real content work, biggest unknown of the list.
7. **Investigate the Circle of Healing "consumes resources, heals nothing" bug** (see the ⚠️ row
   above). Debug logging added 2026-09-22, waiting on a reproduction with console output.
8. **Circle of Healing hover-targeting** (see the design-idea note above the Player Skills table) —
   always castable, guaranteed mouse-over ally as one of the targets when hovering a living ally.

#1 and #2 are done (2026-09-22). #5 is partially done. #3 (Quick Heal tooltip), #6 (Boss 5) and #8
(hover-targeting) need more design/investigation before implementing. #7 is actively being
investigated (debug logs in place, uncommitted).
