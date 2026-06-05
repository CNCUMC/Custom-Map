# Fungame 结构重构实施方案

> **命名空间约定**：所有 Data 类放在 `Data/` 目录下，命名空间 `CustomFungamePack.Data`

## 概述
将扁平单文件结构重构为多文件目录结构，所有 Feature 数据拆分为独立文件。

## 新旧结构对比

### 旧结构（单文件）
```
Fungame/fungame.json (全部内容)
├── name, id, version, author, description
├── map_data / custom_structures / build_mode_save (互斥)
├── feature: { fullbright, forgiving_level, gravity, ..., mine: {...}, xp: {...} }
├── command: { once_commands, loop_commands, loop_interval }
├── waypoints, items, spawn, x, y, type
└── skip_terrain, skip_structures, skip_background
```

### 新结构（多文件）
```
Fungame/
├── fungame.json                          # 仅元数据
├── level/
│   └── l1.json                           # map_data, custom_structures, build_mode_save (按序不互斥), type, spawn, x, y, waypoints, items
├── feature/
│   ├── world/
│   │   ├── settings.json                 # fullbright, forgiving_level, gravity, jump_limit, climb_limit, skip_terrain, skip_structures, skip_background
│   │   ├── mine.json                     # MineData
│   │   ├── jump_pad.json                 # JumpPadData
│   │   ├── turret.json                   # TurretData
│   │   ├── sound_cannon.json             # SoundCannonData
│   │   ├── spike_stabber.json            # SpikeStabberData
│   │   ├── geyser.json                   # GeyserData
│   │   └── beartrap.json                 # BearTrapData
│   └── player/
│       └── xp.json                       # XpData
├── item/                                 # 暂空
├── lang/                                 # 暂空
└── command.json                          # CommandData
```

## 影响文件

