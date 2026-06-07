using MossLib.Base;

namespace CustomFungamePack.Lang
{
    public class EnLangGenerator : ModLangGenBase
    {
        protected override string LanguageCode => "EN";

        protected override void BuildLocaleData()
        {
            // Config
            Add("config.more_logs.name", "More logs");
            Add("config.more_logs.description", "Display more logs");
            Add("config.start_game_use_fungame.name", "Start game use fungame");
            Add("config.start_game_use_fungame.description", "Use the selected Fungame when starting a new game.");
            Add("config.first_use_fungame.name", "First use fungame");
            Add("config.first_use_fungame.description", "The Fungame ID to use when starting a new game. Requires 'Start Use Fungame' to be enabled.");
            Add("config.progress_update_interval.name", "Progress update interval");
            Add("config.progress_update_interval.description", "Number of blocks between progress text updates during map generation. Lower values update more frequently but may impact performance.");
            
            // Fungame Format
            Add("format.author", "by {0}");
            Add("format.features", "Features: {0}");
            
            // Feature
            Add("feature.full_bright", "Full Bright");
            Add("feature.forgiving_level", "Forgiving Level");
            Add("feature.gravity", "Gravity");
            Add("feature.jump_limit", "Jump Limit");
            Add("feature.climb_limit", "Climb Limit");
            Add("feature.world_settings_data", "World Settings");
            Add("feature.skip_terrain", "Skip Terrain");
            Add("feature.skip_structures", "Skip Structures");
            Add("feature.skip_background", "Skip Background");
            Add("feature.mine_data", "Mine");
            Add("feature.jump_pad_data", "Jump Pad");
            Add("feature.turret_data", "Turret");
            Add("feature.sound_cannon_data", "Sound Cannon");
            Add("feature.spike_stabber_data", "Spike Stabber");
            Add("feature.geyser_data", "Geyser");
            Add("feature.beartrap_data", "Bear Trap");

            // Feature - Child properties
            Add("feature.mine.undestroy", "Undestroy");
            Add("feature.mine.cooldown", "Cooldown");
            Add("feature.jump_pad.cooldown", "Cooldown");
            Add("feature.jump_pad.force", "Force");
            Add("feature.jump_pad.no_light", "No Light");
            Add("feature.turret.cooldown", "Cooldown");
            Add("feature.turret.shot_power_multiplier", "Shot Power");
            Add("feature.turret.undestroy", "Undestroy");
            Add("feature.turret.no_light", "No Light");
            Add("feature.turret.range", "Range");
            Add("feature.sound_cannon.cooldown", "Cooldown");
            Add("feature.sound_cannon.max_distance", "Range");
            Add("feature.sound_cannon.charge_time", "Charge Time");
            Add("feature.sound_cannon.undestroy", "Undestroy");
            Add("feature.spike_stabber.damage_mult", "Damage");
            Add("feature.spike_stabber.undestroy", "Undestroy");
            Add("feature.spike_stabber.cooldown", "Cooldown");
            Add("feature.spike_stabber.no_light", "No Light");
            Add("feature.spike_stabber.sound", "Sound");
            Add("feature.geyser.cooldown", "Cooldown");
            Add("feature.geyser.activate_duration", "Duration");
            Add("feature.geyser.no_liquid", "No Liquid");
            Add("feature.geyser.rumble_time", "Rumble Time");
            Add("feature.geyser.range", "Range");
            Add("feature.beartrap.damage_mult", "Damage");
            Add("feature.beartrap.undestroy", "Undestroy");
            Add("feature.beartrap.cooldown", "Cooldown");

            // Command - Fungame
            Add("command.fungame.description", "Fungame related commands");
            Add("command.fungame.string", "Select function");
            Add("command.fungame.parameter", "Function parameter");
            Add("command.fungame.help", "Available subcommands:\n  " +
                "reload  - Reload current map\n  " +
                "info    - Show map info\n  " +
                "spawn   - Teleport to spawn\n  " +
                "select  - Select a Fungame\n  " +
                "list    - List all Fungames\n  " +
                "feature - Manage features\n  " +
                "waypoint- Manage waypoints (list/get)\n  " +
                "save    - Save current Fungame to local file\n  " +
                "save as - Interactively select area and save as map data");

            // Command - Fungame - Info
            Add("command.fungame.info.name", "Name: {0}");
            Add("command.fungame.info.id", "ID: {0}");
            Add("command.fungame.info.version", "Version: {0}");
            Add("command.fungame.info.authors", "Authors: {0}");
            Add("command.fungame.info.description", "Description: {0}");
            Add("command.fungame.info.features", "Features: {0}");
            Add("command.fungame.info.spawn", "Spawn point: {0}");

            // Command - Fungame - Spawn
            Add("command.fungame.spawn", "Teleporting back to the spawn point {0} now...");

            // Command - Fungame - Waypoint
            Add("command.fungame.waypoint.help", "Waypoint subcommands:\n  " +
                "list - List all waypoints\n  " +
                "get <id or name> - Teleport to waypoint");
            Add("command.fungame.waypoint.list_header", "Available waypoints ({0}):");
            Add("command.fungame.waypoint.list_item", "  {0}. {1} - Position: {2}");
            Add("command.fungame.waypoint.teleport", "Teleporting to waypoint '{0}' at position {1}...");
            Add("command.fungame.waypoint.not_found", "Waypoint not found: {0}");
            Add("command.fungame.waypoint.invalid_index", "Invalid index {0}, please enter a number between 1 and {1}");
            Add("command.fungame.waypoint.get_no_id", "Please specify a waypoint ID or index to teleport to");
            Add("command.fungame.waypoint.unknown_subcommand", "Unknown waypoint subcommand: {0}");

            // Command - Fungame - List
            Add("command.fungame.list.header", "Loaded {0} Fungame(s):");
            Add("command.fungame.list.item", "{0}[{1}] {2} (ID: {3}, Version: {4}, Authors: {5})");
            Add("command.fungame.list.empty", "No Fungames available");

            // Command - Fungame - Select
            Add("command.fungame.select.no_key", "Please provide a Fungame ID or name to select");
            Add("command.fungame.select.not_found", "Fungame not found: {0}");
            Add("command.fungame.select.success", "Selected {0} (ID: {1})");
            Add("command.fungame.select.without_world", "Selected {0}, but world is not loaded. Map will be loaded when you start a game.");
            Add("command.fungame.select.invalid_index", "Invalid index {0}, please enter a number between 1 and {1}");
            
            // Command - Fungame - Config
            Add("command.fungame.config.set_missing_params", "Please specify configuration name and value to set");
            Add("command.fungame.config.list_header", "Current configuration settings:");
            Add("command.fungame.config.set_success", "Configuration '{0}' has been set to {1}");
            Add("command.fungame.config.set_failed", "Failed to set configuration '{0}': {1}");
            Add("command.fungame.config.not_found", "Configuration not found: {0}");
            Add("command.fungame.config.invalid_value", "The value of {0} is invalid: {1}");

            // Command - Fungame - Exit
            Add("command.fungame.exiting", "Returning to {0}...");
            Add("command.fungame.exit.invalid_target", "Unknown exit target: {0}, available: none, tutorial");
            Add("command.fungame.exit.target.none", "vanilla game");
            Add("command.fungame.exit.target.tutorial", "tutorial");

            // Command - Fungame - Save
            Add("command.fungame.save.success", "Fungame '{0}' saved to: {1}");
            Add("command.fungame.save.failed", "Failed to save Fungame '{0}': {1}");
            Add("command.fungame.save.no_directory", "Current Fungame has no associated directory path, cannot save");
            Add("command.fungame.save.invalid_position", "Invalid position format. Use Vector2 format: \"x,y\" (e.g. \"86,-11\")");
            Add("command.fungame.save.area_empty", "Specified area is outside world bounds or empty");
            Add("command.fungame.save.area_success", "Area block map saved: ({0},{1}) to ({2},{3}), size {4}x{5}, {6} block types → {7}");
            Add("command.fungame.save.target_not_found", "Target Fungame folder '{0}' not found");
            Add("command.fungame.save.missing_end_position", "Missing end position, please provide both positions (e.g. fg save 86,-11 2,45)");
            Add("command.fungame.save.as.default_description", "Saved from area scan");
            Add("command.fungame.save.as.start_position", "Left-click to select the start position...");
            Add("command.fungame.save.as.end_position", "Left-click to select the end position...");
            Add("command.fungame.save.as.confirm", "Position recorded. Re-enter the same command to confirm and save.");

            // Command - Fungame - Feature
            Add("command.fungame.feature.unknown_subcommand", "Unknown feature subcommand: {0}");
            Add("command.fungame.feature.set_missing_params", "Please specify feature name and value to set");
            Add("command.fungame.feature.list_header", "Current feature settings:");
            Add("command.fungame.feature.get_success", "Feature '{0}' = {1}");
            Add("command.fungame.feature.set_success", "Feature '{0}' set to {1}");
            Add("command.fungame.feature.not_found", "Feature not found: {0}");
            Add("command.fungame.feature.invalid_value", "Invalid value for {0}: {1}");

            // Log - Fungame Check
            Add("log.fungame_check.id_format_warning", "ID format is incorrect, will be automatically corrected");
            Add("log.fungame_check.author_not_string", "Author element {0} is not a string, removed");
            Add("log.fungame_check.author_empty", "Author array is empty, set default value");
            Add("log.fungame_check.version_format_warning", "Version format '{0}' is incorrect, will use default version '1.0.0'");

            // Log - World Generation
            Add("log.world_generation.scene_type_set", "Set scene type to: {0}");
            Add("log.world_generation.no_features_enabled", "No features enabled");
            Add("log.world_generation.feature_enabled", "{0} enabled");
            Add("log.world_generation.feature_enabled_with_value", "{0} enabled: {1}");
            Add("log.world_generation.unknown_feature", "Unknown feature: {0}");
            Add("log.world_generation.skip_generation", "Skipped {0} generation");
            Add("log.world_generation.phase.preparing", "Preparing Fungame: {0}...");
            Add("log.world_generation.phase.generating", "{0} - Generating world...");
            Add("log.world_generation.phase.skipping", "{0} - Skipped {1}");
            Add("log.world_generation.phase.placing_blocks", "{0} - Placing blocks: {1} success, {2} failed / {3} total ({4}%)");
            Add("log.world_generation.phase.spawning_map", "{0} - Spawning map...");
            Add("log.world_generation.phase.spawning_custom_structures", "{0} - Spawning custom structures...");
            Add("log.world_generation.phase.spawning_build_mode_save", "{0} - Spawning build mode save...");
            Add("log.world_generation.phase.applying_settings", "{0} - Applying settings...");
            Add("log.world_generation.loading_start", "Started loading Fungame: {0}");
            Add("log.world_generation.no_map_data", "Fungame {0} does not contain map data");
            Add("log.world_generation.no_content_type", "Fungame '{0}' has no content type defined (MapData, CustomStructures, or BuildModeSave)");
            Add("log.world_generation.no_commands", "No {0} enabled");
            Add("log.world_generation.exited_fungame", "Exited Fungame");
            Add("log.world_generation.executing_command", "Executing {0}: {1}");
            Add("log.world_generation.executing_loop_command", "Executing loop {0}: {1}");
            Add("log.world_generation.start_game_fungame", "Starting game with configured Fungame: {0} (ID: {1})");
            Add("log.world_generation.start_game_fungame_not_found", "Configured Fungame (ID: {0}) not found, using default");
            Add("log.world_generation.no_fungame_selected", "No Fungame selected, generating vanilla world");
            Add("log.world_generation.applying_settings_overrides", "Applying settings overrides, count={0}");
            Add("log.world_generation.settings_override_not_found", "Settings override not found: {0}");
            Add("log.world_generation.settings_override_applied", "Applied settings override: {0} = {1}");
            Add("log.world_generation.settings_override_failed", "Failed to apply settings override: {0}");

            // Log - Validation
            Add("log.validation.map_invalid_type", "map field format is incorrect");
            Add("log.validation.map_missing_field", "Map missing required field: {0}");
            Add("log.validation.map_field_type_error", "Map {0} field must be {1}");
            Add("log.validation.map_map_empty", "Map map array cannot be empty");
            Add("log.validation.map_row_not_string", "Map map row {0} must be a string");
            Add("log.validation.map_item_row_not_array", "Map items row {0} must be an array");
            Add("log.validation.map_item_not_string", "Map items[{0}][{1}] must be a string");
            Add("log.validation.multiple_content_types", "Cannot use multiple content types (map_data, custom_structures, build_mode_save) at the same time, only one is allowed");
            Add("log.validation.missing_content_type", "Missing content type (map_data, custom_structures, or build_mode_save)");
            Add("log.validation.custom_structures_without_mod", "Detected custom_structures field, but Custom Structures mod is not installed. Please install the mod first.");
            Add("log.validation.features_invalid_type", "features field must be an array or object");
            Add("log.validation.features_empty", "features array is empty, will be ignored");
            Add("log.validation.features_element_invalid", "features element {0} format is incorrect, skipped");
            Add("log.validation.no_data", "No {1} data in {0}");
            Add("log.validation.row_data_empty", "{0} row data is empty");
            Add("log.validation.field_missing_default", "Missing required field: {0}, using default value \"{1}\"");
            Add("log.validation.field_null_default", "Field is null: {0}, using default value \"{1}\"");
            Add("log.validation.field_empty_string_default", "Field is empty string: {0}, using default value \"{1}\"");
            Add("log.validation.field_must_be_array_default", "Missing required field: {0}, using default value [\"{1}\"]");
            Add("log.validation.field_null_array_default", "Field is null: {0}, using default value [\"{1}\"]");
            Add("log.validation.field_convert_to_array", "{0} field must be an array, converted to array");
            Add("log.validation.array_empty_default", "{0} array is empty, set default value");
            Add("log.validation.array_empty_removed", "{0} array is empty, removed");

            // Log - Map Loader
            Add("log.map_loader.load_error", "Fungame or map data is null");
            Add("log.map_loader.invalid_format", "Invalid map format, missing map field");
            Add("log.map_loader.key_missing", "Error: String map format missing 'key' definition");
            Add("log.map_loader.string_map_applied", "String map applied, placed {0} blocks, {1} items, {2} failed");
            Add("log.map_loader.load_success", "Successfully loaded map: start position({0}, {1}), size({2}x{3})");
            Add("log.map_loader.load_failed", "Failed to load map: {0}");
            Add("log.map_loader.place_failed", "Failed to place {2} {3} at ({0}, {1}): {4}");
            Add("log.map_loader.multiple_blocks_in_list", "Multiple blocks detected in list at ({0}, {1}), only the first one will be generated");
            Add("log.map_loader.unsupported_value_type", "Unsupported value type: {0}, position ({1}, {2})");
            Add("log.map_loader.nested_structure_not_supported", "Nested structure not supported, position ({0}, {1})");
            Add("log.map_loader.unexpected_token_type", "Unexpected token type: {0}, position ({1}, {2})");
            Add("log.map_loader.reload_success", "Successfully reloaded map: {0}");
            Add("log.map_loader.reload_failed", "Failed to reload map: {0}");
            Add("log.map_loader.restarting_scene", "Restarting scene...");
            Add("log.map_loader.scene_reloading", "Reloading scene: {0}");
            Add("log.map_loader.scene_reloaded", "Scene reloaded");
            Add("log.map_loader.scene_reload_failed", "Failed to reload scene: {0}");
            Add("log.map_loader.no_current_fungame", "No current Fungame configuration loaded");
            Add("log.map_loader.custom_structures_not_supported", "Custom structures are not supported for map loading: {0}");
            Add("log.map_loader.no_features_enabled", "No features enabled");
            Add("log.map_loader.feature_enabled", "{0} enabled");
            Add("log.map_loader.feature_enabled_with_value", "{0} enabled: {1}");
            Add("log.map_loader.skip_generation", "Skipped {0} generation");
            Add("log.map_loader.no_directory_path", "Fungame directory path is null or empty");
            Add("log.map_loader.fungame_json_not_found", "fungame.json not found in: {0}");
            Add("log.map_loader.fungame_deserialize_failed", "Failed to deserialize Fungame from disk");
            Add("log.map_loader.fungame_reloaded_from_disk", "Reloaded Fungame from disk: {0}");
            Add("log.map_loader.fungame_reload_failed", "Failed to reload Fungame from disk: {0}");
            Add("log.map_loader.validation.no_data", "No {1} data in {0}");
            Add("log.map_loader.validation.row_data_empty", "{0} row data is empty");

            // Log - Error
            Add("log.error.no_fungame_file", "Cannot find fungame.json file: {0}");
            Add("log.error.no_valid_directories", "No valid Fungame directories, please check the Fungames folder");
            Add("log.error.custom_structures_mod_not_loaded", "Fungame '{0}' requires Custom Structures mod, but the mod is not loaded");
            Add("log.error.multiple_content_types", "Fungame '{0}' has multiple content types defined (MapData, CustomStructures, BuildModeSave). Only one type is allowed.");

            // Log - Fungame Load
            Add("log.fungame_load.empty_target_path", "Target path cannot be null or empty");
            Add("log.fungame_load.unauthorized", "No permission to read file '{0}': {1}");
            Add("log.fungame_load.io_error", "Failed to read file '{0}': {1}");
            Add("log.fungame_load.file_empty", "File '{0}' is empty, will create default configuration");
            Add("log.fungame_load.deserialize_null", "File '{0}' deserialization returned null, will create default configuration");
            Add("log.fungame_load.invalid_json", "File '{0}' has invalid JSON format: {1}");
            Add("log.fungame_load.no_folder_name", "Could not resolve a valid folder name from path '{0}'");

            // Log - Common
            Add("log.common.map", "Map");
            Add("log.common.item", "Item");
            Add("log.common.block", "Block");
            Add("log.common.terrain", "Terrain");
            Add("log.common.structure", "Structure");
            Add("log.common.background", "Background");
            Add("log.common.startup_command", "Startup commands");
            Add("log.common.loop_command", "Loop command");

            // Log - Mod Command
            Add("log.mod_command.empty_type", "Unknown command type");
            Add("log.mod_command.world_not_loaded", "World not loaded");
            Add("log.mod_command.no_waypoints", "No waypoints defined in current Fungame");
            Add("log.mod_command.exit_no_target", "Please specify exit target: none (vanilla) or tutorial");
            Add("log.mod_command.register_failed", "Failed to register custom commands: {0}\n{1}");
            Add("log.mod_command.no_fungame", "No Fungame available");

            // Log - Custom Structures Loader
            Add("log.custom_structures_loader.loading", "Loading custom structure: {0}");
            Add("log.custom_structures_loader.failed", "Failed to load custom structure ({0}): {1}");
            Add("log.custom_structures_loader.not_found", "{0} not found, reflection failed");
            Add("log.custom_structures_loader.not_found_custom_structures", "Custom structure file not found");
            Add("log.custom_structures_loader.suppress.structure_loader_not_found", "Could not find StructureLoader type");
            Add("log.custom_structures_loader.suppress.cleared_definitions", "Suppressed Custom Structures auto-generation (cleared StructureDefinitions)");
            Add("log.custom_structures_loader.suppress.cleared_field", "Suppressed Custom Structures auto-generation (cleared {0})");
            Add("log.custom_structures_loader.suppress.no_registry", "Custom Structures mod found but no structure registry to clear");
            Add("log.custom_structures_loader.suppress.failed", "Failed to suppress Custom Structures auto-generation: {0}");

            // Log - Build Mode Save Loader
            Add("log.build_mode_save_loader.loading", "Loading Build Mode save: {0} (blocks: {1}, liquids: {2}, backgrounds: {3})");
            Add("log.build_mode_save_loader.failed", "Failed to load Build Mode save ({0}): {1}");
            Add("log.build_mode_save_loader.not_found_buildmode_save", "Build Mode save file not found");
            Add("log.build_mode_save_loader.bg_sprite_missing", "Background sprite not found: {0}");

            // Log - Map Loader (Build Mode)
            Add("log.map_loader.build_mode_save_applied", "Build Mode save applied: {0} blocks, {1} liquids, {2} backgrounds, {3} failed");
            Add("log.map_loader.not_found_buildmode_save", "Build Mode save file not found");

            // Log - Fungame Directory Loader
            Add("log.loader.directory_not_found", "Directory not found: {0}");
            Add("log.loader.fungame_json_not_found", "fungame.json not found in: {0}");
            Add("log.loader.fungame_json_failed", "Failed to deserialize fungame.json: {0}");
            Add("log.loader.success", "Successfully loaded Fungame: {0} (ID: {1}, Version: {2})");
            Add("log.loader.failed", "Failed to load Fungame from {0}: {1}");
            Add("log.loader.level_dir_not_found", "Level directory not found: {0}");
            Add("log.loader.no_level_files", "No level files found in: {0}");
            Add("log.loader.loaded_level", "Loaded level: {0}");
            Add("log.loader.failed_to_load_level", "Failed to load level file {0}: {1}");
            Add("log.loader.no_world_settings", "No world settings found, using defaults");
            Add("log.loader.missing_type", "Missing 'type' property in {0}, expected '{1}'");
            Add("log.loader.type_mismatch", "Type mismatch in {0}: expected '{1}', got '{2}'");
            Add("log.loader.failed_to_load_file", "Failed to load {0}: {1}");
        }
    }
}