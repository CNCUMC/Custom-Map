using System.Collections;
using System.Reflection;

namespace CustomMap.Loader;

/// <summary>
/// CUCoreLib StructureRegistry 辅助类
/// </summary>
public static class StructureRegistryHelper
{
    /// <summary>
    /// 注册结构 JSON
    /// </summary>
    public static bool RegisterFromJson(string structureId, string json)
    {
        try
        {
            CUCoreLib.Registries.StructureRegistry.RegisterFromJson(structureId, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 注册文件中的结构
    /// </summary>
    public static bool RegisterFromFile(string structureId, string filePath)
    {
        try
        {
            CUCoreLib.Registries.StructureRegistry.RegisterFromFile(structureId, filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置生成数量
    /// </summary>
    public static bool TrySetSpawnCounts(string structureId, params int[] counts)
    {
        try
        {
            CUCoreLib.Registries.StructureRegistry.TrySetSpawnCounts(structureId, counts);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 清除所有注册的结构
    /// </summary>
    public static bool Clear()
    {
        try
        {
            var field = typeof(CUCoreLib.Registries.StructureRegistry).GetField("RegisteredDefinitions",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null) return false;

            var dict = field.GetValue(null) as IDictionary;
            if (dict == null || dict.Count == 0) return false;

            dict.Clear();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
