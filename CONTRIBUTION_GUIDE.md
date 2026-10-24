# Contribution Guide

## How to Contribute
1. Fork/clone the repo
2. Create feature branch
3. Make changes
4. Test thoroughly
5. Submit PR

## Adding Content
- Tales: Add JSON to `Assets/Resources/Tales/`
- Characters: Add to tale JSON + portrait to `Assets/Resources/Portraits/`
- 3D Models: Add prefab to `Assets/Resources/Models/Characters/`
- Always validate with `Detective Game > Validate Tale Content`

## Code Style
- No comments in code (per project rules)
- Follow existing patterns
- Use Unity 2022.3 APIs
- Test in Editor

## Testing
Run validation before committing:
```bash
python3 /tmp/validate_tales2.py
python3 /tmp/sim_cases2.py
```

## Case Involvement
Is there a case involved for game testing?
Yes - the game IS about solving cases. Testing means:
- Playing through cases end-to-end
- Verifying clue collection and deduction
- Testing accusation mechanics
- Ensuring randomization works correctly

See TESTING_GUIDE.md for full checklist.
