![alt text](Covor.png)

[中文指南](README_ZH.md)

# Custom Fungame Pack

[GitHub](https://github.com/Explosive-Hydra/Custom-Fungame-Pack) / [NexusMods](https://www.nexusmods.com/scavprototype/mods/131)

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
    - [Fungame Directory Structure](#fungame-directory-structure)
    - [Fungame JSON](#fungame-json)
    - [Content Types](#content-types)
        - [MapData](#mapdata)
        - [CustomStructures](#customstructures)
        - [BuildModeSave](#buildmodesave)
    - [Features](#features)
    - [Commands](#commands)
    - [XP Configuration](#xp-configuration)
    - [Localization](#localization)
- [Project Structure](#project-structure)

---

## Overview

**Custom Fungame Pack** is a BepInEx plugin that introduces a custom map/gamemode system called "Fungame" to Casualties
Unknown. It allows players to load, manage, and create custom maps with unique features, waypoints, items, and game
logic.

---

## Installation

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx) for Casualties Unknown.
2. Install [Moss Lib](https://github.com/Explosive-Hydra/Moss-Lib).
3. Download the latest release from the [Releases](https://github.com/Explosive-Hydra/Custom-Fungame-Pack/releases) page.
4. Extract the downloaded archive and place the entire `Custom Fungame Pack` folder into your `BepInEx/plugins/` folder.
5. Create a `Fungames/` folder in your game's root directory (next to `CasualtiesUnknown.exe`).
6. Place your Fungame folders inside `Fungames/` (see [Fungame Directory Structure](#fungame-directory-structure)).

### Folder Structure

```
Casualties Unknown Demo/
├── BepInEx/
│   └── plugins/
│       ├── Moss Lib
│       │   └── Moss Lib.dll
│       └── Custom Fungame Pack
│           ├── Lang/                  # Auto-generated locale files
│           ├── CustomFungamePack.dll
│           ├── LICENSE.md
│           ├── README.md
│           └── README_ZH.md
├── Fungames/                          # User-created Fungames
│   ├── MyCustomMap/
│   │   ├── fungame.json               # Metadata (id, author, version)
│   │   ├── level/                     # Level data files
│   │   ├── world/                     # World feature files
│   │   ├── player/                    # Player feature files
│   │   └── lang/                      # Localized name/description
│   └── AnotherMap/
│       ├── fungame.json
│       ├── level/
│       └── lang/
└── CasualtiesUnknown.exe
```

---

## Config Options

Configure via `BepInEx/config/blackmoss.customfungamepack.cfg`:

| Key                 | Type     | Default    | Description                                            |
|---------------------|----------|------------|--------------------------------------------------------|
| `more_logs`         | `bool`   | `false`    | Enable verbose logging                                 |
| `start_use_fungame` | `bool`   | `false`    | Automatically load a Fungame on new game start         |
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

Each Fungame is a folder inside the `Fungames/` directory.

### Fungame Directory Structure

A Fungame directory can contain the following files and subdirectories:

```
MyCustomMap/
├── fungame.json              # Metadata (required)
├── level/                    # Level data files
│   ├── level1.json           # First level (auto-generated)
│   └── level2.json           # Additional levels
├── world/                    # World feature data
│   ├── settings.json         # World settings (Fullbright, Gravity, etc.)
│   ├── mine.json             # Mine feature
│   ├── jump_pad.json         # Jump pad feature
│   ├── turret.json           # Turret feature
│   ├── sound_cannon.json     # Sound cannon feature
│   ├── spike_stabber.json    # Spike stabber feature
│   ├── geyser.json           # Geyser feature
│   └── beartrap.json         # Bear trap feature
├── player/                   # Player feature data
│   └── xp.json               # XP configuration
├── command.json              # Console commands
└── lang/                     # Localization files
    ├── zh-CN.json            # Simplified Chinese
    ├── EN.json               # English
    └── ZhTw.json             # Traditional Chinese
```

> Most files are auto-generated when using `fg save as`. You only need to manually create `fungame.json`.

### Fungame JSON

The `fungame.json` file contains metadata. Note that `name` and `description` are stored in per-Fungame
localization files under the `lang/` directory, not in this JSON.

```json
{
  "id": "my_custom_map",
  "version": "1.0.0",
  "author": [
    "YourName"
  ],
  "type": "fungame"
}
```

| Field     | Type       | Required | Description                                                     |
|-----------|------------|----------|-----------------------------------------------------------------|
| `id`      | `string`   | No       | Unique identifier (auto-generated from folder name if omitted)  |
| `version` | `string`   | No       | Version string (default: `1.0.0`)                               |
| `author`  | `string[]` | No       | List of authors (default: `["Unknown"]`)                        |
| `type`    | `string`   | No       | Always `"fungame"` for validation                               |

> **Localization:** The display `name` and `description` are read from `lang/{currentLocale}.json` under the
> key `fungame.name` and `fungame.description`. If no locale file exists, the Fungame object's raw property values are
> used as fallback.

### Content Types

Each Fungame's level can have one of three content types.

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

### Localization

Each Fungame can have localized name and description text in its own `lang/` directory.

The locale file follows this format:

```json
{
  "fungame": {
    "name": "My Custom Map",
    "description": "A cool custom map"
  }
}
```

The file name must match the game's current locale setting (e.g., `zh-CN.json` for Simplified Chinese,
`EN.json` for English). The game's current language is determined by `PlayerPrefs.GetString("locale", "EN")`.

When displaying a Fungame's name or description, the system:
1. Reads from `{FungameDir}/lang/{currentLocale}.json` → `fungame.name` / `fungame.description`
2. Falls back to the raw `Name` / `Description` property on the Fungame object if no localized text is found

> **Note:** The `author` field is NOT localized—it is always read from `fungame.json`.

---

## Project Structure

```
CustomFungamePack/
├── Plugin.cs                       # Main plugin entry point (BepInEx)
├── ModConfigs.cs                   # Static config accessors
├── ModLocale.cs                    # Plugin-wide localization (ModLocaleBase)
├── ModCommand.cs                   # fg console command handler
├── Fungame.cs                      # Fungame data model
├── FungameCheck.cs                 # Fungame directory scanner & initializer
├── FungameLocale.cs                # Per-Fungame localization helper
├── Data/
│   ├── CommandData.cs              # Command configuration model
│   ├── MapData.cs                  # Map data model (grid + key)
│   ├── SpikeStabberData.cs         # Spike stabber feature data
│   ├── WaypointData.cs             # Waypoint data
│   └── Feature/
│       ├── Player/
│       │   └── XpData.cs           # XP configuration model
│       └── World/
│           ├── BearTrapData.cs     # Bear trap feature data
│           ├── ExplosionParamsData.cs
│           ├── GeyserData.cs       # Geyser feature data
│           ├── ItemData.cs         # Starting item data
│           ├── JumpPadData.cs      # Jump pad feature data
│           ├── LevelData.cs        # Level data model (core)
│           ├── MineData.cs         # Mine feature data
│           ├── SoundCannonData.cs  # Sound cannon feature data
│           ├── TurretData.cs       # Turret feature data
│           └── WorldSettingsData.cs # World settings (Fullbright, Gravity, etc.)
├── Lang/
│   ├── EnLangGenerator.cs          # English locale file generator
│   ├── ZhCnLangGenerator.cs        # Simplified Chinese locale file generator
│   └── ZhTwLangGenerator.cs        # Traditional Chinese locale file generator
├── Loader/
│   ├── BuildModeSaveLoader.cs      # Build Mode Save loader (soft dependency)
│   ├── CustomStructuresLoader.cs   # Custom Structures loader (soft dependency)
│   ├── FungameDirectoryLoader.cs   # Fungame directory serialization (load/save)
│   └── MapLoader.cs                # Map data parser & block placer
├── Patch/
│   ├── BearTrapScriptPatch.cs      # Bear trap Harmony patches
│   ├── BodyPatch.cs                # Player body Harmony patches (multi-jump, multi-climb)
│   ├── GeyserScriptPatch.cs        # Geyser Harmony patches
│   ├── JumpPadScriptPatch.cs       # Jump pad Harmony patches
│   ├── MineScriptPatch.cs          # Mine Harmony patches
│   ├── SoundCannonScriptPatch.cs   # Sound cannon Harmony patches
│   ├── SpikeStabberScriptPatch.cs  # Spike stabber Harmony patches
│   ├── TurretScriptPatch.cs        # Turret Harmony patches
│   └── WorldGenerationPatch.cs     # World generation Harmony patches (core)
├── Logo.png                        # Plugin logo
├── Covor.png                       # Cover image
├── CustomFungamePack.csproj        # Project file
├── LICENSE.md                      # GNU General Public License v3.0
├── README.md                       # English documentation
├── README_ZH.md                    # Chinese documentation
├── StartGame.ps1                   # Game launcher PowerShell script
└── .gitignore
```

---

## License

This project is licensed under the GNU General Public License v3.0. See [`LICENSE.md`](LICENSE.md) for details.
