using System;
using System.Collections.Generic;
using System.IO;
using CUCoreLib.Helpers;
using CustomMap.Patch;

namespace CustomMap;

public static class MapUtils
{
    public static readonly string MapsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");

    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Map> Maps = [];
    public static Map CurrentMap => WorldGenerationPatch.CurrentMap;

    public static bool HasMap => CurrentMap != null;
    public static bool IsInMapWorld => CUCoreUtils.IsInWorld() && HasMap;

    public static string GetMapPath(Map map = null)
    {
        var target = map ?? CurrentMap;
        return string.IsNullOrEmpty(target?.DirectoryPath)
            ? null
            : target.DirectoryPath;
    }
}