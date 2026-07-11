![Logo](Logo.png)

[中文指南](README_ZH.md)

# Custom Map

[GitHub](https://github.com/CNCUMC/Custom-Map) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/436) | [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)

_A custom map loader for [Casualties Unknown](https://store.steampowered.com/app/3624440/Casualties_Unknown_Demo/), built on top of [Bark](https://github.com/CNCUMC/Bark) and [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)._

---

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Commands](#commands)
- [Map Structure](#map-structure)
- [Features](#features)
- [Settings](#settings)
- [License](#license)

---

## Overview

**Custom Map** allows you to load custom-designed maps in Casualties Unknown. Maps are defined using a string-based format with a key-value mapping system, enabling precise control over every block and entity placement.

- **String map format** — Each character in the map grid maps to a block ID or entity ID via a JSON key dictionary
- **Feature system** — Per-map configuration for mines, turrets, jump pads, spike stabbers, geysers, bear traps, and more
- **Custom loading screen** — Shows real-time progress during map generation
- **Command interface** — Full `cm` command set for managing maps in-game
- **Multi-language support** — English, 简体中文, 繁體中文
- **Mod compatibility** — Soft integration with [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) and [Build Mode](https://www.nexusmods.com/scavprototype/mods/24) — load `.txt` / `.ms.json` / `.ms2.json`  structures and `.alexx_BMsave` saves alongside your map

---

## Installation

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx) for Casualties Unknown.
2. Install [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) ≥ 1.0.2 — place `CUCoreLib.dll` in `BepInEx/plugins/`.
3. Install [Bark](https://github.com/CNCUMC/Bark) ≥ 1.0.2 — place `Bark.dll` in `BepInEx/plugins/Bark/`.
4. Install & Place Custom Map in `BepInEx/plugins/Custom Map`.
5. Place your map folders in `Maps/` (next to the game executable).

---

## Quick Start

### Loading the Built-in Template

Custom Map includes a built-in template map. Enable **Start Game Use Map** in the mod settings, select the template, and start a new game.

### Using a Custom Map

1. Create a map folder under `Maps/YourMapName/` (see [Map Structure](#map-structure)).
2. Start the game. The map will appear in the dropdown under `Mod Settings → Custom Map → First Use Map`.
3. Use `cm select YourMapName` in the console, or select it from the dropdown before starting.

---

## Commands

All commands use the `cm` prefix (or `custommap`).

| Command                 | Description                                     |
|-------------------------|-------------------------------------------------|
| `cm help`               | Show help                                       |
| `cm list`               | List all available maps                         |
| `cm list <id>`          | Select and load a map by ID or index            |
| `cm select <id>`        | Select a map (reloads world if loaded)          |
| `cm reload`             | Reload current map from disk                    |
| `cm info`               | Show current map details                        |
| `cm spawn`              | Teleport to map spawn point                     |
| `cm feature`            | List or modify map features                     |
| `cm waypoint`           | List or teleport to waypoints                   |
| `cm save <start> <end>` | Save an area to current map                     |
| `cm save as [name]`     | Save an area as a new map (click two positions) |
| `cm exit`               | Exit custom map and return to vanilla           |

---

## Map Structure

A map is a folder under `Maps/` with the following layout:

```
Maps/
└── YourMapName/
    ├── map.json                      # Map metadata (name, id, version, author, description)
    ├── level/
    │   └── level1.json               # Level data (map grid, key, spawn, items, waypoints)
    ├── feature/
    │   ├── world/
    │   │   ├── settings.json         # World settings (gravity, full bright, skip flags)
    │   │   ├── mine.json
    │   │   ├── turret.json
    │   │   ├── jump_pad.json
    │   │   ├── spike_stabber.json
    │   │   ├── sound_cannon.json
    │   │   ├── geyser.json
    │   │   └── beartrap.json
    │   └── player/
    │       └── xp.json               # XP multiplier settings
    ├── lang/
    │   └── zh-CN.json                # Map name/description localizations
    ├── AbandonedLab.ms.json          # (optional) Custom Structures file
    └── MyBuild.alexx_BMsave          # (optional) Build Mode save file
```

### map.json

```json
{
  "name": "My Map",
  "id": "mymap",
  "version": "1.0.0",
  "author": [
    "AuthorName"
  ],
  "description": "A custom map",
  "type": "map"
}
```

### level/level1.json

```json
{
  "map_data": {
    "map": [
      "1111111",
      "1000001",
      "1111111"
    ],
    "key": {
      "0": 0,
      "1": 6
    }
  },
  "custom_structures": "MyStructure",
  "build_mode_save": "MyBuild",
  "scene_type": "Debug",
  "spawn": [
    0.0,
    0.0
  ],
  "x": -68,
  "y": 62,
  "waypoints": [],
  "items": [],
  "type": "level"
}
```

- `map` — Grid of characters (top to bottom = top to bottom in world)
- `key` — Maps each character to a block ID (number) or entity ID (string)
- `custom_structures` — (optional) Name of a `.txt` / `.ms.json` / `.ms2.json` structure file in the map root. Extension auto-detected if omitted.
- `build_mode_save` — (optional) Name of a Build Mode `.alexx_BMsave` file in the map root (without extension).
- `scene_type` — `"Debug"`, `"Wasteland"`, `"TemperateForest"`, `"RockDesert"`, or `"None"`
- `x`, `y` — Bottom-left corner of the map in world coordinates

---

## Features

Each feature file overrides the default behavior of in-game entities:

| Feature              | Description                                                                                    |
|----------------------|------------------------------------------------------------------------------------------------|
| `settings.json`      | World settings: `full_bright`, `gravity`, `skip_terrain`, `skip_structures`, `skip_background` |
| `mine.json`          | Mine explosion parameters, undestroyable flag, cooldown                                        |
| `turret.json`        | Turret targeting range, fire rate, damage                                                      |
| `jump_pad.json`      | Jump pad force, cooldown                                                                       |
| `spike_stabber.json` | Spike damage multiplier, sound, undestroyable flag                                             |
| `sound_cannon.json`  | Sound cannon parameters                                                                        |
| `geyser.json`        | Geyser eruption parameters                                                                     |
| `beartrap.json`      | Bear trap damage, escape difficulty                                                            |
| `xp.json`            | XP multiplier overrides                                                                        |

---

## Settings

| Setting                    | Description                              | Default    |
|----------------------------|------------------------------------------|------------|
| `More Logs`                | Enable verbose debug logging             | `false`    |
| `Start Game Use Map`       | Auto-load a map when starting a new game | `false`    |
| `First Use Map`            | Which map to load on first game start    | `template` |
| `Progress Update Interval` | Milliseconds between progress updates    | `333`      |

---

## Compatibility

Custom Map supports soft-integration with the following mods:

| Mod                                                                 | GUID                           | Description                                                                             |
|---------------------------------------------------------------------|--------------------------------|-----------------------------------------------------------------------------------------|
| [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) | `com.Jimmyking.morestructures` | Place `.txt` / `.ms.json` / `.ms2.json` files in the map root. Extension auto-detected. |
| [Build Mode](https://www.nexusmods.com/scavprototype/mods/24)       | `com.alexx_.buildmode`         | Place `.alexx_BMsave` files in the map root. Delegates to Build Mode's own load code.   |

Neither mod is required — Custom Map works fully standalone. If installed, reference files in `level1.json` via `custom_structures` and `build_mode_save` fields (can be without extension).

---

## License

[LGPL v3](LICENSE.md)
