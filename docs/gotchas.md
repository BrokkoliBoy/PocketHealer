# Gotchas

Non-obvious traps discovered while working on this codebase. Read this before assuming a bug is
where it looks like it is.

## Two different "the skill bar" instances exist — know which one you're looking at

The live in-combat action bar and the pre-fight "Configuration" screen's bar are **two separate
`SkillBar` components**, often literally named the same thing in the hierarchy (e.g. both called
"Skill Bar - NormalHc"). Only the Configuration screen's bar is wired to
`PlayerSkillConfiguration`'s data-list events. The live combat bar (owned by a `SkillUiManager`) is
populated by a one-way copy at encounter start and is visually drag-enabled but never writes back.
This caused a real bug (2026-09-21/22): dragging a skill during combat silently deleted it from the
save data, because `DragAndDropZoneSkill.DropPhysically` unconditionally calls
`PlayerSkillConfiguration.RemoveSkillFromLists` (hardcoded, independent of which bar owns the zone),
but nothing was listening for the corresponding "add it back" event on that bar. Fixed by forcing
`IsInConfigurationMenu` closed on `EncounterManager.OnEncounterInitialize`, so dragging is blocked
outside the Configuration screen entirely, rather than trying to make the live bar persist changes.
**If a drag-and-drop bug ever resurfaces, check which `SkillBar` instance is actually involved
before assuming the fix from 2026-09-22 regressed** — grep for `SkillBar.Initialize` call sites
(`PlayerSkillConfiguration.Awake()` vs. `SkillUiManager.Start()`) to tell them apart.

## `PlayerSkillConfiguration.AddSkillToList`'s index-based placement fails silently

If you call it with an explicit `index` and that slot is already occupied, it returns `false` and
does **nothing** — the caller is responsible for a fallback. The three `OnSkillDroppedPhysically*`
handlers (`OnSkillDroppedPhysicallyAvailable/NormalHc/Mythic`) now retry with no index (first free
slot) if the indexed placement fails, so a skill can't silently vanish from the list on a collision.
If you add a new caller of `AddSkillToList` with an explicit index, remember to check the return
value.

## Doozy `UIButton` can't deactivate its own GameObject inside its own click handler

