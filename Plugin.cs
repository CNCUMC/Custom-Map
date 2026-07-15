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

[BepInDependency("net.cucorelib", "1.0.2")]
[BepInDependency("org.cncumc.bark", "1.0.3")]
[BepInDependency("com.Jimmyking.morestructures", "1.2.1")]
[BepInDependency("com.alexx_.buildmode", BepInDependency.DependencyFlags.SoftDependency)]
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

    public static readonly Map TemplateMap = new()
    {
        Id = "template",
        Version = Version,
        Author = ["Black_Moss"],
        Layers =
        [
            new Layer
            {
                Type = WorldGeneration.OverrideSceneType.Debug,
                X = -68,
                Y = 62,
                SkipBackground = false,
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
                }
            }
        ],
        XpData = new XpData
        {
            StrXp = 999,
            ResXp = 999,
            IntXp = 999
        }
    };

    private readonly Harmony _harmony = new(Guid);

    internal static bool BuildModeLoaded => Chainloader.PluginInfos.ContainsKey("com.alexx_.buildmode");

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
        var mapsDir = MapCheck.MapsPath;

        // Add TemplateMap - register English default and use localized name
        var templateKey = $"custommap.custommap.first_use_map{TemplateMap.Id}";
        BetterLocale.SetDefault("EN", "option", templateKey, $"{Name} Template");
        BetterLocale.Flush();
        var templateDisplayName = BetterLocale.GetOther(templateKey);
        AddMapChoice(choices, TemplateMap.Id, templateDisplayName);

        Logger.LogMessage($"Maps: '{mapsDir}'");
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

                        // Skip if this map has the same Id as TemplateMap to avoid duplicates
                        if (map.Id == TemplateMap.Id) continue;

                        MapCheck.DetectMissingMods(map);

                        // Register English default and use localized name
                        var key = $"custommap.custommap.first_use_map{map.Id}";
                        BetterLocale.SetDefault("EN", "option", key, MapLocale.GetDisplayName(map));
                        BetterLocale.Flush();
                        var displayName = BetterLocale.GetOther(key);
                        AddMapChoice(choices, map.Id, displayName);
                    }
                    catch
                    {
                        // ignored
                    }
                }
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

    private static void AddMapChoice(List<ModDropdownChoice> choices, string id, string displayName)
    {
        choices.Add(new ModDropdownChoice(id, displayName));
    }
}