# Changelog (AI-assisted sessions)

Dated log of what an AI session changed and why, so a future session doesn't rediscover or redo the
same work. Append to this, don't rewrite history.

## 2026-09-20/21 — resumed after a pause, WebGL build, login/save-flow bugs

- WebGL build made and published as an unlisted itch.io draft (`https://brokkoliboy.itch.io/pocket-healer`).
- Fixed `GameFileManager.LoadGameFile` never actually calling `GameFlowManager.StartGame` (the call
  was commented out, and even the commented-out version passed the wrong type — `SafeFile` where
  `GameFile` was expected). Restored as `GameFlowManager.Instance.StartGame(_currentSafeFile.GameFile);`.
  Removed related dead debug scaffolding (`GameFlowManager.YOYO()`, a stray `Debug.Log("ASD")`).
- Fixed a Doozy `UIButton` coroutine crash from a button deactivating its own GameObject inside its
  own click handler — see gotchas.md.
- Fixed "can't type a name for a new save" — root cause was a Doozy Nody node-graph panel
  transition racing the code-side login logic on the same button. Fixed by deciding
  login-vs-new-game at slot-initialization time (`GameFileUI.ApplyGameFileUi`) instead of reactively
  on button press.
- Added a Delete button to save slots with an existing character (`GameFileManager.DeleteSafeFile` +
  `GameFileUiManager.ShowUis()` refresh; immediate delete, no confirmation, by design).
- Checked and documented the real Mythic-difficulty unlock criterion (see gotchas.md).

## 2026-09-21/22 — drag-and-drop skill-disappearing bug

Initial static-analysis theory (an index collision in `PlayerSkillConfiguration.AddSkillToList`
with no rollback) turned out to be a secondary issue, not the root cause — only found after adding
temporary debug logging and watching a live repro. Real root cause and fix: see the "two different
skill bar instances" gotcha above. Also hardened the three `OnSkillDroppedPhysically*` handlers with
a fallback-to-first-free-slot if the target index turns out occupied, as defense in depth.

## 2026-09-21/22 — Unity 6.6 upgrade (2022.3.62f3 → 6000.6.2f1)

- Installed `com.unity.pipeline` for this project, so a live Editor can be driven via the `unity`
  CLI instead of hand-editing scene/prefab YAML.
- Added `com.unity.ugui: 2.6.0` to `Packages/manifest.json` — see gotchas.md, this doesn't survive
  the automatic upgrade migration.
- Fixed 5 vendored-plugin compile errors from Unity 6.6's `EntityId` API migration (see gotchas.md
  for the general pattern):
  - `Assets/Extern/AllIn1SpriteShader/Scripts/AllIn1ShaderWindow.cs` — `AssetDatabase.GetAssetPath(obj.GetInstanceID())` → `AssetDatabase.GetAssetPath(obj)`.
  - `Assets/Extern/Doozy/Engine/Progress/Progressor.cs`, `Assets/Extern/Doozy/Engine/UI/UIPopup/UIPopup.cs`, `Assets/Extern/Doozy/Engine/UI/Animation/UIAnimator.cs` — `GetInstanceID()` → `GetEntityId()` (string-id construction, drop-in swap).
  - `Assets/Extern/Doozy/Editor/GUI/Scripts/DGUI/Utils.cs` — `DoCreateCodeFile` migrated from `EndNameEditAction` to `AssetCreationEndAction`; call site now passes `EntityId.None` instead of `0`.
  - `Assets/Extern/Doozy/Editor/Nody/Windows/NodyWindowOpen.cs` — `[OnOpenAsset]` callback signature changed from `int instanceId` to `EntityId instanceId`, using `EditorUtility.EntityIdToObject` instead of the obsolete `InstanceIDToObject`.
- Everything else in that commit is expected upgrade fallout: mass `.meta` reserialization, a TMP
  Essentials reimport (new shader/shadergraph files), and new default `ProjectSettings` assets
  (`MultiplayerManager.asset`, `PhysicsCoreProjectSettings2D.asset`, `ProjectAuditorSettings.asset`).

## 2026-09-22 — content inventory, Power Word Shield fix, Encounter 3 registered, boss/skill balancing

