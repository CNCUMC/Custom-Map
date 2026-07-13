using Bark.Base;

namespace CustomMap.Lang;

public class ZhCnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "zh-CN";

    protected override void BuildLocaleData()
    {
        // Config - 设置标签和描述
        Option("custommap.custommap.more_logs", "更多日志", "显示更多日志");
        Option("custommap.custommap.start_game_use_map", "开始游戏使用地图", "开启新游戏时使用选中的地图");
        Option("custommap.custommap.first_use_map", "首选地图", "开始新游戏时使用的地图ID。需要启用'开始游戏使用地图'");
        Option("custommap.custommap.progress_update_interval", "进度更新间隔", "放置方块时每 N 个方块更新一次进度显示。数值越小更新越频繁，但可能影响生成性能");

        // Map Format
        Other("format.author", "作者: {0}");
        Other("format.features", "特性: {0}");

        // Feature
        Feature("full_bright", "全亮");
        Feature("forgiving_level", "仁慈关卡");
        Feature("gravity", "重力");
        Feature("jump_limit", "跳跃极限");
        Feature("climb_limit", "攀爬极限");
        Feature("world_settings_data", "世界设置");
        Feature("skip_terrain", "跳过地形");
        Feature("skip_structures", "跳过结构");
        Feature("skip_background", "跳过背景");
        Feature("mine_data", "地雷");
        Feature("jump_pad_data", "弹跳板");
        Feature("turret_data", "炮塔");
        Feature("sound_cannon_data", "音波炮");
        Feature("spike_stabber_data", "尖刺陷阱");
        Feature("geyser_data", "间歇泉");
        Feature("beartrap_data", "捕兽夹");
        Feature("xp_data", "经验值");

        // Feature - 子属性
        Feature("mine.undestroy", "不毁");
        Feature("mine.cooldown", "冷却");
        Feature("jump_pad.cooldown", "冷却");
        Feature("jump_pad.force", "力度");
        Feature("jump_pad.no_light", "无光");
        Feature("turret.cooldown", "冷却");
        Feature("turret.shot_power_multiplier", "射击力度");
        Feature("turret.undestroy", "不毁");
        Feature("turret.no_light", "无光");
        Feature("turret.range", "范围");
        Feature("sound_cannon.cooldown", "冷却");
        Feature("sound_cannon.max_distance", "范围");
        Feature("sound_cannon.charge_time", "充能时间");
        Feature("sound_cannon.undestroy", "不毁");
        Feature("spike_stabber.damage_mult", "伤害倍率");
        Feature("spike_stabber.undestroy", "不毁");
        Feature("spike_stabber.cooldown", "冷却");
        Feature("spike_stabber.no_light", "无光");
        Feature("spike_stabber.sound", "音效");
        Feature("geyser.cooldown", "冷却");
        Feature("geyser.activate_duration", "喷发时长");
        Feature("geyser.no_liquid", "无液体");
        Feature("geyser.rumble_time", "震动时长");
        Feature("geyser.range", "触发范围");
        Feature("beartrap.damage_mult", "伤害倍率");
        Feature("beartrap.undestroy", "不毁");
        Feature("beartrap.cooldown", "冷却");

        // XP 子属性
        Feature("xp.str_xp", "力量等级");
        Feature("xp.res_xp", "韧性等级");
        Feature("xp.int_xp", "智力等级");
        Feature("xp.exp_str", "力量经验");
        Feature("xp.exp_res", "韧性经验");
        Feature("xp.exp_int", "智力经验");
        Feature("xp.min_str", "最小力量经验");
        Feature("xp.max_str", "最大力量经验");
        Feature("xp.min_res", "最小韧性经验");
        Feature("xp.max_res", "最大韧性经验");
        Feature("xp.min_int", "最小智力经验");
        Feature("xp.max_int", "最大智力经验");

        // Command - Map
        Command("custommap.description", "地图的相关指令");
        Command("custommap.string", "选择功能");
        Command("custommap.parameter", "功能参数");
        Command("custommap.help.header", "可用子命令:");
        Command("custommap.help.help", "显示此帮助信息");
        Command("custommap.help.reload", "重新加载当前地图");
        Command("custommap.help.load", "重新加载 Maps 文件夹");
        Command("custommap.load.success", "地图重新加载成功");
        Command("custommap.help.info", "显示地图信息");
        Command("custommap.help.spawn", "传送回出生点");
        Command("custommap.help.select", "选择地图");
        Command("custommap.help.list", "列出所有地图");
        Command("custommap.help.feature", "管理特性");
        Command("custommap.help.waypoint", "管理路径点 (list/get)");
        Command("custommap.help.save", "保存当前地图");
        Command("custommap.help.save_as", "交互式选取区域并保存为地图数据");
        Command("custommap.help.level", "切换地图层级");
        Command("custommap.help.exit", "退出地图");

        // Command - Map - Info
        Command("custommap.info.name", "名称: {0}");
        Command("custommap.info.id", "ID: {0}");
        Command("custommap.info.version", "版本: {0}");
        Command("custommap.info.authors", "作者: {0}");
        Command("custommap.info.description", "描述: {0}");
        Command("custommap.info.features", "特性: {0}");
        Command("custommap.info.spawn", "出生点: {0}");

        // Command - Map - Spawn
        Command("custommap.spawn", "传送回出生点{0}中...");

        // Command - Map - Waypoint
        Command("custommap.waypoint.help", "路径点子命令:\n  " +
                                           "list - 列出所有路径点\n  " +
                                           "get <id或名称> - 传送到指定路径点");
        Command("custommap.waypoint.list_header", "可用路径点 ({0}):");
        Command("custommap.waypoint.list_item", "  {0}. {1} - 位置: {2}");
        Command("custommap.waypoint.teleport", "正在传送到路径点'{0}'，位置 {1}...");
        Command("custommap.waypoint.not_found", "未找到路径点: {0}");
        Command("custommap.waypoint.invalid_index", "无效的索引 {0}，请输入 1 到 {1} 之间的数字");
        Command("custommap.waypoint.get_no_id", "请指定路径点 ID 或索引来传送");
        Command("custommap.waypoint.unknown_subcommand", "未知的路径点子命令：{0}");

        // Command - Map - List
        Command("custommap.list.header", "已加载 {0} 个地图:");
        Command("custommap.list.item", "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})");
        Command("custommap.list.empty", "没有可用的地图");

        // Command - Map - Select
        Command("custommap.select.no_key", "请提供要选择的地图ID 或名称");
        Command("custommap.select.not_found", "未找到地图: {0}");
        Command("custommap.select.success", "已选择 {0} (ID: {1})");
        Command("custommap.select.without_world", "已选择 {0}，但世界未加载。地图将在开始游戏时加载。");
        Command("custommap.select.invalid_index", "无效的索引 {0}，请输入 1 到 {1} 之间的数字");

        // Command - Map - Layer
        Command("custommap.layer.current", "当前层级: {0}/{1}");
        Command("custommap.layer.switched", "已切换到层级 {0}");
        Command("custommap.layer.already", "已在层级 {0}");
        Command("custommap.layer.invalid", "无效的层级，请输入 1 到 {0} 之间的数字");
        Command("custommap.layer.no_layers", "此地图没有可用的层级");

        // Command - Map - Config
        Command("custommap.config.set_missing_params", "请指定配置名称和值来设置");
        Command("custommap.config.list_header", "当前配置设置：");
        Command("custommap.config.set_success", "配置 '{0}' 已设置为 {1}");
        Command("custommap.config.set_failed", "设置配置 '{0}' 失败: {1}");
        Command("custommap.config.not_found", "未找到配置：{0}");
        Command("custommap.config.invalid_value", "{0} 的值无效：{1}");

        // Command - Map - Exit
        Command("custommap.exiting", "正在返回{0}...");
        Command("custommap.exit.invalid_target", "未知的退出目标: {0}，可用: none, tutorial");
        Command("custommap.exit.target.none", "原版游戏");
        Command("custommap.exit.target.tutorial", "教程");

        // Command - Map - Save
        Command("custommap.save.success", "已将地图 '{0}' 保存到: {1}");
        Command("custommap.save.failed", "保存地图 '{0}' 失败: {1}");
        Command("custommap.save.no_directory", "当前地图没有关联的目录路径，无法保存");
        Command("custommap.save.invalid_position", "无效的位置格式，请使用 Vector2 格式：'x,y' (例如 '86,-11')");
        Command("custommap.save.area_empty", "指定区域超出世界边界或为空");
        Command("custommap.save.area_success", "已保存区域物块地图: ({0},{1}) 到 ({2},{3}), 尺寸 {4}x{5}, {6} 种物块类型 → {7}");
        Command("custommap.save.target_not_found", "目标地图文件夹 '{0}' 未找到");
        Command("custommap.save.missing_end_position", "缺少结束位置，请提供两个位置参数 (例如: fg save 86,-11 2,45)");
        Command("custommap.save.as.default_description", "区域扫描保存");
        Command("custommap.save.as.start_position", "请鼠标左键点击选择起始位置...");
        Command("custommap.save.as.end_position", "请鼠标左键点击选择结束位置...");
        Command("custommap.save.as.confirm", "位置已记录，再次输入相同指令以确认保存。");

        // Command - Map - Feature
        Command("custommap.feature.unknown_subcommand", "未知的特性子命令：{0}");
        Command("custommap.feature.set_missing_params", "请指定特性名称和值来设置");
        Command("custommap.feature.list_header", "当前特性设置：");
        Command("custommap.feature.get_success", "特性 '{0}' = {1}");
        Command("custommap.feature.set_success", "特性 '{0}' 已设置为 {1}");
        Command("custommap.feature.not_found", "未找到特性：{0}");
        Command("custommap.feature.invalid_value", "{0} 的值无效：{1}");

        // Log - Map Check
        Log("map_check.id_format_warning", "ID格式不正确，将自动修正");
        Log("map_check.author_not_string", "作者元素 {0} 不是字符串，已移除");
        Log("map_check.author_empty", "作者数组为空，已设置默认值");
        Log("map_check.version_format_warning", "版本格式'{0}'不正确，将使用默认版本'1.0.0'");
        Log("map_check.missing_build_mode_mod", "地图 '{0}' 需要 Build Mode 模组，但未安装");
        Log("map_check.requires_mod", "[需要 {0}]");

        // Log - World Generation
        Log("world_generation.scene_type_set", "设置场景类型为: {0}");
        Log("world_generation.no_features_enabled", "未启用任何特性");
        Log("world_generation.feature_enabled", "已启用 {0}");
        Log("world_generation.feature_enabled_with_value", "已启用 {0}: {1}");
        Log("world_generation.unknown_feature", "未知的特性: {0}");
        Log("world_generation.skip_generation", "已跳过 {0} 生成");
        Log("world_generation.phase.preparing", "正在准备地图: {0}...");
        Log("world_generation.phase.generating", "{0} - 世界生成中...");
        Log("world_generation.phase.skipping", "{0} - 已跳过{1}");
        Log("world_generation.phase.placing_blocks", "{0} - 放置方块: 成功{1} 失败{2} / 总计{3} ({4}%)");
        Log("world_generation.phase.spawning_map", "{0} - 正在生成地图...");
        Log("world_generation.phase.spawning_custom_structures", "{0} - 正在生成自定义结构...");
        Log("world_generation.phase.spawning_build_mode_save", "{0} - 正在生成建筑模式存档...");
        Log("world_generation.phase.applying_settings", "{0} - 正在应用设置...");
        Log("world_generation.loading_start", "开始加载地图: {0}");
        Log("world_generation.no_map_data", "地图 {0} 不包含地图数据");
        Log("world_generation.no_content_type",
            "地图 '{0}' 未定义任何内容类型（MapData、Structures 或 BuildModeSave）");
        Log("world_generation.no_commands", "未启用任何 {0}");
        Log("world_generation.exited_map", "已退出地图");
        Log("world_generation.executing_command", "执行 {0}: '{1}'");
        Log("world_generation.executing_loop_command", "执行循环 {0}: '{1}'");
        Log("world_generation.start_game_map", "开始游戏时自动使用配置的地图: {0} (ID: {1})");
        Log("world_generation.start_game_map_not_found", "未找到配置的地图 (ID: {0})，将使用默认");
        Log("world_generation.scan_maps_failed", "扫描地图目录 '{0}' 失败: {1}");
        Log("world_generation.no_map_selected", "未选择地图，生成原版世界");
        Log("world_generation.no_valid_directories", "没有有效的地图目录，请检查 Maps 文件夹");
        Log("world_generation.applying_settings_overrides", "正在应用 Settings 覆盖，共 {0} 项");
        Log("world_generation.settings_override_not_found", "未找到 Settings 覆盖项: {0}");
        Log("world_generation.settings_override_applied", "已应用 Settings 覆盖: {0} = {1}");
        Log("world_generation.settings_override_failed", "应用 Settings 覆盖失败: {0}");

        // Log - Validation
        Log("validation.map_invalid_type", "map 字段格式不正确");
        Log("validation.map_missing_field", "地图缺少必需字段: {0}");
        Log("validation.map_field_type_error", "地图 {0} 字段必须是{1}");
        Log("validation.map_map_empty", "地图 map 数组不能为空");
        Log("validation.map_row_not_string", "地图 map 第 {0} 行必须是字符串");
        Log("validation.map_item_row_not_array", "地图 items 第 {0} 行必须是数组");
        Log("validation.map_item_not_string", "地图 items[{0}][{1}] 必须是字符串");
        Log("validation.multiple_content_types", "不能同时使用多种内容类型（map_data、custom_structures、build_mode_save），只能选择一种");
        Log("validation.missing_content_type", "缺少内容类型（map_data、custom_structures 或 build_mode_save）");
        Log("validation.custom_structures_without_mod",
            "检测到 custom_structures 字段，但未安装自定义结构模组（Custom Structures），请先安装该模组");
        Log("validation.features_invalid_type", "features 字段必须是数组或对象");
        Log("validation.features_empty", "features 数组为空，将被忽略");
        Log("validation.features_element_invalid", "features 第 {0} 个元素格式不正确，已跳过");
        Log("validation.no_data", "{0} 中没有 {1} 数据");
        Log("validation.row_data_empty", "{0} 行数据为空");
        Log("validation.field_missing_default", "缺少必需字段: {0}，已使用默认值 '{1}'");
        Log("validation.field_null_default", "字段为空: {0}，已使用默认值 '{1}'");
        Log("validation.field_empty_string_default", "字段为空字符串: {0}，已使用默认值 '{1}'");
        Log("validation.field_must_be_array_default", "缺少必需字段: {0}，已使用默认值 ['{1}']");
        Log("validation.field_null_array_default", "字段为空: {0}，已使用默认值 ['{1}']");
        Log("validation.field_convert_to_array", "{0} 字段必须是数组，已转换为数组");
        Log("validation.array_empty_default", "{0} 数组为空，已设置默认值");
        Log("validation.array_empty_removed", "{0} 数组为空，已移除");

        // Log - Map Loader
        Log("map_loader.load_error", "地图或地图数据为空");
        Log("map_loader.invalid_format", "无效的地图格式，缺少 map 字段");
        Log("map_loader.key_missing", "错误: 字符串地图格式缺少 'key' 定义");
        Log("map_loader.string_map_applied", "字符串地图应用完成，成功 {0} 个，失败 {1} 个");
        Log("map_loader.load_success", "成功加载地图: 起始坐标({0}, {1}), 尺寸({2}x{3})");
        Log("map_loader.load_failed", "加载地图失败: {0}");
        Log("map_loader.place_failed", "在 ({0}, {1}) 放置{2} {3} 失败: {4}");
        Log("map_loader.multiple_blocks_in_list", "在 ({0}, {1}) 检测到列表中有多个物块，只生成第一个");
        Log("map_loader.unsupported_value_type", "不支持的值类型: {0}，位置 ({1}, {2})");
        Log("map_loader.nested_structure_not_supported", "嵌套结构不被支持，位置 ({0}, {1})");
        Log("map_loader.unexpected_token_type", "意外的令牌类型: {0}，位置 ({1}, {2})");
        Log("map_loader.reload_success", "成功重新加载地图: {0}");
        Log("map_loader.reload_failed", "重新加载地图失败: {0}");
        Log("map_loader.restarting_scene", "正在重启场景...");
        Log("map_loader.scene_reloading", "正在重新加载场景: {0}");
        Log("map_loader.scene_reloaded", "场景已重新加载");
        Log("map_loader.scene_reload_failed", "重新加载场景失败: {0}");
        Log("map_loader.no_current_map", "当前没有加载的地图 配置");
        Log("map_loader.custom_structures_not_supported", "自定义结构不支持用于地图加载: {0}");
        Log("map_loader.no_features_enabled", "未启用任何特性");
        Log("map_loader.feature_enabled", "已启用 {0}");
        Log("map_loader.feature_enabled_with_value", "已启用 {0}: {1}");
        Log("map_loader.skip_generation", "已跳过 {0} 生成");
        Log("map_loader.no_directory_path", "地图目录路径为空");
        Log("map_loader.map_json_not_found", "在 {0} 中找不到 map.json");
        Log("map_loader.map_deserialize_failed", "反序列化地图 失败");
        Log("map_loader.map_reloaded_from_disk", "已从磁盘重新加载地图: {0}");
        Log("map_loader.map_reload_failed", "重新加载地图 失败: {0}");
        Log("map_loader.validation.no_data", "{0} 中没有 {1} 数据");
        Log("map_loader.validation.row_data_empty", "{0} 行数据为空");
        Log("map_load.empty_target_path", "目标路径不能为空");
        Log("map_load.unauthorized", "无权读取文件 '{0}': {1}");
        Log("map_load.io_error", "读取文件 '{0}' 失败: {1}");
        Log("map_load.file_empty", "文件 '{0}' 为空，将创建默认配置");
        Log("map_load.deserialize_null", "文件 '{0}' 反序列化失败（返回 null），将创建默认配置");
        Log("map_load.invalid_json", "文件 '{0}' JSON 格式无效: {1}");
        Log("map_load.no_folder_name", "无法从路径 '{0}' 解析有效的文件夹名称");
        
        // Log - Error
        Log("error.no_map_file", "找不到 map.json 文件: {0}");
        Log("error.no_valid_directories", "没有有效的地图目录，请检查 Maps 文件夹");
        Log("error.custom_structures_mod_not_loaded", "地图 '{0}' 需要自定义结构模组，但该模组未加载");
        Log("error.multiple_content_types",
            "地图 '{0}' 同时定义了多种内容类型（MapData、Structures、BuildModeSave），只允许定义一种");

        // Log - Common
        Log("common.map", "地图");
        Log("common.item", "物品");
        Log("common.block", "方块");
        Log("common.terrain", "地形");
        Log("common.structure", "结构");
        Log("common.background", "背景");
        Log("common.startup_command", "启动命令");
        Log("common.loop_command", "循环命令");

        // Log - Mod Command
        Log("mod_command.empty_type", "未知的指令类型");
        Log("mod_command.world_not_loaded", "未加载世界");
        Log("mod_command.no_waypoints", "当前地图未定义路径点");
        Log("mod_command.exit_no_target", "请指定退出目标: none (原版) 或 tutorial (教程关)");
        Log("mod_command.register_failed", "注册自定义指令失败: {0}\n{1}");
        Log("mod_command.no_map", "当前没有可用的地图");

        // Log - Custom Structures Loader
        Log("custom_structures_loader.loading", "正在加载自定义结构: {0}");
        Log("custom_structures_loader.failed", "加载自定义结构({0})失败: {1}");
        Log("custom_structures_loader.not_found_custom_structures", "未找到自定义结构文件");
        Log("custom_structures_loader.suppress.cleared_definitions", "已清除自定义结构注册表（StructureDefinitions），已抑制自动生成");
        Log("custom_structures_loader.structure_registry.registered", "结构 '{0}' 已通过 StructureRegistry 注册");

        // Log - Build Mode Save Loader
        Log("build_mode_save_loader.loading", "正在加载 Build Mode 存档: {0} (方块: {1}, 液体: {2}, 背景: {3})");
        Log("build_mode_save_loader.failed", "加载 Build Mode 存档({0})失败: {1}");
        Log("build_mode_save_loader.not_found_buildmode_save", "未找到 Build Mode 存档文件");
        Log("build_mode_save_loader.bg_sprite_missing", "未找到背景精灵: {0}");

        // Log - Map Loader (Build Mode)
        Log("map_loader.build_mode_save_applied", "Build Mode 存档应用完成: {0} 个方块, {1} 个液体, {2} 个背景, 失败 {3} 个");
        Log("map_loader.not_found_buildmode_save", "未找到 Build Mode 存档文件");
        Log("map_loader.build_mode_save_invalid_size", "Build Mode 存档尺寸无效 ({0}x{1})，文件可能已损坏");

        // Log - Map Directory Loader
        Log("loader.directory_not_found", "找不到目录: {0}");
        Log("loader.map_json_not_found", "在 {0} 中找不到 map.json");
        Log("loader.map_json_failed", "反序列化 map.json 失败: {0}");
        Log("loader.success", "成功加载地图: {0} (ID: {1}, 版本: {2})");
        Log("loader.failed", "从 {0} 加载地图失败: {1}");
        Log("loader.level_dir_not_found", "找不到关卡目录: {0}");
        Log("loader.no_level_files", "在 {0} 中未找到关卡文件");
        Log("loader.loaded_level", "已加载关卡: {0}");
        Log("loader.failed_to_load_level", "加载关卡文件 {0} 失败: {1}");
        Log("loader.no_world_settings", "未找到世界设置，使用默认值");
        Log("loader.missing_type", "{0} 缺少 'type' 属性，期望值 '{1}'");
        Log("loader.type_mismatch", "{0} 类型不匹配: 期望 '{1}'，实际 '{2}'");
        Log("loader.failed_to_load_file", "加载 {0} 失败: {1}");
    }

    private void Feature(string key, string value)
    {
        Other($"feature.{key}", value);
    }
}