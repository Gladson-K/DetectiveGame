# MARKED V2 - Authoring Guide (Tales, Characters, Portraits, Randomization)

This guide explains how to add your **own storylines, characters and art** to the game
without touching the C# code. Everything is data-driven from JSON files plus PNG images.

---

## 1. The big picture

```
Assets/
  Resources/
    Tales/       <- one JSON file per TALE (your storyline). Drop new .json files here.
    Portraits/   <- one image per character, named {characterId}.png
  Scripts/       <- game systems (you normally do not edit these)
```

A **Tale** is a storyline (e.g. "MARKED", "THE VELVET STATIC").
A Tale contains **Arcs** (a menace/killer and their set of locations).
Every Arc contains **case slots** that are **procedurally generated per player**.

Every player gets a **unique detective identity** (a random seed stored on their device).
That seed decides which victim, culprit, suspects, clues, room layout and briefing text
appear in each case - so no two players see the same case files, and re-rolls (Reshuffle)
give you a brand new set of cases within an arc.

---

## 2. Adding your own characters (+ your drawings)

### Step 1 - draw the portrait
Drop a PNG into `Assets/Resources/Portraits/` named after the character id:

```
Assets/Resources/Portraits/vs_zoya.png
```

Square, 512-1024px works best. Unity auto-imports it as a Sprite (the project
sets the import settings for you). If no image exists, the game draws a coloured
icon placeholder automatically, so you can add art later.

### Step 2 - add the character to a tale JSON

```json
{
  "id": "vs_zoya",
  "name": "Zoya Qureshi",
  "role": "Sound Engineer",
  "city": "Mumbai",
  "age": 28,
  "bio": "Kept every recording the festival ever made.",
  "colorHex": "#38BDF8",
  "canBeVictim": false,
  "canBeSuspect": true,
  "canBeCulprit": true,
  "alibi": "Was restoring a vintage console in her studio, alone.",
  "statements": ["A bolt is a sound, not a knife.", "Every venue has a back door."]
}
```

Fields:
| field | meaning |
|---|---|
| `id` | unique id (used to link clues, portraits, culprits). Prefix per tale, e.g. `vs_`. |
| `name`, `role`, `age`, `city`, `bio` | shown in briefings, cards, dossiers. |
| `colorHex` | accent colour used for placeholder art and highlights. |
| `portraitId` | optional: load a different image file than the id. |
| `canBeVictim` | can this person be the murder victim? |
| `canBeSuspect` | can this person appear in the suspect lineup? |
| `canBeCulprit` | can this person be the actual killer of a generated case? |
| `alibi` | shown on the Persons of Interest card (read these to deduce). |
| `statements` | 1-3 short in-character quotes. |

**Rules of thumb**
- A victim should not also be a suspect (set `canBeSuspect: false`).
- Provide at least 4-6 `canBeCulprit: true` suspects so re-rolls feel different.
- Provide more victims than `casesPerArc` so each case has a different victim.

---

## 3. Adding clues (the hidden objects)

```json
{
  "id": "vs_mixercable",
  "name": "Frayed Mixer Cable",
  "desc": "Cut with precision tooling only a sound engineer carries.",
  "shape": "Capsule",
  "colorHex": "#0B0B0B",
  "size": 0.4,
  "kind": "Guilty",
  "implicates": "",
  "arcs": ["arc_static"]
}
```

| field | meaning |
|---|---|
| `shape` | `Cube, Sphere, Cylinder, Capsule, Flat, MarkX, MarkInfinity, MarkCircle, MarkTriangle, MarkClaw, MarkZig, MarkWave` |
| `colorHex` / `size` | how the prop looks in the 3D room (`size` ~0.2 small, ~0.5 large). |
| `kind` | `Guilty` = points at the killer; `Herring` = points at an innocent; `Flavor` = atmosphere only. |
| `implicates` | character id this clue points to. Leave empty on `Guilty` clues and the game auto-points them at the real culprit. Herrings **must** name an innocent suspect. |
| `arcs` | list of arc ids this clue belongs to (empty = every arc in the tale). |

The deduction loop: every guilty clue that points at the culprit is real evidence;
herring clues point at innocent suspects. The journal lists "Evidence points to: NAME",
so a careful player can name the killer from the clues alone.

