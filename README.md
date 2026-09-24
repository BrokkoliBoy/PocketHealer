# Pocket Healer

A 2D game about being the healer, and only the healer.

Your party fights automatically — you never deal damage yourself. Your only job is to keep four
other characters alive against a boss, using a kit of priest-style healing spells, mana management,
and a 5-slot action bar you configure yourself.

▶ [Playable web build on itch.io](https://brokkoliboy.itch.io/pocket-healer)

## Why I built this

I once played a mobile game with almost this exact premise — pure healing gameplay, no damage
rotation to think about — and had a lot of fun with it. Since I also generally enjoy playing healers
in games, I figured why not build my own take on the idea. This is a pure passion project, and I
deliberately aimed to make it quite hard to beat. I also wanted to find out what building a 2D game
like this actually takes, end to end — content tooling, UI plumbing, save systems, and all.

## Concept
The game is structured around individual boss encounters. Your group deals damage passively while
taking damage in return, and it's your job to counteract that with your healing spells, which are
loosely modeled after the WoW Priest's healing kit.

Spells cost mana and either heal directly or apply a temporary effect — a heal-over-time, a
damage-absorbing shield, or other buffs like increased damage dealt or reduced damage taken. You can
only bring 5 spells into an encounter at a time, and you decide which ones, which action-bar slot
they sit in, and which hotkey triggers them. You start with two basic healing spells, and beating an
encounter unlocks new ones over time.

Each boss can be fought on two difficulties, Normal and Heroic — same fight, sharper numbers.
Beating Normal unlocks Heroic for that boss.

## Under the hood

Built solo in Unity, entirely in a single scene — a deliberate choice to keep the game feeling
instant to play, with zero loading screens between menus and fights. That trade-off pushed the whole
codebase toward long-lived, reusable systems (singletons for skills, encounters, and save state)
instead of the throwaway-object patterns a multi-scene project can lean on.

A few things worth calling out for anyone poking around the code:

- A **custom drag-and-drop skill bar system** shared between the loadout-configuration screen and
  the live in-combat action bar — two different bar instances that intentionally behave differently
  (one persists your choices, one is a read-only snapshot for the current fight).
- **JSON-based save slots** that auto-populate on disk — delete a save file by hand and the game
  quietly rebuilds a fresh slot next launch, no special-casing required.
- Integrated a third-party UI framework (Doozy) for menu transitions and click feedback, then went
  deep enough into its internals to extend and repurpose it beyond what it exposes out of the box —
  including reverse-engineering its node-graph-driven navigation to safely retire a menu without
  touching the graph asset itself.
- **In-editor authoring tools** for both spells and boss encounters, so tuning a fight's damage
  numbers or a spell's cooldown doesn't require touching code.

## Where it stands

| | Progress |
|---|---|
| Bosses | 6/10 (3 of 5 planned bosses, on both Normal and Heroic) |
| Healing spells | 5/8 implemented and obtainable |
| Core systems (combat loop, saving, skill config, UI) | ~90% complete |

Development is currently paused, with two more bosses and three more spells mapped out for whenever
it picks back up.

## Looking Ahead

If I pick this back up, beyond the usual "more bosses, more spells" content work, here's what's on
my list:

- General UI polish
- Sound effects
- Visual effects for spells, so casting something actually feels like it's doing something
- A third "Mythic" difficulty tier — prototyped early on, then shelved since it worked better to
  finish Normal and Heroic across all bosses first
