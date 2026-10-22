# Changelog

## V2.0
- **Multiple Tales**: tale select menu with progress per tale; three shipped tales
  (MARKED, THE VELVET STATIC, PAPER TIGERS), each with multiple menaces (arcs).
- **Per-player randomized cases**: a personal detective identity seeds every case -
  victim, culprit, suspects, clue set, clue placement, room layout (3 variants) and
  briefing text are generated fresh. Reshuffle per arc; New Identity resets everything.
- **Characters & portraits**: 50+ characters; drop PNGs in `Resources/Portraits/` and
  they are used in briefings, suspect cards, the 3D victim frame and tale cards.
  Procedural placeholder icons when art is missing.
- **Persons of Interest + Accusation**: suspect alibis/statements, clues that name who
  they point to, accusation any time, +30s wrong-guess penalty, chief's tip after two
  mistakes. Cases only close when the right suspect is named.
- **Deduction content model**: Guilty / Herring / Flavor clues and per-clue implications.
- **New mark shapes**: bolt, wave, claw, triangle, circle (plus X and infinity).
- **Procedural sound effects** (no audio assets needed).
- **Data-driven authoring**: JSON tales in `Resources/Tales`, auto sprite import,
  and a **Validate Tale Content** menu tool (missing portraits, broken ids, thin clue pools).
- Added `Docs/V2_AUTHORING_GUIDE.md`.

## V1.0
- Basic gameplay complete: 6 cases, clue collection, journal, popup, counter, timer,
  case solved, killer capture progress.
- Windows, macOS, Linux, Android, WebGL build menu.
