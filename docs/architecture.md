# Architecture

A WoW-Priest-inspired 2D healing sim: heal your group through boss fights across two difficulty
tiers (Normal / Heroic), with a spell loadout you configure between fights. A Mythic tier (plus a
stubbed-out Mythic+) exists in the data model and UI plumbing but its selection buttons are
force-hidden (2026-09-23) — the dev decided against designing/implementing it, at least for the
next release; see the "Mythic difficulty" note below and gotchas.md.

Namespace is `Gavi` throughout (a few files use `Gavi.Base`, `Gavi.Encounter`, `Gavi.Skills`, etc.
as sub-namespaces). Most systems are singleton `MonoBehaviour`s (`public static X Instance`) set in
their own `Awake()`, with no null-checking on `Instance` access elsewhere — assume they exist once
the scene is running.

## Save / login flow

- **`SafeFile`** (in `Assets/Scripts/Game/Game File/GameFileManager.cs`) — a `[Serializable]` class,
  JSON-serialized via `JsonUtility`, one per save slot. Holds `CharacterName`, `FileIndex`, and a
  `GameFile` (skills + encounter progress). Written to
  `GameFileManager.SafeFileDirectoryPath + "/" + FileNamePlusExtension` (`Application.dataPath +
  "/SaveFiles"`, so inside `Assets/` in the Editor).
- **`GameFileManager`** — singleton. `GenerateGameFilesFromDisk()` reads all `.json` files in the
  save folder and **auto-fills any missing slot** with a fresh `SafeFile.CreateNewGameFile()` — this
  is why manually deleting a save file "just works" without any special-casing.
  `LoadGameFile(SafeFile)` calls `GameFlowManager.Instance.StartGame(file.GameFile)`.
- **`GameFileUiManager`** — calls `GenerateGameFilesFromDisk()` and applies each `SafeFile` to one of
  3 `GameFileUI` slot components via `ApplyGameFileUi`.
- **`GameFileUI`** (one per save slot) — owns the Login / "enter name" / Delete buttons for that
  slot. `ApplyGameFileUi` decides slot state purely from whether `CharacterName` is empty/default:
  shows the name-entry UI for a fresh slot, otherwise shows Login + Delete. **Doozy `UIButton`
  gotcha**: a button cannot synchronously `SetActive(false)` its own GameObject inside its own click
  handler (Doozy needs to `StartCoroutine` on it right after); both the Login-button-hide and the
  Delete-button-hide are deferred by one frame via a coroutine for this reason — see gotchas.md.
- **`GameFlowManager`** — `StartGame(GameFile)`: if the file has no skill data yet, calls
  `SkillLearnSystem.LearnInitialSkills()` (assigns the starter skill set) and saves; otherwise loads
  the skill list and encounter progress from the `GameFile`.
- **`GameFileManager.DeleteSafeFile(SafeFile)`** (added 2026-09-21) deletes the `.json` on disk;
  callers must trigger a `GameFileUiManager.ShowUis()` refresh afterward so the slot regenerates.

## Skill system — two separate bar instances, don't conflate them

This is the single most important architectural gotcha in the codebase — see gotchas.md for the
bug this caused.

- **`PlayerSkillConfiguration`** (`Assets/Scripts/Game/Skills/PlayerSkillConfiguration.cs`) —
  singleton, the single source of truth for which skills the player has and which are slotted where.
  Owns three lists: `_skillsAvailable`, `_skillsChosenNormalHc`, `_skillsChosenMythic` (all
  `List<Skill>`, empty slots are `null`, index = bar position). Owns the **"Configuration" screen's**
  `SkillBar`s (`_skillBarNormalHc`, `_skillBarMythic`, `_skillBarsAvailable`) and is the only place
  that listens to those bars' `SkillDragged` / `SkillDroppedPhysically` events — i.e. **dragging only
  ever mutates the data model when it happens on one of these specific bar instances.**
  `IsInConfigurationMenu` is a single global bool gating all dragging everywhere (see
  `DragAndDroppable.StartDrag()`), toggled by `OnOpenPanelConfiguration()` /
  `OnClosePanelConfiguration()`, and forced closed on `EncounterManager.OnEncounterInitialize` (added
  2026-09-22) so it can't leak `true` into combat.
