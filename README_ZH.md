![alt text](Covor.png)

[English Guide](README.md)

# Custom Fungame Pack

[GitHub](https://github.com/Explosive-Hydra/Custom-Fungame-Pack) / [NexusMods](https://www.nexusmods.com/scavprototype/mods/131)

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
    - [Fungame 目录结构](#fungame-目录结构)
    - [Fungame JSON](#fungame-json)
    - [内容类型](#内容类型)
        - [MapData（地图数据）](#mapdata地图数据)
        - [CustomStructures（自定义结构）](#customstructures自定义结构)
        - [BuildModeSave（建筑模式存档）](#buildmodesave建筑模式存档)
    - [特性（Features）](#特性features)
    - [命令（Commands）](#命令commands)
    - [经验值配置（XP Configuration）](#经验值配置xp-configuration)
    - [本地化（Localization）](#本地化localization)
- [项目结构](#项目结构)

---

## 概述

**Custom Fungame Pack** 是一个 BepInEx 插件，为 Casualties Unknown 引入了名为 "Fungame"
的自定义地图/游戏模式系统。玩家可以加载、管理和创建具有独特特性、路径点、物品和游戏逻辑的自定义地图。

---

## 安装

1. 安装适用于 Casualties Unknown 的 [BepInEx 5.x](https://github.com/BepInEx/BepInEx)。
2. 安装 [Moss Lib](https://github.com/Explosive-Hydra/Moss-Lib)。
3. 从 [Releases](https://github.com/Explosive-Hydra/Custom-Fungame-Pack/releases) 页面下载最新的发布包。
4. 解压后将整个 `Custom Fungame Pack` 文件夹放入 `BepInEx/plugins/` 文件夹。
5. 在游戏根目录（与 `CasualtiesUnknown.exe` 同目录）创建 `Fungames/` 文件夹。
6. 将你的 Fungame 文件夹放入 `Fungames/`（详见 [Fungame 目录结构](#fungame-目录结构)）。

### 文件夹结构

```
Casualties Unknown Demo/
├── BepInEx/
│   └── plugins/
│       ├── Moss Lib
│       │   └── Moss Lib.dll
│       └── Custom Fungame Pack
│           ├── Lang/                    # 自动生成的语言文件
│           ├── CustomFungamePack.dll
│           ├── LICENSE.md
│           ├── README.md
│           └── README_ZH.md
├── Fungames/                            # 用户创建的 Fungame
│   ├── MyCustomMap/
│   │   ├── fungame.json                 # 元数据（id、author、version）
│   │   ├── level/                       # 关卡数据文件
│   │   ├── world/                       # 世界特性文件
│   │   ├── player/                      # 玩家特性文件
│   │   └── lang/                        # 本地化名称/描述
│   └── AnotherMap/
│       ├── fungame.json
│       ├── level/
│       └── lang/
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

每个 Fungame 是 `Fungames/` 目录下的一个文件夹。

### Fungame 目录结构

一个 Fungame 目录可以包含以下文件和子目录：

```
MyCustomMap/
├── fungame.json              # 元数据（必需）
├── level/                    # 关卡数据文件
│   ├── level1.json           # 第一关（自动生成）
│   └── level2.json           # 额外关卡
├── world/                    # 世界特性数据
│   ├── settings.json         # 世界设置（Fullbright、Gravity 等）
│   ├── mine.json             # 地雷特性
│   ├── jump_pad.json         # 弹跳板特性
│   ├── turret.json           # 炮塔特性
│   ├── sound_cannon.json     # 音波炮特性
│   ├── spike_stabber.json    # 尖刺特性
│   ├── geyser.json           # 间歇泉特性
│   └── beartrap.json         # 捕熊夹特性
├── player/                   # 玩家特性数据
│   └── xp.json               # 经验值配置
├── command.json              # 控制台命令
└── lang/                     # 本地化文件
    ├── zh-CN.json            # 简体中文
    ├── EN.json               # 英语
    └── ZhTw.json             # 繁体中文
```

> 大部分文件在使用 `fg save as` 时会自动生成。你只需要手动创建 `fungame.json`。

### Fungame JSON

`fungame.json` 文件包含元数据。注意 `name` 和 `description` 存储在 `lang/` 目录下的每个 Fungame 的本地化文件中，
而不是此 JSON 中。

```json
{
  "id": "my_custom_map",
  "version": "1.0.0",
  "author": [
    "你的名字"
  ],
  "type": "fungame"
}
```

| 字段      | 类型       | 必需  | 描述                              |
|---------|----------|------|---------------------------------|
| `id`      | `string`   | 否    | 唯一标识符（省略时从文件夹名自动生成） |
| `version` | `string`   | 否    | 版本字符串（默认：`1.0.0`）         |
| `author`  | `string[]` | 否    | 作者列表（默认：`["Unknown"]`）     |
| `type`    | `string`   | 否    | 始终为 `"fungame"` 用于验证         |

> **本地化：** 显示的 `name` 和 `description` 从 `lang/{当前语言}.json` 文件的 `fungame.name` 和
> `fungame.description` 键读取。如果没有本地化文件，则回退到 Fungame 对象的原始属性值。

### 内容类型

每个 Fungame 的关卡可以有三种内容类型之一。

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
> `exp_int`（当前经验值）会根据技能等级自动计算，无需手动指定。

### 本地化（Localization）

每个 Fungame 可以在自己的 `lang/` 目录中拥有本地化的名称和描述文本。

语言文件的格式如下：

```json
{
  "fungame": {
    "name": "我的自定义地图",
    "description": "一张很酷的自定义地图"
  }
}
```

文件名必须与游戏的当前语言设置匹配（例如简体中文为 `zh-CN.json`，英语为 `EN.json`）。
游戏当前语言由 `PlayerPrefs.GetString("locale", "EN")` 决定。

显示 Fungame 的名称或描述时，系统：
1. 从 `{Fungame目录}/lang/{当前语言}.json` → `fungame.name` / `fungame.description` 读取
2. 如果未找到本地化文本，则回退到 Fungame 对象的原始 `Name` / `Description` 属性

> **注意：** `author` 字段不会本地化——始终从 `fungame.json` 读取。

---

## 项目结构

```
CustomFungamePack/
├── Plugin.cs                       # 主插件入口点（BepInEx）
├── ModConfigs.cs                   # 静态配置访问器
├── ModLocale.cs                    # 插件级本地化（ModLocaleBase）
├── ModCommand.cs                   # fg 控制台命令处理器
├── Fungame.cs                      # Fungame 数据模型
├── FungameCheck.cs                 # Fungame 目录扫描器与初始化器
├── FungameLocale.cs                # 每个 Fungame 的本地化助手
├── Data/
│   ├── CommandData.cs              # 命令配置模型
│   ├── MapData.cs                  # 地图数据模型（网格 + 键映射）
│   ├── SpikeStabberData.cs         # 尖刺特性数据
│   ├── WaypointData.cs             # 路径点数据
│   └── Feature/
│       ├── Player/
│       │   └── XpData.cs           # 经验值配置模型
│       └── World/
│           ├── BearTrapData.cs     # 捕熊夹特性数据
│           ├── ExplosionParamsData.cs
│           ├── GeyserData.cs       # 间歇泉特性数据
│           ├── ItemData.cs         # 初始物品数据
│           ├── JumpPadData.cs      # 弹跳板特性数据
│           ├── LevelData.cs        # 关卡数据模型（核心）
│           ├── MineData.cs         # 地雷特性数据
│           ├── SoundCannonData.cs  # 音波炮特性数据
│           ├── TurretData.cs       # 炮塔特性数据
│           └── WorldSettingsData.cs # 世界设置（Fullbright、Gravity 等）
├── Lang/
│   ├── EnLangGenerator.cs          # 英语语言文件生成器
│   ├── ZhCnLangGenerator.cs        # 简体中文语言文件生成器
│   └── ZhTwLangGenerator.cs        # 繁体中文语言文件生成器
├── Loader/
│   ├── BuildModeSaveLoader.cs      # 建筑模式存档加载器（软依赖）
│   ├── CustomStructuresLoader.cs   # 自定义结构加载器（软依赖）
│   ├── FungameDirectoryLoader.cs   # Fungame 目录序列化（加载/保存）
│   └── MapLoader.cs                # 地图数据解析器与方块放置器
├── Patch/
│   ├── BearTrapScriptPatch.cs      # 捕熊夹 Harmony 补丁
│   ├── BodyPatch.cs                # 玩家身体 Harmony 补丁（多段跳、多段攀爬）
│   ├── GeyserScriptPatch.cs        # 间歇泉 Harmony 补丁
│   ├── JumpPadScriptPatch.cs       # 弹跳板 Harmony 补丁
│   ├── MineScriptPatch.cs          # 地雷 Harmony 补丁
│   ├── SoundCannonScriptPatch.cs   # 音波炮 Harmony 补丁
│   ├── SpikeStabberScriptPatch.cs  # 尖刺 Harmony 补丁
│   ├── TurretScriptPatch.cs        # 炮塔 Harmony 补丁
│   └── WorldGenerationPatch.cs     # 世界生成 Harmony 补丁（核心）
├── Logo.png                        # 插件图标
├── Covor.png                       # 封面图像
├── CustomFungamePack.csproj        # 项目文件
├── LICENSE.md                      # GNU General Public License v3.0
├── README.md                       # 英文文档
├── README_ZH.md                    # 中文文档
├── StartGame.ps1                   # 游戏启动 PowerShell 脚本
└── .gitignore
```

---

## 许可证

本项目使用 GNU General Public License v3.0 许可证。详情请参阅 [`LICENSE.md`](LICENSE.md)。