**The killer's mark**: the game automatically adds a mark clue to every case using the
arc's `markShape` / `markName` / `markColorHex`. Add one clue per arc with the same
`shape` as the arc mark to give it custom flavour text (see `mark_static` in the sample).

---

## 4. Adding an arc (a menace / killer)

```json
{
  "id": "arc_static",
  "title": "The Velvet Static",
  "menaceName": "the Velvet Static",
  "markName": "Bolt",
  "markShape": "MarkZig",
  "markColorHex": "#40D9F2",
  "accentHex": "#241A33",
  "cities": ["Goa", "Mumbai", "Hyderabad"]
}
```

- `cities` limits which victims can appear (a victim's `city` must be in this list).
- `accentHex` tints the crime-scene room and menus for this arc.
- Capture progress is tracked per arc: close all `casesPerArc` cases of an arc and the
  menace is ARRESTED.

---

## 5. Adding a whole new tale (your own storyline)

Create a new file, e.g. `Assets/Resources/Tales/my_story.json`:

```json
{
  "id": "my_story",
  "title": "MY STORY",
  "subtitle": "One line hook",
  "description": "Paragraph shown on the tale card.",
  "order": 10,
  "casesPerArc": 4,
  "arcs": [ ... at least one arc ... ],
  "characters": [ ... victims + suspects ... ],
  "clues": [ ... at least 4 Guilty + 2 Herring per arc ... ],
  "titleTemplates": ["The {MARK} at {CITY}", "The Case of {VICTIM_LAST}"],
  "situationTemplates": ["{VICTIM}, {VICTIM_ROLE}, was found in {CITY}..."],
  "briefingTemplates": ["A {MARK} was left behind. {MENACE} is back. Find every clue."]
}
```

Text templates support these tokens:
`{VICTIM}` `{VICTIM_LAST}` `{VICTIM_ROLE}` `{CITY}` `{MENACE}` `{MARK}` `{ARC}`
`{CULPRIT}` `{CULPRIT_LAST}` `{SUSPECT}`

`order` controls the position of the tale on the main menu (lowest first).

You can copy one of the three shipped tales as a starting template:
`marked_x_infinity.json` (the original six cases), `velvet_static.json`,
`paper_tigers.json`.

---

## 6. How randomization works (and the player identity)

- Each install gets a random **detective identity** (`#XXXXXX` shown on the menu).
- Case content is derived from `identity + tale + arc + slot + roll`. Same player,
  same arc = stable cases; different players = different cases.
- **Reshuffle Case Files** on an arc gives a new roll -> a completely new set of cases.
  Reshuffle is allowed only while that arc has no solved cases, so progress stays fair.
- **New Identity** on the main menu wipes all progress and gives a brand new unique
  set of case files for every tale.
- Room layout (3 variants), victim, suspects, culprit, clue set, clue positions,
  briefing text and theme tint are all randomized.

Because the culprit is randomized each roll, the same clue can point at the killer in
one player's game and be a red herring in another -> replay value.

---

## 7. Validating your content

In Unity: **Detective Game > Validate Tale Content** (menu bar).

It prints a report to the Console:
- missing / duplicate ids,
- arcs with too few victims, suspects, culprits, or clues,
- clues that implicate an unknown character,
- **which portraits are still missing** (placeholders will be used for those).

Fix the `ERROR` lines; `WARN` and `NOTE` lines are usually safe to ignore while drafting.

---

## 8. Quick checklist for a new tale

- [ ] 1+ arc in `arcs`, each with `cities`, `markShape`, `markColorHex`, `accentHex`.
- [ ] 4+ characters flagged `canBeVictim` whose `city` is in the arc's cities.
- [ ] 4+ characters flagged `canBeSuspect` + `canBeCulprit`.
- [ ] Every suspect has an `alibi` and at least one `statement`.
- [ ] 4+ `Guilty` clues (leave `implicates` empty) and 2+ `Herring` clues
      (each `implicates` an innocent suspect) per arc.
- [ ] One clue per arc using the arc `markShape` for custom mark text.
- [ ] Portrait PNGs dropped in `Assets/Resources/Portraits/{id}.png`.
- [ ] Run **Validate Tale Content** and clear the errors.
- [ ] Press Play -> your tale appears on the menu.
