using MossLib.Base;

namespace CustomFungamePack.Lang
{
    public class ZhTwLangGenerator : ModLangGenBase
    {
        protected override string LanguageCode => "zh-TW";

        protected override void BuildLocaleData()
        {
            // Config
            Add("config.more_logs.name", "更多日誌");
            Add("config.more_logs.description", "顯示更多日日誌");
            Add("config.start_game_use_fungame.name", "開始遊戲使用 Fungame");
            Add("config.start_game_use_fungame.description", "開啟新遊戲時使用選中的 Fungame");
            Add("config.first_use_fungame.name", "首選 Fungame");
            Add("config.first_use_fungame.description", "開始新遊戲時使用的 Fungame ID。需要啟用“開始遊戲使用 Fungame”");
            
            // Feature
            Add("feature.full_bright", "全亮");
            Add("feature.forgiving_level", "仁慈關卡");
            Add("feature.gravity", "重力");
            Add("feature.jump_limit", "跳躍極限");
            Add("feature.climb_limit", "攀爬極限");
            Add("feature.mine_data", "地雷");
            Add("feature.jump_pad_data", "彈跳板");
            Add("feature.turret_data", "炮塔");
            Add("feature.sound_cannon_data", "音波炮");
            Add("feature.spike_stabber_data", "尖刺陷阱");
            Add("feature.geyser_data", "間歇泉");
            Add("feature.beartrap_data", "捕獸夾");

            // Feature - 子屬性
            Add("feature.mine.undestroy", "不毀");
            Add("feature.mine.cooldown", "冷卻");
            Add("feature.jump_pad.cooldown", "冷卻");
            Add("feature.jump_pad.force", "力度");
            Add("feature.jump_pad.no_light", "無光");
            Add("feature.turret.cooldown", "冷卻");
            Add("feature.turret.shot_power_multiplier", "射擊力度");
            Add("feature.turret.undestroy", "不毀");
            Add("feature.turret.no_light", "無光");
            Add("feature.turret.range", "範圍");
            Add("feature.sound_cannon.cooldown", "冷卻");
            Add("feature.sound_cannon.max_distance", "範圍");
            Add("feature.sound_cannon.charge_time", "充能時間");
            Add("feature.sound_cannon.undestroy", "不毀");
            Add("feature.spike_stabber.damage_mult", "傷害倍率");
            Add("feature.spike_stabber.undestroy", "不毀");
            Add("feature.spike_stabber.cooldown", "冷卻");
            Add("feature.spike_stabber.no_light", "無光");
            Add("feature.spike_stabber.sound", "音效");
            Add("feature.geyser.cooldown", "冷卻");
            Add("feature.geyser.activate_duration", "噴發時長");
            Add("feature.geyser.no_liquid", "無液體");
            Add("feature.geyser.rumble_time", "震動時長");
            Add("feature.geyser.range", "觸發範圍");
            Add("feature.beartrap.damage_mult", "傷害倍率");
            Add("feature.beartrap.undestroy", "不毀");
            Add("feature.beartrap.cooldown", "冷卻");
            
            // Command - Fungame
            Add("command.fungame.description", "Fungame 的相關指令");
            Add("command.fungame.string", "選擇功能");
            Add("command.fungame.parameter", "功能參數");
            Add("command.fungame.help", "可用子命令:\n  " +
                "reload  - 重新加載當前地圖\n  " +
                "info    - 顯示地圖信息\n  " +
                "spawn   - 傳送回出生點\n  " +
                "select  - 選擇 Fungame\n  " +
                "list    - 列出所有 Fungame\n  " +
                "feature - 管理特性\n  " +
                "waypoint- 管理路徑點 (list/get)\n  " +
                "save    - 保存當前Fungame到本地\n  " +
                "save as - 交互式選取區域並保存為地圖數據");

            // Command - Fungame - Info
            Add("command.fungame.info.name", "名稱: {0}");
            Add("command.fungame.info.id", "ID: {0}");
            Add("command.fungame.info.version", "版本: {0}");
            Add("command.fungame.info.authors", "作者: {0}");
            Add("command.fungame.info.description", "描述: {0}");
            Add("command.fungame.info.features", "特性: {0}");
            Add("command.fungame.info.spawn", "出生點: {0}");

            // Command - Fungame - Spawn
            Add("command.fungame.spawn", "傳送回出生點{0}中...");

            // Command - Fungame - Waypoint
            Add("command.fungame.waypoint.help", "路徑點子命令:\n  " +
                                                 "list - 列出所有路徑點\n  " +
                                                 "get <id或名稱> - 傳送到指定路徑點");
            Add("command.fungame.waypoint.list_header", "可用路徑點 ({0}):");
            Add("command.fungame.waypoint.list_item", "  {0}. {1} - 位置: {2}");
            Add("command.fungame.waypoint.teleport", "正在傳送到路徑點'{0}'，位置 {1}...");
            Add("command.fungame.waypoint.not_found", "未找到路徑點: {0}");
            Add("command.fungame.waypoint.invalid_index", "無效的索引 {0}，請輸入 1 到 {1} 之間的數字");
            Add("command.fungame.waypoint.get_no_id", "請指定路徑點 ID 或索引來傳送");
            Add("command.fungame.waypoint.unknown_subcommand", "未知的路徑點子命令：{0}");

            // Command - Fungame - List
            Add("command.fungame.list.header", "已加載 {0} 個 Fungame:");
            Add("command.fungame.list.item", "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})");
            Add("command.fungame.list.empty", "沒有可用的 Fungame");

            // Command - Fungame - Select
            Add("command.fungame.select.no_key", "請提供要選擇的 Fungame ID 或名稱");
            Add("command.fungame.select.not_found", "未找到 Fungame: {0}");
            Add("command.fungame.select.success", "已選擇 {0} (ID: {1})");
            Add("command.fungame.select.without_world", "已選擇 {0}，但世界未加載。地圖將在開始遊戲時加載。");
            Add("command.fungame.select.invalid_index", "無效的索引 {0}，請輸入 1 到 {1} 之間的數字");

            // Command - Fungame - Config
            Add("command.fungame.config.set_missing_params", "請指定配置名稱和值來設定");
            Add("command.fungame.config.list_header", "目前組態設定：");
            Add("command.fungame.config.item", " {0}({1}): {2}");
            Add("command.fungame.config.item_description", " {0}");
            Add("command.fungame.config.set_success", "配置 '{0}' 已設定為 {1}");
            Add("command.fungame.config.set_failed", "設定配置 '{0}' 失敗: {1}");
            Add("command.fungame.config.not_found", "未找到設定：{0}");
            Add("command.fungame.config.invalid_value", "{0} 的值無效：{1}");
            
            // Command - Fungame - Exit
            Add("command.fungame.exiting", "正在返回{0}...");
            Add("command.fungame.exit.invalid_target", "未知的退出目標: {0}，可用: none, tutorial");
            Add("command.fungame.exit.target.none", "原版遊戲");
            Add("command.fungame.exit.target.tutorial", "教程");

            // Command - Fungame - Save
            Add("command.fungame.save.success", "已將 Fungame '{0}' 保存到: {1}");
            Add("command.fungame.save.failed", "保存 Fungame '{0}' 失敗: {1}");
            Add("command.fungame.save.no_directory", "當前 Fungame 沒有關聯的目錄路徑，無法保存");
            Add("command.fungame.save.invalid_position", "無效的位置格式，請使用 Vector2 格式：\"x,y\" (例如 \"86,-11\")");
            Add("command.fungame.save.area_empty", "指定區域超出世界邊界或為空");
            Add("command.fungame.save.area_success", "已保存區域物塊地圖: ({0},{1}) 到 ({2},{3}), 尺寸 {4}x{5}, {6} 種物塊類型 → {7}");
            Add("command.fungame.save.target_not_found", "目標 Fungame 文件夾 '{0}' 未找到");
            Add("command.fungame.save.missing_end_position", "缺少結束位置，請提供兩個位置參數 (例如: fg save 86,-11 2,45)");
            Add("command.fungame.save.as.default_description", "區域掃描保存");
            Add("command.fungame.save.as.start_position", "請鼠標左鍵點擊選擇起始位置...");
            Add("command.fungame.save.as.end_position", "請鼠標左鍵點擊選擇結束位置...");
            Add("command.fungame.save.as.confirm", "位置已記錄，再次輸入相同指令以確認保存。");

            // Command - Fungame - Feature
            Add("command.fungame.feature.unknown_subcommand", "未知的特性子命令：{0}");
            Add("command.fungame.feature.set_missing_params", "請指定特性名稱和值來設置");
            Add("command.fungame.feature.list_header", "當前特性設置：");
            Add("command.fungame.feature.item", "    {0}({1}): {2}");
            Add("command.fungame.feature.parent_item", "    {0}({1}):");
            Add("command.fungame.feature.get_success", "特性 '{0}' = {1}");
            Add("command.fungame.feature.set_success", "特性 '{0}' 已設置為 {1}");
            Add("command.fungame.feature.not_found", "未找到特性：{0}");
            Add("command.fungame.feature.invalid_value", "{0} 的值無效：{1}");

            // Log - Fungame Check
            Add("log.fungame_check.id_format_warning", "ID格式不正確，將自動修正");
            Add("log.fungame_check.author_not_string", "作者元素 {0} 不是字符串，已移除");
            Add("log.fungame_check.author_empty", "作者數組為空，已設置默認值");
            Add("log.fungame_check.version_format_warning", "版本格式'{0}'不正確，將使用默認版本'1.0.0'");

            // Log - World Generation
            Add("log.world_generation.scene_type_set", "設置場景類型為: {0}");
            Add("log.world_generation.no_features_enabled", "未啟用任何特性");
            Add("log.world_generation.feature_enabled", "已啟用 {0}");
            Add("log.world_generation.feature_enabled_with_value", "已啟用 {0}: {1}");
            Add("log.world_generation.unknown_feature", "未知的特性: {0}");
            Add("log.world_generation.skip_generation", "已跳過 {0} 生成");
            Add("log.world_generation.initializing_world", "初始化Fungame地圖...");
            Add("log.world_generation.loading_fungame_map", "正在加載Fungame地圖: {0}");
            Add("log.world_generation.no_map_data", "Fungame {0} 不包含地圖數據");
            Add("log.world_generation.no_content_type", "Fungame '{0}' 未定義任何內容類型（MapData、CustomStructures 或 BuildModeSave）");
            Add("log.world_generation.no_commands", "未啟用任何 {0}");
            Add("log.world_generation.exited_fungame", "已退出 Fungame");
            Add("log.world_generation.executing_command", "執行 {0}: '{1}'");
            Add("log.world_generation.executing_loop_command", "執行循環 {0}: '{1}'");
            Add("log.world_generation.start_game_fungame", "開始遊戲時自動使用配置的 Fungame: {0} (ID: {1})");
            Add("log.world_generation.start_game_fungame_not_found", "未找到配置的 Fungame (ID: {0})，將使用默認");
            Add("log.world_generation.no_fungame_selected", "未選擇 Fungame，生成原版世界");
            Add("log.world_generation.applying_settings_overrides", "正在應用 Settings 覆蓋，共 {0} 項");
            Add("log.world_generation.settings_override_not_found", "未找到 Settings 覆蓋項: {0}");
            Add("log.world_generation.settings_override_applied", "已應用 Settings 覆蓋: {0} = {1}");
            Add("log.world_generation.settings_override_failed", "應用 Settings 覆蓋失敗: {0}");

            // Log - Validation
            Add("log.validation.map_invalid_type", "map 字段格式不正確");
            Add("log.validation.map_missing_field", "地圖缺少必需字段: {0}");
            Add("log.validation.map_field_type_error", "地圖 {0} 字段必須是{1}");
            Add("log.validation.map_map_empty", "地圖 map 數組不能為空");
            Add("log.validation.map_row_not_string", "地圖 map 第 {0} 行必須是字符串");
            Add("log.validation.map_item_row_not_array", "地圖 items 第 {0} 行必須是數組");
            Add("log.validation.map_item_not_string", "地圖 items[{0}][{1}] 必須是字符串");
            Add("log.validation.multiple_content_types", "不能同時使用多種內容類型（map_data、custom_structures、build_mode_save），只能選擇一種");
            Add("log.validation.missing_content_type", "缺少內容類型（map_data、custom_structures 或 build_mode_save）");
            Add("log.validation.custom_structures_without_mod", "檢測到 custom_structures 字段，但未安裝自定義結構模組（Custom Structures），請先安裝該模組");
            Add("log.validation.features_invalid_type", "features 字段必須是數組或對象");
            Add("log.validation.features_empty", "features 數組為空，將被忽略");
            Add("log.validation.features_element_invalid", "features 第 {0} 個元素格式不正確，已跳過");
            Add("log.validation.no_data", "{0} 中沒有 {1} 數據");
            Add("log.validation.row_data_empty", "{0} 行數據為空");
            Add("log.validation.field_missing_default", "缺少必需字段: {0}，已使用默認值 \"{1}\"");
            Add("log.validation.field_null_default", "字段為空: {0}，已使用默認值 \"{1}\"");
            Add("log.validation.field_empty_string_default", "字段為空字符串: {0}，已使用默認值 \"{1}\"");
            Add("log.validation.field_must_be_array_default", "缺少必需字段: {0}，已使用默認值 [\"{1}\"]");
            Add("log.validation.field_null_array_default", "字段為空: {0}，已使用默認值 [\"{1}\"]");
            Add("log.validation.field_convert_to_array", "{0} 字段必須是數組，已轉換為數組");
            Add("log.validation.array_empty_default", "{0} 數組為空，已設置默認值");
            Add("log.validation.array_empty_removed", "{0} 數組為空，已移除");

            // Log - Map Loader
            Add("log.map_loader.load_error", "Fungame 或地圖數據為空");
            Add("log.map_loader.invalid_format", "無效的地圖格式，缺少 map 字段");
            Add("log.map_loader.key_missing", "錯誤: 字符串地圖格式缺少 'key' 定義");
            Add("log.map_loader.string_map_applied", "字符串地圖應用完成，放置 {0} 個方塊，{1} 個物品，失敗 {2} 個");
            Add("log.map_loader.load_success", "成功加載地圖: 起始坐標({0}, {1}), 尺寸({2}x{3})");
            Add("log.map_loader.load_failed", "加載地圖失敗: {0}");
            Add("log.map_loader.place_failed", "在 ({0}, {1}) 放置{2} {3} 失敗: {4}");
            Add("log.map_loader.multiple_blocks_in_list", "在 ({0}, {1}) 檢測到列表中有多個物塊，只生成第一個");
            Add("log.map_loader.unsupported_value_type", "不支持的值類型: {0}，位置 ({1}, {2})");
            Add("log.map_loader.nested_structure_not_supported", "嵌套結構不被支持，位置 ({0}, {1})");
            Add("log.map_loader.unexpected_token_type", "意外的令牌類型: {0}，位置 ({1}, {2})");
            Add("log.map_loader.reload_success", "成功重新加載地圖: {0}");
            Add("log.map_loader.reload_failed", "重新加載地圖失敗: {0}");
            Add("log.map_loader.restarting_scene", "正在重啟場景...");
            Add("log.map_loader.scene_reloading", "正在重新加載場景: {0}");
            Add("log.map_loader.scene_reloaded", "場景已重新加載");
            Add("log.map_loader.scene_reload_failed", "重新加載場景失敗: {0}");
            Add("log.map_loader.no_current_fungame", "當前沒有加載的 Fungame 配置");
            Add("log.map_loader.custom_structures_not_supported", "自定義結構不支持用於地圖加載: {0}");
            Add("log.map_loader.no_features_enabled", "未啟用任何特性");
            Add("log.map_loader.feature_enabled", "已啟用 {0}");
            Add("log.map_loader.feature_enabled_with_value", "已啟用 {0}: {1}");
            Add("log.map_loader.skip_generation", "已跳過 {0} 生成");
            Add("log.map_loader.no_directory_path", "Fungame 目錄路徑為空");
            Add("log.map_loader.fungame_json_not_found", "在 {0} 中找不到 fungame.json");
            Add("log.map_loader.fungame_deserialize_failed", "反序列化 Fungame 失敗");
            Add("log.map_loader.fungame_reloaded_from_disk", "已從磁碟重新加載 Fungame: {0}");
            Add("log.map_loader.fungame_reload_failed", "重新加載 Fungame 失敗: {0}");
            Add("log.map_loader.validation.no_data", "{0} 中沒有 {1} 數據");
            Add("log.map_loader.validation.row_data_empty", "{0} 行數據為空");

            // Log - Error
            Add("log.error.no_fungame_file", "找不到 fungame.json 文件: {0}");
            Add("log.error.no_valid_directories", "沒有有效的 Fungame 目錄，請檢查 Fungames 文件夾");
            Add("log.error.custom_structures_mod_not_loaded", "Fungame '{0}' 需要自定義結構模組，但該模組未加載");
            Add("log.error.multiple_content_types", "Fungame '{0}' 同時定義了多種內容類型（MapData、CustomStructures、BuildModeSave），只允許定義一種");

            // Log - Fungame Load
            Add("log.fungame_load.empty_target_path", "目標路徑不能為空");
            Add("log.fungame_load.unauthorized", "無權讀取文件 '{0}': {1}");
            Add("log.fungame_load.io_error", "讀取文件 '{0}' 失敗: {1}");
            Add("log.fungame_load.file_empty", "文件 '{0}' 為空，將創建默認配置");
            Add("log.fungame_load.deserialize_null", "文件 '{0}' 反序列化失敗（返回 null），將創建默認配置");
            Add("log.fungame_load.invalid_json", "文件 '{0}' JSON 格式無效: {1}");
            Add("log.fungame_load.no_folder_name", "無法從路徑 '{0}' 解析有效的文件夾名稱");

            // Log - Common
            Add("log.common.map", "地圖");
            Add("log.common.item", "物品");
            Add("log.common.block", "方塊");
            Add("log.common.terrain", "地形");
            Add("log.common.structure", "結構");
            Add("log.common.background", "背景");
            Add("log.common.startup_command", "啟動命令");
            Add("log.common.loop_command", "循環命令");

            // Log - Mod Command
            Add("log.mod_command.empty_type", "未知的指令類型");
            Add("log.mod_command.world_not_loaded", "未加載世界");
            Add("log.mod_command.no_waypoints", "當前 Fungame 未定義路徑點");
            Add("log.mod_command.exit_no_target", "請指定退出目標: none (原版) 或 tutorial (教程關)");
            Add("log.mod_command.register_failed", "註冊自定義指令失敗: {0}\n{1}");
            Add("log.mod_command.no_fungame", "當前沒有可用的 Fungame");

            // Log - Custom Structures Loader
            Add("log.custom_structures_loader.loading", "正在加載自定義結構: {0}");
            Add("log.custom_structures_loader.failed", "加載自定義結構({0})失敗: {1}");
            Add("log.custom_structures_loader.not_found", "未找到 {0}, 反射失敗");
            Add("log.custom_structures_loader.not_found_custom_structures", "未找到自定義結構文件");
            Add("log.custom_structures_loader.suppress.structure_loader_not_found", "未找到 StructureLoader 類型");
            Add("log.custom_structures_loader.suppress.cleared_definitions", "已清除自定義結構註冊表（StructureDefinitions），已抑制自動生成");
            Add("log.custom_structures_loader.suppress.cleared_field", "已清除 {0}，已抑制 Custom Structures 自動生成");
            Add("log.custom_structures_loader.suppress.no_registry", "找到 Custom Structures 模組但無可清除的結構註冊表");
            Add("log.custom_structures_loader.suppress.failed", "抑制 Custom Structures 自動生成失敗: {0}");

            // Log - Build Mode Save Loader
            Add("log.build_mode_save_loader.loading", "正在加載 Build Mode 存檔: {0} (方塊: {1}, 液體: {2}, 背景: {3})");
            Add("log.build_mode_save_loader.failed", "加載 Build Mode 存檔({0})失敗: {1}");
            Add("log.build_mode_save_loader.not_found_buildmode_save", "未找到 Build Mode 存檔文件");
            Add("log.build_mode_save_loader.bg_sprite_missing", "未找到背景精靈: {0}");

            // Log - Map Loader (Build Mode)
            Add("log.map_loader.build_mode_save_applied", "Build Mode 存檔應用完成: {0} 個方塊, {1} 個液體, {2} 個背景, 失敗 {3} 個");
            Add("log.map_loader.not_found_buildmode_save", "未找到 Build Mode 存檔文件");

            // Log - Fungame Directory Loader
            Add("log.loader.directory_not_found", "找不到目錄: {0}");
            Add("log.loader.fungame_json_not_found", "在 {0} 中找不到 fungame.json");
            Add("log.loader.fungame_json_failed", "反序列化 fungame.json 失敗: {0}");
            Add("log.loader.success", "成功加載 Fungame: {0} (ID: {1}, 版本: {2})");
            Add("log.loader.failed", "從 {0} 加載 Fungame 失敗: {1}");
            Add("log.loader.level_dir_not_found", "找不到關卡目錄: {0}");
            Add("log.loader.no_level_files", "在 {0} 中未找到關卡文件");
            Add("log.loader.loaded_level", "已加載關卡: {0}");
            Add("log.loader.failed_to_load_level", "加載關卡文件 {0} 失敗: {1}");
            Add("log.loader.no_world_settings", "未找到世界設置，使用默認值");
            Add("log.loader.missing_type", "{0} 缺少 'type' 屬性，期望值 '{1}'");
            Add("log.loader.type_mismatch", "{0} 類型不匹配: 期望 '{1}'，實際 '{2}'");
            Add("log.loader.failed_to_load_file", "加載 {0} 失敗: {1}");
        }
    }
}