![Logo](Logo.png)

[English Guide](README.md) | [Руководство на русском](README_RU.md)

# Custom Map

[GitHub](https://github.com/CNCUMC/Custom-Map) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/436) | [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)

_一个为 [Casualties Unknown](https://store.steampowered.com/app/4576490/)
打造的自定义地图加载器，基于 [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9)、[Bark](https://github.com/CNCUMC/Bark)
和 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib)。_

**_本模组支持俄语 (感谢 @Crescia1949 和 [Mimo](https://mimo.mi.com/)大模型!)_**

_@Crescia1949 不是俄语母语者，AI翻译也不一定准确，有任何建议在 [NexusMods post](https://www.nexusmods.com/scavprototype/mods/436)
或 [GitHub Issues](https://github.com/CNCUMC/Custom-Map/issues) 及时报告。_

---

## 目录

- [概述](#概述)
- [安装](#安装)
- [快速开始](#快速开始)
- [指令](#指令)
- [地图结构](#地图结构)
- [层级](#层级)
- [指令系统](#指令系统)
- [特性系统](#特性系统)
- [模组设置](#模组设置)
- [兼容性](#兼容性)
- [许可证](#许可证)

---

## 概述

**Custom Map** 允许你在 Casualties Unknown 中加载自定义地图。

- **层级地图** — 支持多层级，每个层级独立设置、结构和指令
- **特性系统** — 按地图配置地雷、炮塔、弹跳板、尖刺陷阱、声波炮、间歇泉、捕兽夹等
- **自定义加载界面** — 地图生成时显示实时进度
- **指令接口** — 完整的 `cm` 指令集用于管理地图
- **多语言支持** — English、简体中文、繁體中文、Русский
- **基于 Custom Structures** — 扩展 [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9)，
  提供多层级支持、特性系统和游戏内指令

---

## 安装

1. 为 Casualties Unknown 安装 [BepInEx 5.x](https://github.com/BepInEx/BepInEx)。
2. 安装 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) — 将 `CUCoreLib.dll` 放入 `BepInEx/plugins/`。
3. 安装 [Bark](https://github.com/CNCUMC/Bark) — 将 `Bark.dll` 放入 `BepInEx/plugins/Bark/`。
4. 安装 [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) — 放入 `BepInEx/plugins/`。
5. 安装 Custom Map，放入 `BepInEx/plugins/Custom Map`。
6. 将地图文件夹放入 `Maps/`（游戏可执行文件同级目录）。

---

## 快速开始

### 加载内置模板

Custom Map 内置了一个模板地图。在模组设置中启用 **Start Game Use Map**，选择模板，然后开始新游戏。

### 使用自定义地图

1. 在 `Maps/你的地图名/` 下创建地图文件夹（参见[地图结构](#地图结构)）。
2. 启动游戏。地图会出现在 `模组设置 → Custom Map → First Use Map` 下拉菜单中。
3. 在控制台输入 `cm select 你的地图名`，或在开始前从下拉菜单选择。

---

## 指令

所有指令使用 `cm` 前缀。

| 指令                        | 描述                                |
|---------------------------|-----------------------------------|
| `cm help`                 | 显示帮助                              |
| `cm reload`               | 从磁盘重新加载当前地图                       |
| `cm load`                 | 重新加载 Maps 文件夹中的所有地图               |
| `cm savereload`           | 保存并重新加载当前地图                       |
| `cm info`                 | 显示当前地图详情                          |
| `cm spawn`                | 传送到地图出生点                          |
| `cm select <id>`          | 按 ID、名称或序号选择地图                    |
| `cm list`                 | 列出所有可用地图                          |
| `cm feature [名称] [值]`     | 列出或设置地图特性                         |
| `cm waypoint [list\|get]` | 管理坐标点                             |
| `cm save`                 | 保存当前地图到磁盘                         |
| `cm layer [n]`            | 查看或切换层级                           |
| `cm exit <目标>`            | 退出地图 — `none`（原版）或 `tutorial`（教程） |

---

## 地图结构

地图是 `Maps/` 下的一个文件夹，结构如下：

```
Maps/
└── 你的地图名/
    ├── map.json                      # 地图元数据
    ├── layers/
    │   ├── layer1/                   # 层级 1 文件夹
    │   │   ├── layer.json            # 层级属性
    │   │   ├── command.json          # 层级指令（可选）
    │   │   └── *.m2.json             # 结构文件（可选）
    │   └── layer2/                   # 层级 2 文件夹（可选）
    │       ├── layer.json
    │       ├── command.json
    │       └── *.m2.json
    ├── command.json                  # 全局指令 — 作为所有层级的回退（可选）
    ├── feature/
    │   ├── world/
    │   │   ├── settings.json         # 世界设置（重力、全亮、跳过标志）
    │   │   ├── mine.json             # 地雷配置（可选）
    │   │   ├── jump_pad.json         # 弹跳板配置（可选）
    │   │   ├── turret.json           # 炮塔配置（可选）
    │   │   ├── sound_cannon.json     # 声波炮配置（可选）
    │   │   ├── spike_stabber.json    # 尖刺陷阱配置（可选）
    │   │   ├── geyser.json           # 间歇泉配置（可选）
    │   │   └── beartrap.json         # 捕兽夹配置（可选）
    │   └── player/
    │       └── xp.json               # XP 配置（可选）
    └── lang/
        ├── EN.json                   # 英文本地化
        ├── zh-CN.json                # 简体中文本地化
        └── zh-TW.json                # 繁体中文本地化
```

### map.json

```json
{
  "id": "mymap",
  "version": "1.0.0",
  "author": [
    "作者名"
  ]
}
```

| 字段        | 类型         | 描述              |
|-----------|------------|-----------------|
| `id`      | `string`   | 地图唯一标识符         |
| `version` | `string`   | 地图版本（如 `1.0.0`） |
| `author`  | `string[]` | 作者列表            |

---

## 层级

每个层级是 `layers/` 下的一个文件夹（如 `layers/layer1/`）。

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

| 字段                | 类型         | 默认值     | 描述                                                                        |
|-------------------|------------|---------|---------------------------------------------------------------------------|
| `type`            | `string`   | `Debug` | 场景类型：`Debug`、`Wasteland`、`TemperateForest`、`RockDesert`、`Tutorial`、`None` |
| `spawn`           | `number[]` | `[0,0]` | 出生点坐标 `[x, y]`                                                            |
| `x`               | `int`      | `0`     | 层级的世界 X 偏移                                                                |
| `y`               | `int`      | `0`     | 层级的世界 Y 偏移                                                                |
| `skip_terrain`    | `bool`     | `true`  | 跳过原版地形生成                                                                  |
| `skip_structures` | `bool`     | `true`  | 跳过原版结构生成                                                                  |
| `skip_background` | `bool`     | `true`  | 跳过原版背景生成                                                                  |
| `waypoints`       | `array`    | `[]`    | 坐标点列表（用于 `cm waypoint`）                                                   |
| `items`           | `array`    | `[]`    | 出生时给予的物品                                                                  |

### 结构文件 (*.m2.json)

层级文件夹中的每个 `.m2.json` 文件定义一个结构放置：

```json
{
  "structure": "StructureName",
  "x": 10,
  "y": 20
}
```

| 字段          | 类型       | 描述                              |
|-------------|----------|---------------------------------|
| `structure` | `string` | 结构名称（通过 Custom Structures 模组注册） |
| `x`         | `int`    | 世界 X 坐标                         |
| `y`         | `int`    | 世界 Y 坐标                         |

---

## 指令系统

### 全局指令（地图根目录 `command.json`）

应用于**所有层级**作为回退。如果层级有自己的 `command.json`，层级级优先。

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

### 层级指令（层级文件夹 `command.json`）

覆盖特定层级的全局指令。格式同上。

| 字段              | 类型         | 描述           |
|-----------------|------------|--------------|
| `once_commands` | `string[]` | 地图加载时执行一次的指令 |
| `loop_commands` | `string[]` | 循环执行的指令      |
| `loop_interval` | `float`    | 循环指令间隔（秒）    |

---

## 特性系统

`feature/world/` 或 `feature/player/` 中的每个特性文件配置对应的游戏实体。

| 特性                   | 描述                                                                        |
|----------------------|---------------------------------------------------------------------------|
| `settings.json`      | 世界设置：`full_bright`、`gravity`、`jump_limit`、`climb_limit`、`forgiving_level` |
| `mine.json`          | 地雷：冷却时间、不毁                                                                |
| `jump_pad.json`      | 弹跳板：冷却时间、力度、无光                                                            |
| `turret.json`        | 炮塔：冷却时间、射击力度、范围、不毁、无光                                                     |
| `sound_cannon.json`  | 声波炮：冷却时间、范围、充能时间、不毁                                                       |
| `spike_stabber.json` | 尖刺陷阱：伤害、冷却时间、不毁、无光、音效                                                     |
| `geyser.json`        | 间歇泉：冷却时间、喷发时长、范围、震动时长、无液体                                                 |
| `beartrap.json`      | 捕兽夹：伤害、冷却时间、不毁                                                            |
| `xp.json`            | XP：力量/韧性/智力等级和经验范围                                                        |

---

## 模组设置

| 设置                         | 描述           | 默认值        |
|----------------------------|--------------|------------|
| `More Logs`                | 启用详细调试日志     | `false`    |
| `Start Game Use Map`       | 开始新游戏时自动加载地图 | `false`    |
| `First Use Map`            | 首次开始游戏时加载的地图 | `template` |
| `Progress Update Interval` | 进度更新间隔（方块数）  | `333`      |

---

## 兼容性

Custom Map 支持与以下模组的软集成：

| 模组                                                                  | 必需 | 描述                                           |
|---------------------------------------------------------------------|----|----------------------------------------------|
| [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) | 否  | 将 `.txt` / `.ms.json` / `.ms2.json` 文件用于结构放置 |

---

## 许可证

[LGPL-3.0](LICENSE.md)
