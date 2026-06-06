using MossLib.Base;

namespace CustomFungamePack.Lang
{
    public class ZhCnLangGenerator : ModLangGenBase
    {
        protected override string LanguageCode => "zh-CN";

        protected override void BuildLocaleData()
        {
            // Config
            Add("config.more_logs.name", "更多日志");
            Add("config.more_logs.description", "显示更多日日志");
            Add("config.start_game_use_fungame.name", "开始游戏使用 Fungame");
            Add("config.start_game_use_fungame.description", "开启新游戏时使用选中的 Fungame");
            Add("config.first_use_fungame.name", "首选 Fungame");
            Add("config.first_use_fungame.description", "开始新游戏时使用的 Fungame ID。需要启用“开始游戏使用 Fungame”");
            
            // Fungame Format
            Add("format.name_version", "{0} v{1}");
            Add("format.author", "作者: {0}");
            Add("format.features", "特性: {0}");
            
            // Feature
            Add("feature.full_bright", "全亮");
            Add("feature.forgiving_level", "仁慈关卡");
            Add("feature.gravity", "重力");
            Add("feature.jump_limit", "跳跃极限");
            Add("feature.climb_limit", "攀爬极限");
            Add("feature.mine_data", "地雷");
            Add("feature.jump_pad_data", "弹跳板");
            Add("feature.turret_data", "炮塔");
            Add("feature.sound_cannon_data", "音波炮");
            Add("feature.spike_stabber_data", "尖刺陷阱");
            Add("feature.geyser_data", "间歇泉");
            Add("feature.beartrap_data", "捕兽夹");

            // Feature - 子属性
            Add("feature.mine.undestroy", "不毁");
            Add("feature.mine.cooldown", "冷却");
            Add("feature.jump_pad.cooldown", "冷却");
            Add("feature.jump_pad.force", "力度");
            Add("feature.jump_pad.no_light", "无光");
            Add("feature.turret.cooldown", "冷却");
            Add("feature.turret.shot_power_multiplier", "射击力度");
            Add("feature.turret.undestroy", "不毁");
            Add("feature.turret.no_light", "无光");
            Add("feature.turret.range", "范围");
            Add("feature.sound_cannon.cooldown", "冷却");
            Add("feature.sound_cannon.max_distance", "范围");
            Add("feature.sound_cannon.charge_time", "充能时间");
            Add("feature.sound_cannon.undestroy", "不毁");
            Add("feature.spike_stabber.damage_mult", "伤害倍率");
            Add("feature.spike_stabber.undestroy", "不毁");
            Add("feature.spike_stabber.cooldown", "冷却");
            Add("feature.spike_stabber.no_light", "无光");
            Add("feature.spike_stabber.sound", "音效");
            Add("feature.geyser.cooldown", "冷却");
            Add("feature.geyser.activate_duration", "喷发时长");
            Add("feature.geyser.no_liquid", "无液体");
            Add("feature.geyser.rumble_time", "震动时长");
            Add("feature.geyser.range", "触发范围");
            Add("feature.beartrap.damage_mult", "伤害倍率");
            Add("feature.beartrap.undestroy", "不毁");
            Add("feature.beartrap.cooldown", "冷却");

            // Command - Fungame
            Add("command.fungame.description", "Fungame 的相关指令");
            Add("command.fungame.string", "选择功能");
            Add("command.fungame.parameter", "功能参数");
            Add("command.fungame.help", "可用子命令:\n  " +
                "reload  - 重新加载当前地图\n  " +
                "info    - 显示地图信息\n  " +
                "spawn   - 传送回出生点\n  " +
                "select  - 选择 Fungame\n  " +
                "list    - 列出所有 Fungame\n  " +
                "feature - 管理特性\n  " +
                "waypoint- 管理路径点 (list/get)\n  " +
                "save    - 保存当前Fungame到本地\n  " +
                "save as - 交互式选取区域并保存为地图数据");

            // Command - Fungame - Info
            Add("command.fungame.info.name", "名称: {0}");
            Add("command.fungame.info.id", "ID: {0}");
            Add("command.fungame.info.version", "版本: {0}");
            Add("command.fungame.info.authors", "作者: {0}");
            Add("command.fungame.info.description", "描述: {0}");
            Add("command.fungame.info.features", "特性: {0}");
            Add("command.fungame.info.spawn", "出生点: {0}");

            // Command - Fungame - Spawn
            Add("command.fungame.spawn", "传送回出生点{0}中...");

            // Command - Fungame - Waypoint
            Add("command.fungame.waypoint.help", "路径点子命令:\n  " +
                                                 "list - 列出所有路径点\n  " +
                                                 "get <id或名称> - 传送到指定路径点");
            Add("command.fungame.waypoint.list_header", "可用路径点 ({0}):");
            Add("command.fungame.waypoint.list_item", "  {0}. {1} - 位置: {2}");
            Add("command.fungame.waypoint.teleport", "正在传送到路径点'{0}'，位置 {1}...");
            Add("command.fungame.waypoint.not_found", "未找到路径点: {0}");
            Add("command.fungame.waypoint.invalid_index", "无效的索引 {0}，请输入 1 到 {1} 之间的数字");
            Add("command.fungame.waypoint.get_no_id", "请指定路径点 ID 或索引来传送");
            Add("command.fungame.waypoint.unknown_subcommand", "未知的路径点子命令：{0}");

            // Command - Fungame - List
            Add("command.fungame.list.header", "已加载 {0} 个 Fungame:");
            Add("command.fungame.list.item", "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})");
            Add("command.fungame.list.empty", "没有可用的 Fungame");

            // Command - Fungame - Select
            Add("command.fungame.select.no_key", "请提供要选择的 Fungame ID 或名称");
            Add("command.fungame.select.not_found", "未找到 Fungame: {0}");
            Add("command.fungame.select.success", "已选择 {0} (ID: {1})");
            Add("command.fungame.select.without_world", "已选择 {0}，但世界未加载。地图将在开始游戏时加载。");
            Add("command.fungame.select.invalid_index", "无效的索引 {0}，请输入 1 到 {1} 之间的数字");

            // Command - Fungame - Config
            Add("command.fungame.config.set_missing_params", "请指定配置名称和值来设置");
            Add("command.fungame.config.list_header", "当前配置设置：");
            Add("command.fungame.config.item", "    {0}({1}): {2}");
            Add("command.fungame.config.item_description", "        {0}");
            Add("command.fungame.config.set_success", "配置 '{0}' 已设置为 {1}");
            Add("command.fungame.config.set_failed", "设置配置 '{0}' 失败: {1}");
            Add("command.fungame.config.not_found", "未找到配置：{0}");
            Add("command.fungame.config.invalid_value", "{0} 的值无效：{1}");
            
            // Command - Fungame - Exit
            Add("command.fungame.exiting", "正在返回{0}...");
            Add("command.fungame.exit.invalid_target", "未知的退出目标: {0}，可用: none, tutorial");
            Add("command.fungame.exit.target.none", "原版游戏");
            Add("command.fungame.exit.target.tutorial", "教程");

            // Command - Fungame - Save
            Add("command.fungame.save.success", "已将 Fungame '{0}' 保存到: {1}");
            Add("command.fungame.save.failed", "保存 Fungame '{0}' 失败: {1}");
            Add("command.fungame.save.no_directory", "当前 Fungame 没有关联的目录路径，无法保存");
            Add("command.fungame.save.invalid_position", "无效的位置格式，请使用 Vector2 格式：\"x,y\" (例如 \"86,-11\")");
            Add("command.fungame.save.area_empty", "指定区域超出世界边界或为空");
            Add("command.fungame.save.area_success", "已保存区域物块地图: ({0},{1}) 到 ({2},{3}), 尺寸 {4}x{5}, {6} 种物块类型 → {7}");
            Add("command.fungame.save.target_not_found", "目标 Fungame 文件夹 '{0}' 未找到");
            Add("command.fungame.save.missing_end_position", "缺少结束位置，请提供两个位置参数 (例如: fg save 86,-11 2,45)");
            Add("command.fungame.save.as.default_description", "区域扫描保存");
            Add("command.fungame.save.as.start_position", "请鼠标左键点击选择起始位置...");
            Add("command.fungame.save.as.end_position", "请鼠标左键点击选择结束位置...");
            Add("command.fungame.save.as.confirm", "位置已记录，再次输入相同指令以确认保存。");

            // Command - Fungame - Feature
            Add("command.fungame.feature.unknown_subcommand", "未知的特性子命令：{0}");
            Add("command.fungame.feature.set_missing_params", "请指定特性名称和值来设置");
            Add("command.fungame.feature.list_header", "当前特性设置：");
            Add("command.fungame.feature.item", "    {0}({1}): {2}");
            Add("command.fungame.feature.parent_item", "    {0}({1}):");
            Add("command.fungame.feature.get_success", "特性 '{0}' = {1}");
            Add("command.fungame.feature.set_success", "特性 '{0}' 已设置为 {1}");
            Add("command.fungame.feature.not_found", "未找到特性：{0}");
            Add("command.fungame.feature.invalid_value", "{0} 的值无效：{1}");

            // Log - Fungame Check
            Add("log.fungame_check.id_format_warning", "ID格式不正确，将自动修正");
            Add("log.fungame_check.author_not_string", "作者元素 {0} 不是字符串，已移除");
            Add("log.fungame_check.author_empty", "作者数组为空，已设置默认值");
            Add("log.fungame_check.version_format_warning", "版本格式'{0}'不正确，将使用默认版本'1.0.0'");

            // Log - World Generation
            Add("log.world_generation.scene_type_set", "设置场景类型为: {0}");
            Add("log.world_generation.no_features_enabled", "未启用任何特性");
            Add("log.world_generation.feature_enabled", "已启用 {0}");
            Add("log.world_generation.feature_enabled_with_value", "已启用 {0}: {1}");
            Add("log.world_generation.unknown_feature", "未知的特性: {0}");
            Add("log.world_generation.skip_generation", "已跳过 {0} 生成");
            Add("log.world_generation.initializing_world", "初始化Fungame地图...");
            Add("log.world_generation.loading_fungame_map", "正在加载Fungame地图: {0}");
            Add("log.world_generation.no_map_data", "Fungame {0} 不包含地图数据");
            Add("log.world_generation.no_content_type", "Fungame '{0}' 未定义任何内容类型（MapData、CustomStructures 或 BuildModeSave）");
            Add("log.world_generation.no_commands", "未启用任何 {0}");
            Add("log.world_generation.exited_fungame", "已退出 Fungame");
            Add("log.world_generation.executing_command", "执行 {0}: '{1}'");
            Add("log.world_generation.executing_loop_command", "执行循环 {0}: '{1}'");
            Add("log.world_generation.start_game_fungame", "开始游戏时自动使用配置的 Fungame: {0} (ID: {1})");
            Add("log.world_generation.start_game_fungame_not_found", "未找到配置的 Fungame (ID: {0})，将使用默认");
            Add("log.world_generation.no_fungame_selected", "未选择 Fungame，生成原版世界");
            Add("log.world_generation.applying_settings_overrides", "正在应用 Settings 覆盖，共 {0} 项");
            Add("log.world_generation.settings_override_not_found", "未找到 Settings 覆盖项: {0}");
            Add("log.world_generation.settings_override_applied", "已应用 Settings 覆盖: {0} = {1}");
            Add("log.world_generation.settings_override_failed", "应用 Settings 覆盖失败: {0}");

            // Log - Validation
            Add("log.validation.map_invalid_type", "map 字段格式不正确");
            Add("log.validation.map_missing_field", "地图缺少必需字段: {0}");
            Add("log.validation.map_field_type_error", "地图 {0} 字段必须是{1}");
            Add("log.validation.map_map_empty", "地图 map 数组不能为空");
            Add("log.validation.map_row_not_string", "地图 map 第 {0} 行必须是字符串");
            Add("log.validation.map_item_row_not_array", "地图 items 第 {0} 行必须是数组");
            Add("log.validation.map_item_not_string", "地图 items[{0}][{1}] 必须是字符串");
            Add("log.validation.multiple_content_types", "不能同时使用多种内容类型（map_data、custom_structures、build_mode_save），只能选择一种");
            Add("log.validation.missing_content_type", "缺少内容类型（map_data、custom_structures 或 build_mode_save）");
            Add("log.validation.custom_structures_without_mod", "检测到 custom_structures 字段，但未安装自定义结构模组（Custom Structures），请先安装该模组");
            Add("log.validation.features_invalid_type", "features 字段必须是数组或对象");
            Add("log.validation.features_empty", "features 数组为空，将被忽略");
            Add("log.validation.features_element_invalid", "features 第 {0} 个元素格式不正确，已跳过");
            Add("log.validation.no_data", "{0} 中没有 {1} 数据");
            Add("log.validation.row_data_empty", "{0} 行数据为空");
            Add("log.validation.field_missing_default", "缺少必需字段: {0}，已使用默认值 \"{1}\"");
            Add("log.validation.field_null_default", "字段为空: {0}，已使用默认值 \"{1}\"");
            Add("log.validation.field_empty_string_default", "字段为空字符串: {0}，已使用默认值 \"{1}\"");
            Add("log.validation.field_must_be_array_default", "缺少必需字段: {0}，已使用默认值 [\"{1}\"]");
            Add("log.validation.field_null_array_default", "字段为空: {0}，已使用默认值 [\"{1}\"]");
            Add("log.validation.field_convert_to_array", "{0} 字段必须是数组，已转换为数组");
            Add("log.validation.array_empty_default", "{0} 数组为空，已设置默认值");
            Add("log.validation.array_empty_removed", "{0} 数组为空，已移除");

            // Log - Map Loader
            Add("log.map_loader.load_error", "Fungame 或地图数据为空");
            Add("log.map_loader.invalid_format", "无效的地图格式，缺少 map 字段");
            Add("log.map_loader.key_missing", "错误: 字符串地图格式缺少 'key' 定义");
            Add("log.map_loader.string_map_applied", "字符串地图应用完成，放置 {0} 个方块，{1} 个物品，失败 {2} 个");
            Add("log.map_loader.load_success", "成功加载地图: 起始坐标({0}, {1}), 尺寸({2}x{3})");
            Add("log.map_loader.load_failed", "加载地图失败: {0}");
            Add("log.map_loader.place_failed", "在 ({0}, {1}) 放置{2} {3} 失败: {4}");
            Add("log.map_loader.multiple_blocks_in_list", "在 ({0}, {1}) 检测到列表中有多个物块，只生成第一个");
            Add("log.map_loader.unsupported_value_type", "不支持的值类型: {0}，位置 ({1}, {2})");
            Add("log.map_loader.nested_structure_not_supported", "嵌套结构不被支持，位置 ({0}, {1})");
            Add("log.map_loader.unexpected_token_type", "意外的令牌类型: {0}，位置 ({1}, {2})");
            Add("log.map_loader.reload_success", "成功重新加载地图: {0}");
            Add("log.map_loader.reload_failed", "重新加载地图失败: {0}");
            Add("log.map_loader.restarting_scene", "正在重启场景...");
            Add("log.map_loader.scene_reloading", "正在重新加载场景: {0}");
            Add("log.map_loader.scene_reloaded", "场景已重新加载");
            Add("log.map_loader.scene_reload_failed", "重新加载场景失败: {0}");
            Add("log.map_loader.no_current_fungame", "当前没有加载的 Fungame 配置");
            Add("log.map_loader.custom_structures_not_supported", "自定义结构不支持用于地图加载: {0}");
            Add("log.map_loader.no_features_enabled", "未启用任何特性");
            Add("log.map_loader.feature_enabled", "已启用 {0}");
            Add("log.map_loader.feature_enabled_with_value", "已启用 {0}: {1}");
            Add("log.map_loader.skip_generation", "已跳过 {0} 生成");
            Add("log.map_loader.no_directory_path", "Fungame 目录路径为空");
            Add("log.map_loader.fungame_json_not_found", "在 {0} 中找不到 fungame.json");
            Add("log.map_loader.fungame_deserialize_failed", "反序列化 Fungame 失败");
            Add("log.map_loader.fungame_reloaded_from_disk", "已从磁盘重新加载 Fungame: {0}");
            Add("log.map_loader.fungame_reload_failed", "重新加载 Fungame 失败: {0}");
            Add("log.map_loader.validation.no_data", "{0} 中没有 {1} 数据");
            Add("log.map_loader.validation.row_data_empty", "{0} 行数据为空");

            // Log - Error
            Add("log.error.no_fungame_file", "找不到 fungame.json 文件: {0}");
            Add("log.error.no_valid_directories", "没有有效的 Fungame 目录，请检查 Fungames 文件夹");
            Add("log.error.custom_structures_mod_not_loaded", "Fungame '{0}' 需要自定义结构模组，但该模组未加载");
            Add("log.error.multiple_content_types", "Fungame '{0}' 同时定义了多种内容类型（MapData、CustomStructures、BuildModeSave），只允许定义一种");

            // Log - Fungame Load
            Add("log.fungame_load.empty_target_path", "目标路径不能为空");
            Add("log.fungame_load.unauthorized", "无权读取文件 '{0}': {1}");
            Add("log.fungame_load.io_error", "读取文件 '{0}' 失败: {1}");
            Add("log.fungame_load.file_empty", "文件 '{0}' 为空，将创建默认配置");
            Add("log.fungame_load.deserialize_null", "文件 '{0}' 反序列化失败（返回 null），将创建默认配置");
            Add("log.fungame_load.invalid_json", "文件 '{0}' JSON 格式无效: {1}");
            Add("log.fungame_load.no_folder_name", "无法从路径 '{0}' 解析有效的文件夹名称");

            // Log - Common
            Add("log.common.map", "地图");
            Add("log.common.item", "物品");
            Add("log.common.block", "方块");
            Add("log.common.terrain", "地形");
            Add("log.common.structure", "结构");
            Add("log.common.background", "背景");
            Add("log.common.startup_command", "启动命令");
            Add("log.common.loop_command", "循环命令");

            // Log - Mod Command
            Add("log.mod_command.empty_type", "未知的指令类型");
            Add("log.mod_command.world_not_loaded", "未加载世界");
            Add("log.mod_command.no_waypoints", "当前 Fungame 未定义路径点");
            Add("log.mod_command.exit_no_target", "请指定退出目标: none (原版) 或 tutorial (教程关)");
            Add("log.mod_command.register_failed", "注册自定义指令失败: {0}\n{1}");
            Add("log.mod_command.no_fungame", "当前没有可用的 Fungame");

            // Log - Custom Structures Loader
            Add("log.custom_structures_loader.loading", "正在加载自定义结构: {0}");
            Add("log.custom_structures_loader.failed", "加载自定义结构({0})失败: {1}");
            Add("log.custom_structures_loader.not_found", "未找到 {0}, 反射失败");
            Add("log.custom_structures_loader.not_found_custom_structures", "未找到自定义结构文件");
            Add("log.custom_structures_loader.suppress.structure_loader_not_found", "未找到 StructureLoader 类型");
            Add("log.custom_structures_loader.suppress.cleared_definitions", "已清除自定义结构注册表（StructureDefinitions），已抑制自动生成");
            Add("log.custom_structures_loader.suppress.cleared_field", "已清除 {0}，已抑制 Custom Structures 自动生成");
            Add("log.custom_structures_loader.suppress.no_registry", "找到 Custom Structures 模组但无可清除的结构注册表");
            Add("log.custom_structures_loader.suppress.failed", "抑制 Custom Structures 自动生成失败: {0}");

            // Log - Build Mode Save Loader
            Add("log.build_mode_save_loader.loading", "正在加载 Build Mode 存档: {0} (方块: {1}, 液体: {2}, 背景: {3})");
            Add("log.build_mode_save_loader.failed", "加载 Build Mode 存档({0})失败: {1}");
            Add("log.build_mode_save_loader.not_found_buildmode_save", "未找到 Build Mode 存档文件");
            Add("log.build_mode_save_loader.bg_sprite_missing", "未找到背景精灵: {0}");

            // Log - Map Loader (Build Mode)
            Add("log.map_loader.build_mode_save_applied", "Build Mode 存档应用完成: {0} 个方块, {1} 个液体, {2} 个背景, 失败 {3} 个");
            Add("log.map_loader.not_found_buildmode_save", "未找到 Build Mode 存档文件");

            // Log - Fungame Directory Loader
            Add("log.loader.directory_not_found", "找不到目录: {0}");
            Add("log.loader.fungame_json_not_found", "在 {0} 中找不到 fungame.json");
            Add("log.loader.fungame_json_failed", "反序列化 fungame.json 失败: {0}");
            Add("log.loader.success", "成功加载 Fungame: {0} (ID: {1}, 版本: {2})");
            Add("log.loader.failed", "从 {0} 加载 Fungame 失败: {1}");
            Add("log.loader.level_dir_not_found", "找不到关卡目录: {0}");
            Add("log.loader.no_level_files", "在 {0} 中未找到关卡文件");
            Add("log.loader.loaded_level", "已加载关卡: {0}");
            Add("log.loader.failed_to_load_level", "加载关卡文件 {0} 失败: {1}");
            Add("log.loader.no_world_settings", "未找到世界设置，使用默认值");
            Add("log.loader.missing_type", "{0} 缺少 'type' 属性，期望值 '{1}'");
            Add("log.loader.type_mismatch", "{0} 类型不匹配: 期望 '{1}'，实际 '{2}'");
            Add("log.loader.failed_to_load_file", "加载 {0} 失败: {1}");
        }
    }
}