using Bark.Base;

namespace CustomMap.Lang;

public class ZhTwLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "zh-TW";

    protected override void BuildLocaleData()
    {
        // Config - 設置標籤和描述
        Option("custommap.custommap.more_logs", "更多日誌", "顯示更多日誌");
        Option("custommap.custommap.start_game_use_map", "開始遊戲使用地圖", "開啟新遊戲時使用選中的地圖");
        Option("custommap.custommap.first_use_map", "首選地圖", "開始新遊戲時使用的地圖ID。需要啟用\"開始遊戲使用地圖\"");
        Option("custommap.custommap.progress_update_interval", "進度更新間隔", "放置方塊時每 N 個方塊更新一次進度顯示。數值越小更新越頻繁，但可能影響生成效能");

        // Map Format
        Other("format.author", "作者: {0}");
        Other("format.features", "特性: {0}");

        // Feature
        Feature("full_bright", "全亮");
        Feature("forgiving_level", "仁慈關卡");
        Feature("gravity", "重力");
        Feature("jump_limit", "跳躍極限");
        Feature("climb_limit", "攀爬極限");
        Feature("world_settings_data", "世界設置");
        Feature("skip_terrain", "跳過地形");
        Feature("skip_structures", "跳過結構");
        Feature("skip_background", "跳過背景");
        Feature("mine_data", "地雷");
        Feature("jump_pad_data", "彈跳板");
        Feature("turret_data", "炮塔");
        Feature("sound_cannon_data", "音波炮");
        Feature("spike_stabber_data", "尖刺陷阱");
        Feature("geyser_data", "間歇泉");
        Feature("beartrap_data", "捕獸夾");
        Feature("xp_data", "經驗值");

        // Feature - 子屬性
        Feature("mine.undestroy", "不毀");
        Feature("mine.cooldown", "冷卻");
        Feature("jump_pad.cooldown", "冷卻");
        Feature("jump_pad.force", "力度");
        Feature("jump_pad.no_light", "無光");
        Feature("turret.cooldown", "冷卻");
        Feature("turret.shot_power_multiplier", "射擊力度");
        Feature("turret.undestroy", "不毀");
        Feature("turret.no_light", "無光");
        Feature("turret.range", "範圍");
        Feature("sound_cannon.cooldown", "冷卻");
        Feature("sound_cannon.max_distance", "範圍");
        Feature("sound_cannon.charge_time", "充能時間");
        Feature("sound_cannon.undestroy", "不毀");
        Feature("spike_stabber.damage_mult", "傷害倍率");
        Feature("spike_stabber.undestroy", "不毀");
        Feature("spike_stabber.cooldown", "冷卻");
        Feature("spike_stabber.no_light", "無光");
        Feature("spike_stabber.sound", "音效");
        Feature("geyser.cooldown", "冷卻");
        Feature("geyser.activate_duration", "噴發時長");
        Feature("geyser.no_liquid", "無液體");
        Feature("geyser.rumble_time", "震動時長");
        Feature("geyser.range", "觸發範圍");
        Feature("beartrap.damage_mult", "傷害倍率");
        Feature("beartrap.undestroy", "不毀");
        Feature("beartrap.cooldown", "冷卻");

        // XP 子屬性
        Feature("xp.str_xp", "力量等級");
        Feature("xp.res_xp", "韌性等級");
        Feature("xp.int_xp", "智力等級");
        Feature("xp.exp_str", "力量經驗");
        Feature("xp.exp_res", "韌性經驗");
        Feature("xp.exp_int", "智力經驗");
        Feature("xp.min_str", "最小力量經驗");
        Feature("xp.max_str", "最大力量經驗");
        Feature("xp.min_res", "最小韌性經驗");
        Feature("xp.max_res", "最大韌性經驗");
        Feature("xp.min_int", "最小智力經驗");
        Feature("xp.max_int", "最大智力經驗");

        // Command - Map
        Command("custommap.description", "地圖的相關指令");
        Command("custommap.string", "選擇功能");
        Command("custommap.parameter", "功能參數");
        Command("custommap.help.header", "可用子命令:");
        Command("custommap.help.help", "顯示此幫助信息");
        Command("custommap.help.reload", "重新加載當前地圖");
        Command("custommap.help.info", "顯示地圖信息");
        Command("custommap.help.spawn", "傳送回出生點");
        Command("custommap.help.select", "選擇地圖");
        Command("custommap.help.list", "列出所有地圖");
        Command("custommap.help.feature", "管理特性");
        Command("custommap.help.waypoint", "管理路徑點 (list/get)");
        Command("custommap.help.save", "保存當前地圖");
        Command("custommap.help.save_as", "交互式選取區域並保存為地圖數據");
        Command("custommap.help.exit", "退出地圖");

        // Command - Map - Info
        Command("custommap.info.name", "名稱: {0}");
        Command("custommap.info.id", "ID: {0}");
        Command("custommap.info.version", "版本: {0}");
        Command("custommap.info.authors", "作者: {0}");
        Command("custommap.info.description", "描述: {0}");
        Command("custommap.info.features", "特性: {0}");
        Command("custommap.info.spawn", "出生點: {0}");

        // Command - Map - Spawn
        Command("custommap.spawn", "傳送回出生點{0}中...");

        // Command - Map - Waypoint
        Command("custommap.waypoint.help", "路徑點子命令:\n  " +
                                             "list - 列出所有路徑點\n  " +
                                             "get <id或名稱> - 傳送到指定路徑點");
        Command("custommap.waypoint.list_header", "可用路徑點 ({0}):");
        Command("custommap.waypoint.list_item", "  {0}. {1} - 位置: {2}");
        Command("custommap.waypoint.teleport", "正在傳送到路徑點'{0}'，位置 {1}...");
        Command("custommap.waypoint.not_found", "未找到路徑點: {0}");
        Command("custommap.waypoint.invalid_index", "無效的索引 {0}，請輸入 1 到 {1} 之間的數字");
        Command("custommap.waypoint.get_no_id", "請指定路徑點 ID 或索引來傳送");
        Command("custommap.waypoint.unknown_subcommand", "未知的路徑點子命令：{0}");

        // Command - Map - List
        Command("custommap.list.header", "已加載 {0} 個地圖:");
        Command("custommap.list.item", "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})");
        Command("custommap.list.empty", "沒有可用的地圖");

        // Command - Map - Select
        Command("custommap.select.no_key", "請提供要選擇的地圖ID 或名稱");
        Command("custommap.select.not_found", "未找到地圖: {0}");
        Command("custommap.select.success", "已選擇 {0} (ID: {1})");
        Command("custommap.select.without_world", "已選擇 {0}，但世界未加載。地圖將在開始遊戲時加載。");
        Command("custommap.select.invalid_index", "無效的索引 {0}，請輸入 1 到 {1} 之間的數字");

        // Command - Map - Config
        Command("custommap.config.set_missing_params", "請指定配置名稱和值來設置");
        Command("custommap.config.list_header", "當前配置設置：");
        Command("custommap.config.set_success", "配置 '{0}' 已設置為 {1}");
        Command("custommap.config.set_failed", "設置配置 '{0}' 失敗: {1}");
        Command("custommap.config.not_found", "未找到配置：{0}");
        Command("custommap.config.invalid_value", "{0} 的值無效：{1}");

        // Command - Map - Exit
        Command("custommap.exiting", "正在返回{0}...");
        Command("custommap.exit.invalid_target", "未知的退出目標: {0}，可用: none, tutorial");
        Command("custommap.exit.target.none", "原版遊戲");
        Command("custommap.exit.target.tutorial", "教程");

        // Command - Map - Save
        Command("custommap.save.success", "已將地圖 '{0}' 保存到: {1}");
        Command("custommap.save.failed", "保存地圖 '{0}' 失敗: {1}");
        Command("custommap.save.no_directory", "當前地圖沒有關聯的目錄路徑，無法保存");
        Command("custommap.save.invalid_position", "無效的位置格式，請使用 Vector2 格式：\"x,y\" (例如 \"86,-11\")");
        Command("custommap.save.area_empty", "指定區域超出世界邊界或為空");
        Command("custommap.save.area_success", "已保存區域物塊地圖: ({0},{1}) 到 ({2},{3}), 尺寸 {4}x{5}, {6} 種物塊類型 → {7}");
        Command("custommap.save.target_not_found", "目標地圖文件夾 '{0}' 未找到");
        Command("custommap.save.missing_end_position", "缺少結束位置，請提供兩個位置參數 (例如: fg save 86,-11 2,45)");
        Command("custommap.save.as.default_description", "區域掃描保存");
        Command("custommap.save.as.start_position", "請鼠標左鍵點擊選擇起始位置...");
        Command("custommap.save.as.end_position", "請鼠標左鍵點擊選擇結束位置...");
        Command("custommap.save.as.confirm", "位置已記錄，再次輸入相同指令以確認保存。");

        // Command - Map - Feature
        Command("custommap.feature.unknown_subcommand", "未知的特性子命令：{0}");
        Command("custommap.feature.set_missing_params", "請指定特性名稱和值來設置");
        Command("custommap.feature.list_header", "當前特性設置：");
        Command("custommap.feature.get_success", "特性 '{0}' = {1}");
        Command("custommap.feature.set_success", "特性 '{0}' 已設置為 {1}");
        Command("custommap.feature.not_found", "未找到特性：{0}");
        Command("custommap.feature.invalid_value", "{0} 的值無效：{1}");

        // Log - Map Check
        Log("map_check.id_format_warning", "ID格式不正確，將自動修正");
        Log("map_check.author_not_string", "作者元素 {0} 不是字符串，已移除");
        Log("map_check.author_empty", "作者數組為空，已設置默認值");
        Log("map_check.version_format_warning", "版本格式'{0}'不正確，將使用默認版本'1.0.0'");

        // Log - World Generation
        Log("world_generation.scene_type_set", "設置場景類型為: {0}");
        Log("world_generation.no_features_enabled", "未啟用任何特性");
        Log("world_generation.feature_enabled", "已啟用 {0}");
        Log("world_generation.feature_enabled_with_value", "已啟用 {0}: {1}");
        Log("world_generation.unknown_feature", "未知的特性: {0}");
        Log("world_generation.skip_generation", "已跳過 {0} 生成");
        Log("world_generation.phase.preparing", "正在準備地圖: {0}...");
        Log("world_generation.phase.generating", "{0} - 世界生成中...");
        Log("world_generation.phase.skipping", "{0} - 已跳過{1}");
        Log("world_generation.phase.placing_blocks", "{0} - 放置方塊: 成功{1} 失敗{2} / 總計{3} ({4}%)");
        Log("world_generation.phase.spawning_map", "{0} - 正在生成地圖...");
        Log("world_generation.phase.spawning_custom_structures", "{0} - 正在生成自定義結構...");
        Log("world_generation.phase.spawning_build_mode_save", "{0} - 正在生成建築模式存檔...");
        Log("world_generation.phase.applying_settings", "{0} - 正在應用設置...");
        Log("world_generation.loading_start", "開始加載地圖: {0}");
        Log("world_generation.no_map_data", "地圖 {0} 不包含地圖數據");
        Log("world_generation.no_content_type",
            "地圖 '{0}' 未定義任何內容類型（MapData、CustomStructures 或 BuildModeSave）");
        Log("world_generation.no_commands", "未啟用任何 {0}");
        Log("world_generation.exited_map", "已退出地圖");
        Log("world_generation.executing_command", "執行 {0}: '{1}'");
        Log("world_generation.executing_loop_command", "執行循環 {0}: '{1}'");
        Log("world_generation.start_game_map", "開始遊戲時自動使用配置的地圖: {0} (ID: {1})");
        Log("world_generation.start_game_map_not_found", "未找到配置的地圖 (ID: {0})，將使用默認");
        Log("world_generation.no_map_selected", "未選擇地圖，生成原版世界");
        Log("world_generation.applying_settings_overrides", "正在應用 Settings 覆蓋，共 {0} 項");
        Log("world_generation.settings_override_not_found", "未找到 Settings 覆蓋項: {0}");
        Log("world_generation.settings_override_applied", "已應用 Settings 覆蓋: {0} = {1}");
        Log("world_generation.settings_override_failed", "應用 Settings 覆蓋失敗: {0}");

        // Log - Validation
        Log("validation.map_invalid_type", "map 字段格式不正確");
        Log("validation.map_missing_field", "地圖缺少必需字段: {0}");
        Log("validation.map_field_type_error", "地圖 {0} 字段必須是{1}");
        Log("validation.map_map_empty", "地圖 map 數組不能為空");
        Log("validation.map_row_not_string", "地圖 map 第 {0} 行必須是字符串");
        Log("validation.map_item_row_not_array", "地圖 items 第 {0} 行必須是數組");
        Log("validation.map_item_not_string", "地圖 items[{0}][{1}] 必須是字符串");
        Log("validation.multiple_content_types", "不能同時使用多種內容類型（map_data、custom_structures、build_mode_save），只能選擇一種");
        Log("validation.missing_content_type", "缺少內容類型（map_data、custom_structures 或 build_mode_save）");
        Log("validation.custom_structures_without_mod",
            "檢測到 custom_structures 字段，但未安裝自定義結構模組（Custom Structures），請先安裝該模組");
        Log("validation.features_invalid_type", "features 字段必須是數組或對象");
        Log("validation.features_empty", "features 數組為空，將被忽略");
        Log("validation.features_element_invalid", "features 第 {0} 個元素格式不正確，已跳過");
        Log("validation.no_data", "{0} 中沒有 {1} 數據");
        Log("validation.row_data_empty", "{0} 行數據為空");
        Log("validation.field_missing_default", "缺少必需字段: {0}，已使用默認值 \"{1}\"");
        Log("validation.field_null_default", "字段為空: {0}，已使用默認值 \"{1}\"");
        Log("validation.field_empty_string_default", "字段為空字符串: {0}，已使用默認值 \"{1}\"");
        Log("validation.field_must_be_array_default", "缺少必需字段: {0}，已使用默認值 [\"{1}\"]");
        Log("validation.field_null_array_default", "字段為空: {0}，已使用默認值 [\"{1}\"]");
        Log("validation.field_convert_to_array", "{0} 字段必須是數組，已轉換為數組");
        Log("validation.array_empty_default", "{0} 數組為空，已設置默認值");
        Log("validation.array_empty_removed", "{0} 數組為空，已移除");

        // Log - Map Loader
        Log("map_loader.load_error", "地圖或地圖數據為空");
        Log("map_loader.invalid_format", "無效的地圖格式，缺少 map 字段");
        Log("map_loader.key_missing", "錯誤: 字符串地圖格式缺少 'key' 定義");
        Log("map_loader.string_map_applied", "字符串地圖應用完成，成功 {0} 個，失敗 {1} 個");
        Log("map_loader.load_success", "成功加載地圖: 起始坐標({0}, {1}), 尺寸({2}x{3})");
        Log("map_loader.load_failed", "加載地圖失敗: {0}");
        Log("map_loader.place_failed", "在 ({0}, {1}) 放置{2} {3} 失敗: {4}");
        Log("map_loader.multiple_blocks_in_list", "在 ({0}, {1}) 檢測到列表中有多個物塊，只生成第一個");
        Log("map_loader.unsupported_value_type", "不支持的值類型: {0}，位置 ({1}, {2})");
        Log("map_loader.nested_structure_not_supported", "嵌套結構不被支持，位置 ({0}, {1})");
        Log("map_loader.unexpected_token_type", "意外的令牌類型: {0}，位置 ({1}, {2})");
        Log("map_loader.reload_success", "成功重新加載地圖: {0}");
        Log("map_loader.reload_failed", "重新加載地圖失敗: {0}");
        Log("map_loader.restarting_scene", "正在重啟場景...");
        Log("map_loader.scene_reloading", "正在重新加載場景: {0}");
        Log("map_loader.scene_reloaded", "場景已重新加載");
        Log("map_loader.scene_reload_failed", "重新加載場景失敗: {0}");
        Log("map_loader.no_current_map", "當前沒有加載的地圖 配置");
        Log("map_loader.custom_structures_not_supported", "自定義結構不支持用於地圖加載: {0}");
        Log("map_loader.no_features_enabled", "未啟用任何特性");
        Log("map_loader.feature_enabled", "已啟用 {0}");
        Log("map_loader.feature_enabled_with_value", "已啟用 {0}: {1}");
        Log("map_loader.skip_generation", "已跳過 {0} 生成");
        Log("map_loader.no_directory_path", "地圖 目錄路徑為空");
        Log("map_loader.map_json_not_found", "在 {0} 中找不到 map.json");
        Log("map_loader.map_deserialize_failed", "反序列化地圖 失敗");
        Log("map_loader.map_reloaded_from_disk", "已從磁碟重新加載地圖: {0}");
        Log("map_loader.map_reload_failed", "重新加載地圖 失敗: {0}");
        Log("map_loader.validation.no_data", "{0} 中沒有 {1} 數據");
        Log("map_loader.validation.row_data_empty", "{0} 行數據為空");

        // Log - Error
        Log("error.no_map_file", "找不到 map.json 文件: {0}");
        Log("error.no_valid_directories", "沒有有效的地圖目錄，請檢查 Maps 文件夾");
        Log("error.custom_structures_mod_not_loaded", "地圖 '{0}' 需要自定義結構模組，但該模組未加載");
        Log("error.multiple_content_types",
            "地圖 '{0}' 同時定義了多種內容類型（MapData、CustomStructures、BuildModeSave），只允許定義一種");

        // Log - Map Load
        Log("map_load.empty_target_path", "目標路徑不能為空");
        Log("map_load.unauthorized", "無權讀取文件 '{0}': {1}");
        Log("map_load.io_error", "讀取文件 '{0}' 失敗: {1}");
        Log("map_load.file_empty", "文件 '{0}' 為空，將創建默認配置");
        Log("map_load.deserialize_null", "文件 '{0}' 反序列化失敗（返回 null），將創建默認配置");
        Log("map_load.invalid_json", "文件 '{0}' JSON 格式無效: {1}");
        Log("map_load.no_folder_name", "無法從路徑 '{0}' 解析有效的文件夾名稱");

        // Log - Common
        Log("common.map", "地圖");
        Log("common.item", "物品");
        Log("common.block", "方塊");
        Log("common.terrain", "地形");
        Log("common.structure", "結構");
        Log("common.background", "背景");
        Log("common.startup_command", "啟動命令");
        Log("common.loop_command", "循環命令");

        // Log - Mod Command
        Log("mod_command.empty_type", "未知的指令類型");
        Log("mod_command.world_not_loaded", "未加載世界");
        Log("mod_command.no_waypoints", "當前地圖 未定義路徑點");
        Log("mod_command.exit_no_target", "請指定退出目標: none (原版) 或 tutorial (教程關)");
        Log("mod_command.register_failed", "註冊自定義指令失敗: {0}\n{1}");
        Log("mod_command.no_map", "當前沒有可用的地圖");

        // Log - Custom Structures Loader
        Log("custom_structures_loader.loading", "正在加載自定義結構: {0}");
        Log("custom_structures_loader.failed", "加載自定義結構({0})失敗: {1}");
        Log("custom_structures_loader.not_found", "未找到 {0}, 反射失敗");
        Log("custom_structures_loader.not_found_custom_structures", "未找到自定義結構文件");
        Log("custom_structures_loader.suppress.structure_loader_not_found", "未找到 StructureLoader 類型");
        Log("custom_structures_loader.suppress.cleared_definitions", "已清除自定義結構註冊表（StructureDefinitions），已抑制自動生成");
        Log("custom_structures_loader.suppress.cleared_field", "已清除 {0}，已抑制 Custom Structures 自動生成");
        Log("custom_structures_loader.suppress.no_registry", "找到 Custom Structures 模組但無可清除的結構註冊表");
        Log("custom_structures_loader.suppress.failed", "抑制 Custom Structures 自動生成失敗: {0}");

        // Log - Build Mode Save Loader
        Log("build_mode_save_loader.loading", "正在加載 Build Mode 存檔: {0} (方塊: {1}, 液體: {2}, 背景: {3})");
        Log("build_mode_save_loader.failed", "加載 Build Mode 存檔({0})失敗: {1}");
        Log("build_mode_save_loader.not_found_buildmode_save", "未找到 Build Mode 存檔文件");
        Log("build_mode_save_loader.bg_sprite_missing", "未找到背景精靈: {0}");

        // Log - Map Loader (Build Mode)
        Log("map_loader.build_mode_save_applied", "Build Mode 存檔應用完成: {0} 個方塊, {1} 個液體, {2} 個背景, 失敗 {3} 個");
        Log("map_loader.not_found_buildmode_save", "未找到 Build Mode 存檔文件");

        // Log - Map Directory Loader
        Log("loader.directory_not_found", "找不到目錄: {0}");
        Log("loader.map_json_not_found", "在 {0} 中找不到 map.json");
        Log("loader.map_json_failed", "反序列化 map.json 失敗: {0}");
        Log("loader.success", "成功加載地圖: {0} (ID: {1}, 版本: {2})");
        Log("loader.failed", "從 {0} 加載地圖失敗: {1}");
        Log("loader.level_dir_not_found", "找不到關卡目錄: {0}");
        Log("loader.no_level_files", "在 {0} 中未找到關卡文件");
        Log("loader.loaded_level", "已加載關卡: {0}");
        Log("loader.failed_to_load_level", "加載關卡文件 {0} 失敗: {1}");
        Log("loader.no_world_settings", "未找到世界設置，使用默認值");
        Log("loader.missing_type", "{0} 缺少 'type' 屬性，期望值 '{1}'");
        Log("loader.type_mismatch", "{0} 類型不匹配: 期望 '{1}'，實際 '{2}'");
        Log("loader.failed_to_load_file", "加載 {0} 失敗: {1}");
    }
    
    private void Feature(string key, string value)
    {
        Other($"feature.{key}", value);
    }
}