# End-to-End Testing Guide

## Quick Test (Automated)
```bash
cd /mnt/d/Workspace_Glad/Projects/DetectiveGame
python3 /tmp/validate_tales2.py
python3 /tmp/sim_cases2.py
```

## Unity Editor Testing

### 1. Launch
1. Open Unity Hub → Add project → Select DetectiveGame
2. Open with Unity 2022.3.45f1
3. Wait for compilation

### 2. Menu Test
- [ ] Main menu appears
- [ ] 3 tales visible with portraits
- [ ] Identity code shown
- [ ] Sound toggle works
- [ ] New Identity button works

### 3. Tale Flow Test
- [ ] Click tale → shows arcs
- [ ] Click arc → shows cases
- [ ] Reshuffle works (if no solved cases)
- [ ] Click case → briefing appears
- [ ] Click Begin Investigation → enters scene

### 4. Gameplay Test
- [ ] HUD shows clue counter
- [ ] Click objects → clues collected
- [ ] Journal (J) shows clues
- [ ] Persons (P) shows suspects
- [ ] Accuse works
- [ ] Wrong accusation gives penalty
- [ ] Correct accusation solves case
- [ ] Next case unlocks
- [ ] Arc completes → arrest shown

### 5. Custom Content Test
- [ ] Add custom tale JSON → appears in menu
- [ ] Add portrait → displays correctly
- [ ] Add 3D model → loads if configured
- [ ] Validation catches errors

## Expected Results
All automated tests pass (0 failures). Game flows from menu to solved case without errors.
