![Logo](Logo.png)

[中文指南](README_ZH.md)

# Custom Map

[GitHub](https://github.com/CNCUMC/Custom-Map) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/436) | [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)

_A custom map loader for [Casualties Unknown](https://store.steampowered.com/app/4576490/), built on top
of [Bark](https://github.com/CNCUMC/Bark) and [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)._

---

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Commands](#commands)
- [Map Structure](#map-structure)
- [Layer](#layer)
- [Command System](#command-system)
- [Features](#features)
- [Settings](#settings)
- [Compatibility](#compatibility)
- [License](#license)

---

## Overview

**Custom Map** allows you to load custom-designed maps in Casualties Unknown.

- **Layer-based maps** — Multi-layer support with independent settings, structures, and commands per layer
- **Feature system** — Per-map configuration for mines, turrets, jump pads, spike stabbers, geysers, bear traps, and
  more
- **Custom loading screen** — Shows real-time progress during map generation
- **Command interface** — Full `cm` command set for managing maps in-game
- **Multi-language support** — English, 简体中文, 繁體中文, Русский
- **Mod compatibility** — Soft integration with [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9)

---

## Installation

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx) for Casualties Unknown.
2. Install [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) — place `CUCoreLib.dll` in
   `BepInEx/plugins/`.
3. Install [Bark](https://github.com/CNCUMC/Bark) — place `Bark.dll` in `BepInEx/plugins/Bark/`.
4. Install Custom Map, place in `BepInEx/plugins/Custom Map`.
5. Place map folders in `Maps/` (next to the game executable).

---

## Quick Start

### Loading the Built-in Template

Custom Map includes a built-in template map. Enable **Start Game Use Map** in mod settings, select the template, then
start a new game.

### Using Custom Maps

1. Create a map folder under `Maps/your-map-name/` (see [Map Structure](#map-structure)).
2. Launch the game. Maps will appear in `Mod Settings → Custom Map → First Use Map` dropdown.
3. Type `cm select your-map-name` in console, or select from the dropdown before starting.

---

## Commands

All commands use the `cm` prefix.

| Command                   | Description                               |
|---------------------------|-------------------------------------------|
| `cm help`                 | Show help                                 |
| `cm reload`               | Reload current map from disk              |
| `cm load`                 | Reload all maps from Maps folder          |
| `cm savereload`           | Save and reload current map               |
| `cm info`                 | Show current map details                  |
| `cm spawn`                | Teleport to map spawn point               |
| `cm select <id>`          | Select a map by ID, name, or index        |
| `cm list`                 | List all available maps                   |
| `cm feature [name] [val]` | List or set map features                  |
| `cm waypoint [list\|get]` | Manage waypoints                          |
| `cm save`                 | Save current map to disk                  |
| `cm layer [n]`            | Show or switch layer                      |
| `cm exit <target>`        | Exit map — `none` (vanilla) or `tutorial` |

---

## Map Structure

A map is a folder under `Maps/` with the following structure:

```
Maps/
└── your-map-name/
    ├── map.json                      # Map metadata
    ├── layers/
    │   ├── layer1/                   # Layer 1 folder
    │   │   ├── layer.json            # Layer properties
    │   │   ├── command.json          # Layer commands (optional)
    │   │   └── *.m2.json             # Structure files (optional)
    │   └── layer2/                   # Layer 2 folder (optional)
    │       ├── layer.json
    │       ├── command.json
    │       └── *.m2.json
    ├── command.json                  # Global commands — applied to all layers as fallback (optional)
    ├── feature/
    │   ├── world/
    │   │   ├── settings.json         # World settings (gravity, full bright, skip flags)
    │   │   ├── mine.json             # Mine configuration (optional)
    │   │   ├── jump_pad.json         # Jump pad configuration (optional)
    │   │   ├── turret.json           # Turret configuration (optional)
    │   │   ├── sound_cannon.json     # Sound cannon configuration (optional)
    │   │   ├── spike_stabber.json    # Spike stabber configuration (optional)
    │   │   ├── geyser.json           # Geyser configuration (optional)
    │   │   └── beartrap.json         # Bear trap configuration (optional)
    │   └── player/
    │       └── xp.json               # XP configuration (optional)
    └── lang/
        ├── EN.json                   # English localization
        ├── zh-CN.json                # Simplified Chinese localization
        └── zh-TW.json                # Traditional Chinese localization
```

### map.json

```json
{
  "id": "mymap",
  "version": "1.0.0",
  "author": [
    "AuthorName"
  ]
}
```

| Field     | Type       | Description                |
|-----------|------------|----------------------------|
| `id`      | `string`   | Unique map identifier      |
| `version` | `string`   | Map version (e.g. `1.0.0`) |
| `author`  | `string[]` | List of author names       |

---

## Layer

Each layer is a folder under `layers/` (e.g. `layers/layer1/`).

### layer.json

```json
{
  "type": "Debug",
  "spawn": [
    0,
    0
  ],
  "x": -68,
  "y": 62,
  "skip_terrain": true,
  "skip_structures": true,
  "skip_background": true,
  "waypoints": [],
  "items": []
}
```

| Field             | Type       | Default | Description                                                                           |
|-------------------|------------|---------|---------------------------------------------------------------------------------------|
| `type`            | `string`   | `Debug` | Scene type: `Debug`, `Wasteland`, `TemperateForest`, `RockDesert`, `Tutorial`, `None` |
| `spawn`           | `number[]` | `[0,0]` | Spawn position `[x, y]`                                                               |
| `x`               | `int`      | `0`     | World X offset of the layer                                                           |
| `y`               | `int`      | `0`     | World Y offset of the layer                                                           |
| `skip_terrain`    | `bool`     | `true`  | Skip vanilla terrain generation                                                       |
| `skip_structures` | `bool`     | `true`  | Skip vanilla structure generation                                                     |
| `skip_background` | `bool`     | `true`  | Skip vanilla background generation                                                    |
| `waypoints`       | `array`    | `[]`    | Waypoint list (for `cm waypoint`)                                                     |
| `items`           | `array`    | `[]`    | Items to give on spawn                                                                |

### Structure Files (*.m2.json)

Each `.m2.json` file in a layer folder defines a single structure placement:

```json
{
  "structure": "StructureName",
  "x": 10,
  "y": 20
}
```

| Field       | Type     | Description                                           |
|-------------|----------|-------------------------------------------------------|
| `structure` | `string` | Structure name (registered via Custom Structures mod) |
| `x`         | `int`    | World X coordinate                                    |
| `y`         | `int`    | World Y coordinate                                    |

---

## Command System

### Global Commands (`command.json` in map root)

Applied to **all layers** as a fallback. If a layer has its own `command.json`, the layer-level one takes priority.

```json
{
  "once_commands": [
    "command1",
    "command2"
  ],
  "loop_commands": [
    "loop_command1"
  ],
  "loop_interval": 10.0
}
```

### Layer Commands (`command.json` in layer folder)

Override global commands for a specific layer. Same format as above.

| Field           | Type       | Description                               |
|-----------------|------------|-------------------------------------------|
| `once_commands` | `string[]` | Commands executed once on map load        |
| `loop_commands` | `string[]` | Commands executed repeatedly              |
| `loop_interval` | `float`    | Interval in seconds between loop commands |

---

## Features

Each feature file in `feature/world/` or `feature/player/` configures the corresponding game entity.

| Feature              | Description                                                                              |
|----------------------|------------------------------------------------------------------------------------------|
| `settings.json`      | World settings: `full_bright`, `gravity`, `jump_limit`, `climb_limit`, `forgiving_level` |
| `mine.json`          | Mine: cooldown, undestroy                                                                |
| `jump_pad.json`      | Jump pad: cooldown, force, no_light                                                      |
| `turret.json`        | Turret: cooldown, shot power, range, undestroy, no_light                                 |
| `sound_cannon.json`  | Sound cannon: cooldown, range, charge time, undestroy                                    |
| `spike_stabber.json` | Spike stabber: damage, cooldown, undestroy, no_light, sound                              |
| `geyser.json`        | Geyser: cooldown, duration, range, rumble time, no_liquid                                |
| `beartrap.json`      | Bear trap: damage, cooldown, undestroy                                                   |
| `xp.json`            | XP: strength/resilience/intelligence levels and EXP ranges                               |

---

## Settings

| Setting                    | Description                                    | Default    |
|----------------------------|------------------------------------------------|------------|
| `More Logs`                | Enable verbose logging                         | `false`    |
| `Start Game Use Map`       | Automatically use selected map when starting   | `false`    |
| `First Use Map`            | Map ID to use when starting a new game         | `template` |
| `Progress Update Interval` | Number of blocks between progress text updates | `333`      |

---

## Compatibility

Custom Map supports soft integration with:

| Mod                                                                 | Required | Description                                                           |
|---------------------------------------------------------------------|----------|-----------------------------------------------------------------------|
| [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) | No       | Place `.txt` / `.ms.json` / `.ms2.json` files for structure placement |

---

## License

[LGPL-3.0](LICENSE.md)
