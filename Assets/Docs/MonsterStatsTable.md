# Monster Stats Table — Alfheim Echoes of Light RPG

## Formulas

- **HP** = (25 + Level × 12) × TypeMultiplier × GapMultiplier  
- **EXP** = (12 × Level²) × TypeMultiplier × (0.15 if player 4+ above, else 1.0)

### Level Gap Color Logic

| Gap (Player − Enemy) | Color  | HP Mult | Damage (Player→Enemy) | Enemy Attack | EXP |
|----------------------|--------|---------|------------------------|--------------|-----|
| **+2 or more**       | Green  | 0.8×    | 1.2×                   | 0.8×         | 15% if 4+ above |
| **−3 to +1**         | Yellow | 1.0×    | 1.0×                   | 1.0×         | 100% |
| **−4 or less**       | Red    | 1.2×    | 0.8×                   | 1.2×         | 100% |

### Type Multipliers

| Type     | HP & EXP Mult |
|----------|----------------|
| Normal   | 1×             |
| Elite    | 1.5×           |
| Mini Boss| 2×             |
| Boss     | 4×             |

---

## DeepForest Monsters (Base Stats)

| Monster   | Level | Base HP | Base EXP | Type   |
|-----------|-------|---------|----------|--------|
| Skeleton  | 5     | 85      | 300      | Normal |
| Knight    | 3     | 61      | 108      | Normal |
| Mushroom  | 3     | 61      | 108      | Normal |
| Flying Eye| 2     | 49      | 48       | Normal |

*Base HP = 25 + (Level × 12), Base EXP = 12 × Level²*

---

## Final HP & EXP by Player Level

### Skeleton (Level 5) — Base HP 85, Base EXP 300

| Player Lv | Gap | Color  | Final HP | Final EXP |
|-----------|-----|--------|----------|-----------|
| 1         | −4  | Red    | 102      | 300       |
| 2–6       | −3 to +1 | Yellow | 85    | 300       |
| 7+        | +2+ | Green | 68       | 45 (15%)  |

### Knight (Level 3) — Base HP 61, Base EXP 108

| Player Lv | Gap | Color  | Final HP | Final EXP |
|-----------|-----|--------|----------|-----------|
| 1         | −2  | Yellow | 61       | 108       |
| 2–4       | −1 to +1 | Yellow | 61    | 108       |
| 5+        | +2+ | Green | 49       | 16 (15%)  |

### Mushroom (Level 3) — Base HP 61, Base EXP 108

| Player Lv | Gap | Color  | Final HP | Final EXP |
|-----------|-----|--------|----------|-----------|
| 1         | −2  | Yellow | 61       | 108       |
| 2–4       | −1 to +1 | Yellow | 61    | 108       |
| 5+        | +2+ | Green | 49       | 16 (15%)  |

### Flying Eye (Level 2) — Base HP 49, Base EXP 48

| Player Lv | Gap | Color  | Final HP | Final EXP |
|-----------|-----|--------|----------|-----------|
| 1         | −1  | Yellow | 49       | 48        |
| 2–3       | 0 to +1 | Yellow | 49    | 48        |
| 4+        | +2+ | Green | 39       | 7 (15%)   |

---

## Player EXP to Level Up

| Level | EXP Required |
|-------|--------------|
| 1→2   | 200          |
| 2→3   | 400          |
| 3→4   | 600          |
| 4→5   | 800          |
| 5→6   | 1,000        |
| 6→7   | 1,200        |
| 7→8   | 1,400        |
| 8→9   | 1,600        |
| 9→10  | 1,800        |
| 10→11 | 2,000        |
| 11→12 | 2,200        |
| 12→13 | 2,400        |
| 13→14 | 2,600        |
| 14→15 | 2,800        |
| 15→16 | 3,000        |
| 16→17 | 3,200        |
| 17→18 | 3,400        |
| 18→19 | 3,600        |
| 19→20 | 3,800        |
| **Total to Lv 20** | **38,000** |

---

## Progression Summary

- **Early (Lv 1–5):** Flying Eye, Knight, Mushroom — ~50–110 EXP per kill.
- **Mid (Lv 5–10):** Skeleton — 300 EXP when Yellow, 45 when Green.
- **Late (Lv 10+):** Green enemies give 15% EXP to reduce farming.