- **`SkillUiManager`** (`Assets/Scripts/Game/Skills/Skill Ui/SkillUiManager.cs`) — a generic
  "render this Character's active skills as a `SkillBar`" component. The **live in-combat action
  bar** is a `SkillUiManager` instance with its **own separate `SkillBar`**, populated **once**, at
  encounter start, by `SkillManagerPlayer.AssignSkillsChosen()` copying
  `PlayerSkillConfiguration.GetSkills(difficulty)` into it via `AddUi`/`_skillBar.AddSkillManually`.
  It is visually a drag-and-drop bar (same `DragAndDropZoneSkill` zones), but its drop events are
  **not** wired to `PlayerSkillConfiguration` — dragging on it does not persist.
- **`SkillBar`** (`Assets/Scripts/Game/Skills/Skill Bar/SkillBar.cs`) — the reusable bar component
  itself. `Initialize()` runs once (`_isInitialized` guard) at `Start()`, scans existing child drop
  zones under `_parentSkillDropZones`, and wires each one's `PreSkillDragged` /
  `PostSkillDroppedPhysically` / `PostSkillDroppedManually` events to its own
  `OnSkillDragged`/`OnSkillDroppedPhysically`/`OnSkillDroppedManually`, which re-broadcast as the
  bar's own `SkillDragged`/`SkillDroppedPhysically`/`SkillDroppedManually` `UnityEvent`s. Whoever
  wants to react to drops on a given bar must subscribe to **that specific bar instance's** events —
  there is no global "a skill was dropped somewhere" event.
- **Drag-and-drop primitives** (`Assets/Scripts/Game/Misc/Drag and Drop/`): `DragAndDroppable`
  (mouse-driven drag lifecycle, gated by `PlayerSkillConfiguration.IsInConfigurationMenu`),
  `DragAndDropZone` (a slot; tracks `_currentDroppable`), `DragAndDroppableSkill` /
  `DragAndDropZoneSkill` (skill-specific subclasses). `DragAndDropZoneSkill.DropPhysically` has a
  hand-written recursive swap workaround (with its own long inline comment) for the two-skills-swap
  case, and unconditionally calls `PlayerSkillConfiguration.RemoveSkillFromLists` regardless of which
  bar the zone belongs to — this is exactly why a skill can be silently removed from the data model
  even on a bar nobody is listening to (see gotchas.md).
- **`SkillLearnSystem`** — `_skillsToUnlock` maps an encounter number+difficulty to a skill prefab;
  on `EncounterManager.OnEncounterSuccess` it unlocks any matching skill via
  `PlayerSkillConfiguration.UnlockSkill(prefab, BarType.ActiveNormalHc)` (appends to the first free
  slot, falls back to the Available list if the active bar is full).

## Encounters / bosses

- **`EncounterManager`** — registry of `Encounter` prefabs (`GatherEncounterInfo()`).
  `HighestEncounterIndexNormal`/`HighestEncounterIndexHeroic` are just **counts** of registered
  Normal/Heroic encounters, not "the last boss ever added" — see gotchas.md for the Mythic-unlock
  trap this causes. Fires `OnEncounterInitialize` (fight start), `OnEncounterSuccess` (boss died),
  `OnEncounterClear` (leaving/resetting the encounter).
- **`GameProgress`** — tracks beaten encounters per difficulty as `List<int>` (encounter numbers).
  `HasEncounterSuccess(number, difficulty)` checks membership in that specific list, nothing fuzzier.
- **`MenuPanelChooseEncounter.BuildMenu()`** — decides which difficulty buttons are visible per
  encounter. **Mythic and Mythic+ buttons are hard-hidden as of 2026-09-23** (`button.Show(false)`,
  unconditionally) — Mythic is not planned for now, so its old unlock check (would-be shown once
  `HasEncounterSuccess(HighestEncounterIndexNormal, Normal)` **and** the same for Heroic are both
  true, i.e. "beaten the current highest-numbered registered boss on both difficulties", not "beaten
  every boss ever" — misleading local variable names `hasSuccessInAllNormal`/`hasSuccessInAllHeroic`
  notwithstanding) is left commented out in place, ready to restore if Mythic ever ships.

## UI framework notes (Doozy)

- `UIButton.OnPointerClick` → `TriggerButtonBehavior(OnClick)` → `InitiateClick()` →
  `StartCoroutine(RunOnClickEnumerator)` → `ExecuteClick()` (for `ClickMode.Instant`, the default).
  A button's own click handler running inside this chain cannot deactivate its own GameObject
  synchronously — see gotchas.md.
- Doozy's "Nody" node graphs independently drive some panel transitions (e.g. the login screen used
  to have a node-graph-side transition racing the code-side login logic — since fixed, see
  changelog.md) — when a Doozy-driven UI flow behaves unexpectedly, check both the C# side **and**
  the Nody graph/`OnClick` persistent-call wiring in the Inspector, not just the script.
