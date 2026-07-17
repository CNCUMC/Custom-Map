using System.Collections.Generic;
using Bark.Base;

namespace CustomMap;

internal class LangGenerator : ModLangGenMultiBase
{
    protected override string NameSpace => Plugin.NameSpace;

    protected override IEnumerable<string> LanguageCodes =>
    [
        "EN",
        "zh-CN",
        "zh-TW",
        "ru-RU"
    ];

    protected override void BuildLocaleData()
    {
        // Config - 设置标签和描述
        Option("custom_map.more_logs",
            "More logs", "Display more logs",
            "更多日志", "显示更多日志",
            "更多日誌", "顯示更多日誌",
            "Больше логов", "Отображать больше логов");
        Option("custom_map.start_game_use_map",
            "Start game use map", "Use the selected Map when starting a new game.",
            "开始游戏使用地图", "开启新游戏时使用选中的地图",
            "開始遊戲使用地圖", "開啟新遊戲時使用選中的地圖",
            "Начать игру с картой", "Использовать выбранную карту при начале новой игры");
        Option("custom_map.first_use_map",
            "First use map", "The Map ID to use when starting a new game. Requires 'Start Use Map' to be enabled.",
            "首选地图", "开始新游戏时使用的地图ID。需要启用'开始游戏使用地图'",
            "首選地圖", "開始新遊戲時使用的地圖ID。需要啟用'開始遊戲使用地圖'",
            "Первая карта", "ID карты для начала новой игры. Требуется включенная опция 'Начать игру с картой'");
        // Option("custom_map.progress_update_interval",
        //     "Progress update interval", "Number of blocks between progress text updates during map generation. Lower values update more frequently but may impact performance.",
        //     "进度更新间隔", "放置方块时每 N 个方块更新一次进度显示。数值越小更新越频繁，但可能影响生成性能",
        //     "進度更新間隔", "放置方塊時每 N 個方塊更新一次進度顯示。數值越小更新越頻繁，但可能影響生成效能",
        //     "Интервал обновления прогресса", "Количество блоков между обновлениями текста прогресса во время генерации карты. Меньшие значения обновляются чаще, но могут влиять на производительность");

        // Map Format
        Other("format.author",
            "by '{0}'",
            "作者: '{0}'",
            "作者: '{0}'",
            "автор: '{0}'");
        Other("format.features",
            "Features: '{0}'",
            "特性: '{0}'",
            "特性: '{0}'",
            "Особенности: '{0}'");

        // Feature
        Other("feature.full_bright",
            "Full Bright",
            "全亮",
            "全亮",
            "Полная яркость");
        Other("feature.forgiving_level",
            "Forgiving Level",
            "仁慈关卡",
            "仁慈關卡",
            "Прощающий уровень");
        Other("feature.gravity",
            "Gravity",
            "重力",
            "重力",
            "Гравитация");
        Other("feature.jump_limit",
            "Jump Limit",
            "跳跃极限",
            "跳躍極限",
            "Лимит прыжков");
        Other("feature.climb_limit",
            "Climb Limit",
            "攀爬极限",
            "攀爬極限",
            "Лимит взбирания");
        Other("feature.world_settings",
            "World Settings",
            "世界设置",
            "世界設置",
            "Настройки мира");
        Other("feature.skip_terrain",
            "Skip Terrain",
            "跳过地形",
            "跳過地形",
            "Пропустить террейн");
        Other("feature.skip_structures",
            "Skip Structures",
            "跳过结构",
            "跳過結構",
            "Пропустить структуры");
        Other("feature.skip_background",
            "Skip Background",
            "跳过背景",
            "跳過背景",
            "Пропустить фон");
        Other("feature.mine",
            "Mine",
            "地雷",
            "地雷",
            "Мина");
        Other("feature.jump_pad",
            "Jump Pad",
            "弹跳板",
            "彈跳板",
            "Прыжковая платформа");
        Other("feature.turret",
            "Turret",
            "炮塔",
            "炮塔",
            "Турель");
        Other("feature.sound_cannon",
            "Sound Cannon",
            "音波炮",
            "音波炮",
            "Звуковая пушка");
        Other("feature.spike_stabber",
            "Spike Stabber",
            "尖刺陷阱",
            "尖刺陷阱",
            "Шипастый ударник");
        Other("feature.geyser",
            "Geyser",
            "间歇泉",
            "間歇泉",
            "Гейзер");
        Other("feature.beartrap",
            "Bear Trap",
            "捕兽夹",
            "捕獸夾",
            "Медвежий капкан");
        Other("feature.xp",
            "XP",
            "经验值",
            "經驗值",
            "Опыт");

        // Feature - 子属性
        Other("feature.mine.undestroy",
            "Undestroy",
            "不毀",
            "不毀",
            "Неразрушимый");
        Other("feature.mine.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");
        Other("feature.jump_pad.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");
        Other("feature.jump_pad.force",
            "Force",
            "力度",
            "力度",
            "Сила");
        Other("feature.jump_pad.no_light",
            "No Light",
            "无光",
            "無光",
            "Без света");
        Other("feature.turret.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");
        Other("feature.turret.shot_power_multiplier",
            "Shot Power",
            "射击力度",
            "射擊力度",
            "Сила выстрела");
        Other("feature.turret.undestroy",
            "Undestroy",
            "不毁",
            "不毀",
            "Неразрушимый");
        Other("feature.turret.no_light",
            "No Light",
            "无光",
            "無光",
            "Без света");
        Other("feature.turret.range",
            "Range",
            "范围",
            "範圍",
            "Дальность");
        Other("feature.sound_cannon.cooldown",
            "Cooldown",
            "冷却1",
            "冷卻",
            "Перезарядка");
        Other("feature.sound_cannon.max_distance",
            "Range",
            "范围",
            "範圍",
            "Дальность");
        Other("feature.sound_cannon.charge_time",
            "Charge Time",
            "充能时间",
            "充能時間",
            "Время зарядки");
        Other("feature.sound_cannon.undestroy",
            "Undestroy",
            "不毀",
            "不毀",
            "Неразрушимый");
        Other("feature.spike_stabber.damage_mult",
            "Damage",
            "伤害倍率",
            "傷害倍率",
            "Урон");
        Other("feature.spike_stabber.undestroy",
            "Undestroy",
            "不毀",
            "不毀",
            "Неразрушимый");
        Other("feature.spike_stabber.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");
        Other("feature.spike_stabber.no_light",
            "No Light",
            "无光",
            "無光",
            "Без света");
        Other("feature.spike_stabber.sound",
            "Sound",
            "音效",
            "音效",
            "Звук");
        Other("feature.geyser.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");
        Other("feature.geyser.activate_duration",
            "Duration",
            "喷发时长",
            "噴發時長",
            "Длительность");
        Other("feature.geyser.no_liquid",
            "No Liquid",
            "无液体",
            "無液體",
            "Без жидкости");
        Other("feature.geyser.rumble_time",
            "Rumble Time",
            "振动时长",
            "震動時長",
            "Время тряски");
        Other("feature.geyser.range",
            "Range",
            "触发范围",
            "觸發範圍",
            "Радиус");
        Other("feature.beartrap.damage_mult",
            "Damage",
            "伤害倍率",
            "傷害倍率",
            "Урон");
        Other("feature.beartrap.undestroy",
            "Undestroy",
            "不毀",
            "不毀",
            "Неразрушимый");
        Other("feature.beartrap.cooldown",
            "Cooldown",
            "冷却",
            "冷卻",
            "Перезарядка");

        // XP 子属性
        Other("feature.xp.str_xp",
            "Strength Level",
            "力量等级",
            "力量等級",
            "Уровень силы");
        Other("feature.xp.res_xp",
            "Resilience Level",
            "韌性等级",
            "韌性等級",
            "Уровень упругости");
        Other("feature.xp.int_xp",
            "Intelligence Level",
            "智力等级",
            "智力等級",
            "Уровень интеллекта");
        Other("feature.xp.exp_str",
            "Strength EXP",
            "力量经验",
            "力量經驗",
            "Опыт силы");
        Other("feature.xp.exp_res",
            "Resilience EXP",
            "韌性经验",
            "韌性經驗",
            "Опыт упругости");
        Other("feature.xp.exp_int",
            "Intelligence EXP",
            "智力经验",
            "智力經驗",
            "Опыт интеллекта");
        Other("feature.xp.min_str",
            "Min Strength EXP",
            "最小力量经验",
            "最小力量經驗",
            "Мин опыт силы");
        Other("feature.xp.max_str",
            "Max Strength EXP",
            "最大力量经验",
            "最大力量經驗",
            "Макс опыт силы");
        Other("feature.xp.min_res",
            "Min Resilience EXP",
            "最小韌性经验",
            "最小韌性經驗",
            "Мин опыт упругости");
        Other("feature.xp.max_res",
            "Max Resilience EXP",
            "最大韌性经验",
            "最大韌性經驗",
            "Макс опыт упругости");
        Other("feature.xp.min_int",
            "Min Intelligence EXP",
            "最小智力经验",
            "最小智力經驗",
            "Мин опыт интеллекта");
        Other("feature.xp.max_int",
            "Max Intelligence EXP",
            "最大智力经验",
            "最大智力經驗",
            "Макс опыт интеллекта");

        // Command - Map
        Command("cm.description",
            "Map related commands",
            "地图的相关指令",
            "地圖的相關指令",
            "Команды карты");
        Command("cm.string",
            "Select function",
            "选择功能",
            "選擇功能",
            "Выбрать функцию");
        Command("cm.parameter",
            "Function parameter",
            "功能参数",
            "功能參數",
            "Параметр функции");
        Command("cm.help.header",
            "Available subcommands:",
            "可用子命令:",
            "可用子命令:",
            "Доступные подкоманды:");
        Command("cm.help.help",
            "Show this help message",
            "显示此帮助信息",
            "顯示此幫助訊息",
            "Показать это справочное сообщение");
        Command("cm.help.reload",
            "Reload current map",
            "重新加载当前地图",
            "重新載入當前地圖",
            "Перезагрузить текущую карту");
        Command("cm.help.load",
            "Reload Maps folder",
            "重新加载 Maps 文件夹",
            "重新載入 Maps 資料夾",
            "Перезагрузить папку Maps");
        Command("cm.help.savereload",
            "Save and reload current map",
            "保存并重新加载当前地图",
            "保存並重新載入當前地圖",
            "Сохранить и перезагрузить текущую карту");
        Command("cm.reload.success",
            "Maps reloaded successfully",
            "地图重新加载成功",
            "地圖重新載入成功",
            "Карты успешно перезагружены");
        Command("cm.load.success",
            "Maps loaded successfully",
            "地图加载成功",
            "地圖載入成功",
            "Карты успешно заряж");
        Command("cm.help.info",
            "Show map info",
            "显示地图信息",
            "顯示地圖資訊",
            "Показать информацию о карте");
        Command("cm.help.spawn",
            "Teleport to spawn",
            "传送回出生点",
            "傳送回出生點",
            "Телепортация на спавн");
        Command("cm.help.select",
            "Select a Map",
            "选择地图",
            "選擇地圖",
            "Выбрать карту");
        Command("cm.help.list",
            "List all Maps",
            "列出所有地图",
            "列出所有地圖",
            "Список всех карт");
        Command("cm.help.feature",
            "Manage features",
            "管理特性",
            "管理特性",
            "Управление особенностями");
        Command("cm.help.waypoint",
            "Manage waypoints (list/get)",
            "管理路径点 (list/get)",
            "管理路徑點 (list/get)",
            "Управление путевыми точками (list/get)");
        Command("cm.help.save",
            "Save current Map",
            "保存当前地图",
            "保存當前地圖",
            "Сохранить текущую карту");
        Command("cm.help.layer",
            "Switch map layer",
            "切换地图层级",
            "切換地圖層級",
            "Переключить уровень карты");
        Command("cm.help.exit",
            "Exit Map",
            "退出地图",
            "退出地圖",
            "Выйти с карты");

        // Command - Map - Info
        Command("cm.info.name",
            "Name: '{0}'",
            "名称: '{0}'",
            "名稱: '{0}'",
            "Имя: '{0}'");
        Command("cm.info.id",
            "ID: '{0}'",
            "ID: '{0}'",
            "ID: '{0}'",
            "ID: '{0}'");
        Command("cm.info.version",
            "Version: '{0}'",
            "版本: '{0}'",
            "版本: '{0}'",
            "Версия: '{0}'");
        Command("cm.info.authors",
            "Authors: '{0}'",
            "作者: '{0}'",
            "作者: '{0}'",
            "Авторы: '{0}'");
        Command("cm.info.description",
            "Description: '{0}'",
            "描述: '{0}'",
            "描述: '{0}'",
            "Описание: '{0}'");
        Command("cm.info.features",
            "Features: '{0}'",
            "特性: '{0}'",
            "特性: '{0}'",
            "Особенности: '{0}'");
        Command("cm.info.spawn",
            "Spawn point: '{0}'",
            "出生点: '{0}'",
            "出生點: '{0}'",
            "Точка спавна: '{0}'");

        // Command - Map - Spawn
        Command("cm.spawn",
            "Teleporting back to the spawn point {0} now...",
            "传送回出生点{0}中...",
            "傳送回出生點{0}中...",
            "Телепортация на точку спавна {0}...");

        // Command - Map - Waypoint
        Command("cm.waypoint.help",
            "Waypoint subcommands: list, get <id or name>",
            "路径点子命令: list, get <id或名称>",
            "路徑點子命令: list, get <id或名稱>",
            "Подкоманды путевых точек: list, get <id или имя>");
        Command("cm.waypoint.list_header",
            "Available waypoints ({0}):",
            "可用路径点 ({0}):",
            "可用路徑點 ({0}):",
            "Доступные путевые точки ({0}):");
        Command("cm.waypoint.list_item",
            "  {0}. {1} - Position: {2}",
            "  {0}. {1} - 位置: {2}",
            "  {0}. {1} - 位置: {2}",
            "  {0}. {1} - Позиция: {2}");
        Command("cm.waypoint.teleport",
            "Teleporting to waypoint '{0}' at position {1}...",
            "正在传送到路径点'{0}'，位置 {1}...",
            "正在傳送到路徑點'{0}'，位置 {1}...",
            "Телепортация к путевой точке '{0}' на позицию {1}...");
        Command("cm.waypoint.not_found",
            "Waypoint not found: '{0}'",
            "未找到路径点: '{0}'",
            "未找到路徑點: '{0}'",
            "Путевая точка не найдена: '{0}'");
        Command("cm.waypoint.invalid_index",
            "Invalid index {0}, please enter a number between 1 and {1}",
            "无效的索引 '{0}'，请输入 1 到 {1} 之间的数字",
            "無效的索引 '{0}'，請輸入 1 到 {1} 之間的數字",
            "Неверный индекс {0}, введите число от 1 до {1}");
        Command("cm.waypoint.get_no_id",
            "Please specify a waypoint ID or index to teleport to",
            "请指定路径点 ID 或索引来传送",
            "請指定路徑點 ID 或索引來傳送",
            "Укажите ID или индекс путевой точки для телепортации");
        Command("cm.waypoint.unknown_subcommand",
            "Unknown waypoint subcommand: '{0}'",
            "未知的路径点子命令: '{0}'",
            "未知的路徑點子命令: '{0}'",
            "Неизвестная подкоманда путевой точки: '{0}'");

        // Command - Map - List
        Command("cm.list.header",
            "Loaded '{0}' Map(s):",
            "已加载 '{0}' 个地图:",
            "已載入 '{0}' 個地圖:",
            "Загружено карт: '{0}'");
        Command("cm.list.item",
            "{0}[{1}] {2} (ID: {3}, Version: {4}, Authors: {5})",
            "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})",
            "{0}[{1}] {2} (ID: {3}, 版本: {4}, 作者: {5})",
            "{0}[{1}] {2} (ID: {3}, Версия: {4}, Авторы: {5})");
        Command("cm.list.empty",
            "No Maps available",
            "没有可用的地图",
            "沒有可用的地圖",
            "Нет доступных карт");

        // Command - Map - Select
        Command("cm.select.no_key",
            "Please provide a Map ID or name to select",
            "请提供要选择的地图ID 或名称",
            "請提供要選擇的地圖ID 或名稱",
            "Укажите ID или имя карты для выбора");
        Command("cm.select.not_found",
            "Map not found: '{0}'",
            "未找到地图: '{0}'",
            "未找到地圖: '{0}'",
            "Карта не найдена: '{0}'");
        Command("cm.select.success",
            "Selected '{0}' (ID: {1})",
            "已选择 '{0}' (ID: {1})",
            "已選擇 '{0}' (ID: {1})",
            "Выбрана '{0}' (ID: {1})");
        Command("cm.select.without_world",
            "Selected {0}, but world is not loaded. Map will be loaded when you start a game.",
            "已选择 '{0}'，但世界未加载。地图将在开始游戏时加载。",
            "已選擇 '{0}'，但世界未載入。地圖將在開始遊戲時載入。",
            "Выбрана {0}, но мир не загружен. Карта будет загружена при начале игры.");
        Command("cm.select.invalid_index",
            "Invalid index {0}, please enter a number between 1 and {1}",
            "无效的索引 '{0}'，请输入 1 到 {1} 之间的数字",
            "無效的索引 '{0}'，請輸入 1 到 {1} 之間的數字",
            "Неверный индекс {0}, введите число от 1 до {1}");

        // Command - Map - Layer
        Command("cm.layer.current",
            "Current layer: {0}/{1}",
            "当前层级: {0}/{1}",
            "當前層級: {0}/{1}",
            "Текущий слой: {0}/{1}");
        Command("cm.layer.switched",
            "Switched to layer '{0}'",
            "已切换到层级 '{0}'",
            "已切換到層級 '{0}'",
            "Переключено на слой '{0}'");
        Command("cm.layer.already",
            "Already on layer '{0}'",
            "已在层级 '{0}'",
            "已在層級 '{0}'",
            "Уже на слое '{0}'");
        Command("cm.layer.invalid",
            "Invalid layer, please enter a number between 1 and '{0}'",
            "无效的层级，请输入 1 到 '{0}' 之间的数字",
            "無效的層級，請輸入 1 到 '{0}' 之間的數字",
            "Неверный слой, введите число от 1 до '{0}'");
        Command("cm.layer.no_layers",
            "No layers available for this map",
            "此地图没有可用的层级",
            "此地圖沒有可用的層級",
            "Нет доступных слоев для этой карты");

        // Command - Map - Config
        Command("cm.config.set_missing_params",
            "Please specify configuration name and value to set",
            "请指定配置名称和值来设置",
            "請指定配置名稱和值來設置",
            "Укажите имя и значение конфигурации для установки");
        Command("cm.config.list_header",
            "Current configuration settings:",
            "当前配置设置: ",
            "當前配置設定: ",
            "Текущие настройки конфигурации:");
        Command("cm.config.set_success",
            "Configuration '{0}' has been set to {1}",
            "配置 '{0}' 已设置为 '{1}'",
            "配置 '{0}' 已設置為 '{1}'",
            "Конфигурация '{0}' установлена на {1}");
        Command("cm.config.set_failed",
            "Failed to set configuration '{0}': {1}",
            "设置配置 '{0}' 失败: '{1}'",
            "設置配置 '{0}' 失敗: '{1}'",
            "Не удалось установить конфигурацию '{0}': {1}");
        Command("cm.config.not_found",
            "Configuration not found: '{0}'",
            "未找到配置: '{0}'",
            "未找到配置: '{0}'",
            "Конфигурация не найдена: '{0}'");
        Command("cm.config.invalid_value",
            "The value of '{0}' is invalid: {1}",
            "'{0}' 的值无效: '{1}'",
            "'{0}' 的值無效: '{1}'",
            "Значение '{0}' неверно: {1}");

        // Command - Map - Exit
        Command("cm.exiting",
            "Returning to {0}...",
            "正在返回{0}...",
            "正在返回{0}...",
            "Возврат к {0}...");
        Command("cm.exit.invalid_target",
            "Unknown exit target: {0}, available: none, tutorial",
            "未知的退出目标: '{0}'，可用: 'none', 'tutorial'",
            "未知的退出目標: '{0}'，可用: 'none', 'tutorial'",
            "Неизвестная цель выхода: {0}, доступно: none, tutorial");
        Command("cm.exit.target.none",
            "vanilla game",
            "原版游戏",
            "原版遊戲",
            "ванильная игра");
        Command("cm.exit.target.tutorial",
            "tutorial",
            "教程",
            "教程",
            "туториал");

        // Command - Map - Save
        Command("cm.save.success",
            "Map '{0}' saved to: {1}",
            "已将地图 '{0}' 保存到: '{1}'",
            "已將地圖 '{0}' 保存到: '{1}'",
            "Карта '{0}' сохранена в: {1}");
        Command("cm.save.failed",
            "Failed to save Map '{0}': {1}",
            "保存地图 '{0}' 失败: '{1}'",
            "保存地圖 '{0}' 失敗: '{1}'",
            "Не удалось сохранить карту '{0}': {1}");
        Command("cm.save.no_directory",
            "Current Map has no associated directory path, cannot save",
            "当前地图没有关联的目录路径，无法保存",
            "當前地圖沒有關聯的目錄路徑，無法保存",
            "Текущая карта не имеет связанного пути к каталогу, невозможно сохранить");
        Command("cm.save.target_not_found",
            "Target Map folder '{0}' not found",
            "目标地图文件夹 '{0}' 未找到",
            "目標地圖資料夾 '{0}' 未找到",
            "Папка целевой карты '{0}' не найдена");

        // Command - Map - Feature
        Command("cm.feature.unknown_subcommand",
            "Unknown feature subcommand: '{0}'",
            "未知的特性子命令: '{0}'",
            "未知的特性子命令: '{0}'",
            "Неизвестная подкоманда особенности: '{0}'");
        Command("cm.feature.set_missing_params",
            "Please specify feature name and value to set",
            "请指定特性名称和值来设置",
            "請指定特性名稱和值來設置",
            "Укажите имя и значение особенности для установки");
        Command("cm.feature.list_header",
            "Current feature settings:",
            "当前特性设置: ",
            "當前特性設定: ",
            "Текущие настройки особенностей:");
        Command("cm.feature.get_success",
            "Feature '{0}' = {1}",
            "特性 '{0}' = '{1}'",
            "特性 '{0}' = '{1}'",
            "Особенность '{0}' = {1}");
        Command("cm.feature.set_success",
            "Feature '{0}' set to {1}",
            "特性 '{0}' 已设置为 '{1}'",
            "特性 '{0}' 已設置為 '{1}'",
            "Особенность '{0}' установлена на {1}");
        Command("cm.feature.not_found",
            "Feature not found: '{0}'",
            "未找到特性: '{0}'",
            "未找到特性: '{0}'",
            "Особенность не найдена: '{0}'");
        Command("cm.feature.invalid_value",
            "Invalid value for '{0}': '{1}'",
            "'{0}' 的值无效: '{1}'",
            "'{0}' 的值無效: '{1}'",
            "Неверное значение для '{0}': '{1}'");

        // Log - World Generation
        Log("world_generation_patch.type_set",
            "Set scene type to: '{0}'",
            "设置场景类型为: '{0}'",
            "設置場景類型為: '{0}'",
            "Установлен тип сцены: '{0}'");
        Log("world_generation_patch.no_features_enabled",
            "No features enabled",
            "未启用任何特性",
            "未啟用任何特性",
            "Особенности не включены");
        Log("world_generation_patch.feature_enabled",
            "'{0}' enabled",
            "已启用 '{0}'",
            "已啟用 '{0}'",
            "'{0}' включено");
        Log("world_generation_patch.feature_enabled_with_value",
            "'{0}' enabled: {1}",
            "已启用 '{0}': '{1}'",
            "已啟用 '{0}': '{1}'",
            "'{0}' включено: {1}");
        Log("world_generation_patch.unknown_feature",
            "Unknown feature: '{0}'",
            "未知特性: '{0}'",
            "未知特性: '{0}'",
            "Неизвестная особенность: '{0}'");
        Log("world_generation_patch.skip_generation",
            "Skipped '{0}' generation",
            "已跳过 '{0}' 生成",
            "已跳過 '{0}' 生成",
            "Пропущена генерация '{0}'");
        Log("world_generation_patch.phase.preparing",
            "Preparing Map: '{0}'...",
            "准备地图: '{0}'...",
            "準備地圖: '{0}'...",
            "Подготовка карты: '{0}'...");
        Log("world_generation_patch.phase.generating",
            "'{0}' - Generating world...",
            "'{0}' - 正在生成世界...",
            "'{0}' - 正在生成世界...",
            "'{0}' - Генерация мира...");
        Log("world_generation_patch.phase.skipping",
            "'{0}' - Skipped {1}",
            "'{0}' - 已跳过 '{1}'",
            "'{0}' - 已跳過 '{1}'",
            "'{0}' - Пропущено {1}");
        Log("world_generation_patch.phase.placing_blocks",
            "'{0}' - Placing blocks: {1} success, {2} failed / {3} total ({4}%)",
            "'{0}' - 放置方块: {1} 成功, {2} 失败 / {3} 共 ({4}%)",
            "'{0}' - 放置方塊: {1} 成功, {2} 失敗 / {3} 共 ({4}%)",
            "'{0}' - Размещение блоков: {1} успешно, {2} неудачно / {3} всего ({4}%)");
        Log("world_generation_patch.phase.spawning_map",
            "'{0}' - Spawning map...",
            "'{0}' - 正在生成地图...",
            "'{0}' - 正在生成地圖...",
            "'{0}' - Генерация карты...");
        Log("world_generation_patch.phase.spawning_custom_structures",
            "'{0}' - Spawning custom structures...",
            "'{0}' - 正在生成自定义结构...",
            "'{0}' - 正在生成自訂結構...",
            "'{0}' - Генерация пользовательских структур...");
        // Log("world_generation_patch.phase.spawning_build_mode_save",
        //     "'{0}' - Spawning build mode save...",
        //     "'{0}' - 正在生成建筑模式存档...",
        //     "'{0}' - 正在生成建築模式存檔...",
        //     "'{0}' - Генерация сохранения строительного режима...");
        Log("world_generation_patch.phase.applying_settings",
            "'{0}' - Applying settings...",
            "'{0}' - 正在应用设置...",
            "'{0}' - 正在應用設定...",
            "'{0}' - Применение настроек...");
        Log("world_generation_patch.loading_start",
            "Started loading Map: '{0}'",
            "开始加载地图: '{0}'",
            "開始載入地圖: '{0}'",
            "Начата загрузка карты: '{0}'");
        Log("world_generation_patch.exited_map",
            "Exited Map",
            "已退出地图",
            "已退出地圖",
            "Вышли с карты");
        Log("world_generation_patch.executing_command",
            "Executing '{0}': '{1}'",
            "执行 '{0}': '{1}'",
            "執行 '{0}': '{1}'",
            "Выполнение '{0}': '{1}'");
        Log("world_generation_patch.start_game_map",
            "Starting game with configured Map: '{0}' (ID: {1})",
            "开始游戏时自动使用配置的地图: '{0}' (ID: {1})",
            "開始遊戲時自動使用配置的地圖: '{0}' (ID: {1})",
            "Начало игры с настроенной картой: '{0}' (ID: {1})");
        Log("world_generation_patch.start_game_map_not_found",
            "Configured Map (ID: {0}) not found, using default",
            "未找到配置的地图 (ID: '{0}')，将使用默认",
            "未找到配置的地圖 (ID: '{0}')，將使用預設",
            "Настроенная карта (ID: {0}) не найдена, используется по умолчанию");
        Log("world_generation_patch.scan_maps_failed",
            "Failed to scan maps directory '{0}': {1}",
            "扫描地图目录 '{0}' 失败: '{1}'",
            "掃描地圖目錄 '{0}' 失敗: '{1}'",
            "Не удалось просканировать каталог карт '{0}': {1}");
        Log("world_generation_patch.no_map_selected",
            "No Map selected, generating vanilla world",
            "未选择地图，将生成原版世界",
            "未選擇地圖，將生成原版世界",
            "Карта не выбрана, генерируется ванильный мир");
        Log("world_generation_patch.no_valid_directories",
            "No valid Map directories, please check the Maps folder",
            "没有有效的地图目录，请检查 Maps 文件夹",
            "沒有有效的地圖目錄，請檢查 Maps 資料夾",
            "Нет допустимых каталогов карт, проверьте папку Maps");
        Log("world_generation_patch.applying_settings_overrides",
            "Applying settings overrides, count={0}",
            "正在应用设置覆盖, 数量={0}",
            "正在應用設定覆蓋, 數量={0}",
            "Применение переопределений настроек, количество={0}");
        Log("world_generation_patch.settings_override_not_found",
            "Settings override not found: '{0}'",
            "未找到设置覆盖: '{0}'",
            "未找到設定覆蓋: '{0}'",
            "Переопределение настроек не найдено: '{0}'");
        Log("world_generation_patch.settings_override_applied",
            "Applied settings override: '{0}' = {1}",
            "已应用设置覆盖: '{0}' = '{1}'",
            "已應用設定覆蓋: '{0}' = '{1}'",
            "Применено переопределение настроек: '{0}' = {1}");
        Log("world_generation_patch.settings_override_failed",
            "Failed to apply settings override: '{0}'",
            "应用设置覆盖失败: '{0}'",
            "應用設定覆蓋失敗: '{0}'",
            "Не удалось применить переопределение настроек: '{0}'");
        Log("world_generation_patch.map",
            "Map",
            "地图",
            "地圖",
            "Карта");
        Log("world_generation_patch.item",
            "Item",
            "物品",
            "物品",
            "Предмет");
        Log("world_generation_patch.block",
            "Block",
            "方块",
            "方塊",
            "Блок");
        Log("world_generation_patch.terrain",
            "Terrain",
            "地形",
            "地形",
            "Террейн");
        Log("world_generation_patch.structure",
            "Structure",
            "结构",
            "結構",
            "Структура");
        Log("world_generation_patch.background",
            "Background",
            "背景",
            "背景",
            "Фон");
        Log("world_generation_patch.startup_command",
            "Startup commands",
            "启动命令",
            "啟動命令",
            "Команды запуска");
        Log("world_generation_patch.loop_command",
            "Loop command",
            "循环命令",
            "迴圈命令",
            "Команда цикла");

        // Log - Map Loader
        Log("map_loader.load_success",
            "Successfully loaded map: start position({0}, {1}), size({2}x{3})",
            "成功加载地图: 起始位置({0}, {1})，大小({2}x{3})",
            "成功載入地圖: 起始位置({0}, {1})，大小({2}x{3})",
            "Карта успешно загружена: начальная позиция({0}, {1}), размер({2}x{3})");
        Log("map_loader.load_failed",
            "Failed to load map: '{0}'",
            "加载地图失败: '{0}'",
            "載入地圖失敗: '{0}'",
            "Не удалось загрузить карту: '{0}'");
        Log("map_loader.reload_failed",
            "Failed to reload map: '{0}'",
            "重新加载地图失败: '{0}'",
            "重新載入地圖失敗: '{0}'",
            "Не удалось перезагрузить карту: '{0}'");
        Log("map_loader.scene_reloading",
            "Reloading scene: '{0}'",
            "正在重新加载场景: '{0}'",
            "正在重新載入場景: '{0}'",
            "Перезагрузка сцены: '{0}'");
        Log("map_loader.scene_reloaded",
            "Scene reloaded",
            "场景已重新加载",
            "場景已重新載入",
            "Сцена перезагружена");
        Log("map_loader.scene_reload_failed",
            "Failed to reload scene: '{0}'",
            "重新加载场景失败: '{0}'",
            "重新載入場景失敗: '{0}'",
            "Не удалось перезагрузить сцену: '{0}'");
        Log("map_loader.no_current_map",
            "No current Map configuration loaded",
            "当前没有加载的地图配置",
            "當前沒有載入的地圖配置",
            "Текущая конфигурация карты не загружена");
        Log("map_loader.custom_structures_not_supported",
            "Custom structures are not supported for map loading: '{0}'",
            "地图加载不支持自定义结构: '{0}'",
            "地圖載入不支援自訂結構: '{0}'",
            "Пользовательские структуры не поддерживаются для загрузки карты: '{0}'");
        Log("map_loader.no_features_enabled",
            "No features enabled",
            "未启用任何特性",
            "未啟用任何特性",
            "Особенности не включены");
        Log("map_loader.feature_enabled",
            "'{0}' enabled",
            "已启用 '{0}'",
            "已啟用 '{0}'",
            "'{0}' включено");
        Log("map_loader.feature_enabled_with_value",
            "'{0}' enabled: {1}",
            "已启用 '{0}': '{1}'",
            "已啟用 '{0}': '{1}'",
            "'{0}' включено: {1}");
        Log("map_loader.skip_generation",
            "Skipped '{0}' generation",
            "已跳过 '{0}' 生成",
            "已跳過 '{0}' 生成",
            "Пропущена генерация '{0}'");
        Log("map_loader.no_directory_path",
            "Map directory path is null or empty",
            "地图目录路径为空",
            "地圖目錄路徑為空",
            "Путь к каталогу карты пуст");
        Log("map_loader.map_json_not_found",
            "map.json not found in: '{0}'",
            "在 '{0}' 中找不到 map.json",
            "在 '{0}' 中找不到 map.json",
            "map.json не найден в: '{0}'");
        Log("map_loader.map_deserialize_failed",
            "Failed to deserialize Map from disk",
            "反序列化地图失败",
            "反序列化地圖失敗",
            "Не удалось десериализовать карту с диска");
        Log("map_loader.map_reloaded_from_disk",
            "Reloaded Map from disk: '{0}'",
            "已从磁盘重新加载地图: '{0}'",
            "已從磁碟重新載入地圖: '{0}'",
            "Карта перезагружена с диска: '{0}'");
        Log("map_loader.map_reload_failed",
            "Failed to reload Map from disk: '{0}'",
            "重新加载地图失败: '{0}'",
            "重新載入地圖失敗: '{0}'",
            "Не удалось перезагрузить карту с диска: '{0}'");
        Log("map_loader.validation.no_data",
            "No '{1}' data in '{0}'",
            "'{0}' 中没有 '{1}' 数据",
            "'{0}' 中沒有 '{1}' 資料",
            "Нет данных '{1}' в '{0}'");
        Log("map_loader.validation.row_data_empty",
            "'{0}' row data is empty",
            "'{0}' 行数据为空",
            "'{0}' 行資料為空",
            "Данные строки '{0}' пусты");
        
        // Log - Map Load
        Log("map_load.empty_target_path",
            "Target path cannot be null or empty",
            "目标路径不能为空",
            "目標路徑不能為空",
            "Целевой путь не может быть пустым");
        Log("map_load.unauthorized",
            "No permission to read file '{0}': {1}",
            "没有权限读取文件 '{0}': {1}",
            "沒有權限讀取檔案 '{0}': {1}",
            "Нет разрешения на чтение файла '{0}': {1}");
        Log("map_load.io_error",
            "Failed to read file '{0}': {1}",
            "读取文件 '{0}' 失败: {1}",
            "讀取檔案 '{0}' 失敗: {1}",
            "Не удалось прочитать файл '{0}': {1}");
        Log("map_load.file_empty",
            "File '{0}' is empty, will create default configuration",
            "文件 '{0}' 为空，将创建默认配置",
            "檔案 '{0}' 為空，將建立預設配置",
            "Файл '{0}' пуст, будет создана конфигурация по умолчанию");
        Log("map_load.deserialize_null",
            "File '{0}' deserialization returned null, will create default configuration",
            "文件 '{0}' 反序列化返回 null，将创建默认配置",
            "檔案 '{0}' 反序列化返回 null，將建立預設配置",
            "Десериализация файла '{0}' вернула null, будет создана конфигурация по умолчанию");
        Log("map_load.invalid_json",
            "File '{0}' has invalid JSON format: {1}",
            "文件 '{0}' JSON 格式无效: {1}",
            "檔案 '{0}' JSON 格式無效: {1}",
            "Файл '{0}' имеет неверный формат JSON: {1}");
        Log("map_load.no_folder_name",
            "Could not resolve a valid folder name from path '{0}'",
            "无法从路径 '{0}' 解析有效的文件夹名称",
            "無法從路徑 '{0}' 解析有效的資料夾名稱",
            "Не удалось определить допустимое имя папки из пути '{0}'");

        // Log - Mod Command
        Log("mod_command.no_waypoints",
            "No waypoints defined in current Map",
            "当前地图未定义路径点",
            "當前地圖未定義路徑點",
            "Путевые точки не определены в текущей карте");
        Log("mod_command.exit_no_target",
            "Please specify exit target: none (vanilla) or tutorial",
            "请指定退出目标: 'none' (普通) 或 'tutorial' (教程)",
            "請指定退出目標: 'none' (普通) 或 'tutorial' (教程)",
            "Укажите цель выхода: none (ваниль) или tutorial");
        Log("mod_command.register_failed",
            "Failed to register custom commands: {0}\n{1}",
            "注册自定义命令失败: {0}\n{1}",
            "註冊自訂命令失敗: {0}\n{1}",
            "Не удалось зарегистрировать пользовательские команды: {0}\n{1}");
        Log("mod_command.no_map",
            "No Map available",
            "没有可用的地图",
            "沒有可用的地圖",
            "Нет доступных карт");

        // Log - Custom Structures Loader
        Log("custom_structures_loader.loading",
            "Loading custom structure: '{0}'",
            "正在加载自定义结构: '{0}'",
            "正在載入自訂結構: '{0}'",
            "Загрузка пользовательской структуры: '{0}'");
        Log("custom_structures_loader.failed",
            "Failed to load custom structure ({0}): {1}",
            "加载自定义结构失败 ({0}): {1}",
            "載入自訂結構失敗 ({0}): {1}",
            "Не удалось загрузить пользовательскую структуру ({0}): {1}");
        Log("custom_structures_loader.not_found_custom_structures",
            "Custom structure file not found",
            "未找到自定义结构文件",
            "未找到自訂結構檔案",
            "Файл пользовательской структуры не найден");
        Log("custom_structures_loader.suppress.cleared_definitions",
            "Suppressed Custom Structures auto-generation (cleared StructureDefinitions)",
            "已抑制自定义结构自动生成 (已清除 StructureDefinitions)",
            "已抑制自訂結構自動生成 (已清除 StructureDefinitions)",
            "Подавлена автоматическая генерация пользовательских структур (очищены StructureDefinitions)");
        Log("custom_structures_loader.structure_registry.registered",
            "Structure '{0}' registered via StructureRegistry",
            "结构 '{0}' 已通过 StructureRegistry 注册",
            "結構 '{0}' 已透過 StructureRegistry 註冊",
            "Структура '{0}' зарегистрирована через StructureRegistry");

        // Log - Build Mode Save Loader
        // Log("build_mode_save_loader.loading",
        //     "Loading Build Mode save: '{0}' (blocks: {1}, liquids: {2}, backgrounds: {3})",
        //     "正在加载建筑模式存档: '{0}' (方块: {1}, 液体: {2}, 背景: {3})",
        //     "正在載入建築模式存檔: '{0}' (方塊: {1}, 液體: {2}, 背景: {3})",
        //     "Загрузка сохранения строительного режима: '{0}' (блоки: {1}, жидкости: {2}, фоны: {3})");
        // Log("build_mode_save_loader.failed",
        //     "Failed to load Build Mode save ({0}): {1}",
        //     "加载建筑模式存档失败 ({0}): {1}",
        //     "載入建築模式存檔失敗 ({0}): {1}",
        //     "Не удалось загрузить сохранение строительного режима ({0}): {1}");
        // Log("build_mode_save_loader.not_found_save",
        //     "Build Mode save file not found",
        //     "未找到建筑模式存档文件",
        //     "未找到建築模式存檔檔案",
        //     "Файл сохранения строительного режима не найден");

        // Log - Map Loader (Build Mode)
        // Log("map_loader.build_mode_save_applied",
        //     "Build Mode save applied: '{0}' blocks, {1} liquids, {2} backgrounds, {3} failed",
        //     "建筑模式存档已应用: '{0}' 方块, {1} 液体, {2} 背景, {3} 失败",
        //     "建築模式存檔已應用: '{0}' 方塊, {1} 液體, {2} 背景, {3} 失敗",
        //     "Сохранение строительного режима применено: '{0}' блоков, {1} жидкостей, {2} фонов, {3} неудачно");
        // Log("map_loader.not_found_buildmode_save",
        //     "Build Mode save file not found",
        //     "未找到建筑模式存档文件",
        //     "未找到建築模式存檔檔案",
        //     "Файл сохранения строительного режима не найден");
        // Log("map_loader.build_mode_save_invalid_size",
        //     "Build Mode save has invalid dimensions ({0}x{1}), file may be corrupted",
        //     "建筑模式存档尺寸无效 ({0}x{1})，文件可能已损坏",
        //     "建築模式存檔尺寸無效 ({0}x{1})，檔案可能已損壞",
        //     "Сохранение строительного режима имеет неверные размеры ({0}x{1}), файл может быть поврежден");

        // Log - Map Directory Loader
        Log("loader.directory_not_found",
            "Directory not found: '{0}'",
            "目录未找到: '{0}'",
            "目錄未找到: '{0}'",
            "Каталог не найден: '{0}'");
        Log("loader.map_json_not_found",
            "map.json not found in: '{0}'",
            "在 '{0}' 中找不到 map.json",
            "在 '{0}' 中找不到 map.json",
            "map.json не найден в: '{0}'");
        Log("loader.map_json_failed",
            "Failed to deserialize map.json: '{0}'",
            "反序列化 map.json 失败: '{0}'",
            "反序列化 map.json 失敗: '{0}'",
            "Не удалось десериализовать map.json: '{0}'");
        Log("loader.success",
            "Successfully loaded Map: '{0}' (ID: {1}, Version: {2})",
            "成功加载地图: '{0}' (ID: {1}, 版本: {2})",
            "成功載入地圖: '{0}' (ID: {1}, 版本: {2})",
            "Карта успешно загружена: '{0}' (ID: {1}, Версия: {2})");
        Log("loader.failed",
            "Failed to load Map from '{0}': '{1}'",
            "从 '{0}' 加载地图失败: {1}",
            "從 '{0}' 載入地圖失敗: {1}",
            "Не удалось загрузить карту из '{0}': '{1}'");
        Log("loader.level_dir_not_found",
            "Level directory not found: '{0}'",
            "关卡目录未找到: '{0}'",
            "關卡目錄未找到: '{0}'",
            "Каталог уровня не найден: '{0}'");
        Log("loader.no_level_files",
            "No level files found in: '{0}'",
            "在 '{0}' 中找不到关卡文件",
            "在 '{0}' 中找不到關卡檔案",
            "Файлы уровней не найдены в: '{0}'");
        Log("loader.loaded_level",
            "Loaded level: '{0}'",
            "已加载关卡: '{0}'",
            "已載入關卡: '{0}'",
            "Уровень загружен: '{0}'");
        Log("loader.failed_to_load_level",
            "Failed to load level file '{0}': '{1}'",
            "加载关卡文件 '{0}' 失败: {1}",
            "載入關卡檔案 '{0}' 失敗: {1}",
            "Не удалось загрузить файл уровня '{0}': '{1}'");
        Log("loader.no_world_settings",
            "No world settings found, using defaults",
            "未找到世界设置，使用默认值",
            "未找到世界設定，使用預設值",
            "Настройки мира не найдены, используются значения по умолчанию");
        Log("loader.missing_type",
            "Missing 'type' property in {0}, expected '{1}'",
            "'{0}' 中缺少 'type' 属性，期望 '{1}'",
            "'{0}' 中缺少 'type' 屬性，期望 '{1}'",
            "Отсутствует свойство 'type' в {0}, ожидалось '{1}'");
        Log("loader.type_mismatch",
            "Type mismatch in {0}: expected '{1}', got '{2}'",
            "'{0}' 中类型不匹配: 期望 '{1}'，实际 '{2}'",
            "'{0}' 中類型不匹配: 期望 '{1}'，實際 '{2}'",
            "Несоответствие типа в {0}: ожидалось '{1}', получено '{2}'");
        Log("loader.failed_to_load_file",
            "Failed to load '{0}': '{1}'",
            "加载 '{0}' 失败: {1}",
            "載入 '{0}' 失敗: {1}",
            "Не удалось загрузить '{0}': '{1}'");
    }
}