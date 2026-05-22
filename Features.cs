using System.Collections.Generic;

namespace CustomFungamePack;

public static class Features
{
    public static readonly Dictionary<string, object> Defaults = new()
    {
        { "fullbright", false },
        { "forgiving_level", false },
        { "gravity", 0.3f },
        { "skip_terrain", true},
        { "skip_structures", true},
        { "skip_background", true}
    };

    public static object GetDefault(string featureName)
    {
        return Defaults.TryGetValue(featureName.ToLower(), out var value) ? value : 1f;
    }
}