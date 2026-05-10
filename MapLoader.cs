using System;
using System.IO;
using BepInEx.Logging;
using MossLib;
using Newtonsoft.Json;

namespace CustomFungamePack;

public static class MapLoader
{
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void LoadAndApplyMapFromFungame(Fungame fungame)
    {
        try
        {
            if (fungame?.Map == null)
            {
                Logger.LogError("Fungame 或地图数据为空");
                return;
            }

            var mapData = fungame.Map;
            ValidateAndApplyMap(mapData);
            
            var width = mapData.Blocks.Length;
            var height = mapData.Blocks.Length > 0 ? mapData.Blocks[0].Length : 0;
            Logger.LogInfo($"成功加载地图: 起始坐标({mapData.X}, {mapData.Y}), 尺寸({width}x{height})");
        }
        catch (Exception ex)
        {
            Logger.LogError($"加载地图失败: {ex.Message}");
        }
    }

    public static void LoadAndApplyMap(string mapFilePath)
    {
        try
        {
            if (!File.Exists(mapFilePath))
            {
                Logger.LogError($"地图文件不存在: {mapFilePath}");
                return;
            }

            var jsonContent = File.ReadAllText(mapFilePath);
            var mapData = JsonConvert.DeserializeObject<MapData>(jsonContent);

            if (mapData == null)
            {
                Logger.LogError("地图数据解析失败");
                return;
            }

            ValidateAndApplyMap(mapData);
            
            var width = mapData.Blocks.Length;
            var height = mapData.Blocks.Length > 0 ? mapData.Blocks[0].Length : 0;
            Logger.LogInfo($"成功加载地图: 起始坐标({mapData.X}, {mapData.Y}), 尺寸({width}x{height})");
        }
        catch (Exception ex)
        {
            Logger.LogError($"加载地图失败: {ex.Message}");
        }
    }
    
    
    private static void ValidateAndApplyMap(MapData mapData)
    {
        if (mapData.Blocks == null || mapData.Blocks.Length == 0)
        {
            Logger.LogWarning("地图中没有方块数据");
            return;
        }

        var rowCount = mapData.Blocks.Length;
        var maxColCount = 0;
        
        foreach (var row in mapData.Blocks)
        {
            if (row != null && row.Length > maxColCount)
            {
                maxColCount = row.Length;
            }
        }

        if (maxColCount == 0)
        {
            Logger.LogWarning("地图行数据为空");
            return;
        }

        var isIrregular = false;
        for (int i = 0; i < rowCount; i++)
        {
            if (mapData.Blocks[i] == null || mapData.Blocks[i].Length != maxColCount)
            {
                isIrregular = true;
                break;
            }
        }

        if (isIrregular)
        {
            Logger.LogWarning($"检测到不规则地图形状！正在填充为规则长方形 ({rowCount}x{maxColCount})...");
            NormalizeMapShape(mapData, rowCount, maxColCount);
        }

        Tools.CheckForWorld();
        
        var blockCount = 0;
        var failCount = 0;
        
        for (int row = 0; row < rowCount; row++)
        {
            if (mapData.Blocks[row] == null)
            {
                Logger.LogWarning($"第 {row} 行为空，跳过");
                continue;
            }

            for (int col = 0; col < mapData.Blocks[row].Length; col++)
            {
                var blockType = mapData.Blocks[row][col];
                
                if (blockType < 0)
                {
                    Logger.LogWarning($"无效的方块类型 ({blockType}) 在位置 ({col}, {row})，跳过");
                    continue;
                }
                
                var worldX = mapData.X + col;
                var worldY = mapData.Y - row;
                
                try
                {
                    Tools.SetBlock(worldX, worldY, (ushort)blockType);
                    blockCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    if (failCount <= 5)
                    {
                        Logger.LogError($"在 ({worldX}, {worldY}) 放置方块 {blockType} 失败: {ex.Message}");
                    }
                }
            }
        }

        if (failCount > 0)
        {
            Logger.LogWarning($"地图应用完成，成功放置 {blockCount} 个方块，失败 {failCount} 个");
        }
        else
        {
            Logger.LogInfo($"地图应用完成，共放置 {blockCount} 个方块");
        }
    }

    private static void NormalizeMapShape(MapData mapData, int rowCount, int maxColCount)
    {
        var normalizedBlocks = new int[rowCount][];
        
        for (int row = 0; row < rowCount; row++)
        {
            normalizedBlocks[row] = new int[maxColCount];
            
            if (mapData.Blocks[row] != null)
            {
                var sourceLength = mapData.Blocks[row].Length;
                for (int col = 0; col < maxColCount; col++)
                {
                    if (col < sourceLength)
                    {
                        normalizedBlocks[row][col] = mapData.Blocks[row][col];
                    }
                    else
                    {
                        normalizedBlocks[row][col] = 0;
                    }
                }
            }
            else
            {
                for (int col = 0; col < maxColCount; col++)
                {
                    normalizedBlocks[row][col] = 0;
                }
            }
        }
        
        mapData.Blocks = normalizedBlocks;
        Logger.LogInfo($"地图已规范化为 {rowCount}x{maxColCount} 的长方形，缺失部分已用空气方块填充");
    }}
