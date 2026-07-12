using System;
using System.Collections.Generic;
using System.IO;
using Bark.BetterCCL;
using Bark.Constant;
using Bark.Tool;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using CUCoreLib.Data;
using CustomMap.Data;
using CustomMap.Data.Feature.Player;
using CustomMap.Data.Feature.World;
using CustomMap.Lang;
using CustomMap.Loader;
using HarmonyLib;

namespace CustomMap;

[BepInDependency("org.cncumc.bark", "1.0.2")]
[BepInDependency("net.cucorelib", "1.0.2")]
[BepInDependency("com.alexx_.buildmode", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.Jimmyking.morestructures", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.custommap";
    public const string Name = "Custom Map";
    public const string Version = "1.0.0";
    private const string NameSpace = "custommap";
    internal new static ManualLogSource Logger;

    public static bool MoreLogs;
    public static bool StartGameUseMap;
    public static float ProgressUpdateInterval;
    public static string FirstUseMap;

    internal static bool BuildModeLoaded => Chainloader.PluginInfos.ContainsKey("com.alexx_.buildmode");
    internal static bool CustomStructuresLoaded => Chainloader.PluginInfos.ContainsKey("com.Jimmyking.morestructures");

    public static readonly Map TemplateMap = new()
    {
        Name = $"{Name} Template",
        Id = "template",
        Version = Version,
        Author = ["Black_Moss"],
        Description = "a map template",
        Levels =
        [
            new LevelData
            {
                X = -68,
                Y = 62,
                Spawn = [0, 0],
                // Waypoints =
                // [
                //     new()
                //     {
                //         Id = "default",
                //         X = 0,
                //         Y = 0
                //     }
                // ],
                MapData = new MapData
                {
                    Map =
                    [
                        "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                        "1122222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222211",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                   1111111111111111111",
                        "11                                                                                                                   1111111111111111111",
                        "11                                                                                              11111111111111111    11               11",
                        "11                                                                                              11111111111111111                     11",
                        "11                                                                                              11             11                     11",
                        "11                                                                                              11             11                     11",
                        "11                                                                                              11                      p p p p p p p 11",
                        "11                                                                                              11 b b b b b         1111111111111111111",
                        "11                                                                                              11                   1111111111111111111",
                        "11                                                                                              11111111111111111    11t             t1",
                        "11                                                                                              11111111111111111    11               11",
                        "11                                                                                              11111111111111111    11t             t11",
                        "11                                                                                              11iiiiiiiiiiiii11                     11",
                        "11                                                                                              11iiiiiiiiiiiii11                     11",
                        "11                                                                                              11iiiiiiiiiiiii11                     11",
                        "11                                                                                              11iiiiiiiiiiiii11          1111111111111",
                        "11                                                                                              11iiiiiiiiiiiii11          1111111111111",
                        "11                                                                                              11iiiiiiiiiiiii11          11         11",
                        "11                                                                                              11iiiiiiiiiiiii11          11         11",
                        "11                                                                                              11iiiigiiiiiiii11          11         11",
                        "11                                                                                              11111111111111111    11111111         11",
                        "11                                                                                              11111111111111111    11111111         11",
                        "11                                                                                                                                    11",
                        "11                                                                                                                                    11",
                        "11 s                                                                                                                                  11",
                        "11                     l                                              7   8   9                                              jjjjjjjjj11",
                        "1122222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222211",
                        "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
                    ],
                    Key =
                    {
                        { " ", Blocks.Air.Id },
                        { "1", Blocks.SteelTile.Id },
                        { "2", Blocks.HeatResistantAlloy.Id },
                        { "l", "landmine" },
                        { "j", "jumppad" },
                        { "t", "turret" },
                        { "s", "soundcannon" },
                        { "p", "spikestabber" },
                        { "g", "geyser" },
                        { "i", "lifepodpump" },
                        { "b", "beartrap" },
                        { "9", "trader1" },
                        { "8", "trader2" },
                        { "7", "trader3" }
                    }
                },
                Items =
                [
                    new ItemData
                    {
                        Id = Items.Rifle,
                        Slot = Slots.MainHand
                    },
                    new ItemData
                    {
                        Id = Items.MindWipe,
                        Slot = Slots.Mouth
                    },
                    new ItemData
                    {
                        Id = Items.GravBag,
                        Slot = Slots.UpperBck
                    },
                    new ItemData
                    {
                        Id = Items.Aed,
                        Slot = Slots.MiddleBack
                    },
                    new ItemData
                    {
                        Id = Items.Lrd,
                        Slot = Slots.LowerBack
                    }
                ]
            }
        ],
        MineData = new MineData
        {
            Undestroy = true,
            ExplosionParamsData = new ExplosionParamsData
            {
                Range = 3
            }
        },
        JumpPadData = new JumpPadData
        {
            Force = 0.5f,
            NoLight = true,
            Cooldown = 0f
        },
        TurretData = new TurretData
        {
            Cooldown = 0f,
            NoLight = true
        },
        SoundCannonData = new SoundCannonData
        {
            MaxDistance = 20,
            Cooldown = 3,
            Undestroy = true
        },
        SpikeStabberData = new SpikeStabberData
        {
            Undestroy = true,
            NoLight = true,
            Cooldown = 3
        },
        GeyserData = new GeyserData
        {
            Cooldown = 1
        },
        BearTrapData = new BearTrapData
        {
            Undestroy = true,
            Cooldown = 1
        },
        XpData = new XpData
        {
            StrXp = 999,
            ResXp = 999,
            IntXp = 999
        },
        WorldSettingsData = new WorldSettingsData
        {
            SkipBackground = false
        }
    };

    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;

        new EnLangGenerator().Initialize(Logger);
        new ZhCnLangGenerator().Initialize(Logger);
        new ZhTwLangGenerator().Initialize(Logger);

        BetterOptions.Bool(NameSpace, "more_logs", NameSpace, false, v => MoreLogs = v);
        BetterOptions.Bool(NameSpace, "start_game_use_map", NameSpace, false, v => StartGameUseMap = v);
        BetterOptions.Float(NameSpace, "progress_update_interval", NameSpace, 333, 10, 1000,
            v => ProgressUpdateInterval = v);
        RegisterMapOption();

        BetterLocale.Flush();

        MapCheck.Initialize();
        _harmony.PatchAll();

        UpdateUtil.Check("CNCUMC/Custom-Map", Name, Version, Logger);
    }

    private static void RegisterMapOption()
    {
        var choices = new List<ModDropdownChoice>();
        BetterLocale.SetDefault("EN", "option", $"custommap.custommap.first_use_map{TemplateMap.Id}", TemplateMap.Name);
        AddMapChoice(choices, TemplateMap.Id, MapLocale.GetName(TemplateMap));
        var mapsDir = MapCheck.MapsPath;
        Logger.LogInfo($"Maps directory: '{mapsDir}'");
        try
        {
            if (Directory.Exists(mapsDir))
                foreach (var dir in Directory.GetDirectories(mapsDir))
                {
                    var mapJsonPath = Path.Combine(dir, "map.json");
                    if (!File.Exists(mapJsonPath)) continue;
                    try
                    {
                        var map = CustomMapDirectoryLoader.LoadFromDirectory(dir);
                        if (map == null) continue;
                        
                        // 检测缺失的模组
                        MapCheck.DetectMissingMods(map);
                        
                        // 跳过有缺失模组的地图
                        if (map.MissingMods.Count > 0)
                        {
                            Logger.LogWarning(BetterLocale.GetLog("map_check.missing_mods_skipped", map.Name));
                            continue;
                        }
                        
                        BetterLocale.SetDefault("EN", "option", $"custommap.custommap.first_use_map{map.Id}", map.Name);
                        AddMapChoice(choices, map.Id, MapLocale.GetName(map));
                    }
                    catch
                    {
                        // ignored
                    }
                }

            BetterLocale.Flush();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(BetterLocale.GetLog("world_generation.scan_maps_failed", mapsDir, ex.Message));
        }

        var arr = choices.ToArray();
        BetterOptions.Dropdown(NameSpace, "first_use_map", NameSpace,
            0, arr,
            i => FirstUseMap = i >= 0 && i < arr.Length
                ? arr[i].Key
                : TemplateMap.Id);
    }

    private static void AddMapChoice(List<ModDropdownChoice> choices, string id, string displayName) =>
        choices.Add(new ModDropdownChoice(id, displayName));
}