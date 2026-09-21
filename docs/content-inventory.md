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

| Skill (internal name) | Mana | Cast | Channel | Cooldown | Target | Unlock | Description |
|---|---|---|---|---|---|---|---|
| Greater Heal | 5 | 2s | - | 0 | Ally | Starting skill | Mana-efficient, low-output heal for 40 health. |
| Renew | 5 | instant | - | 4s | Ally | Starting skill | HoT: heals 10 every 3s for 15s (5 ticks, 50 total). |
| Shadow Word: Death | 4 | 1.5s | - | 0 | Enemy | Unlocked on beating **Boss 3 Normal** *(fixed 2026-09-22 — used to also be a starting skill, which made this unlock condition dead code; see changelog.md)* | Deals 12 damage. |
| Circle of Healing | 8 | instant | - | 7s | Up to 5 allies (AoE) | Unlocked on beating **Boss 1 Normal** | Heals up to 5 allies for 40 health each. |
| Power Word: Shield | 5 | instant | - | 4s | Ally | Unlocked on beating **Boss 2 Normal** | Shields the target for 15s, absorbing 40 damage. *(Tooltip text fixed 2026-09-22 — used to show a leftover placeholder string.)* |
| Penance | 4 | - | 1.5s (tick every 0.5s) | 5s | Ally (per its `SkillRange`, despite the description also mentioning an enemy-damage variant) | **Not obtainable yet** — meant to unlock on beating **Boss 4**, which doesn't exist yet | Channeled heal, ticks for 11 per 0.5s. *(Rebalanced 2026-09-22: mana 10→4, cooldown 0s→5s.)* |
| Despell | 3 | instant | - | 5s | Ally | **Not obtainable** — intentionally left out of the unlock chain, may end up unused | Removes one random negative status effect from the target. |
| Quick Heal | 10 | instant | - | 0 | Ally | **Not obtainable** — intentionally left out for now, possibly a future special/unusual unlock | **Tooltip is broken**: shows the literal string "ERROR" instead of real text (its `{{cast:0.0}}` template points at an empty `_effectsCastFinish` list — same bug class as the old Power Word Shield issue, not yet fixed). |
| *(DEBUG) Kill Enemy* | 0 | 0.5s | - | 0 | Ally **and** Enemy | **Intentionally** part of every new save's starting loadout (per 2026-09-22 design decision) — kept in deliberately, dev-managed | Deals 10–20 damage. Its internal `Skill.Name` is also `"Shadow Word: Death"` — identical display name to the real damage skill, so it's indistinguishable in the UI. Not a bug — the dev wants it to stay for now. |

Starting bar (`_initialSkillsChosenPrefabs`, 5 slots total): Greater Heal, Renew, [DEBUG] Kill Enemy
*(fixed 2026-09-22 — Shadow Word: Death removed from this list, see changelog.md)*, 2 free slots
(filled by Circle of Healing after Boss 1 Normal, then Power Word: Shield after Boss 2 Normal).

## Bosses / Encounters

Boss ability data pulled the same way (live-instantiated + `Awake()` invoked). `-` means the field
doesn't apply or the effect prefab didn't expose a value this way.

### Boss 1 — Encounter 1

| Difficulty | Ability | Cast/Channel | Cooldown | Damage/Effect | Description authored? |
|---|---|---|---|---|---|
| Normal | Auto Attack | 2.5s cast | 0 | 20 dmg, prioritizes tanks, bonus vs healers/DDs | Yes |
| Normal | Toss Boulder | 1s cast | 5s | 30 dmg, prioritizes healers/DDs | Yes |
| Normal | Enrage | 3s cast | 25s | Self-buff: +40% haste & cooldown reduction for 12s | Yes |
| Heroic | Auto Attack | 2s cast | 0 | 25 dmg | Yes |
| Heroic | Toss Boulder | 1s cast | 4s | 35 dmg, prioritizes healers/DDs | Yes *(fixed 2026-09-22, also renamed from generic "Skill Damage")* |
| Heroic | Enrage | 3s cast | 15s | Self-buff: +40% haste & cooldown reduction for 12s (same as Normal) | Yes *(fixed 2026-09-22, also renamed from generic "Skill Damage")* |

