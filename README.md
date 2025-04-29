# 🦶 BodySound — Sound & Interface Audio Tweaker for SPT

## 🎯 Description

**BodySound** is a client-side mod for SPT that allows you to **control the volume of sounds related to the local player** (e.g., footsteps, gear sounds, jumps) as well as various interface, medical, and in-game interaction sounds.

The goal is to enhance audio immersion by adjusting the volume for specific sound categories exclusively for the local player.

---

## ⚙️ Features

- 🎧 Dynamically adjusts volume for different sound categories:
  - **Movement sounds**: `walk_`, `gear_`, `turn_`, `stop_`, `sprint_`, `vault_`, `jump_`, etc.
  - **Medical sounds**: `_medkit_`, `_bandage_`, `_surgicalkit_`, `_rollupkit_`, `_injector_`, `_zvezda_`, `_splint_`, `_pillsbottle_`.
  - **Interface sounds**: bags, notifications, etc.
  - **In-game actions sounds**: weapon handling, gear switching, etc.
- 👤 Affects **only the local player** – other entities are untouched.
- 🔁 Works via Harmony patches on `SoundBank.Play()` and `SoundBank.PickClipsByDistance()`.
- 🔊 Sound volume is configurable per category.
- ⚡ Lightweight – minimal performance impact.
- 🆕 Session-Based Sound Management (in-raid vs. out-of-raid).
- 🆕 Improved control over breathing and injury sounds.

---

## 🎧 Supported Sound Details

### 👉 Medical sounds

The following medical consumable sounds are supported:

- Medkits (`_medkit_`)
- Bandages (`_bandage_`)
- Surgical kits (`_surgicalkit_`)
- Rollable kits (`_rollupkit_`)
- Injectors (`_injector_`)
- Survival stim (`_zvezda_`)
- Splints (`_splint_`)
- Pill bottles (`_pillsbottle_`)

### 👉 Interface sounds

You can adjust the volume or mute the following interface-related sounds:

- 🎒 **Bags**: opening and closing backpacks.
- 📢 **Notifications**: in-game notification sounds.
- 🖐️ **Pickup**: item pickup sounds (`_pickup`).
- 📉 **Drop**: item drop sounds (`_drop`).
- ⏱️ **Hover**: sounds triggered when hovering over an item (`_over`).
- 🔧 **Weapon modding**: sounds related to installing or modifying weapon attachments (`_install_`).
- 🔍 **Item inspection**: sounds when inspecting items (`_inspector_`).

### 👉 Weapon sounds

The following weapon-related sounds can also be controlled:

- 👐 **Weapon handling**: sounds triggered when gripping a weapon (HandGrip).
- 🔄 **Weapon switching**: sounds activated when switching light scope (`Switcher`).
- 🔄 **Weapon in/out**: sounds for taking out or putting away a weapon (`weap_in`, `weap_out`).

### 🆕 Physiological Sounds

- 🫁 **Breathing**: Improved control over breathing sound effects.
- 🩸 **Injuries**: Sounds related to injuries and pain.

---

---

## 🔧 Advanced Configuration

### 🆕 Session-Based Hook Control

You can configure the mod to activate hooks only during raids or everywhere:
- Enable `EnableAllHooksOnlyInRaid` to restrict sound effects adjustments to raids only.
- Disable it to apply sound adjustments everywhere (Hideout, menus, raids).

### 🆕 Weight-Based Volume Adjustment

The mod now supports volume adjustment based on the equipment weight:
- Enable `UseWeightBasedVolume` for a more immersive experience where your gear weight affects the volume of movement sounds.

---

## 🛠️ Installation

1. Download the compiled mod (`BodySound.dll`).
2. Place the file into the following folder:  
   `BepInEx/plugins/BodySound/`.
3. Launch the game.

---

## 📚 Logs (if needed)

To enable logs in debug mode, configure the following:  
- `debug.cfg` and `body_log.txt` available in:  
  `BepInEx/plugins/BodySound`.

---

## 🔍 How it works

The mod intercepts Unity audio playback related to the local player's actions.  
When a clip name matches a detected category (e.g., `walk_`, `gear_`), the mod automatically adjusts its volume using the following code: