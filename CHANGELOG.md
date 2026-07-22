# Changelog

## v1.0.0

### Features

- **Layer-based map structure** — Maps now use a folder-based layer system (`layers/layer1/`) instead of flat JSON files
- **Per-layer commands** — Each layer can have its own `command.json` with startup and loop commands
- **Global command fallback** — Map-root `command.json` applies to all layers as a fallback
- **Inline structures** — Structures defined directly in `layer.json` via the `structures` array
- **Multi-language UI** — Full localization support for English, Simplified Chinese, Traditional Chinese, and Russian
- **Custom loading screen** — Real-time progress display during map generation
- **Waypoint system** — Define and teleport to waypoints within layers
- **Layer switching** — Switch between layers in-game with `cm layer`
- **Feature system** — Per-map configuration for mines, turrets, jump pads, spike stabbers, geysers, bear traps, and more
- **Map save/export** — Save current map state back to disk

### Commands

- `cm help` — Show help
- `cm reload` — Reload current map from disk
- `cm load` — Reload all maps from Maps folder
- `cm savereload` — Save and reload current map
- `cm info` — Show current map details
- `cm spawn` — Teleport to spawn point
- `cm select <id>` — Select a map by ID, name, or index
- `cm list` — List all available maps
- `cm feature [name] [value]` — List or set map features
- `cm waypoint [list|get]` — Manage waypoints
- `cm save` — Save current map to disk
- `cm layer [n]` — Show or switch layer
- `cm exit <target>` — Exit to vanilla (`none`) or tutorial (`tutorial`)

### Map Structure

```
Maps/your-map-name/
├── map.json              # Map metadata (id, version, author)
├── layers/
│   ├── layer1/
│   │   ├── layer.json    # Layer properties + structures
│   │   └── command.json  # Layer commands (optional)
│   └── layer2/
│       └── ...
├── command.json          # Global commands - applied to all layers (optional)
├── feature/
│   ├── world/            # World feature configs
│   └── player/           # Player feature configs
└── lang/                 # Localization files
```