### Boss 2 — Encounter 2

| Difficulty | Ability | Cast/Channel | Cooldown | Damage/Effect | Description authored? |
|---|---|---|---|---|---|
| Normal | Auto Attack | 1.5s cast | 0 | 10 dmg | Yes |
| Normal | AoE | 1.5s channel, 3 ticks (every 0.5s) | 10s | hits up to 5 players, 12 dmg per tick | Yes *(fixed 2026-09-22)* |
| Heroic | Auto Attack | 1.5s cast | 0 | 12 dmg | Yes |
| Heroic | AoE | 1.5s channel, 3 ticks (every 0.5s) | 10s | hits up to 5 players, 12 dmg per tick (same as Normal) | Yes *(fixed 2026-09-22)* |
| Heroic | Throw Rock *(HC-only extra ability)* | 1s cast | 12s | 30 dmg, single target | Yes *(fixed 2026-09-22)* |

Boss 2 is the simplest boss overall — no dedicated "State Data" ability prefab exists for it (unlike
Boss 1's Rage and Boss 3's Dark Pact), consistent with it being the least fleshed-out of the three.

### Boss 3 — Encounter 3

| Difficulty | Ability | Cast/Channel | Cooldown | Damage/Effect | Description authored? |
|---|---|---|---|---|---|
| Normal | Auto Attack | 2.5s cast | 0 | **0 dmg** (unfinished/untuned) | Yes (text says "Deals 0 damage...") |
| Normal | Skill Damage *(presumably "Dark Pact")* | 1.5s cast | 0 | no description text | **No** |
| Heroic | — | — | — | **Doesn't exist.** `Encounter 3 HC` folder is empty, no prefab. | — |

**Correction vs. the 2026-09-22 scouting report:** that report said Boss 3 Normal's enemy prefab had
*zero* `Skill` components, based on a text grep for the `Skill.cs` script GUID. That grep was
wrong — `Enemy 3 Normal.prefab` is a **prefab variant**, so its inherited components don't show up
as literal GUID references in its own file. Querying the live Editor (as done for this inventory)
shows it actually has 2 skills, but Auto Attack currently deals 0 damage and the second ability has
no description — so "not functional yet" still holds, just not for the reason originally stated.
Also see [gotchas.md](gotchas.md) for the Encounter-3-was-never-finished background and
[changelog.md](changelog.md) for the 2026-09-22 fix that added Encounter 3 Normal to
`EncounterManager`'s list.

### Mythic difficulty

No `Encounter` prefab currently has `Difficulty == Mythic` (or `MythicPlus`). The Mythic button in
`MenuPanelChooseEncounter` can become visible once its unlock condition is met (see
[gotchas.md](gotchas.md) for that logic), but selecting it will fail —
`EncounterManager.SetEncounterIndex` logs "no match was found" and doesn't start anything, since
there's no Mythic-tier boss content yet.

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
| Boss 4 (new boss, not implemented) | Encounter 4 Normal | Penance | **TODO** — Boss 4 doesn't exist yet. Penance's stats are already rebalanced (mana 4, cooldown 5s) and ready for whenever this unlock condition is added. |
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
4. **Build Boss 4** (new `Encounter 4` prefab set, Normal at least) with a ramping/enrage-over-time
   mechanic, plus a new `SkillUnlockCondition` unlocking Penance on its success. This is real content
   work (enemy prefab, abilities, tuning, registering in `EncounterManager`) — not a one-line fix.
5. **Finish Boss 3** (currently placeholder: 0-damage Auto Attack, undescribed "Dark Pact"-ish
   ability, no HC version) with the "frequent heavy hits" design above. Real content work, same
   caveat as #4.
6. **Design & build Boss 5** (idea stage only — mechanic above needs to be fleshed out before it's
   buildable). Real content work, biggest unknown of the list.

#1 and #2 are done (2026-09-22). #3 (Quick Heal tooltip) is mechanically small but wasn't explicitly
requested yet. #4–#6 are genuine content/design work (new bosses, abilities, balancing) that need
more back-and-forth on the actual mechanic before implementing, not just a go-ahead.