- Fixed Power Word Shield's tooltip (leftover placeholder text) and registered Encounter 3 Normal in
  `EncounterManager`'s list (previously unreachable in-game). See gotchas.md and
  content-inventory.md for details.
- Added `docs/content-inventory.md` — full stat table for every player skill and boss ability, pulled
  live from the Editor, plus the dev's design roadmap for early progression (Boss 3/4/5 ideas, Mind
  Blast rename idea). Keep this file updated going forward.
- Added missing names/descriptions for Boss 1 HC ("Skill Damage" x2 → "Toss Boulder"/"Enrage") and
  Boss 2's AoE (Normal + HC) and HC's Throw Rock, using real values read from each ability's
  `SkillEffectDamage`/`SkillEffectAddState` component rather than guessing.
- Removed Shadow Word: Death from the starting skill loadout (`SkillLearnSystem._initialSkillsChosenPrefabs`)
  — it was previously both a starting skill and (uselessly) unlocked again on beating Boss 3 Normal.
  That unlock condition now actually fires.
- Rebalanced Penance: mana cost 10 → 4, cooldown 0s → 5s.

## 2026-09-22 (later same day) — Boss 2 tooltip correction, Boss 3 first pass, Circle of Healing bug hunt

- Fixed Boss 2's Auto Attack tooltip (Normal + HC) to say "two single targets" instead of "a single
  target" — `SkillRangeCustom._numberTargets` was already 2 (matching the fight's 2 tanks), only the
  wording was wrong.
- Boss 3 Normal, first pass at real abilities (previously 0-damage/undescribed placeholders):
  - Auto Attack now deals 20 damage, targeting priority "Tank" (randomly hits one of the fight's 2
    tanks for free, see gotchas.md).
  - Renamed the second ability "Skill Damage" → "Necrotic Curse": 1s cast, 8s cooldown, 40 direct
    damage plus a new debuff (`State Data - Boss 3 Necrotic Curse Debuff.prefab`, a `StateData` with
    `PeriodicCooldown=1`/`MaxDuration=4` ticking a 15-damage `SkillEffectDamage`, i.e. 60 damage over
    4 seconds).
  - Found and worked around a real regex bug in `Skill.GenerateDescription` that breaks with two
    `{{cast:X.Y}}` placeholders in one description — see gotchas.md.
  - Fixed `Encounter 3 Normal`'s forced party size: was accidentally 20 characters, now the intended
    10 (2 Tank / 2 Heal / 6 DPS).
  - Not done yet: Boss 3 HC (still doesn't exist), the existing unused "Dark Pact" state data
    (deliberately not wired in yet, starting small).
- Circle of Healing: dev reported mana/cooldown being consumed with no heal happening, intermittently
  (worked before). Static prefab inspection found nothing wrong with its own configuration. Added
  temporary (uncommitted) debug logging to `Skill.PerformSkill`, `SkillRangeCustom.GetPool`/
  `GetTargets`, `SkillPerformance.Perform`, and `SkillEffectHeal.PerformEffect` to catch it live -
  root cause not yet found, waiting on a reproduction with console output.
- Noted as a design TODO (not yet implemented): Circle of Healing should always be castable, using
  the mouse-over ally as a guaranteed target when hovering a living ally, and falling back to 5 random
  living allies otherwise (currently it seems to just fail to activate when hovering a dead ally).

## 2026-09-22 (later still) — root-caused the Enemy.Die() crash, removed debug logging

- Found the real cause of the intermittent `NullReferenceException` in `Enemy.Die()` (previously
  suspected to be an Encounter-lifecycle/targeting-cache bug): a leftover `Enemy 3 Normal(Clone)`
  GameObject, orphaned from an earlier live-Editor `GameObject.Instantiate()` call during this same
  session's content-inventory work, was still sitting in the open `MainMenu` scene (in-memory only,
  never saved to disk) with a live, hoverable `UiTargetCharacter`. A multi-target skill (Shadow Word:
  Death) picked it up alongside the real boss; the real boss's death fired `SuccessEncounter()`
  (nulling `EncounterManager.CurrentEncounter`) mid-loop, and the second, stale target's death then
  crashed on the now-null reference. Destroyed the stray object; confirmed fixed by reproducing twice
  (error gone, Circle of Healing also works correctly on the second battle).
