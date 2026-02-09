# Combat Demo Setup Guide

## Getting the Files Into Your Project

The combat scripts are on the `claude/objective-hellman` branch. To get them into your main project:

1. Open GitHub Desktop (or terminal)
2. Merge `claude/objective-hellman` into `main`:
   ```
   git checkout main
   git merge claude/objective-hellman
   git push
   ```
3. Open Unity — the scripts will appear under `Assets/Sean/Combat/`
4. Wait for Unity to compile (might take a sec)

---

## File Overview

```
Assets/Sean/Combat/
  Input/
    CombatActions.inputactions    -- input bindings (WASD, arrows, space)
    CombatActions.cs              -- C# wrapper so scripts can read input
  ScriptableObjects/
    Scripts/
      CombatConfigSO.cs           -- all player/timing config values
      EnemyProfileSO.cs           -- enemy stats and attack timing
  Scripts/
    Core/
      CombatEnums.cs              -- shared enums (AttackDirection, etc.)
      CombatEvents.cs             -- static event bus, how scripts talk to each other
      CombatManager.cs            -- runs the fight, handles win/loss
    Player/
      PlayerCombatController.cs   -- reads input, state machine (idle/parry/dodge/punch)
      PlayerEnergy.cs             -- tracks player energy
    Enemy/
      EnemyCombatController.cs    -- AI loop: wait → telegraph → attack → repeat
      EnemyEnergy.cs              -- tracks enemy energy
      EnemyAttackTelegraph.cs     -- fades enemy color + shows direction arrow
    UI/
      EnergyBarUI.cs              -- fills/drains the energy bar + shows numbers
      CombatNotificationUI.cs     -- floating "PARRY!" "HIT!" text popups
      CombatHUD.cs                -- game over screen + restart button
    Visual/
      FighterVisual.cs            -- color flashes (blue=parry, green=dodge, red=hit)
      DirectionIndicator.cs       -- shows/hides the 4 arrow children on enemy
    Editor/
      CombatDemoSceneBuilder.cs   -- auto-builds the scene (optional shortcut)
```

---

## Quick Setup (Automated)

If you just want to see it running fast:

1. In Unity menu bar: **Combat Demo > Build Scene**
2. That creates `Assets/Sean/Scenes/CombatDemo.unity` with everything wired up
3. Hit Play

---

## Manual Scene Setup

If you'd rather build it yourself (or the auto-builder has issues):

### Step 1: Create ScriptableObject Assets

- Right-click in Project window > **Create > Combat > Combat Config**
  - Save as `Assets/Sean/Combat/ScriptableObjects/Definitions/CombatConfig`
  - Defaults are fine, tweak in Inspector later
- Right-click > **Create > Combat > Enemy Profile**
  - Save as `Assets/Sean/Combat/ScriptableObjects/Definitions/EnemyProfile_Basic`

### Step 2: New Scene

- File > New Scene (Empty)
- Save as `Assets/Sean/Scenes/CombatDemo.unity`

### Step 3: Camera + Light

