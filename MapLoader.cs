using System;
using System.Linq;
using BepInEx.Logging;
using MossLib;

namespace CustomFungamePack;

public static class MapLoader
{
    private const string LogPrefix = "log.";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void LoadAndApplyMapFromFungame(Fungame fungame)
    {
        try
        {
            if (fungame?.MapData == null)
            {
                Error("map_loader.load_error");
                return;
            }

            var mapData = fungame.MapData;

            if (mapData.Map == null || mapData.Map.Length == 0)
            {
                Error("map_loader.invalid_format");
                return;
            }

            ParseAndApplyStringMap(fungame);

            Info("map_loader.load_success", mapData.X, mapData.Y, mapData.Map.Length,
                mapData.Map.Max(row => row?.Length ?? 0));
        }
        catch (Exception ex)
        {
            Error("map_loader.load_failed", ex.Message);
        }
    }

    private static void ParseAndApplyStringMap(Fungame fungame)
    {
        var mapData = fungame.MapData;
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            Warning("validation.no_data", ModLocale.GetFormat("log.common.map"), "string map");
            return;
        }

        if (mapData.Key == null || mapData.Key.Count == 0)
        {
            Error("map_loader.key_missing");
            return;
        }

        var rowCount = mapData.Map.Length;
        var maxColCount = mapData.Map.Max(row => row?.Length ?? 0);

        if (maxColCount == 0)
        {
            Warning("validation.row_data_empty", "string map");
            return;
        }

        var worldY = mapData.Y;
        var blockCount = 0;
        var itemCount = 0;
        var failCount = 0;

        for (int row = 0; row < rowCount; row++)
        {
            var mapRow = mapData.Map[row];
            if (string.IsNullOrEmpty(mapRow))
            {
                worldY--;
                continue;
            }

            var worldX = mapData.X;

            foreach (var charStr in mapRow.Select(t => t.ToString()))
            {
                if (!mapData.Key.TryGetValue(charStr, out var value))
                {
                    worldX++;
                    continue;
                }

                switch (value)
                {
                    case long intValue:
                    {
                        if (intValue > 0)
                        {
                            try
                            {
                                Tools.SetBlock(worldX, worldY, (ushort)intValue);
                                blockCount++;
                            }
                            catch (Exception ex)
                            {
                                Error("map_loader.place_failed", worldX, worldY,
                                    ModLocale.GetFormat("log.common.block"), intValue, ex.Message);
                                failCount++;
                            }
                        }

                        break;
                    }
                    case double doubleValue:
                    {
                        if (doubleValue > 0)
                        {
                            try
                            {
                                Tools.SetBlock(worldX, worldY, (ushort)doubleValue);
                                blockCount++;
                            }
                            catch (Exception ex)
                            {
                                Error("map_loader.place_failed", worldX, worldY,
                                    ModLocale.GetFormat("log.common.block"), doubleValue, ex.Message);
                                failCount++;
                            }
                        }

                        break;
                    }
                    case string stringValue:
                    {
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            try
                            {
                                Tools.SetItem(worldX, worldY, stringValue);
                                itemCount++;
                            }
                            catch (Exception ex)
                            {
                                Error("map_loader.place_failed", worldX, worldY, ModLocale.GetFormat("log.common.item"),
                                    stringValue, ex.Message);
                                failCount++;
                            }
                        }

                        break;
                    }
                }

                worldX++;
            }

            worldY--;
        }

        Info("map_loader.string_map_applied", blockCount, itemCount, failCount);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogInfo(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogError(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogWarning(message, Logger);
    }
}