- Two earlier fix attempts this session (an `OnEncounterInitialize`-driven target-cache reset in
  `SkillRangeCustom`, and moving `OnEncounterClear` to fire from `StopEncounter()`) were investigating
  a real but different theory and turned out not to be the cause of this specific crash. The second
  attempt caused a severe regression (mass `NullReferenceException`s from destroying party/skills
  while other player-side systems were still updating mid-results-screen) and was reverted immediately.
  The `SkillRangeCustom` cache-reset fix is harmless and was kept.
- Lesson for future live-Editor sessions: always `Destroy`/`DestroyImmediate` any `GameObject.Instantiate()`
  used for live stat/description inspection - a forgotten one can sit in the open scene indefinitely
  and get treated as a real character.
- Removed all temporary `DEBUG` `Debug.Log` calls added earlier while hunting this bug (`Skill.cs`,
  `SkillRangeCustom.cs`, `SkillPerformance.cs`, `SkillEffectHeal.cs`).

## 2026-09-22 (later still) — Boss 3 max health, Boss 3 Necrotic Curse registration, Boss 4 built

- Raised Boss 3 Normal's `CharacterHealth._maxHealth` 100 → 500 (dev balance call).
- Fixed Boss 3 Normal's Necrotic Curse never firing and never showing in the pre-fight ability
  overview: it was fully configured but missing from `SkillManager._initialSkillsPrefab`, so it was
  never instantiated/assigned. Both live combat and `EnemySkillsInfoPanel` read from that same
  assigned-skills list, so it was invisible to both. Added it at index 1.
- Built Boss 4 (`Assets/Prefabs/Encounters/Encounter 4/Encounter 4 Normal/`) from the dev's spec: a
  cyclical self-enrage buff (+20% haste/cooldown reduction for 6s, 12s cooldown), an alternating-tank
  auto attack (60 dmg, 4s cast), and a focused nuke on a random DD/healer (50 dmg, 2s cast, 6s
  cooldown). Two parts of the spec (bonus damage dealt/taken while enraged, and a raid-wide splash on
  the nuke) aren't supported by the current Skill/SkillEffect system and were reported back rather
  than faked — see `content-inventory.md`'s Boss 4 section for exactly why. Built entirely via live
  Editor prefab edits (`PrefabUtility.LoadPrefabContents`/`SaveAsPrefabAsset`), no code touched.
  **Not registered in `EncounterManager`** — see the next entry.

## 2026-09-23 — Boss 4 shelved, next release scoped to 3 bosses, Boss 3 HC is now the active TODO

- Dev decision: the next release ships with exactly the 3 existing bosses, each Normal + HC. Boss 4
  is explicitly **out of scope** and stays shelved (built, committed, but not registered in
  `EncounterManager`, so unreachable in-game) — confirmed by the dev's own playtest (killed Boss
  1/2/3, Boss 4 correctly never appeared). No further action needed to "hide" it.
- Active TODO going forward: **build Boss 3 HC** (`Encounter 3 HC` currently only has an empty
  `.meta`, no prefab) — the last piece needed for the 3×2 release target. See
  `content-inventory.md` for the up-to-date status.
- Built Boss 3 HC (`Assets/Prefabs/Encounters/Encounter 3/Encounter 3 HC/`) right after, in the same
  session: duplicated Boss 3 Normal and scaled numbers using the existing Boss 1 HC / Boss 2 HC
  convention (~+20-25% ability damage, shorter cooldowns, health unchanged) — Auto Attack 20→25 dmg,
  Necrotic Curse 40→50 direct / 60→80 curse-over-4s (15→20 per tick) / cooldown 8s→6s. Registered the
  new `Encounter 3 HC.prefab` in `EncounterManager._encounterPrefabs` (scene edit, `MainMenu.unity`
  saved). The level-select UI already had a "Button - Encounter Chooser 3 Heroic" prepared and gated
  behind beating Boss 3 Normal, so it's immediately playable with no UI changes needed. This completes
  the next release's 3-bosses-×-2-difficulties content scope.
