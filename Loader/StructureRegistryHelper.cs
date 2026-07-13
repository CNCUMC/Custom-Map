using System.Collections;
using System.Reflection;
using CUCoreLib.Registries;

namespace CustomMap.Loader;

public static class StructureRegistryHelper
{
    public static bool RegisterFromJson(string structureId, string json)
    {
        try
        {
            StructureRegistry.RegisterFromJson(structureId, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool RegisterFromFile(string structureId, string filePath)
    {
        try
        {
            StructureRegistry.RegisterFromFile(structureId, filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TrySetSpawnCounts(string structureId, params int[] counts)
    {
        try
        {
            StructureRegistry.TrySetSpawnCounts(structureId, counts);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Clear()
    {
        try
        {
            var field = typeof(StructureRegistry).GetField("RegisteredDefinitions",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null) return false;

            if (field.GetValue(null) is not IDictionary dict || dict.Count == 0) return false;

            dict.Clear();
            return true;
        }
        catch
        {
            return false;
        }
    }
}