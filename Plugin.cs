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
        //     LoopIntervai = 0
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
            },
            BearTrapData = new BearTrapData
            {
                Undestroy = true,
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
                "11                                                                                              11111111111111111    11               11",
                "11                                                                                              11111111111111111                     11",
                "11                                                                                              11             11                     11",
                "11                                                                                              11             11                     11",
                "11                                                                                              11                      p p p p p p p 11",
                "11                                                                                              11                   1111111111111111111",
                "11                                                                                              11 b b b b b         1111111111111111111",
                "11                                                                                              11111111111111111    11t             t1",
                "11                                                                                              11111111111111111    11               11",
                "11                                                                                              11111111111111111    11t             t11",
                "11                                                                                              11             11                     11",
                "11                                                                                              11i i i i i i i11                     11",
                "11                                                                                              11             11                     11",
                "11                                                                                              11i i i i i i i11          1111111111111",
                "11                                                                                              11             11          1111111111111",
                "11                                                                                              11i i i i i i i11          11         11",
                "11                                                                                              11             11          11         11",
                "11                                                                                              11i i i g i i i11          11         11",
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
                { " ", 0 },
                { "1", 6 },
                { "2", 10 },
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