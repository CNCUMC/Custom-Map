![alt text](Covor.png)

[English Guide](README.md)

# Custom Fungame Pack

[GitHub](https://github.com/Black-Moss/Custom-Fungame-Pack) / [NexusMods](https://www.nexusmods.com/games/scavprototype/mods/131)

_**Casualties Unknown** 的自定义地图/游戏模式（"Fungame"）管理系统。_

---

## 目录

- [概述](#概述)
- [安装](#安装)
- [配置选项](#配置选项)
- [命令参考](#命令参考)
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
- [创建 Fungame](#创建-fungame)
    - [Fungame JSON 格式](#fungame-json-格式)
    - [内容类型](#内容类型)
        - [MapData（地图数据）](#mapdata地图数据)
        - [CustomStructures（自定义结构）](#customstructures自定义结构)
        - [BuildModeSave（建筑模式存档）](#buildmodesave建筑模式存档)
    - [特性（Features）](#特性features)
    - [命令（Commands）](#命令commands)
    - [经验值配置（XP Configuration）](#经验值配置xp-configuration)
- [项目结构](#项目结构)

---

## 概述

**Custom Fungame Pack** 是一个 BepInEx 插件，为 Casualties Unknown 引入了名为 "Fungame"
的自定义地图/游戏模式系统。玩家可以加载、管理和创建具有独特特性、路径点、物品和游戏逻辑的自定义地图。

---

## 安装

1. 安装适用于 Casualties Unknown 的 [BepInEx 5.x](https://github.com/BepInEx/BepInEx)。
2. 安装 [Moss Lib](https://www.nexusmods.com/scavprototype/mods/11)。
3. 从 [Releases](https://github.com/Black-Moss/Custom-Fungame-Pack/releases) 页面下载最新的发布包。
4. 解压后将整个 `Custom Fungame Pack` 文件夹放入 `BepInEx/plugins/` 文件夹。
5. 在游戏根目录（与 `CasualtiesUnknown.exe` 同目录）创建 `Fungames/` 文件夹。
6. 将你的 Fungame 文件夹放入 `Fungames/`（每个文件夹包含一个 `fungame.json`）。

### 文件夹结构

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

## 配置选项

通过 `BepInEx/config/blackmoss.customfungamepack.cfg` 进行配置：

| 键名                  | 类型       | 默认值        | 描述                                      |
|---------------------|----------|------------|-----------------------------------------|
| `more_logs`         | `bool`   | `false`    | 启用详细日志记录                                |
| `start_use_fungame` | `bool`   | `false`    | 开始新游戏时自动加载 Fungame                      |
| `first_use_fungame` | `string` | `template` | 启用 `Start Use Fungame` 时要加载的 Fungame ID |

---

## 命令参考

所有命令通过游戏内控制台（按 ~ 键打开）中的 **`fungame`** 或者 **`fg`** 命令访问。

### fg select

通过 ID、名称或索引选择并加载 Fungame。

```
fg select <id|名称|索引>
```

**示例：**

```
fg select my_map              # 通过 ID 选择
fg select 我的酷炫地图          # 通过名称选择
fg select 1                   # 通过列表索引选择
```

### fg list

列出所有可用的 Fungame，包括 ID、版本和作者，同时也能当 `fg select` 用。

```
fg list
```

在其后添加参数即可。

```
fg list <id|名称|索引>
```

**示例：**

```
fg list my_map              # 通过 ID 选择
fg list 我的酷炫地图          # 通过名称选择
fg list 1                   # 通过列表索引选择
```

### fg info

显示当前加载的 Fungame 的信息。

```
fg info
```

### fg spawn

传送回 Fungame 的出生点。

```
fg spawn
```

### fg exit

退出当前 Fungame 并重新加载场景。

```
fg exit none                  # 返回原版游戏
fg exit tutorial              # 返回教程
```

### fg reload

重新加载当前 Fungame 的地图数据。

```
fg reload
```

### fg feature

管理 Fungame 特性（列出/查看/设置）。

```
fg feature list                # 列出所有特性及其值
fg feature get <名称>          # 查看特定特性的值
fg feature set <名称> <值>     # 设置特性值
```

**可用特性：**

| 特性               | 类型      | 默认值     | 描述               |
|------------------|---------|---------|------------------|
| `Fullbright`     | `bool`  | `true`  | 启用全局高亮           |
| `ForgivingLevel` | `bool`  | `false` | 启用仁慈关卡模式         |
| `Gravity`        | `float` | `-9.81` | 自定义重力值           |
| `JumpLimit`      | `int`   | `0`     | 最大跳跃次数（`0` = 无限） |
| `ClimbLimit`     | `int`   | `0`     | 最大攀爬次数（`0` = 无限） |

### fg waypoint

管理 Fungame 路径点（列出/传送）。

```
fg waypoint list               # 列出所有路径点
fg waypoint get <id|索引>      # 传送到指定路径点
```

### fg save

将当前 Fungame 配置保存到磁盘。

```
fg save                        # 保存到 Fungame 的目录
```

### fg save as

交互式选择区域并将其保存为当前 Fungame 的地图数据。

```
fg save as                     # 按屏幕提示操作
```

---

## 创建 Fungame

每个 Fungame 是 `Fungames/` 目录下的一个文件夹，内含 `fungame.json` 文件。

### Fungame JSON 格式

```json
{
  "name": "我的自定义地图",
  "id": "my_custom_map",
  "version": "1.0.0",
  "author": [
    "你的名字"
  ],
  "description": "一张很酷的自定义地图",
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

| 字段                  | 类型         | 必需                                                      | 描述                                                     |
|---------------------|------------|---------------------------------------------------------|--------------------------------------------------------|
| `name`              | `string`   | 是                                                       | 显示名称                                                   |
| `id`                | `string`   | 是                                                       | 唯一标识符（省略时从文件夹名自动生成）                                    |
| `version`           | `string`   | 是                                                       | 版本字符串（默认：`1.0.0`）                                      |
| `author`            | `string[]` | 是                                                       | 作者列表                                                   |
| `description`       | `string`   | 是                                                       | 地图描述                                                   |
| `spawn`             | `float[2]` | 否                                                       | 出生点坐标 `[x, y]`（默认：`[0, 0]`）                            |
| `x`, `y`            | `int`      | 否                                                       | 地图位置原点          （默认：`[0, 0]`）                          |
| `type`              | `string`   | 否                                                       | 场景类型：`Debug`、`None`、`Tutorial`（默认：`Debug`）             |
| `feature`           | `object`   | 否                                                       | 特性（见 [特性](#特性features)）                                |
| `waypoints`         | `array`    | 否                                                       | 路径点列表（见 [路径点](#路径点)）                                   |
| `items`             | `array`    | 否                                                       | 初始物品（见 [物品](#物品)）                                      |
| `skip_terrain`      | `bool`     | 否                                                       | 跳过地形生成（默认：`true`）                                      |
| `skip_structures`   | `bool`     | 否                                                       | 跳过结构生成（默认：`true`）                                      |
| `skip_background`   | `bool`     | 否                                                       | 跳过背景生成（默认：`true`）                                      |
| `xp`                | `object`   | 否                                                       | 经验值配置（见 [经验值配置](#经验值配置xp-configuration)）               |
| `map_data`          | `object`   | `map_data`/ `custom_structures` / `build_mode_save` 三选一 | 地图数据内容（见 [MapData](#mapdata地图数据)）                      |
| `custom_structures` | `string`   | `map_data`/ `custom_structures` / `build_mode_save` 三选一 | 自定义结构文件名（见 [CustomStructures](#customstructures自定义结构)） |
| `build_mode_save`   | `string`   | `map_data`/ `custom_structures` / `build_mode_save` 三选一 | 建筑模式存档文件名（见 [BuildModeSave](#buildmodesave建筑模式存档)）     |
| `command`           | `object`   | 否                                                       | 命令配置（见 [命令](#命令commands)）                              |

> **注意：** 一次只能使用一种内容类型（`map_data`、`custom_structures` 或 `build_mode_save`）。

### 内容类型

#### MapData（地图数据）

将地图定义为字符网格，并使用键将每个字符映射到方块/物品。

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

#### CustomStructures（自定义结构）

引用包含 [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) 数据的 JSON 文件。

```json
{
  "custom_structures": "my_structures.json"
}
```

> **需要：** 安装 [Custom Structures](https://www.nexusmods.com/scavprototype/mods/9) 模组。

#### BuildModeSave（建筑模式存档）

引用建筑模式存档文件（`.alexx_BMsave`）。

```json
{
  "build_mode_save": "my_save.alexx_BMsave"
}
```

> **需要：** 安装 [BuildMod](https://www.nexusmods.com/scavprototype/mods/24) 模组。

### 特性（Features）

特性在 Fungame 激活时修改游戏行为。

| 特性               | 类型      | 默认值     | 描述                  |
|------------------|---------|---------|---------------------|
| `Fullbright`     | `bool`  | `true`  | 所有区域完全明亮            |
| `ForgivingLevel` | `bool`  | `false` | 玩家死亡时不丢失物品          |
| `Gravity`        | `float` | `-9.81` | 覆盖重力（负值 = 向下）       |
| `JumpLimit`      | `int`   | `0`     | 落地前最大跳跃次数（`0` = 无限） |
| `ClimbLimit`     | `int`   | `0`     | 落地前最大攀爬次数（`0` = 无限） |

### 路径点

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

### 物品

Fungame 加载时给予玩家的初始物品。

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

### 命令（Commands）

Fungame 加载时执行游戏控制台命令。

```json
{
  "command": {
    "once_commands": [
      "alert true 欢迎来到我的地图！",
      "tp 0 0"
    ],
    "loop_commands": [
      "alert false 活下去！"
    ],
    "loop_interval": 10.0
  }
}
```

| 字段              | 类型         | 描述                 |
|-----------------|------------|--------------------|
| `once_commands` | `string[]` | Fungame 加载时执行一次的命令 |
| `loop_commands` | `string[]` | 按间隔重复执行的命令         |
| `loop_interval` | `float`    | 循环命令执行的间隔秒数        |

### 经验值配置（XP Configuration）

配置 Fungame 加载时玩家的技能等级和经验值。

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

| 字段            | 类型      | 默认值   | 描述     |
|---------------|---------|-------|--------|
| `str_xp`      | `int`   | `0`   | 力量技能等级 |
| `res_xp`      | `int`   | `0`   | 智谋技能等级 |
| `int_xp`      | `int`   | `0`   | 智力技能等级 |
| `xp_multiple` | `float` | `1.0` | 经验倍率   |

> **注意：** `min_str`/`max_str`、`min_res`/`max_res`、`min_int`/`max_int`（每个等级的经验阈值）和 `exp_str`/`exp_res`/
`exp_int`（当前经验值）会根据技能等级自动计算，无需手动指定。

---

## 项目结构

```
CustomFungamePack/
├── Plugin.cs                    # 主插件入口点（BepInEx）
├── Configs.cs                   # 静态配置访问器
├── ModLocale.cs                 # 本地化封装
├── Fungame.cs                   # Fungame 数据模型
├── FungameCheck.cs              # Fungame 目录扫描器与验证器
├── ModCommand.cs                # fg 命令处理器
├── WorldGenerationPatch.cs      # 世界生成的 Harmony 补丁
├── BodyPatch.cs                 # 玩家身体的 Harmony 补丁
├── Lang/
│   ├── EnLangGenerator.cs       # 英语语言文件生成器
│   ├── ZhCnLangGenerator.cs     # 简体中文语言文件生成器
│   └── ZhTwLangGenerator.cs     # 繁体中文语言文件生成器
└── Loader/
    ├── MapLoader.cs             # Fungame 地图数据加载器
    ├── CustomStructuresLoader.cs # 自定义结构加载器
    └── BuildModeSaveLoader.cs   # 建筑模式存档加载器
```

---

## 许可证

本项目使用 GNU General Public License v3.0 许可证。详情请参阅 [`LICENSE.md`](LICENSE.md)。