Doozy's click pipeline needs to `StartCoroutine` on the button's GameObject right after the click
handler runs; if the handler already called `gameObject.SetActive(false)` on that same object
synchronously, the coroutine start throws ("Coroutine couldn't be started because the game object
... is inactive"). Fix pattern used throughout: defer the `SetActive(false)` by one frame via a
tiny coroutine (`yield return null;` then deactivate) instead of doing it inline. See
`GameFileUI.DeactivateLoginButtonNextFrame` / `DeactivateDeleteButtonNextFrame` for the pattern to
copy if you add another self-deactivating button.

## `IsInConfigurationMenu` is one single global flag, not scoped per-bar or per-context

`DragAndDroppable.StartDrag()` gates **all** dragging, everywhere, on
`PlayerSkillConfiguration.IsInConfigurationMenu`. It doesn't know or care which `SkillBar` a zone
belongs to. Anything that can leave this flag in the wrong state (e.g. opening the Configuration
screen and never explicitly closing it before some other flow starts) silently changes drag-and-drop
behavior everywhere else in the game too.

## Mythic-difficulty unlock is not "beat every boss"

`EncounterManager.HighestEncounterIndexNormal`/`HighestEncounterIndexHeroic` are **counts of
currently-registered encounters**, not "the number of the last boss ever added" or "all bosses
beaten". `MenuPanelChooseEncounter` shows Mythic once the player has beaten whichever encounter
currently holds that highest number, on both Normal and Heroic. This is fragile: adding a new boss
to the roster immediately raises the threshold, so a player who already had Mythic unlocked (under
the old, smaller roster) would need to beat the new top boss again to keep it. Not a bug today
(boss numbers are sequential with no gaps and the roster only has as many bosses as are finished),
but worth remembering before adding boss 3/4/5.

## No dedicated save-file deletion existed before 2026-09-21

`GameFileManager.GenerateGameFilesFromDisk()` auto-fills any save slot whose `.json` is missing with
a fresh file, so manually deleting the file always worked. A proper Delete button
(`GameFileManager.DeleteSafeFile` + a `GameFileUiManager.ShowUis()` refresh, visible only on slots
with a real character) was added 2026-09-21, with no confirmation dialog (deliberate — immediate
delete, by explicit choice, not an oversight).

## Vendored third-party code (`Assets/Extern/Doozy`, `Assets/Extern/AllIn1SpriteShader`) needed hand-patching for Unity 6.6

Neither plugin is actively maintained/updated for newer Unity API changes, so upgrading the Editor
surfaces real compile errors in vendored source, not just package-manifest issues. Already patched
as of 2026-09-22 (see changelog.md for the exact APIs): `Object.GetInstanceID()` →
`Object.GetEntityId()`, `AssetDatabase.GetAssetPath(int)` → the `Object` overload,
`EndNameEditAction` → `AssetCreationEndAction`, `[OnOpenAsset]` callback signature `int` → `EntityId`
+ `EditorUtility.EntityIdToObject`. If a *future* Unity upgrade surfaces more of these, the fix
pattern is the same: don't guess the new signature, check `UnityEditor.xml`/`UnityEngine.CoreModule.xml` inside the installed Editor's `Editor/Data/Managed/` folder for the exact
member, and cross-check against real usage in the Editor's own bundled packages under
`Editor/Data/Resources/PackageManager/BuiltInPackages/**/*.cs` before writing the replacement.

## `com.unity.ugui` / `com.unity.textmeshpro` don't survive automatic project upgrades cleanly

Twice now (this project and ConwaysGameOfLife), upgrading past the Unity version where
`UnityEngine.UI` was split out of the built-in modules into the standalone `com.unity.ugui` package
did **not** automatically add that package to `Packages/manifest.json`, causing every
`UnityEngine.UI` reference (`Image`, `Text`, `Graphic`, `ScrollRect`, `Slider`, `Outline`,
`LayoutElement`, ...) to fail to compile. Fix: look up the exact recommended version from the
installed Editor's own bundled default manifest
(`<EditorInstallPath>/Editor/Data/Resources/PackageManager/Editor/manifest.json`, search for
`com.unity.ugui`) and add it to the project's `Packages/manifest.json` by hand, then have the
Editor regain focus (or restart) to force a re-resolve — a running Editor does not notice an
external `manifest.json` edit until it regains focus.

## `Skill.GenerateDescription`'s `{{cast:X.Y}}` template can't handle two placeholders in one description

The regex (`{{(cast|channel):.*\..*}}`) uses greedy `.*`, so with two `{{...}}` occurrences in the
same string it matches from the **first** `{{` all the way to the **last** `}}`, swallowing
everything in between (including the first placeholder's own closing `}}`) as part of a single
match. This mangles the parsed indices and throws a `FormatException` from `int.Parse` on the
resulting garbage string. Found 2026-09-22 while building Boss 3's "Necrotic Curse" ability
(wanted `"Deals {{cast:0.0}} damage... applies a curse: {{cast:1.0}}"`). Workaround used: hardcode
the description text instead of templating both effects. If this needs fixing for real, the regex
would need to be non-greedy (`.*?`) or effects would need a dedicated multi-placeholder formatter —
not attempted, since a one-off hardcoded string was good enough here.

## Enemy abilities are "Prioritize tanks" for free when there are multiple tanks

`SkillRangeCustom.GetRandomPrioCharacter()` already picks a **random** character among tied
top-priority candidates (`pool[Random.Range(0, pool.Count)]`). So an ability configured to prioritize
the "Tank" role automatically distributes hits randomly across however many tanks are actually in the
fight — no extra config needed to make an attack "randomly target one of the 2 tanks" instead of
always the same one. Boss 3's "Auto Attack" (rebuilt 2026-09-22 for a 2-tank fight) relies on exactly
this — it's configured identically to Boss 1's single-tank Auto Attack.
