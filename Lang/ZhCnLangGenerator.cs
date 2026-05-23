using MossLib.Base;

namespace CustomFungamePack.Lang
{
    public class ZhCnLangGenerator : ModLangGenBase
    {
        protected override string LanguageCode => "zh-CN";

        protected override void BuildLocaleData()
        {
            // Feature
            Add("feature.fullbright", "全亮");
            Add("feature.forgiving_level", "仁慈关卡");
            Add("feature.gravity", "重力");
            Add("feature.skip_terrain", "跳过地形");
            Add("feature.skip_structures", "跳过结构");
            Add("feature.skip_background", "跳过背景");
            Add("feature.skip_background", "跳过背景");

            // Command - Fungame
            Add("command.fungame.description", "Fungame 的相关指令");
            Add("command.fungame.string", "选择功能");
            Add("command.fungame.parameter", "功能参数");
            Add("command.fungame.help", "可用子命令:\n  reload  - 重新加载当前地图\n  info    - 显示地图信息\n  spawn   - 传送回出生点\n  select  - 选择 Fungame\n  list    - 列出所有 Fungame\n  feature - 管理特性 (list/get/set)\n  waypoint- 管理路径点 (list/get)");
            Add("command.fungame.reload", "正在重新加载地图...");
            
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
            Add("command.fungame.waypoint.help", "路径点子命令:\n  list - 列出所有路径点\n  get <id或名称> - 传送到指定路径点");
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
            
            // Command - Fungame - Feature
            Add("command.fungame.feature.help", "特性子命令:\n  list - 列出所有特性\n  get <name> - 获取特性值\n  set <name> <value> - 设置特性值");
            Add("command.fungame.feature.unknown_subcommand", "未知的特性子命令：{0}");
            Add("command.fungame.feature.get_no_name", "请指定要获取的特性名称");
            Add("command.fungame.feature.set_missing_params", "请指定特性名称和值来设置");
            Add("command.fungame.feature.list_header", "当前特性设置：");
            Add("command.fungame.feature.item", "  {0}: {1}");
            Add("command.fungame.feature.get_success", "特性 '{0}' = {1}");
            Add("command.fungame.feature.set_success", "特性 '{0}' 已设置为 {1}");
            Add("command.fungame.feature.not_found", "未找到特性：{0}");
            Add("command.fungame.feature.invalid_value", "{0} 的值无效：{1}");
            Add("command.fungame.feature.invalid_bool", "{0} 的布尔值无效：{1}（使用 true/false）");

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
            Add("log.world_generation.no_commands", "未启用任何 {0}");
            Add("log.world_generation.executing_command", "执行 {0}: '{1}'");
            Add("log.world_generation.executing_loop_command", "执行循环 {0}: '{1}'");
            
            // Log - Validation
            Add("log.validation.map_invalid_type", "map 字段格式不正确");
            Add("log.validation.map_missing_field", "地图缺少必需字段: {0}");
            Add("log.validation.map_field_type_error", "地图 {0} 字段必须是{1}");
            Add("log.validation.map_map_empty", "地图 map 数组不能为空");
            Add("log.validation.map_row_not_string", "地图 map 第 {0} 行必须是字符串");
            Add("log.validation.map_item_row_not_array", "地图 items 第 {0} 行必须是数组");
            Add("log.validation.map_item_not_string", "地图 items[{0}][{1}] 必须是字符串");
            Add("log.validation.map_and_custom_structures_conflict", "不能同时使用 map_data 和 custom_structures");
            Add("log.validation.missing_map_or_custom_structures", "缺少 map_data 或 custom_structures");
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
            
            // Log - Error
            Add("log.error.no_fungame_file", "找不到 fungame.json 文件: {0}");
            Add("log.error.no_valid_directories", "没有有效的 Fungame 目录，请检查 Fungames 文件夹");
            Add("log.error.custom_structures_mod_not_loaded", "Fungame '{0}' 需要自定义结构模组，但该模组未加载");
            
            // Log - Common
            Add("log.common.map", "地图");
            Add("log.common.item", "物品");
            Add("log.common.block", "方块");
            Add("log.common.terrain", "地形");
            Add("log.common.structure", "结构");
            Add("log.common.background", "背景");
            Add("log.common.startup_command", "启动命令");
            Add("log.common.loop_command", "循环命令");
            Add("log.common.forgiving_level_mode", "仁慈关卡模式");
            
            // Log - Mod Command
            Add("log.mod_command.empty_type", "未知的指令类型");
            Add("log.mod_command.world_not_loaded", "未加载世界");
            Add("log.mod_command.no_waypoints", "当前 Fungame 未定义路径点");
            
            // Log - Custom Structures Loader
            Add("log.custom_structures_loader.loading", "正在加载自定义结构: {0}");
            Add("log.custom_structures_loader.failed", "加载自定义结构({0})失败: {1}");
        }
    }
}
