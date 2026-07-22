# 更新日志

## v1.0.0

### 新功能

- **层级地图结构** — 地图现使用文件夹层级系统（`layers/layer1/`），替代旧的扁平 JSON 文件
- **层级级指令** — 每个层级可拥有独立的 `command.json`，包含启动命令和循环命令
- **全局指令回退** — 地图根目录的 `command.json` 作为所有层级的回退指令
- **结构文件** — 每个层级目录中的独立 `.m2.json` 文件用于结构放置
- **多语言 UI** — 完整的本地化支持：English、简体中文、繁體中文、Русский
- **自定义加载界面** — 地图生成时实时显示进度
- **坐标点系统** — 在层级内定义和传送到坐标点
- **层级切换** — 使用 `cm layer` 在游戏中切换层级
- **特性系统** — 按地图配置地雷、炮塔、弹跳板、尖刺陷阱、声波炮、间歇泉、捕兽夹等
- **地图保存/导出** — 将当前地图状态保存到磁盘

### 指令

- `cm help` — 显示帮助
- `cm reload` — 从磁盘重新加载当前地图
- `cm load` — 重新加载 Maps 文件夹中的所有地图
- `cm savereload` — 保存并重新加载当前地图
- `cm info` — 显示当前地图详情
- `cm spawn` — 传送到出生点
- `cm select <id>` — 按 ID、名称或序号选择地图
- `cm list` — 列出所有可用地图
- `cm feature [名称] [值]` — 列出或设置地图特性
- `cm waypoint [list|get]` — 管理坐标点
- `cm save` — 保存当前地图到磁盘
- `cm layer [n]` — 查看或切换层级
- `cm exit <目标>` — 退出到原版（`none`）或教程（`tutorial`）

### 地图结构

```
Maps/你的地图名/
├── map.json              # 地图元数据（id、version、author）
├── layers/
│   ├── layer1/
│   │   ├── layer.json    # 层级属性（type、spawn、skip 标志）
│   │   ├── command.json  # 层级指令（可选）
│   │   └── *.m2.json     # 结构文件（可选）
│   └── layer2/
│       └── ...
├── command.json          # 全局指令 — 应用于所有层级（可选）
├── feature/
│   ├── world/            # 世界特性配置
│   └── player/           # 玩家特性配置
└── lang/                 # 本地化文件
```
