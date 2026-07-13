using Newtonsoft.Json;

namespace CustomMap.Data.Feature.World;

/// <summary>
/// 结构放置配置，包含结构文件名和自定义坐标
/// </summary>
public class StructurePlacement
{
    /// <summary>
    /// 结构文件名（不含扩展名，自动检测 .ms2.json）
    /// </summary>
    [JsonProperty("structure")]
    public string Structure { get; set; }

    /// <summary>
    /// 在世界中的 X 坐标（可选，默认使用层级坐标）
    /// </summary>
    [JsonProperty("x")]
    public int X { get; set; }

    /// <summary>
    /// 在世界中的 Y 坐标（可选，默认使用层级坐标）
    /// </summary>
    [JsonProperty("y")]
    public int Y { get; set; }
}
