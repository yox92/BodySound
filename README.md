# 🦶 BodySound — Body Volume Tweaker for SPT

## 🎯 Description

**BodySound** is a client-side mod for SPT that allows you to **control the volume of local body-related sounds** (footsteps, gear movement, jumping, etc.).

The goal is to enhance audio immersion by reducing certain movement-related audio categories for the local player.

---

## ⚙️ Features

- 🎧 Dynamically adjusts volume for audio clips with prefixes:
  - `walk_`, `gear_`, `run_`, `jump_`, etc.
- 👤 Affects **only the local player** — other entities remain untouched.
- 🔁 Based on Harmony patches on `SoundBank.Play()` and `SoundBank.PickClipsByDistance()`
- 🔊 Volume is configurable per clip prefix
-  Lightweight — no performance impact

---

## 🛠️ Installation

1. Download the compiled mod (`BodySound.dll`)
2. Place the file into:  
   `BepInEx/plugins/BodySound/`
3. Launch the game.

---
## 📚 Log IF ONLY NEED 📚
-  **debug.cfg** - **body_log.txt**  ==> ``BepInEx\plugins\BodySound`` true = debug mod on
---

## 🔍 How it works

The mod intercepts Unity audio playback related to local player movement.  
If a clip name starts with a relevant prefix (`walk_`, `gear_`, etc.), the mod automatically adjusts its volume through:

```csharp
BetterSource.volume = volumeSliderForPrefix;
