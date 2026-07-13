![Logo](Logo.png)

[中文指南](README_ZH.md)

# Custom Map

[GitHub](https://github.com/CNCUMC/Custom-Map) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/436) | [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)

_A custom map loader for [Casualties Unknown](https://store.steampowered.com/app/4576490/), built on top of [Bark](https://github.com/CNCUMC/Bark) and [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)._

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
4. Install Custom Map, place in `BepInEx/plugins/Custom Map`.
5. Place map folders in `Maps/` (next to the game executable).

---

## Quick Start

### Loading the Built-in Template

Custom Map includes a built-in template map. Enable **Start Game Use Map** in mod settings, select the template, then start a new game.

### Using Custom Maps

1. Create a map folder under `Maps/your-map-name/` (see [Map Structure](#map-structure)).
2. Launch the game. Maps will appear in `Mod Settings → Custom Map → First Use Map` dropdown.
3. Type `cm select your-map-name` in console, or select from the dropdown before starting.

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
| `cm load`               | Reload all maps from Maps folder                |
| `cm info`               | Show current map details                        |
| `cm spawn`              | Teleport to map spawn point                     |
| `cm feature`            | List or modify map features                     |
| `cm waypoint`           | List or teleport to waypoints                   |
| `cm save <start> <end>` | Save an area to current map                     |
| `cm save as [name]`     | Save an area as a new map (click two positions) |
| `cm level`              | Show current level number                       |
| `cm level <n>`          | Switch to level n (reloads world if loaded)     |
| `cm exit`               | Exit custom map and return to vanilla           |

---

## Map Structure

A map is a folder under `Maps/` with the following structure:

```
Maps/
└── your-map-name/
    ├── map.json                      # Map metadata (name, ID, version, author, description)
    ├── level/
    │   ├── level_1.json              # Level data (map grid, key table, spawn, items, waypoints)
    │   └── level_2.json              # (optional) Additional levels
    ├── feature/
    │   ├── world/
    │   │   ├── settings.json         # World settings (gravity, full bright, skip flags)
    │   │   ├── mine.json             # Mine configuration
    │   │   ├── jump_pad.json         # Jump pad configuration
    │   │   ├── turret.json           # Turret configuration
    │   │   ├── sound_cannon.json     # Sound cannon configuration
    │   │   ├── spike_stabber.json    # Spike stabber configuration
    │   │   ├── geyser.json           # Geyser configuration
    │   │   └── beartrap.json         # Bear trap configuration
    │   └── player/
    │       └── xp.json               # XP configuration
    └── command.json                  # Commands (startup/loop)
```

---

## Features

Each feature is configured per-map via JSON files in `feature/world/`. Common properties include:

- **Cooldown** — Time between activations
- **Undestroy** — Whether the entity is indestructible
- **NoLight** — Disables the entity's light emission
- **Damage/Speed multipliers** — Adjusts damage output or movement speed

---

## Settings

| Setting                  | Description                                      |
|--------------------------|--------------------------------------------------|
| `more_logs`              | Enable verbose logging                           |
| `start_game_use_map`     | Automatically use selected map when starting     |
| `first_use_map`          | Map ID to use when starting a new game           |
| `progress_update_interval` | Blocks between progress text updates           |

---

## License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md) for details.
