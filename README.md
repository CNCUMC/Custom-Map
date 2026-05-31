![alt text](Cover.png)

[中文指南](README_ZH.md)

# Custom Fungame Pack

[GitHub](https://github.com/Black-Moss/Custom-Fungame-Pack) / [NexusMods](https://www.nexusmods.com/games/scavprototype/mods/131)

_A custom map/gamemode ("Fungame") management system for **Casualties Unknown**._

---

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Config Options](#config-options)
- [Command Reference](#command-reference)
    - [fg select](#fg-select)
    - [fg list](#fg-list)
    - [fg info](#fg-info)
    - [fg spawn](#fg-spawn)
    - [fg exit](#fg-exit)
    - [fg reload](#fg-reload)
    - [fg feature](#fg-feature)
    - [fg waypoint](#fg-waypoint)
    - [fg save](#fg-save)
    - [fg save as](#fg-save-as)
- [Creating a Fungame](#creating-a-fungame)
    - [Fungame JSON Format](#fungame-json-format)
    - [Content Types](#content-types)
        - [MapData](#mapdata)
        - [CustomStructures](#customstructures)
        - [BuildModeSave](#buildmodesave)
    - [Features](#features)
    - [Commands](#commands)
    - [XP Configuration](#xp-configuration)
- [Project Structure](#project-structure)

---

## Overview

**Custom Fungame Pack** is a BepInEx plugin that introduces a custom map/gamemode system called "Fungame" to Casualties
Unknown. It allows players to load, manage, and create custom maps with unique features, waypoints, items, and game
logic.

---

## Installation

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx) for Casualties Unknown.
2. Install [Moss Lib](https://github.com/Black-Moss/Moss-Lib).
3. Download the latest release from the [Releases](https://github.com/Black-Moss/Custom-Fungame-Pack/releases) page.
4. Extract the downloaded archive and place the entire `Custom Fungame Pack` folder into your `BepInEx/plugins/` folder.
5. Create a `Fungames/` folder in your game's root directory (next to `CasualtiesUnknown.exe`).
6. Place your Fungame folders inside `Fungames/` (each containing a `fungame.json`).

### Folder Structure

```
Casualties Unknown Demo/
├── BepInEx/
│   └── plugins/
│       ├── Moss Lib
│       │   └── Moss Lib.dll
│       └── Custom Fungame Pack
│           ├── Lang/
│           ├── CustomFungamePack.dll
│           ├── LICENSE.md
│           ├── README.md
│           └── README_ZH.md
├── Fungames/
│   ├── MyCustomMap/
│   │   └── fungame.json
│   └── AnotherMap/
│       └── fungame.json
└── CasualtiesUnknown.exe
```

---

## Config Options

Configure via `BepInEx/config/blackmoss.customfungamepack.cfg`:

| Key                 | Type     | Default    | Description                                            |
|---------------------|----------|------------|--------------------------------------------------------|
| `more_logs`         | `bool`   | `false`    | Enable verbose logging                                 |
| `start_lse_fungame` | `bool`   | `false`    | Automatically load a Fungame on new game start         |
| `first_use_fungame` | `string` | `template` | Fungame ID to load when `Start Use Fungame` is enabled |

---

## Command Reference

All commands are accessed via the in-game console (press ~ to open) using the **`fungame`** or **`fg`** command.

### fg select

Select and load a Fungame by ID, name, or index.

```
fg select <id|name|index>
```

**Examples:**

```
fg select my_map              # Select by ID
fg select "My Cool Map"       # Select by name
fg select 1                   # Select by list index
```

### fg list

List all available Fungames with their IDs, versions, and authors. Can also be used like `fg select`.

```
fg list
```

Append an argument to select directly.

```
fg list <id|name|index>
```

**Examples:**

```
fg list my_map                # Select by ID
fg list "My Cool Map"         # Select by name
fg list 1                     # Select by list index
```

### fg info

Display information about the currently loaded Fungame.

```
fg info
```

### fg spawn

Teleport back to the Fungame's spawn point.

```
fg spawn
```

### fg exit

Exit the current Fungame and reload the scene.

```
fg exit none                  # Return to vanilla game
fg exit tutorial              # Return to tutorial
```

### fg reload

Reload the current Fungame's map data.

```
fg reload
```

### fg feature

Manage Fungame features (list/get/set).

```
fg feature list                # List all features and their values
fg feature get <name>          # Get a specific feature value
fg feature set <name> <value>  # Set a feature value
```

**Available features:**

| Feature          | Type    | Default | Description                           |
|------------------|---------|---------|---------------------------------------|
| `Fullbright`     | `bool`  | `true`  | Enable global brightness              |
| `ForgivingLevel` | `bool`  | `false` | Enable forgiving level mode           |
| `Gravity`        | `float` | `-9.81` | Custom gravity value                  |
| `JumpLimit`      | `int`   | `0`     | Maximum jump count (`0` = unlimited)  |
| `ClimbLimit`     | `int`   | `0`     | Maximum climb count (`0` = unlimited) |

### fg waypoint

Manage Fungame waypoints (list/get).

```
fg waypoint list               # List all waypoints
fg waypoint get <id|index>     # Teleport to a waypoint
```

### fg save

Save the current Fungame configuration to disk.

```
fg save                        # Save to the Fungame's directory
```

### fg save as

Interactively select an area and save it as map data for the current Fungame.

```
fg save as                     # Follow on-screen prompts
```

---

## Creating a Fungame

Each Fungame is a folder inside the `Fungames/` directory containing a `fungame.json` file.

### Fungame JSON Format

```json
{
  "name": "My Custom Map",
  "id": "my_custom_map",
  "version": "1.0.0",
  "author": [
    "YourName"
  ],
  "description": "A cool custom map",
  "spawn": [
    0,
    0
  ],
  "x": -68,
  "y": 62,
  "type": "Debug",
  "feature": {
    "Fullbright": true,
    "ForgivingLevel": false,
    "Gravity": -9.81,
    "JumpLimit": 0,
    "ClimbLimit": 0
  },
  "waypoints": [
    {
      "id": "center",
      "x": 0,
      "y": 0
    }
  ],
  "items": [
    {
      "id": "rifle",
      "slot": 0,
      "force": true
    }
  ],
  "skip_terrain": true,
  "skip_structures": true,
  "skip_background": true,
  "map_data": {},
  "command": {},
  "custom_structures": "filename.json",
  "build_mode_save": "filename.bms",
  "xp": {}
}
```

| Field               | Type       | Required                                                    | Description                                                            |
|---------------------|------------|-------------------------------------------------------------|------------------------------------------------------------------------|
| `name`              | `string`   | Yes                                                         | Display name                                                           |
| `id`                | `string`   | Yes                                                         | Unique identifier (auto-generated from folder name if omitted)         |
| `version`           | `string`   | Yes                                                         | Version string (default: `1.0.0`)                                      |
| `author`            | `string[]` | Yes                                                         | List of authors                                                        |
| `description`       | `string`   | Yes                                                         | Map description                                                        |
| `spawn`             | `float[2]` | No                                                          | Spawn coordinates `[x, y]` (default: `[0, 0]`)                         |
| `x`, `y`            | `int`      | No                                                          | Map position origin (default: `[0, 0]`)                                |
| `type`              | `string`   | No                                                          | Scene type: `Debug`, `None`, `Tutorial` (default: `Debug`)             |
| `feature`           | `object`   | No                                                          | Feature overrides (see [Features](#features))                          |
| `waypoints`         | `array`    | No                                                          | List of waypoints (see [Waypoints](#waypoints))                        |
| `items`             | `array`    | No                                                          | Starting items (see [Items](#items))                                   |
| `skip_terrain`      | `bool`     | No                                                          | Skip terrain generation (default: `true`)                              |
| `skip_structures`   | `bool`     | No                                                          | Skip structure generation (default: `true`)                            |
| `skip_background`   | `bool`     | No                                                          | Skip background generation (default: `true`)                           |
| `xp`                | `object`   | No                                                          | XP configuration (see [XP Configuration](#xp-configuration))           |
| `map_data`          | `object`   | One of `map_data` / `custom_structures` / `build_mode_save` | Map data content (see [MapData](#mapdata))                             |
| `custom_structures` | `string`   | One of `map_data` / `custom_structures` / `build_mode_save` | Custom Structures filename (see [CustomStructures](#customstructures)) |
| `build_mode_save`   | `string`   | One of `map_data` / `custom_structures` / `build_mode_save` | Build Mode Save filename (see [BuildModeSave](#buildmodesave))         |
| `command`           | `object`   | No                                                          | Command configuration (see [Commands](#commands))                      |

> **Note:** Only **one** content type (`map_data`, `custom_structures`, or `build_mode_save`) can be used at a time.

### Content Types

#### MapData

Define the map as a grid of characters with a key mapping each character to a block/item.

```json
{
  "map_data": {
    "map": [
      "WWWWW",
      "W   W",
      "W   W",
      "W   W",
      "WWWWW"
    ],
    "key": {
      "W": {
        "block": "wall"
      }
    }
  }
}
```

#### CustomStructures

Reference a JSON file containing [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) data.

```json
{
  "custom_structures": "my_structures.json"
}
```

> **Requires:** The [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) mod to be installed.

#### BuildModeSave

Reference a Build Mode save file (`.alexx_BMsave`).

```json
{
  "build_mode_save": "my_save.alexx_BMsave"
}
```

> **Requires:** The [BuildMod](https://www.nexusmods.com/scavprototype/mods/24) mod to be installed.

### Features

Features modify game behavior while the Fungame is active.

| Feature          | Type    | Default | Description                                         |
|------------------|---------|---------|-----------------------------------------------------|
| `Fullbright`     | `bool`  | `true`  | All areas are fully lit                             |
| `ForgivingLevel` | `bool`  | `false` | Player doesn't lose items on death                  |
| `Gravity`        | `float` | `-9.81` | Override gravity (negative = downward)              |
| `JumpLimit`      | `int`   | `0`     | Max jumps before touching ground (`0` = unlimited)  |
| `ClimbLimit`     | `int`   | `0`     | Max climbs before touching ground (`0` = unlimited) |

### Waypoints

```json
{
  "waypoints": [
    {
      "id": "start",
      "x": 0.0,
      "y": 0.0
    },
    {
      "id": "boss",
      "x": 150.0,
      "y": -50.0
    }
  ]
}
```

### Items

Starting items given to the player when the Fungame loads.

```json
{
  "items": [
    {
      "id": "rifle",
      "slot": 0,
      "force": true
    },
    {
      "id": "pistol",
      "slot": 1,
      "force": false
    }
  ]
}
```

### Commands

Execute game console commands when the Fungame loads.

```json
{
  "command": {
    "once_commands": [
      "alert true Welcome to my map!",
      "tp 0 0"
    ],
    "loop_commands": [
      "alert false Stay alive!"
    ],
    "loop_interval": 10.0
  }
}
```

| Field           | Type       | Description                                         |
|-----------------|------------|-----------------------------------------------------|
| `once_commands` | `string[]` | Commands executed once when the Fungame loads       |
| `loop_commands` | `string[]` | Commands executed repeatedly on an interval         |
| `loop_interval` | `float`    | Interval in seconds between loop command executions |

### XP Configuration

Configure the player's skill levels and experience when the Fungame loads.

```json
{
  "xp": {
    "str_xp": 5,
    "res_xp": 3,
    "int_xp": 4,
    "xp_multiple": 2.0
  }
}
```

| Field         | Type    | Default | Description                 |
|---------------|---------|---------|-----------------------------|
| `str_xp`      | `int`   | `0`     | Strength skill level        |
| `res_xp`      | `int`   | `0`     | Resourcefulness skill level |
| `int_xp`      | `int`   | `0`     | Intelligence skill level    |
| `xp_multiple` | `float` | `1.0`   | XP gain multiplier          |

> **Note:** `min_str`/`max_str`, `min_res`/`max_res`, `min_int`/`max_int` (experience thresholds for each level) and
`exp_str`/`exp_res`/`exp_int` (current experience points) are automatically calculated based on the skill levels, so
> they don't need to be specified manually.

---

## Project Structure

```
CustomFungamePack/
├── Plugin.cs                    # Main plugin entry point (BepInEx)
├── Configs.cs                   # Static config accessors
├── ModLocale.cs                 # Localization wrapper
├── Fungame.cs                   # Fungame data model
├── FungameCheck.cs              # Fungame directory scanner & validator
├── ModCommand.cs                # fg command handler
├── WorldGenerationPatch.cs      # Harmony patches for world generation
├── BodyPatch.cs                 # Harmony patches for player body
├── Lang/
│   ├── EnLangGenerator.cs       # English locale generator
│   ├── ZhCnLangGenerator.cs     # Simplified Chinese locale generator
│   └── ZhTwLangGenerator.cs     # Traditional Chinese locale generator
└── Loader/
    ├── MapLoader.cs             # Fungame map data loader
    ├── CustomStructuresLoader.cs # Custom Structures loader
    └── BuildModeSaveLoader.cs   # Build Mode Save loader
```

---

## License

This project is licensed under the GNU General Public License v3.0. See [`LICENSE.md`](LICENSE.md) for details.
