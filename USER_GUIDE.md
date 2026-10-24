# DetectiveGame - User Guide (V2)

## Table of Contents
1. [Quick Start](#quick-start)
2. [Opening in Unity](#opening-in-unity)
3. [Playing the Game](#playing-the-game)
4. [Adding Custom Cases & Storylines](#adding-custom-cases--storylines)
5. [Adding 2D Characters & Portraits](#adding-2d-characters--portraits)
6. [Adding 3D Characters & Models](#adding-3d-characters--models)
7. [Using the Game Launcher](#using-the-game-launcher)
8. [Testing End-to-End](#testing-end-to-end)
9. [Docker Setup (Reproducible Environment)](#docker-setup-reproducible-environment)
10. [Troubleshooting](#troubleshooting)

## Quick Start

**Requirements:**
- Unity 2022.3.45f1 (or 2022.3 LTS)
- Windows 10/11, macOS, or Linux
- 4GB RAM minimum, 8GB recommended
- 2GB free disk space

**Project Location:** `/mnt/d/Workspace_Glad/Projects/DetectiveGame` (or clone to your local drive)

## Opening in Unity

### Step 1: Install Unity Hub
1. Download Unity Hub from [unity.com/download](https://unity.com/download)
2. Install and sign in (Free Personal license is fine)

### Step 2: Add Project
1. Open Unity Hub → Projects → Open → Add project from disk
2. Select the `DetectiveGame` folder
3. Unity Hub will prompt for editor version - select `2022.3.45f1` (install if missing)
4. Open the project (first import takes 2-5 minutes)

### Step 3: Open Scene
1. In Project window, go to `Assets/Scenes/`
2. If `Main.unity` exists, double-click it
3. If missing, the game auto-creates it on first play (via ProjectSetup)

### Step 4: Play
Press Play (▶) button in Unity Editor. The main menu will appear.

## Playing the Game

### Main Menu
- **Identity Code**: Your unique detective ID (e.g., #A3F7) - determines your case randomization
- **Tale Cards**: Select a storyline (MARKED, THE VELVET STATIC, PAPER TIGERS)
- **New Identity**: Generate fresh random cases (resets progress)
- **Sound Toggle**: Enable/disable sounds

### Investigation Flow
1. **Select Tale** → Choose a storyline
2. **Select Arc** → Choose a menace/killer
3. **Select Case** → Pick an unlocked case slot
4. **Briefing** → Read case file, see victim, get overview
5. **Begin Investigation** → Enter crime scene
6. **Collect Clues** → Click objects to find clues (HUD shows count)
7. **Journal (J)** → Review collected clues, see who they point to
8. **Persons of Interest (P)** → View suspects, alibis, statements
9. **Accuse** → When ready, accuse the culprit
10. **Case Solved** → See results, move to next case or menu

### Tips
- Clues in journal say "Evidence points to: [Name]" - use this for deduction
- Wrong accusation costs 30 seconds
- After 2 wrong accusations, chief gives a tip
- Solve all cases in an arc to arrest the menace
- Reshuffle only works if no cases solved in that arc

## Adding Custom Cases & Storylines

### Create a New Tale

1. **Copy Template**: Duplicate `Assets/Resources/Tales/paper_tigers.json` as `my_case.json`
2. **Edit JSON** with your content:

```json
{
  "id": "my_case",
  "title": "MY MYSTERY",
  "subtitle": "A custom investigation",
  "description": "Description shown on tale card",
  "order": 10,
  "casesPerArc": 3,
  "arcs": [...],
  "characters": [...],
  "clues": [...],
  "titleTemplates": ["The {MARK} at {CITY}"],
  "situationTemplates": ["{VICTIM}, {VICTIM_ROLE}, was found dead in {CITY}."],
  "briefingTemplates": ["{MENACE} has struck again. Find the killer."]
}
```

### Add an Arc (Menace)
```json
{
  "id": "arc_mine",
  "title": "The Shadow Case",
  "menaceName": "the Shadow",
  "markName": "Shadow Mark",
  "markShape": "MarkTriangle",
  "markColorHex": "#8B5CF6",
  "accentHex": "#1E1B4B",
  "cities": ["Mumbai", "Delhi", "Chennai"]
}
```

**Available mark shapes:** `MarkX`, `MarkInfinity`, `MarkCircle`, `MarkTriangle`, `MarkClaw`, `MarkZig`, `MarkWave`

### Add Characters
```json
{
  "id": "mc_alex",
  "name": "Alex Sharma",
  "role": "Detective Assistant",
  "city": "Mumbai",
  "age": 32,
  "bio": "Sharp observer with keen eye for detail.",
  "colorHex": "#22D3EE",
  "canBeVictim": false,
  "canBeSuspect": true,
  "canBeCulprit": true,
  "alibi": "Was reviewing evidence at headquarters.",
  "statements": ["I was nowhere near the scene.", "The timeline doesn't add up."]
}
```

**Rules:**
- At least 4 victims per arc (from arc cities)
- At least 4 culprit-capable suspects
- Victims should have `canBeSuspect: false` usually
- Every suspect needs `alibi` and `statements`

### Add Clues
```json
{
  "id": "mc_fingerprint",
  "name": "Bloody Fingerprint",
  "desc": "A partial print matching the killer.",
  "shape": "Capsule",
  "colorHex": "#8B0000",
  "size": 0.4,
  "kind": "Guilty",
  "implicates": "",
  "arcs": ["arc_mine"]
}
```

**Clue kinds:**
- `Guilty` - points to killer (leave `implicates` empty, auto-assigned)
- `Herring` - points to innocent (must set `implicates` to innocent suspect id)
- `Flavor` - atmosphere only

**Need:** 4+ Guilty, 2+ Herring per arc

### Validate
In Unity: `Detective Game > Validate Tale Content` - fixes issues before playing.

## Adding 2D Characters & Portraits

### Quick Add
1. Create/find a portrait image (PNG/JPG, square 512x1024px recommended)
2. Name it `{characterId}.png` (must match character `id` in JSON)
3. Drop into `Assets/Resources/Portraits/`
4. Unity auto-imports as Sprite
5. Done - appears in briefing, suspect cards, victim frame

### Tips
- Use `portraitId` in JSON if filename differs from character id
- Missing portraits show automatic placeholders
- Works on all platforms

## Adding 3D Characters & Models

### Step 1: Import FBX
1. Put FBX in `Assets/Art/Characters/` (or anywhere)
2. Select FBX → Inspector:
   - Rig: Humanoid (for people) or Generic
   - Materials: Extract Materials or use embedded
   - Scale Factor: adjust to fit (usually 1.0)

### Step 2: Create Prefab in Resources
1. Drag FBX into scene, position/scale to test
2. Drag from Hierarchy to `Assets/Resources/Models/Characters/` → creates prefab
3. Delete from scene

### Step 3: Configure in JSON
```json
{
  "id": "mc_detective",
  "name": "Inspector Roy",
  "role": "Lead Detective",
  ...
  "viewMode": "Auto",
  "modelPrefab": "Models/Characters/mc_detective",
  "modelScale": 0.9,
  "modelOffset": [0, 0, 0],
  "modelRotationY": 180,
  "billboard3D": true
}
```

**Options:**
- `viewMode`: `Auto` (use 3D if exists), `Sprite2D` (force 2D), `Model3D` (force 3D)
- `modelPrefab`: Resources path without `.prefab`
- `billboard3D`: Face camera (good for UI/cards)

See `Docs/CHARACTER_2D_3D_GUIDE.md` for detailed instructions.

## Using the Game Launcher

### Creating Launcher (C# / Standalone)

Create `Launcher.cs` for a simple menu:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Box(new Rect(10,10,300,200), "DetectiveGame Launcher");
        if(GUI.Button(new Rect(20,40,260,30), "Play V2 (All Tales)")) LoadGame();
        if(GUI.Button(new Rect(20,80,260,30), "Play V1 Style")) LoadV1Mode();
        if(GUI.Button(new Rect(20,120,260,30), "Run Tests")) RunTests();
        if(GUI.Button(new Rect(20,160,260,30), "Quit")) Application.Quit();
    }
    
    void LoadGame() { SceneManager.LoadScene("Main"); }
    void LoadV1Mode() { /* custom logic */ }
    void RunTests() { /* validation */ }
}
```

Or use the built-in build system: `Detective Game > Build Windows 64` etc.

### Launcher Features to Add
- Tale selection
- Quick play (random case)
- Test mode
- Settings
- Version display

## Testing End-to-End

### Manual Testing Checklist
- [ ] Game launches from menu
- [ ] All 3 tales load correctly
- [ ] Cases generate with different content per identity
- [ ] Clue collection works
- [ ] Journal (J) opens/closes
- [ ] Persons (P) opens/closes
- [ ] Accusation works (correct/wrong)
- [ ] Timer functions
- [ ] Case solved screen shows
- [ ] Arc arrest works
- [ ] Reshuffle works (only when unsolved)
- [ ] New Identity resets progress
- [ ] Custom tale loads
- [ ] Custom portrait displays
- [ ] 3D model loads (if added)

### Automated Validation
In Unity Editor:
1. `Detective Game > Validate Tale Content` - checks JSON structure
2. Play mode test - run through a case
3. Check Console for errors

### Python Tests (Already Passed)
```bash
python3 /tmp/validate_tales2.py  # Content validation
python3 /tmp/sim_cases2.py      # 6000 case generations - 0 failures
```

## Docker Setup (Reproducible Environment)

### Dockerfile
```dockerfile
FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y \
    curl wget git python3 unzip xvfb \
    libgtk-3-0 libnss3 libasound2t64 libgbm1 libxss1 libxtst6 \
    libgl1 libcurl4 ca-certificates

WORKDIR /app

COPY . /app/DetectiveGame

# Install Unity Hub (optional) or use Unity Editor
# For headless testing

CMD ["/bin/bash"]
```

### docker-compose.yml
```yaml
version: '3.8'
services:
  detective-game:
    build: .
    volumes:
      - ./DetectiveGame:/app/DetectiveGame
    environment:
      - DISPLAY=:99
    command: >
      bash -c "Xvfb :99 -screen 0 1920x1080x24 &
               echo 'Environment ready'"
```

### Build and Run
```bash
docker build -t detective-game .
docker run -it -v $(pwd)/DetectiveGame:/app/DetectiveGame detective-game
```

## Troubleshooting

### Unity Won't Open Project
- Check Unity version matches 2022.3.45f1
- Clear Library folder and let reimport
- Check for compile errors in Console

### Portraits Not Showing
- Ensure in `Assets/Resources/Portraits/`
- Named exactly as character `id` + extension
- Check import settings (should be Sprite 2D and UI)

### 3D Models Not Loading
- Prefab must be in `Assets/Resources/Models/Characters/`
- Path in JSON without `.prefab` extension
- Check Console for "prefab not found" warnings
- Run Content Validator

### Cases Not Generating
- Run `Detective Game > Validate Tale Content`
- Check all tales have required clues/suspects
- Clear cache: New Identity or restart

### Performance Issues
- Reduce portrait resolution
- Use lower poly 3D models
- Disable 3D models if not needed

## Support Files
- `Docs/V2_AUTHORING_GUIDE.md` - Detailed authoring
- `Docs/CHARACTER_2D_3D_GUIDE.md` - 2D/3D character guide
- `Docs/IMPLEMENTATION_NOTES.md` - Technical details
- `README.md` - Project overview

## Version
V2.0 - Multiple tales, randomization, 2D/3D characters
