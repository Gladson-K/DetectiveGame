# How to Test DetectiveGame

## Quick Answer
Open in Unity Editor and press Play. Test all three tales.

## Step by Step
1. Install Unity 2022.3.45f1
2. Open project in Unity Hub
3. Open Main scene or just press Play
4. Test: Menu → Tale → Arc → Case → Investigate → Collect clues → Accuse
5. Verify case solves correctly

## Automated Tests
```bash
# Content validation
python3 -c "
import json,glob
for f in glob.glob('Assets/Resources/Tales/*.json'):
    t=json.load(open(f))
    print(t['id'], 'OK')
"
```

## Docker Test
```bash
docker build -t detective-game .
docker run -it -v $(pwd):/workspace detective-game
```

## What to Check
- All tales load
- Cases generate uniquely
- Clues are collectible
- Accusation works
- No console errors