| 文件 | 改动类型 | 说明 |
|------|----------|------|
| `Fungame.cs` | **重写** | 仅保留元数据+运行时属性 |
| `Data/LevelData.cs` | **新建** | 级别数据模型 `Data` 命名空间 |
| `Data/WorldSettingsData.cs` | **新建** | 世界设置数据模型 `Data` 命名空间 |
| `Data/MapData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/CommandData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/WaypointData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/ItemData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/XpData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/MineData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/JumpPadData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/TurretData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/SoundCannonData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/SpikeStabberData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/GeyserData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/BearTrapData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `Data/ExplosionParamsData.cs` | **新建** | 从原 Fungame.cs 拆分 |
| `FungameDirectoryLoader.cs` | **新建** | 统一目录加载器 |
| `FungameCheck.cs` | **重写** | 使用新加载器 |
| `Patch/WorldGenerationPatch.cs` | **修改** | 从Levels读取+遍历 |
| `Loader/MapLoader.cs` | **修改** | 从LevelData.MapData读取 |
| `Loader/CustomStructuresLoader.cs` | **修改** | 从LevelData.CustomStructures读取 |
| `Loader/BuildModeSaveLoader.cs` | **修改** | 从LevelData.BuildModeSave读取 |
| `ModCommand.cs` | **修改** | 多文件写入 |
| `Lang/*.cs` | **修改** | 新增错误键值 |

## 关键代码设计

### 1. Fungame.cs（元数据类）
```csharp
public class Fungame
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("author")] public List<string> Author { get; set; }
    [JsonProperty("description")] public string Description { get; set; }

    // Runtime-only
    [JsonIgnore] public string DirectoryPath { get; set; }
    [JsonIgnore] public List<LevelData> Levels { get; set; } = [];
    [JsonIgnore] public WorldSettingsData WorldSettings { get; set; } = new();
    [JsonIgnore] public CommandData CommandData { get; set; }
    [JsonIgnore] public XpData XpData { get; set; } = new();
    [JsonIgnore] public MineData MineData { get; set; }
    [JsonIgnore] public JumpPadData JumpPadData { get; set; }
    [JsonIgnore] public TurretData TurretData { get; set; }
    [JsonIgnore] public SoundCannonData SoundCannonData { get; set; }
    [JsonIgnore] public SpikeStabberData SpikeStabberData { get; set; }
    [JsonIgnore] public GeyserData GeyserData { get; set; }
    [JsonIgnore] public BearTrapData BearTrapData { get; set; }

    [JsonIgnore] public Vector2 MapPosition => Levels.FirstOrDefault() is { } l
        ? new Vector2(l.X, l.Y) : Vector2.zero;
    [JsonIgnore] public Vector2 SpawnPosition => Levels.FirstOrDefault() is { Spawn: { Length: >= 2 } s }
        ? new Vector2(s[0], s[1]) : Vector2.zero;
    [JsonIgnore] public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author) : "Unknown";

    // 兼容属性 - 消费者代码过渡使用
    [JsonIgnore] public LevelData CurrentLevel => Levels.FirstOrDefault();
    [JsonIgnore] public MapData MapData => CurrentLevel?.MapData;
    [JsonIgnore] public string CustomStructures => CurrentLevel?.CustomStructures;
    [JsonIgnore] public string BuildModeSave => CurrentLevel?.BuildModeSave;
}
```

### 2. LevelData.cs（新建）
```csharp
public class LevelData
{
    [JsonProperty("map_data")] public MapData MapData { get; set; }
    [JsonProperty("custom_structures")] public string CustomStructures { get; set; }
    [JsonProperty("build_mode_save")] public string BuildModeSave { get; set; }
    [JsonProperty("type")] public WorldGeneration.OverrideSceneType Type { get; set; }
    [JsonProperty("spawn")] public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("x")] public int X { get; set; }
    [JsonProperty("y")] public int Y { get; set; }
    [JsonProperty("waypoints")] public List<WaypointData> Waypoints { get; set; } = [];
    [JsonProperty("items")] public List<ItemData> Items { get; set; } = [];
}
```

### 3. WorldSettingsData.cs（新建）
```csharp
public class WorldSettingsData
{
    [JsonProperty("fullbright")] public bool Fullbright { get; set; } = true;
    [JsonProperty("forgiving_level")] public bool ForgivingLevel { get; set; }
    [JsonProperty("gravity")] public float Gravity { get; set; } = Physics2D.gravity.y;
    [JsonProperty("jump_limit")] public int JumpLimit { get; set; }
    [JsonProperty("climb_limit")] public int ClimbLimit { get; set; }
    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;
}
```

### 4. 文件拆分说明
原 `Fungame.cs` 包含所有 Data 类定义（MapData, Feature, CommandData, WaypointData, ItemData, XpData, MineData, JumpPadData, TurretData, SoundCannonData, SpikeStabberData, GeyserData, BearTrapData, ExplosionParamsData）。重构后：
- `Fungame.cs` → 仅元数据 + 运行时属性（留在根命名空间 `CustomFungamePack`）
- `Data/LevelData.cs` → 新增
- `Data/WorldSettingsData.cs` → 新增
- `Data/*.cs` → 从原 Fungame.cs 拆出的所有数据类（命名空间 `CustomFungamePack.Data`）

### 5. FungameDirectoryLoader.cs（新建）
负责读取整个目录结构：
```csharp
public static class FungameDirectoryLoader
{
    public static Fungame LoadFromDirectory(string directoryPath)
    {
        // 1. 加载 fungame.json（元数据）
        // 2. 扫描 level/*.json → 填充 Levels
        // 3. 加载 command.json → 填充 CommandData
        // 4. 加载 feature/world/settings.json → 填充 WorldSettings
        // 5. 加载 feature/world/mine.json → 填充 MineData
        // ... 依次加载其余 feature 文件
        // 6. 加载 feature/player/xp.json → 填充 XpData
    }
}
```

### 5. ModCommand.cs save as 改动
当前 `SaveAreaAsMapData` 直接修改 `fungame.MapData` 并序列化整个对象。
改为：
1. 创建/获取 `level/l1.json` 并写入 MapData
2. 创建 `fungame.json` 写入元数据
3. 创建 `feature/world/mine.json`（如果有 MineData）
4. 创建 `command.json`（如果有 CommandData）

## 执行顺序

1. **Phase 1 数据模型**：先创建新类和重构 Fungame.cs（编译能通过，但功能未接续）
2. **Phase 2 加载系统**：实现加载器，替换 FungameCheck 逻辑（加载可工作）
3. **Phase 3 消费者**：逐个更新消费者文件（功能恢复）
4. **Phase 4 保存**：更新 ModCommand save as
5. **Phase 5 收尾**：本地化、测试
