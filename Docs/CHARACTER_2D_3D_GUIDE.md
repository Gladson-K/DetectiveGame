# Character Import & Storyline Creation Guide (2D + 3D)

This project now supports **both 2D portraits and 3D character models**. You can mix them freely across tales, arcs, and characters.

## 1) Folder layout
```
Assets/
  Resources/
    Portraits/        <- 2D images named {characterId}.png/.jpg (auto-imported as Sprite)
    Models/Characters/ <- 3D prefabs (e.g. mk_meera.prefab) loaded from Resources
    Tales/            <- JSON story files (add your own here)
  Scripts/
    Characters/       <- PortraitLibrary, CharacterPanelUI, CharacterView, Billboard3D
```

## 2) Import 2D portraits (fastest)
1. Drop `{characterId}.png` into `Assets/Resources/Portraits/`. Unity imports as Sprite.
2. Use the character `id` in the tale JSON (or set `portraitId` if the file name differs).
3. If missing, the game shows a procedural placeholder automatically.

Tips: square 512-1024px, PNG with or without transparency, simple file names.

## 3) Import 3D models (characters)
### 3a) Import the FBX
1. Put your FBX + textures in `Assets/` (e.g. `Assets/Art/Characters/`).
2. Select the FBX: set **Rig** to `Humanoid` or `Generic`. For detectives/people use Humanoid if you want to swap.
3. Materials: either extract them or use `Remap Materials`.
4. Scale: adjust `Model/Scale Factor` so the character looks right in the scene (test later).

### 3b) Make a prefab in Resources
1. Drag the model from Project into an empty scene to test.
2. Tweak position/rotation/scale until it fits.
3. Drag that instance from Hierarchy into `Assets/Resources/Models/Characters/` to create a **Prefab**.
4. Delete the test instance from the scene.
5. Keep it lightweight (low-poly preferred). Avoid massive particle systems on the root.

### 3c) Reference from JSON
In your tale JSON under `characters[]`:

```json
{
  "id": "mk_meera",
  "name": "Meera Khatri",
  "role": "Forensic Photographer",
  "city": "Chennai",
  "age": 29,
  "bio": "Photographs everything. Misses nothing.",
  "colorHex": "#38BDF8",
  "canBeVictim": false,
  "canBeSuspect": true,
  "canBeCulprit": true,
  "alibi": "Was developing prints at the lab.",
  "statements": [
    "Film never lies.",
    "I always shoot the doorway first."
  ],
  "viewMode": "Auto",
  "modelPrefab": "Models/Characters/mk_meera",
  "modelScale": 0.9,
  "modelOffset": [0, 0, 0],
  "modelRotationY": 180,
  "billboard3D": true
}
```

Fields:
| field | meaning |
|---|---|
| `viewMode` | `Auto` (use 3D if prefab exists, else 2D), `Sprite2D` (force portrait), `Model3D` (force 3D; falls back to 2D if missing). |
| `modelPrefab` / `modelResourcePath` | **Resources path** without `.prefab`. Put prefabs under `Resources/Models/Characters/`. |
| `modelScale` | Uniform scale multiplier. |
| `modelOffset` | `[x,y,z]` local offset relative to anchor. |
| `modelRotationY` | Extra Y rotation. |
| `billboard3D` | Faces camera (good for portraits/cards). Set `false` if you want fixed orientation. |

## 4) Where characters appear
- **Main menu tale cards**: 2D portraits (grid). 3D not used here by default.
- **Briefing panel**: victim shown as portrait; suspects preview uses portraits.
- **Persons of Interest (P)**: suspect cards use portraits by default. `CharacterView` can be used to render 3D in custom UI.
- **3D crime scene (victim frame)**: the game can spawn a 3D model on a scene anchor (via `CharacterView`). 2D portrait frame remains if model missing.
- **Accusation/Solved screens**: portraits.

For in-scene 3D, the game looks for a prefab in `Resources/` and instantiates under an anchor. `Billboard3D` keeps it facing the camera.

## 5) Create a new storyline from scratch
1. **Plan**: title, hook, 1-2 arcs (menaces), cast (victims + suspects), clue ideas.
2. **Copy template**: duplicate `Assets/Resources/Tales/paper_tigers.json` as `Assets/Resources/Tales/my_mystery.json`.
3. **Edit JSON**:
   - `id` unique (lowercase/underscores)
   - `title`, `subtitle`, `description`, `order`
   - `casesPerArc` (3-4 recommended)
   - `arcs[]`: `id`, `title`, `menaceName`, `markName`, `markShape` (X/Infinity/Circle/Triangle/Claw/Zig/Wave), `markColorHex`, `accentHex`, `cities[]`
   - `characters[]`: add your cast with `canBeVictim/canBeSuspect/canBeCulprit`, `alibi`, `statements`. Add `viewMode/modelPrefab` if using 3D.
   - `clues[]`: `Guilty` (implicates empty), `Herring` (implicates an innocent suspect), `Flavor`. Mark clue uses `shape` matching arc mark shape.
   - `titleTemplates/situationTemplates/briefingTemplates` with tokens: `{VICTIM}`, `{VICTIM_LAST}`, `{VICTIM_ROLE}`, `{CITY}`, `{MENACE}`, `{MARK}`, `{ARC}`, `{CULPRIT}`, `{CULPRIT_LAST}`, `{SUSPECT}`
4. **Add art**: portraits in `Resources/Portraits/`, models in `Resources/Models/Characters/`.
5. **Validate**: `Detective Game > Validate Tale Content` in Unity menu. Fix errors.
6. **Test**: Press Play -> your tale appears on menu.

## 6) Testing
### In Editor (Play Mode)
- Menu loads your tale and shows cast portraits.
- Generate cases: each arc rolls victim/culprit/suspects/clues randomly per identity.
- Check briefing, Persons (P), Journal (J).
- Accuse correctly/wrongly.
- If 3D: verify model appears, scale/offset correct, billboard faces camera.

### Headless / quick checks
From project root:
```bash
# content checks (Python)
python3 /tmp/opencode/check/validate_tales.py

# generation simulation
python3 /tmp/opencode/check/sim_cases.py
```

## 7) Troubleshooting
| issue | fix |
|---|---|
| Model not visible | Check prefab is in `Resources/Models/Characters/`. Verify `modelPrefab` path exactly (no `.prefab`). Check scale/offset. |
| Pink/missing material | Use Standard shader or ensure materials are included in build. Re-extract materials on FBX. |
| Model huge/tiny | Adjust `modelScale`. |
| Faces wrong way | Tweak `modelRotationY` (try 0/90/180). Billboard already flips 180° to face camera. |
| Falls back to 2D | `viewMode` is `Sprite2D`, prefab missing, or Resources load failed. Run validator. |
| UI looks wrong with 3D | Prefer 2D for UI cards; 3D is best in the 3D scene or with RenderTexture. |
| JSON parse errors | Check commas/brackets. Use validator. |

## 8) Performance notes
- Keep per character under ~5k tris if possible.
- One mesh preferred.
- No runtime addressables needed now (Resources is fine for small casts).
- 3D in UI can be expensive; use sparingly.

**Workflow recommendation:** Start with 2D portraits, get your storyline working, then add 3D models one by one and test with `Auto`.
