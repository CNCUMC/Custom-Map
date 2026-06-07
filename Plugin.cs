using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CustomFungamePack.Data;
using CustomFungamePack.Data.Feature.Player;
using CustomFungamePack.Data.Feature.World;
using CustomFungamePack.Lang;
using HarmonyLib;
using MossLib.Constant;
using MossLib.Tool;

namespace CustomFungamePack;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("org.explosivehydra.mosslib")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.explosivehydra.customfungamepack";
    public const string Name = "Custom Fungame Pack";
    public const string Version = "1.1.0";

    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);
    internal static readonly Dictionary<string, ConfigEntryBase> ConfigRegistry = new();

    public static ConfigEntry<bool> MoreLogs;
    public static ConfigEntry<bool> StartGameUseFungame;
    public static ConfigEntry<string> FirstUseFungame;
    public static ConfigEntry<int> ProgressUpdateInterval;

    public void Awake()
    {
        Logger = base.Logger;

        LocaleGenerator.SetLogger(Logger);
        LocaleGenerator.Register(new EnLangGenerator(), Logger);
        LocaleGenerator.Register(new ZhCnLangGenerator(), Logger);
        LocaleGenerator.Register(new ZhTwLangGenerator(), Logger);
        LocaleGenerator.GenerateAll();
        ModLocale.Initialize(Logger);
        _harmony.PatchAll();
        FungameCheck.Initialize();

        MoreLogs = RegisterConfig("more_logs", false);
        StartGameUseFungame = RegisterConfig("start_game_use_fungame", false);
        FirstUseFungame = RegisterConfig("first_use_fungame", TemplateFungame.Id);
        ProgressUpdateInterval = RegisterConfig("progress_update_interval", 33);
        ModConfigs.ReloadConfigs();
    }

    public static readonly Fungame TemplateFungame = new()
    {
        Name = $"{Name} Template",
        Id = "template",
        Version = Version,
        Author = ["Black_Moss"],
        Description = "a map template",
        CommandData = new CommandData
        {
            LoopCommands =
            [
                "heal"
            ],
            LoopInterval = 10
        },
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
                        "11                     l                                                                                                     jjjjjjjjj11",
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
                        { "b", "beartrap" }
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
                        Id = Items._9MmRound,
                        Slot = Slots.Mouth
                    },
                    new ItemData
                    {
                        Id = Items._12Gauge,
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
            StrXp = 9999,
            ResXp = 999,
            IntXp = 999
        },
        WorldSettingsData = new WorldSettingsData
        {
            SkipBackground = false
        }
    };

    private ConfigEntry<T> RegisterConfig<T>(string key, T defaultValue)
    {
        var entry = Config.Bind("General", key, defaultValue, ConfigLocale($"{key}.description"));
        ConfigRegistry[key] = entry;
        return entry;
    }

    public static object GetConfigValue(string key)
    {
        return ConfigRegistry.TryGetValue(key, out var entry)
            ? entry.BoxedValue
            : null;
    }

    public static bool SetConfigValue(string key, object value)
    {
        if (!ConfigRegistry.TryGetValue(key, out var entry))
            return false;

        try
        {
            var converted = System.Convert.ChangeType(value, entry.SettingType);
            entry.BoxedValue = converted;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool HasConfig(string key) => ConfigRegistry.ContainsKey(key);

    private static string ConfigLocale(string key)
    {
        return Locale($"config.{key}");
    }

    private static string Locale(string key)
    {
        return ModLocale.GetFormat(key);
    }
}