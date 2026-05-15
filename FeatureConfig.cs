using System.Collections.Generic;

namespace CustomFungamePack;

public static class FeatureConfig
{
    public static readonly Dictionary<string, float> Defaults = new()
    {
        { "fullbright", 1f },
        { "forgiving_level", 1f },
        { "gravity", 0.3f }
    };

    public static float GetDefault(string featureName)
    {
        return Defaults.TryGetValue(featureName.ToLower(), out var value) ? value : 1f;
    }
}