- Main Camera already exists, just set:
  - Projection: Orthographic
  - Size: 5
  - Position: (0, 0, -10)
  - Background: dark color (#262633 ish)
- Add **GameObject > Light > Global Light 2D** (URP 2D needs this or everything is black)

### Step 4: Player

- Create Empty, name it **Player**
- Position: **(-3, 0, 0)**
- Add **SpriteRenderer** — use Unity's built-in Square sprite (search "Square" in sprite picker) or any white square
- Add components:
  - `PlayerCombatController`
  - `PlayerEnergy`
  - `FighterVisual`
- Wire in Inspector:
  - PlayerCombatController → Config: `CombatConfig` SO, Energy: this object's `PlayerEnergy`, Visual: this object's `FighterVisual`
  - FighterVisual → Sprite Renderer: this object's SpriteRenderer

### Step 5: Enemy

- Create Empty, name it **Enemy**
- Position: **(3, 0, 0)**
- Add **SpriteRenderer** — use a circle sprite (make a white circle PNG or find one)
- Add components:
  - `EnemyCombatController`
  - `EnemyEnergy`
  - `FighterVisual`
  - `EnemyAttackTelegraph`
  - `DirectionIndicator`
- Wire in Inspector:
  - EnemyCombatController → Profile: `EnemyProfile_Basic` SO, CombatConfig: `CombatConfig` SO, Energy: this object's `EnemyEnergy`, Visual: this object's `FighterVisual`
  - FighterVisual → Sprite Renderer: this object's SpriteRenderer
  - EnemyAttackTelegraph → Sprite Renderer: this object's SpriteRenderer, Direction Indicator: this object's `DirectionIndicator`

### Step 6: Direction Arrows (children of Enemy)

Create 4 child GameObjects under Enemy. Each gets a SpriteRenderer with a small triangle/arrow sprite, colored yellow, scaled to ~0.5:

| Name       | Local Position  | Rotation Z |
|------------|----------------|------------|
| ArrowUp    | (0, 1.5, 0)   | 0          |
| ArrowDown  | (0, -1.5, 0)  | 180        |
| ArrowLeft  | (-1.5, 0, 0)  | 90         |
| ArrowRight | (1.5, 0, 0)   | -90        |

**Important: Set all 4 arrows to INACTIVE** (uncheck the box at top of Inspector)

Then on the `DirectionIndicator` component, drag each arrow into its slot.

### Step 7: Canvas + UI

- **GameObject > UI > Canvas**
  - Render Mode: Screen Space - Overlay
  - Canvas Scaler: Scale With Screen Size, Reference: 1920x1080

#### Player Energy Bar (bottom-left)
- Empty child `PlayerEnergyBar`, anchored bottom-left, pos (20,20), size (350, 60)
- Child `Background` — Image, dark gray (#333333), 80% alpha, stretch to fill
- Child `Fill` — Image, **Type: Filled**, **Fill Method: Horizontal**, color: cyan/blue, small padding from edges
- Child `Label` — TextMeshPro, "PLAYER", font 16, upper area
- Child `ValueText` — TextMeshPro, "0 / 0", font 20, center area
- Add `EnergyBarUI` to `PlayerEnergyBar`:
  - Tracked Fighter: **Player**
  - Fill Image: drag in the Fill image
  - Value Text: drag in ValueText

#### Enemy Energy Bar (bottom-right)
Same setup but:
- Anchored bottom-right, pos (-20, 20)
- Fill color: red
- Label: "ENEMY"
- Tracked Fighter: **Enemy**

#### Notification System
- Empty child `NotificationContainer`, stretch to fill canvas
- Add `CombatNotificationUI` component
- Create child `NotificationText`:
  - Add TextMeshProUGUI, font 36, bold, center-aligned, raycast target OFF
  - Size: 200x50
  - **Set it INACTIVE** (it's the template)
- Wire CombatNotificationUI:
  - Notification Prefab: the `NotificationText` object
  - Canvas: the Canvas
  - Config: `CombatConfig` SO

#### Game Over Panel
- Child `GameOverPanel`, centered, size (400, 250)
- Add Image, black with 85% alpha
- Child `ResultText` — TMP, "VICTORY!", font 48, bold, upper half
- Child `RestartButton`:
  - Add Image (dark gray) + Button component
  - Child `Text` — TMP, "RESTART", font 28
- **Set GameOverPanel INACTIVE**
- Add `CombatHUD` to the Canvas:
  - Game Over Panel: `GameOverPanel`
  - Result Text: `ResultText`
  - Restart Button: `RestartButton`

### Step 8: CombatManager

- Create Empty `CombatManager`
- Add `CombatManager` component
- Wire:
  - Config: `CombatConfig` SO
  - Enemy Profile: `EnemyProfile_Basic` SO
  - Player Energy: Player's `PlayerEnergy`
  - Enemy Energy: Enemy's `EnemyEnergy`
  - HUD: Canvas's `CombatHUD`
- Also on `CombatHUD`, set Combat Manager to this object's `CombatManager`

### Step 9: EventSystem

- **GameObject > UI > EventSystem** (if not already created with Canvas)
- Make sure it has `InputSystemUIInputModule` (not the legacy one)

### Step 10 (optional): Controls Hint

- TMP text at top of canvas
- "WASD = Parry | Up Arrow = Punch | Space = Dodge"
- White, 50% alpha, font 18

---

## Controls

| Key | Action |
|-----|--------|
| W | Parry Up |
| A | Parry Left |
| S | Parry Down |
| D | Parry Right |
| Up Arrow | Punch |
| Space | Dodge |

---

## How Combat Works

1. Enemy waits 1.5-3 seconds, picks a random direction
2. Yellow arrow appears on enemy + enemy fades white → red (0.5-1s telegraph)
3. You react:
   - **Parry** (matching WASD direction) → blocks hit, you GAIN energy, enemy gets stunned for 1s
   - **Dodge** (Space) → avoids hit, costs you 2 energy
   - **Do nothing** → you take 5 damage
4. **Punch** (Up Arrow) anytime → costs you 3 energy, deals 3 to enemy. Best used when enemy is stunned.
5. First to 0 energy loses

---

## Tuning Values

All values are in the ScriptableObjects — just select them in Project window and edit in Inspector:

**CombatConfig** (player stuff + timing):
- Player starting energy: 20
- Punch cost: 3, Dodge cost: 2
- Parry energy gain: 5
- Damage taken per hit: 5
- Parry window: 0.3s, Dodge window: 0.4s

**EnemyProfile_Basic** (enemy stuff):
- Enemy starting energy: 50
- Attack cost: 3, Attack damage: 5
- Telegraph: 0.5-1.0s
- Cooldown between attacks: 1.5-3.0s
- Stun duration on parry: 1.0s
