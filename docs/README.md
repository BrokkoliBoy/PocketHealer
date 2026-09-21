# PocketHealer — Codebase Docs (for AI sessions)

This `docs/` folder exists so a fresh Claude Code session (or any other AI
session) working on this repo doesn't have to re-read every script, scene,
and prefab from scratch to get oriented. Read this before exploring the
codebase yourself.

- [architecture.md](architecture.md) — the main systems, what they do, and where they live
- [gotchas.md](gotchas.md) — non-obvious traps and quirks discovered the hard way
- [changelog.md](changelog.md) — dated log of AI-assisted sessions and what they changed, so you don't rediscover or redo the same thing
- [content-inventory.md](content-inventory.md) — living list of every player skill and boss ability (stats, unlock conditions, known gaps) — keep this one updated whenever content changes

## Quick facts

- Unity 6000.6.2f1 (upgraded from 2022.3.62f3 on 2026-09-21/22).
- UI framework: **Doozy** (`Assets/Extern/Doozy`), an older Asset Store plugin with its own node-graph system ("Nody") for panel transitions, plus `UIButton`/`UIView`/`UIPopup` components layered over uGUI.
- Tweening: **DOTween** (`Assets/Plugins/Demigiant/DOTween`).
- The `com.unity.pipeline` package is installed, so a live Unity Editor for this project can be driven directly via the `unity` CLI (`unity status`, `unity command`, etc.) instead of hand-editing scene/prefab YAML — check `unity status` first in any session that needs to touch a scene or GameObject.
- Save data lives on disk as JSON at `Application.dataPath + "/SaveFiles"` (i.e. `Assets/SaveFiles/*.json` in the Editor), not in `PlayerPrefs`.
