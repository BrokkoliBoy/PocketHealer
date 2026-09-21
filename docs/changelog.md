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
