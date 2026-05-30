using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CustomFungamePack.Lang;
using HarmonyLib;
using MossLib.Constant;
using MossLib.Tool;

namespace CustomFungamePack;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("blackmoss.mosslib")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "blackmoss.customfungamepack";
    public const string Name = "Custom Fungame Pack";
    public const string Version = "1.0.0";

    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);
    public static ConfigEntry<bool> MoreLogs;
    public static ConfigEntry<bool> StartGameUseFungame;
    public static ConfigEntry<string> FirstUseFungame;

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

        MoreLogs = Config.Bind(
            "General",
            "MoreLogs",
            false,
            "Enable more logs");

        StartGameUseFungame = Config.Bind(
            "General",
            "Start Use Fungame",
            false,
            "Use the selected Fungame when starting a new game.");

        FirstUseFungame = Config.Bind(
            "General",
            "First Use Fungame",
            TemplateFungame.Id,
            "The Fungame ID to use when starting a new game. Requires 'Start Use Fungame' to be enabled.");

        Configs.ReloadConfigs();
    }

    public static readonly Fungame TemplateFungame = new()
    {
        Name = $"{Name} Template",
        Id = "template",
        Version = Version,
        Author = ["Black_Moss"],
        Description = "a map template",
        Spawn = [0, 0],
        X = -68,
        Y = 62,
        // Waypoints =
        // [
        //     new()
        //     {
        //         Id = "default",
        //         X = 0,
        //         Y = 0
        //     }
        // ],
        // CommandData = new CommandData
        // {
        //     OnceCommands =
        //     [
        //         "alert true Start!"
        //     ],
        //     LoopCommands =
        //     [
        //         "alert false 10s!"
        //     ],
        //     LoopInterval = 0
        // },
        Feature = new Feature
        {
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
            }
        },
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
                "11                                                                                                                   11               11",
                "11                                                                                                                   11               11",
                "11                                                                                                                                    11",
                "11                                                                                                                                    11",
                "11                                                                                                                      7 7 7 7 7 7 7 11",
                "11                                                                                                             11    1111111111111111111",
                "11                                                                                                             11    1111111111111111111",
                "11                                                                                                             11    115             511",
                "11                                                                                                  1111111111111    11               11",
                "11                                                                                                  1111111111111    115             511",
                "11                                                                                                  11                                11",
                "11                                                                                                  119 9 9 9 9 9                     11",
                "11                                                                                                  11                                11",
                "11                                                                                                  119 9 9 9 911          1111111111111",
                "11                                                                                                  11         11          1111111111111",
                "11                                                                                                  119 9 9 9 911          11         11",
                "11                                                                                                  11         11          11         11",
                "11                                                                                                  11 89 9 9 911          11         11",
                "11                                                                                                  1111111111111    11111111         11",
                "11                                                                                                  1111111111111    11111111         11",
                "11                                                                                                                                    11",
                "11                                                                                                                                    11",
                "11 6                                                                                                                                  11",
                "11                     3                                                                                                     44444444411",
                "1122222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222211",
                "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            ],
            Key =
            {
                { " ", 0 },
                { "1", 6 },
                { "2", 10 },
                { "3", "landmine" },
                { "4", "jumppad" },
                { "5", "turret" },
                { "6", "soundcannon" },
                { "7", "spikestabber" },
                { "8", "geyser" },
                { "9", "lifepodpump" },
            }
        },
        Items =
        [
            new ItemData
            {
                Id = "geofruit",
                Slot = 0
            },
            new ItemData
            {
                Id = "geofruit",
                Slot = 2
            },
            new ItemData
            {
                Id = "geofruit",
                Slot = 3
            },
            new ItemData
            {
                Id = "geofruit",
                Slot = 4
            },
            new ItemData
            {
                Id = "geofruit",
                Slot = 5
            }
        ]
    };
}