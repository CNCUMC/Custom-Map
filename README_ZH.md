![Cover](Cover.png)

[English Guide](README.md)

# Custom Map

[GitHub](https://github.com/CNCUMC/Custom-Map) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/436) | [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)

_一个为 [Casualties Unknown](https://store.steampowered.com/app/3624440/Casualties_Unknown_Demo/)
打造的 自定义地图 加载器，基于 [Bark](https://github.com/CNCUMC/Bark) 和 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)。_

---

## 目录

- [概述](#概述)
- [安装](#安装)
- [快速开始](#快速开始)
- [指令](#指令)
- [地图结构](#地图结构)
- [特性系统](#特性系统)
- [模组设置](#模组设置)
- [许可证](#许可证)

---

## 概述

**Custom Map** 允许你在 Casualties Unknown 中加载自定义地图。地图使用基于字符串的格式定义，通过键值映射系统精确控制每个方块和实体的位置。

- **字符串地图格式** — 地图网格中的每个字符通过 JSON key 字典映射到方块 ID 或实体 ID
- **特性系统** — 按地图配置地雷、炮塔、弹跳板、尖刺陷阱、间歇泉、捕兽夹等
- **自定义加载界面** — 地图生成时显示实时进度
- **指令接口** — 完整的 `cm` 指令集用于管理地图
- **多语言支持** — English、简体中文、繁體中文

---

## 安装

1. 为 Casualties Unknown 安装 [BepInEx 5.x](https://github.com/BepInEx/BepInEx)。
2. 安装 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) ≥ 1.0.2 —
   将 `CUCoreLib.dll` 放入 `BepInEx/plugins/CUCoreLib/`。
3. 安装 [Bark](https://github.com/CNCUMC/Bark) ≥ 1.0.3 —
   将 `Bark.dll` 放入 `BepInEx/plugins/Bark/`。
4. 从 [Releases](https://github.com/CNCUMC/Custom-Map/releases) 下载最新版 `CustomMap.dll`。
5. 将 `CustomMap.dll` 放入 `BepInEx/plugins/CustomMap/`。
6. 将地图文件夹放入 `Maps/`（游戏可执行文件同级目录）。

---

## 快速开始

### 加载内置模板

Custom Map 内置了一个模板地图。在模组设置中启用 **开始游戏时使用地图**，选择模板，然后开始新游戏。

### 使用自定义地图

1. 在 `Maps/你的地图名/` 下创建地图文件夹（参见[地图结构](#地图结构)）。
2. 启动游戏。地图会出现在 `模组设置 → Custom Map → 首次使用地图` 下拉菜单中。
3. 在控制台输入 `cm select 你的地图名`，或在开始前从下拉菜单选择。

---

## 指令

所有指令使用 `cm` 前缀（或 `custommap`）。

| 指令                  | 描述                |
|---------------------|-------------------|
| `cm help`           | 显示帮助              |
| `cm list`           | 列出所有可用地图          |
| `cm list <id>`      | 按 ID 或序号选择并加载地图   |
| `cm select <id>`    | 选择地图（世界已加载时重载）    |
| `cm reload`         | 从磁盘重新加载当前地图       |
| `cm info`           | 显示当前地图详情          |
| `cm spawn`          | 传送到地图出生点          |
| `cm feature`        | 列出或修改地图特性         |
| `cm waypoint`       | 列出或传送到坐标点         |
| `cm save <起点> <终点>` | 将区域保存到当前地图        |
| `cm save as [名称]`   | 将区域保存为新地图（点击两个位置） |
| `cm exit`           | 退出自定义地图，返回原版      |

---

## 地图结构

地图是 `Maps/` 下的一个文件夹，结构如下：

```
Maps/
└── 你的地图名/
    ├── map.json              # 地图元数据（名称、ID、版本、作者、描述）
    ├── level/
    │   └── level1.json       # 关卡数据（地图网格、键值表、出生点、物品、坐标点）
    ├── feature/
    │   ├── world/
    │   │   ├── settings.json # 世界设置（重力、全亮、跳过标志）
    │   │   ├── mine.json
    │   │   ├── turret.json
    │   │   ├── jump_pad.json
    │   │   ├── spike_stabber.json
    │   │   ├── sound_cannon.json
    │   │   ├── geyser.json
    │   │   └── beartrap.json
    │   └── player/
    │       └── xp.json       # XP 倍率设置
    └── lang/
        └── zh-CN.json        # 地图名称/描述本地化
```

### map.json

```json
{
  "name": "我的地图",
  "id": "mymap",
  "version": "1.0.0",
  "author": ["作者名"],
  "description": "一张自定义地图",
  "type": "Map"
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
  "scene_type": "Debug",
  "spawn": [0.0, 0.0],
  "x": -68,
  "y": 62,
  "waypoints": [],
  "items": [],
  "type": "level"
}
```

- `map` — 字符网格（从上到下 = 世界中从上到下）
- `key` — 将每个字符映射到方块 ID（数字）或实体 ID（字符串）
- `scene_type` — `"Debug"`、`"Wasteland"`、`"TemperateForest"`、`"RockDesert"` 或 `"None"`
- `x`、`y` — 地图左下角的世界坐标

---

## 特性系统

每个特性文件覆盖对应游戏实体的默认行为：

| 特性                   | 描述                                                                              |
|----------------------|---------------------------------------------------------------------------------|
| `settings.json`      | 世界设置：`full_bright`、`gravity`、`skip_terrain`、`skip_structures`、`skip_background` |
| `mine.json`          | 地雷爆炸参数、无法摧毁标志、冷却时间                                                              |
| `turret.json`        | 炮塔瞄准范围、射速、伤害                                                                    |
| `jump_pad.json`      | 弹跳板力度、冷却时间                                                                      |
| `spike_stabber.json` | 尖刺伤害倍率、音效、无法摧毁标志                                                                |
| `sound_cannon.json`  | 声波炮参数                                                                           |
| `geyser.json`        | 间歇泉喷发参数                                                                         |
| `beartrap.json`      | 捕兽夹伤害、逃脱难度                                                                      |
| `xp.json`            | XP 倍率覆盖                                                                         |

---

## 模组设置

| 设置                         | 描述           | 默认值        |
|----------------------------|--------------|------------|
| `More Logs`                | 启用详细调试日志     | `false`    |
| `Start Game Use Map`       | 开始新游戏时自动加载地图 | `false`    |
| `First Use Map`            | 首次开始游戏时加载的地图 | `template` |
| `Progress Update Interval` | 进度更新间隔（毫秒）   | `333`      |

---

## 兼容性

Custom Map 支持与以下模组的软集成：

| 模组 | GUID | 描述 |
|------|------|------|
| [Custom Structures](https://www.nexusmods.com/scavprototype/mods/) | `com.Jimmyking.morestructures` | 加载放置在地图关卡文件夹中的 `.ms.json` 结构文件 |
| [Build Mode](https://www.nexusmods.com/scavprototype/mods/) | `com.alexx_.buildmode` | 将 `.alexx_BMsave` 建造模式存档加载到地图中 |

两个模组均非必需 — Custom Map 可完全独立运行。若已安装，地图作者可在 `level1.json` 中通过 `custom_structures` 和 `build_mode_save` 字段引用对应文件。

---

## 许可证

[LGPL v3](LICENSE.md)
