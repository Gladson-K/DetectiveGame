# MARKED - Tales of the Marked  (Detective Game, Version 2)

A hidden-object detective game where **every player gets a different set of cases**.

You are a detective tracing serial menaces across India. Each *Tale* is a storyline:
read the case file, search the crime scene, find every clue, study the Persons of
Interest, and **name the killer**. Close a menace's cases and they are captured.

All people and events are fictional; the cities are real-world settings.

## What's new in V2
- **Multiple Tales** (storylines) selectable from the main menu, each with several **Arcs** (menaces).
- **Randomized cases, unique per player**: victim, culprit, suspects, clues, room layout and
  briefing text are all generated from your personal detective identity - no two players see
  the same cases. *Reshuffle Case Files* rerolls an arc; *New Identity* gives a fresh set.
- **Characters & portraits**: 50+ characters. Drop your own drawings into
  `Assets/Resources/Portraits/` and they appear everywhere (briefing, suspect cards, 3D room
  victim frame, tale cards). Missing art gets an auto-generated placeholder icon.
- **Persons of Interest & Accusation**: read alibis and statements, then accuse a suspect.
  Clues in your journal name who they "point to". A wrong accusation costs 30 seconds;
  after two mistakes the chief offers a tip.
- **Deduction + arrests**: close all cases in an arc to arrest that menace. Close every arc
  to finish the tale.
- **Data-driven content**: add new tales, characters, clues and art by editing JSON + PNG -
  no code changes. See `Docs/V2_AUTHORING_GUIDE.md`.
- Procedural sound effects, room layout variants, and more mark shapes (X, infinity, bolt,
  wave, claw, triangle, circle).

Shipped tales:
1. **MARKED** - The X Killer and The Infinity Killer (the original six cases, now randomized).
2. **THE VELVET STATIC** - a phantom DJ, a vanished festival, and a copycat called The Echo.
3. **PAPER TIGERS** - art forgery murders and a forger who signs every fake with a claw mark.

## Requirements
- Unity 2022.3 LTS or Unity 6 (Built-in Render Pipeline). Legacy Input Manager (default).
- If Unity warns about the project version, choose "Continue" / upgrade.
- Install build modules in Unity Hub for the platforms you want (Windows/Mac/Linux, Android, WebGL).

## How to open and test in the Editor (5 minutes)
1. In Unity Hub: Add -> select the `DetectiveGame` folder -> open it (first import takes a few minutes).
2. On first open the project auto-creates `Assets/Scenes/Main.unity` and adds it to Build Settings.
   If the scene is not open, double-click it in `Assets/Scenes`.
3. Press Play. No wiring needed; everything is built at runtime.
4. Menu: pick a Tale -> **Open File** -> **Case 1** -> read the briefing -> **Begin Investigation**.

### Test checklist
- [ ] Main menu shows your identity code and three tale cards with portrait grids.
- [ ] Opening a tale lists its arcs and case slots (each slot has a code like `#7F3A`).
- [ ] A random victim/culprit/clue set is generated; **Reshuffle Case Files** changes them
      (only while the arc has no solved cases).
- [ ] Briefing shows the victim portrait; **Persons (P)** opens suspect cards with portraits
      and alibis; **Journal (J)** logs clues and who they point to.
- [ ] Clicking props collects clues (counter increments, popup + sound).
- [ ] The killer's mark is one of the clues in the room.
- [ ] **Accuse** any time: correct suspect -> CASE CLOSED; wrong -> +30s and a shake.
- [ ] After 2 wrong accusations the chief's tip highlights the culprit.
- [ ] Solving all cases in an arc shows MENACE CAPTURED; the menu shows ARRESTED.
- [ ] **New Identity** wipes progress and produces a different case set.
- [ ] Console shows no errors during a full playthrough.

## Your own storylines & art
- Add a tale: drop a JSON file in `Assets/Resources/Tales/` (copy an existing one as a template).
- Add a character: add them to a tale JSON, then drop `{characterId}.png` in
  `Assets/Resources/Portraits/`.
- Validate everything with **Detective Game > Validate Tale Content** (lists missing portraits,
  broken ids, too-few clues, etc.).
- Full step-by-step guide: **[`Docs/V2_AUTHORING_GUIDE.md`](Docs/V2_AUTHORING_GUIDE.md)**.

## Build for every platform
Menu **Detective Game** -> Build Windows 64 / macOS / Linux 64 / Android APK / WebGL
(output in `Builds/`). Or use File -> Build Settings.

### Android
1. Unity Hub: install Android Build Support (SDK, NDK, OpenJDK).
2. File -> Build Settings -> Android -> Switch Platform.
3. Connect a phone with USB debugging, or use **Detective Game -> Build Android APK**.
4. Touch = tap objects. Landscape is enforced. Use the on-screen Journal/Persons buttons
   (no J/P keys on phones).

## Project layout
```
Assets/
  Resources/
    Tales/       tale_*.json  (MARKED, THE VELVET STATIC, PAPER TIGERS - add your own)
    Portraits/   {characterId}.png  (drop your drawings here)
  Scripts/
    Core/         GameController (flow), CaseGenerator (randomizer), SeedService (identity),
                  CrimeSceneBuilder, ClueManager, ClickDetector, CaseCompletionHandler, SfxBank
    Data/         TaleModels (JSON models), TaleLibrary (loader/validator), ClueDefinition
    Characters/   PortraitLibrary (images + placeholders), CharacterPanelUI (suspects/accusation)
    Interactables IInteractable, CluePickup
    UI/           UIKit (runtime UI helper), MainMenuUI, InGameUI, ClueCounterUI,
                  ClueJournalUI, CluePopupUI, CaseTimer
    Editor/       ProjectSetup, BuildScript, ContentPostprocessors (auto sprite import),
                  ContentValidator (menu validation)
```

## Deviations / notes
- Everything (scene, UI, props) is generated by code, so no manual Inspector wiring is required.
- Content is JSON in `Resources` (works on Windows/macOS/Linux/Android/WebGL, no file IO issues).
- Legacy `UnityEngine.UI.Text` instead of TextMeshPro (no import step).
- No external packages or art required; portraits are optional with procedural placeholders.
- Authored against Unity 2022.3 / C# 9 and compile-checked; if you hit a Unity-only compile
  error, send it back and it can be fixed quickly.
