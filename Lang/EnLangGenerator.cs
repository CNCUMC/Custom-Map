using Bark.Base;

namespace CustomMap.Lang;

public class EnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "EN";

    protected override void BuildLocaleData()
    {
        // Config - Settings labels and descriptions
        Option("custommap.custommap.more_logs", "More logs", "Display more logs");
        Option("custommap.custommap.start_game_use_map", "Start game use map",
            "Use the selected Map when starting a new game.");
        Option("custommap.custommap.first_use_map", "First use map",
            "The Map ID to use when starting a new game. Requires 'Start Use Map' to be enabled.");
        Option("custommap.custommap.progress_update_interval", "Progress update interval",
            "Number of blocks between progress text updates during map generation. Lower values update more frequently but may impact performance.");

        // Map Format
        Other("format.author", "by {0}");
        Other("format.features", "Features: {0}");

        // Feature
        Feature("full_bright", "Full Bright");
        Feature("forgiving_level", "Forgiving Level");
        Feature("gravity", "Gravity");
        Feature("jump_limit", "Jump Limit");
        Feature("climb_limit", "Climb Limit");
        Feature("world_settings_data", "World Settings");
        Feature("skip_terrain", "Skip Terrain");
        Feature("skip_structures", "Skip Structures");
        Feature("skip_background", "Skip Background");
        Feature("mine_data", "Mine");
        Feature("jump_pad_data", "Jump Pad");
        Feature("turret_data", "Turret");
        Feature("sound_cannon_data", "Sound Cannon");
        Feature("spike_stabber_data", "Spike Stabber");
        Feature("geyser_data", "Geyser");
        Feature("beartrap_data", "Bear Trap");
        Feature("xp_data", "XP");

        // Feature - Child properties
        Feature("mine.undestroy", "Undestroy");
        Feature("mine.cooldown", "Cooldown");
        Feature("jump_pad.cooldown", "Cooldown");
        Feature("jump_pad.force", "Force");
        Feature("jump_pad.no_light", "No Light");
        Feature("turret.cooldown", "Cooldown");
        Feature("turret.shot_power_multiplier", "Shot Power");
        Feature("turret.undestroy", "Undestroy");
        Feature("turret.no_light", "No Light");
        Feature("turret.range", "Range");
        Feature("sound_cannon.cooldown", "Cooldown");
        Feature("sound_cannon.max_distance", "Range");
        Feature("sound_cannon.charge_time", "Charge Time");
        Feature("sound_cannon.undestroy", "Undestroy");
        Feature("spike_stabber.damage_mult", "Damage");
        Feature("spike_stabber.undestroy", "Undestroy");
        Feature("spike_stabber.cooldown", "Cooldown");
        Feature("spike_stabber.no_light", "No Light");
        Feature("spike_stabber.sound", "Sound");
        Feature("geyser.cooldown", "Cooldown");
        Feature("geyser.activate_duration", "Duration");
        Feature("geyser.no_liquid", "No Liquid");
        Feature("geyser.rumble_time", "Rumble Time");
        Feature("geyser.range", "Range");
        Feature("beartrap.damage_mult", "Damage");
        Feature("beartrap.undestroy", "Undestroy");
        Feature("beartrap.cooldown", "Cooldown");

        // XP Child properties
        Feature("xp.str_xp", "Strength Level");
        Feature("xp.res_xp", "Resilience Level");
        Feature("xp.int_xp", "Intelligence Level");
        Feature("xp.exp_str", "Strength EXP");
        Feature("xp.exp_res", "Resilience EXP");
        Feature("xp.exp_int", "Intelligence EXP");
        Feature("xp.min_str", "Min Strength EXP");
        Feature("xp.max_str", "Max Strength EXP");
        Feature("xp.min_res", "Min Resilience EXP");
        Feature("xp.max_res", "Max Resilience EXP");
        Feature("xp.min_int", "Min Intelligence EXP");
        Feature("xp.max_int", "Max Intelligence EXP");

        // Command - Map
        Command("custommap", "Map related commands");
        Command("custommap.string", "Select function");
        Command("custommap.parameter", "Function parameter");
        Command("custommap.help.header", "Available subcommands:");
        Command("custommap.help.help", "Show this help message");
        Command("custommap.help.reload", "Reload current map");
        Command("custommap.help.load", "Reload Maps folder");
        Command("custommap.help.savereload", "Save and reload current map");
        Command("custommap.load.success", "Maps reloaded successfully");
        Command("custommap.help.info", "Show map info");
        Command("custommap.help.spawn", "Teleport to spawn");
        Command("custommap.help.select", "Select a Map");
        Command("custommap.help.list", "List all Maps");
        Command("custommap.help.feature", "Manage features");
        Command("custommap.help.waypoint", "Manage waypoints (list/get)");
        Command("custommap.help.save", "Save current Map");
        Command("custommap.help.level", "Switch map level");
        Command("custommap.help.exit", "Exit Map");

        // Command - Map - Info
        Command("custommap.info.name", "Name: {0}");
        Command("custommap.info.id", "ID: {0}");
        Command("custommap.info.version", "Version: {0}");
        Command("custommap.info.authors", "Authors: {0}");
        Command("custommap.info.description", "Description: {0}");
        Command("custommap.info.features", "Features: {0}");
        Command("custommap.info.spawn", "Spawn point: {0}");

        // Command - Map - Spawn
        Command("custommap.spawn", "Teleporting back to the spawn point {0} now...");

        // Command - Map - Waypoint
        Command("custommap.waypoint.help", "Waypoint subcommands: list, get <id or name>");
        Command("custommap.waypoint.list_header", "Available waypoints ({0}):");
        Command("custommap.waypoint.list_item", "  {0}. {1} - Position: {2}");
        Command("custommap.waypoint.teleport", "Teleporting to waypoint '{0}' at position {1}...");
        Command("custommap.waypoint.not_found", "Waypoint not found: {0}");
        Command("custommap.waypoint.invalid_index", "Invalid index {0}, please enter a number between 1 and {1}");
        Command("custommap.waypoint.get_no_id", "Please specify a waypoint ID or index to teleport to");
        Command("custommap.waypoint.unknown_subcommand", "Unknown waypoint subcommand: {0}");

        // Command - Map - List
        Command("custommap.list.header", "Loaded {0} Map(s):");
        Command("custommap.list.item", "{0}[{1}] {2} (ID: {3}, Version: {4}, Authors: {5})");
        Command("custommap.list.empty", "No Maps available");

        // Command - Map - Select
        Command("custommap.select.no_key", "Please provide a Map ID or name to select");
        Command("custommap.select.not_found", "Map not found: {0}");
        Command("custommap.select.success", "Selected {0} (ID: {1})");
        Command("custommap.select.without_world",
            "Selected {0}, but world is not loaded. Map will be loaded when you start a game.");
        Command("custommap.select.invalid_index", "Invalid index {0}, please enter a number between 1 and {1}");

        // Command - Map - Layer
        Command("custommap.layer.current", "Current layer: {0}/{1}");
        Command("custommap.layer.switched", "Switched to layer {0}");
        Command("custommap.layer.already", "Already on layer {0}");
        Command("custommap.layer.invalid", "Invalid layer, please enter a number between 1 and {0}");
        Command("custommap.layer.no_layers", "No layers available for this map");

        // Command - Map - Config
        Command("custommap.config.set_missing_params", "Please specify configuration name and value to set");
        Command("custommap.config.list_header", "Current configuration settings:");
        Command("custommap.config.set_success", "Configuration '{0}' has been set to {1}");
        Command("custommap.config.set_failed", "Failed to set configuration '{0}': {1}");
        Command("custommap.config.not_found", "Configuration not found: {0}");
        Command("custommap.config.invalid_value", "The value of {0} is invalid: {1}");

        // Command - Map - Exit
        Command("custommap.exiting", "Returning to {0}...");
        Command("custommap.exit.invalid_target", "Unknown exit target: {0}, available: none, tutorial");
        Command("custommap.exit.target.none", "vanilla game");
        Command("custommap.exit.target.tutorial", "tutorial");

        // Command - Map - Save
        Command("custommap.save.success", "Map '{0}' saved to: {1}");
        Command("custommap.save.failed", "Failed to save Map '{0}': {1}");
        Command("custommap.save.no_directory", "Current Map has no associated directory path, cannot save");
        Command("custommap.save.target_not_found", "Target Map folder '{0}' not found");

        // Command - Map - Feature
        Command("custommap.feature.unknown_subcommand", "Unknown feature subcommand: {0}");
        Command("custommap.feature.set_missing_params", "Please specify feature name and value to set");
        Command("custommap.feature.list_header", "Current feature settings:");
        Command("custommap.feature.get_success", "Feature '{0}' = {1}");
        Command("custommap.feature.set_success", "Feature '{0}' set to {1}");
        Command("custommap.feature.not_found", "Feature not found: {0}");
        Command("custommap.feature.invalid_value", "Invalid value for {0}: {1}");

        // Log - Map Check
        Log("map_check.id_format_warning", "ID format is incorrect, will be automatically corrected");
        Log("map_check.author_not_string", "Author element {0} is not a string, removed");
        Log("map_check.author_empty", "Author array is empty, set default value");
        Log("map_check.version_format_warning",
            "Version format '{0}' is incorrect, will use default version '1.0.0'");
        Log("map_check.missing_build_mode_mod", "Map '{0}' requires Build Mode mod but it is not installed");
        Log("map_check.requires_mod", "[requires {0}]");

        // Log - World Generation
        Log("world_generation.scene_type_set", "Set scene type to: {0}");
        Log("world_generation.no_features_enabled", "No features enabled");
        Log("world_generation.feature_enabled", "{0} enabled");
        Log("world_generation.feature_enabled_with_value", "{0} enabled: {1}");
        Log("world_generation.unknown_feature", "Unknown feature: {0}");
        Log("world_generation.skip_generation", "Skipped {0} generation");
        Log("world_generation.phase.preparing", "Preparing Map: {0}...");
        Log("world_generation.phase.generating", "{0} - Generating world...");
        Log("world_generation.phase.skipping", "{0} - Skipped {1}");
        Log("world_generation.phase.placing_blocks",
            "{0} - Placing blocks: {1} success, {2} failed / {3} total ({4}%)");
        Log("world_generation.phase.spawning_map", "{0} - Spawning map...");
        Log("world_generation.phase.spawning_custom_structures", "{0} - Spawning custom structures...");
        Log("world_generation.phase.spawning_build_mode_save", "{0} - Spawning build mode save...");
        Log("world_generation.phase.applying_settings", "{0} - Applying settings...");
        Log("world_generation.loading_start", "Started loading Map: {0}");
        Log("world_generation.no_content_type",
            "Map '{0}' has no content type defined (Structures, BuildModeSave, or Items)");
        Log("world_generation.no_commands", "No {0} enabled");
        Log("world_generation.exited_map", "Exited Map");
        Log("world_generation.executing_command", "Executing {0}: {1}");
        Log("world_generation.executing_loop_command", "Executing loop {0}: {1}");
        Log("world_generation.start_game_map", "Starting game with configured Map: {0} (ID: {1})");
        Log("world_generation.start_game_map_not_found",
            "Configured Map (ID: {0}) not found, using default");
        Log("world_generation.scan_maps_failed", "Failed to scan maps directory '{0}': {1}");
        Log("world_generation.no_map_selected", "No Map selected, generating vanilla world");
        Log("world_generation.no_valid_directories",
            "No valid Map directories, please check the Maps folder");
        Log("world_generation.applying_settings_overrides", "Applying settings overrides, count={0}");
        Log("world_generation.settings_override_not_found", "Settings override not found: {0}");
        Log("world_generation.settings_override_applied", "Applied settings override: {0} = {1}");
        Log("world_generation.settings_override_failed", "Failed to apply settings override: {0}");

        // Log - Validation
        Log("validation.multiple_content_types",
            "Cannot use multiple content types (Structures, BuildModeSave) at the same time, only one is allowed");
        Log("validation.missing_content_type",
            "Missing content type (Structures, or BuildModeSave)");

        // Log - Map Loader
        Log("map_loader.load_error", "Map or map data is null");
        Log("map_loader.invalid_format", "Invalid map format, missing map field");
        Log("map_loader.key_missing", "Error: String map format missing 'key' definition");
        Log("map_loader.string_map_applied", "String map applied, {0} successes, {1} failures");
        Log("map_loader.load_success", "Successfully loaded map: start position({0}, {1}), size({2}x{3})");
        Log("map_loader.load_failed", "Failed to load map: {0}");
        Log("map_loader.place_failed", "Failed to place {2} {3} at ({0}, {1}): {4}");
        Log("map_loader.multiple_blocks_in_list",
            "Multiple blocks detected in list at ({0}, {1}), only the first one will be generated");
        Log("map_loader.unsupported_value_type", "Unsupported value type: {0}, position ({1}, {2})");
        Log("map_loader.nested_structure_not_supported", "Nested structure not supported, position ({0}, {1})");
        Log("map_loader.unexpected_token_type", "Unexpected token type: {0}, position ({1}, {2})");
        Log("map_loader.reload_success", "Successfully reloaded map: {0}");
        Log("map_loader.reload_failed", "Failed to reload map: {0}");
        Log("map_loader.restarting_scene", "Restarting scene...");
        Log("map_loader.scene_reloading", "Reloading scene: {0}");
        Log("map_loader.scene_reloaded", "Scene reloaded");
        Log("map_loader.scene_reload_failed", "Failed to reload scene: {0}");
        Log("map_loader.no_current_map", "No current Map configuration loaded");
        Log("map_loader.custom_structures_not_supported",
            "Custom structures are not supported for map loading: {0}");
        Log("map_loader.no_features_enabled", "No features enabled");
        Log("map_loader.feature_enabled", "{0} enabled");
        Log("map_loader.feature_enabled_with_value", "{0} enabled: {1}");
        Log("map_loader.skip_generation", "Skipped {0} generation");
        Log("map_loader.no_directory_path", "Map directory path is null or empty");
        Log("map_loader.map_json_not_found", "map.json not found in: {0}");
        Log("map_loader.map_deserialize_failed", "Failed to deserialize Map from disk");
        Log("map_loader.map_reloaded_from_disk", "Reloaded Map from disk: {0}");
        Log("map_loader.map_reload_failed", "Failed to reload Map from disk: {0}");
        Log("map_loader.validation.no_data", "No {1} data in {0}");
        Log("map_loader.validation.row_data_empty", "{0} row data is empty");

        // Log - Error
        Log("error.no_map_file", "Cannot find map.json file: {0}");
        Log("error.no_valid_directories", "No valid Map directories, please check the Maps folder");
        Log("error.multiple_content_types",
            "Map '{0}' has multiple content types defined (Structures, BuildModeSave). Only one type is allowed.");

        // Log - Map Load
        Log("map_load.empty_target_path", "Target path cannot be null or empty");
        Log("map_load.unauthorized", "No permission to read file '{0}': {1}");
        Log("map_load.io_error", "Failed to read file '{0}': {1}");
        Log("map_load.file_empty", "File '{0}' is empty, will create default configuration");
        Log("map_load.deserialize_null",
            "File '{0}' deserialization returned null, will create default configuration");
        Log("map_load.invalid_json", "File '{0}' has invalid JSON format: {1}");
        Log("map_load.no_folder_name", "Could not resolve a valid folder name from path '{0}'");

        // Log - Common
        Log("common.map", "Map");
        Log("common.item", "Item");
        Log("common.block", "Block");
        Log("common.terrain", "Terrain");
        Log("common.structure", "Structure");
        Log("common.background", "Background");
        Log("common.startup_command", "Startup commands");
        Log("common.loop_command", "Loop command");

        // Log - Mod Command
        Log("mod_command.empty_type", "Unknown command type");
        Log("mod_command.world_not_loaded", "World not loaded");
        Log("mod_command.no_waypoints", "No waypoints defined in current Map");
        Log("mod_command.exit_no_target", "Please specify exit target: none (vanilla) or tutorial");
        Log("mod_command.register_failed", "Failed to register custom commands: {0}\n{1}");
        Log("mod_command.no_map", "No Map available");

        // Log - Custom Structures Loader
        Log("custom_structures_loader.loading", "Loading custom structure: {0}");
        Log("custom_structures_loader.failed", "Failed to load custom structure ({0}): {1}");
        Log("custom_structures_loader.not_found_custom_structures", "Custom structure file not found");
        Log("custom_structures_loader.suppress.cleared_definitions",
            "Suppressed Custom Structures auto-generation (cleared StructureDefinitions)");
        Log("custom_structures_loader.structure_registry.registered",
            "Structure '{0}' registered via StructureRegistry");

        // Log - Build Mode Save Loader
        Log("build_mode_save_loader.loading",
            "Loading Build Mode save: {0} (blocks: {1}, liquids: {2}, backgrounds: {3})");
        Log("build_mode_save_loader.failed", "Failed to load Build Mode save ({0}): {1}");
        Log("build_mode_save_loader.not_found_buildmode_save", "Build Mode save file not found");
        Log("build_mode_save_loader.bg_sprite_missing", "Background sprite not found: {0}");

        // Log - Map Loader (Build Mode)
        Log("map_loader.build_mode_save_applied",
            "Build Mode save applied: {0} blocks, {1} liquids, {2} backgrounds, {3} failed");
        Log("map_loader.not_found_buildmode_save", "Build Mode save file not found");
        Log("map_loader.build_mode_save_invalid_size",
            "Build Mode save has invalid dimensions ({0}x{1}), file may be corrupted");

        // Log - Map Directory Loader
        Log("loader.directory_not_found", "Directory not found: {0}");
        Log("loader.map_json_not_found", "map.json not found in: {0}");
        Log("loader.map_json_failed", "Failed to deserialize map.json: {0}");
        Log("loader.success", "Successfully loaded Map: {0} (ID: {1}, Version: {2})");
        Log("loader.failed", "Failed to load Map from {0}: {1}");
        Log("loader.level_dir_not_found", "Level directory not found: {0}");
        Log("loader.no_level_files", "No level files found in: {0}");
        Log("loader.loaded_level", "Loaded level: {0}");
        Log("loader.failed_to_load_level", "Failed to load level file {0}: {1}");
        Log("loader.no_world_settings", "No world settings found, using defaults");
        Log("loader.missing_type", "Missing 'type' property in {0}, expected '{1}'");
        Log("loader.type_mismatch", "Type mismatch in {0}: expected '{1}', got '{2}'");
        Log("loader.failed_to_load_file", "Failed to load {0}: {1}");
    }

    private void Feature(string key, string value)
    {
        Other($"feature.{key}", value);
    }
}
