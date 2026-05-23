using MossLib.Base;

namespace CustomFungamePack.Lang
{
    public class EnLangGenerator : ModLangGenBase
    {
        protected override string LanguageCode => "EN";

        protected override void BuildLocaleData()
        {
            // Feature
            Add("feature.fullbright", "Fullbright");
            Add("feature.forgiving_level", "Forgiving Level");
            Add("feature.gravity", "Gravity");
            Add("feature.skip_terrain", "Skip Terrain");
            Add("feature.skip_structures", "Skip Structures");
            Add("feature.skip_background", "Skip Background");

            // Command - Fungame
            Add("command.fungame.description", "Fungame related commands");
            Add("command.fungame.string", "Select function");
            Add("command.fungame.parameter", "Function parameter");
            Add("command.fungame.help", "Available subcommands:\n  reload  - Reload current map\n  info    - Show map info\n  spawn   - Teleport to spawn\n  select  - Select a Fungame\n  list    - List all Fungames\n  feature - Manage features (list/get/set)\n  waypoint- Manage waypoints (list/get)");
            Add("command.fungame.reload", "Reloading map...");
            
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
            Add("command.fungame.waypoint.help", "Waypoint subcommands:\n  list - List all waypoints\n  get <id or name> - Teleport to waypoint");
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
            
            // Command - Fungame - Feature
            Add("command.fungame.feature.help", "Feature subcommands:\n  list - List all features\n  get <name> - Get feature value\n  set <name> <value> - Set feature value");
            Add("command.fungame.feature.unknown_subcommand", "Unknown feature subcommand: {0}");
            Add("command.fungame.feature.get_no_name", "Please specify a feature name to get");
            Add("command.fungame.feature.set_missing_params", "Please specify feature name and value to set");
            Add("command.fungame.feature.list_header", "Current feature settings:");
            Add("command.fungame.feature.item", "  {0} ({1}): {2}");
            Add("command.fungame.feature.get_success", "Feature '{0}' = {1}");
            Add("command.fungame.feature.set_success", "Feature '{0}' set to {1}");
            Add("command.fungame.feature.not_found", "Feature not found: {0}");
            Add("command.fungame.feature.invalid_value", "Invalid value for {0}: {1}");
            Add("command.fungame.feature.invalid_bool", "Invalid boolean value for {0}: {1} (use true/false)");

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
            Add("log.world_generation.initializing_world", "Initializing Fungame map...");
            Add("log.world_generation.loading_fungame_map", "Loading Fungame map: {0}");
            Add("log.world_generation.no_map_data", "Fungame {0} does not contain map data");
            Add("log.world_generation.no_commands", "No {0} enabled");
            Add("log.world_generation.executing_command", "Executing {0}: {1}");
            Add("log.world_generation.executing_loop_command", "Executing loop {0}: {1}");
            
            // Log - Validation
            Add("log.validation.map_invalid_type", "map field format is incorrect");
            Add("log.validation.map_missing_field", "Map missing required field: {0}");
            Add("log.validation.map_field_type_error", "Map {0} field must be {1}");
            Add("log.validation.map_map_empty", "Map map array cannot be empty");
            Add("log.validation.map_row_not_string", "Map map row {0} must be a string");
            Add("log.validation.map_item_row_not_array", "Map items row {0} must be an array");
            Add("log.validation.map_item_not_string", "Map items[{0}][{1}] must be a string");
            Add("log.validation.map_and_custom_structures_conflict", "Cannot use map_data and custom_structures at the same time");
            Add("log.validation.missing_map_or_custom_structures", "Missing map_data or custom_structures");
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
            
            // Log - Error
            Add("log.error.no_fungame_file", "Cannot find fungame.json file: {0}");
            Add("log.error.no_valid_directories", "No valid Fungame directories, please check the Fungames folder");
            Add("log.error.custom_structures_mod_not_loaded", "Fungame '{0}' requires Custom Structures mod, but the mod is not loaded");
            
            // Log - Common
            Add("log.common.map", "Map");
            Add("log.common.item", "Item");
            Add("log.common.block", "Block");
            Add("log.common.terrain", "Terrain");
            Add("log.common.structure", "Structure");
            Add("log.common.background", "Background");
            Add("log.common.startup_command", "Startup commands");
            Add("log.common.loop_command", "Loop command");
            Add("log.common.forgiving_level_mode", "Forgiving level mode");
            
            // Log - Mod Command
            Add("log.mod_command.empty_type", "Unknown command type");
            Add("log.mod_command.world_not_loaded", "World not loaded");
            Add("log.mod_command.no_waypoints", "No waypoints defined in current Fungame");
            
            // Log - Custom Structures Loader
            Add("log.custom_structures_loader.loading", "Loading custom structure: {0}");
            Add("log.custom_structures_loader.failed", "Failed to load custom structure ({0}): {1}");
        }
    }
}
