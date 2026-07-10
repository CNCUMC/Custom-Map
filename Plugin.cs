using Bark.BetterCCL;
using Bark.Constant;
using BepInEx;
using BepInEx.Logging;
using CustomMap.Data;
using CustomMap.Data.Feature.Player;
using CustomMap.Data.Feature.World;
using CustomMap.Lang;
using HarmonyLib;

namespace CustomMap;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("org.cncumc.bark", "1.0.2")]
[BepInDependency("net.cucorelib", "1.0.2")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.custommap";
    public const string Name = "Custom Map";
    public const string Version = "1.0.0";
    private const string NameSpace = "quantum";
    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);

    public static bool MoreLogs;
    public static bool StartGameUseMap;
    public static int ProgressUpdateInterval;
    public static string FirstUseMap;

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

    public void Awake()
    {
        Logger = base.Logger;

        new EnLangGenerator().Initialize(Logger);
        new ZhCnLangGenerator().Initialize(Logger);
        new ZhTwLangGenerator().Initialize(Logger);

        BetterOptions.Bool(NameSpace, "more_logs", NameSpace, false, v => MoreLogs = v);
        BetterOptions.Bool(NameSpace, "start_game_use_map", NameSpace, false, v => StartGameUseMap = v);
        BetterOptions.Int(NameSpace, "progress_update_interval", NameSpace, 10, 1000, 333,
            v => ProgressUpdateInterval = v);

        _harmony.PatchAll();
    }

    // private static void RegisterMapOption()
    // {
    //     var choices = new List<ModDropdownChoice>();
    //     var langDir = $"{Application.dataPath}/Maps";
    //     try
    //     {
    //         if (Directory.Exists(langDir))
    //             foreach (var file in Directory.GetFiles(langDir, "*.json"))
    //             {
    //                 var code = Path.GetFileNameWithoutExtension(file);
    //                 BetterLocale.SetDefault("EN", "option", $"quantum.video.bilingual_name{code}", code);
    //                 choices.Add(new ModDropdownChoice(code, code));
    //             }
    //     }
    //     catch (Exception ex)
    //     {
    //         Logger.LogWarning($"Failed to scan language directory '{langDir}': {ex.Message}");
    //     }
    //
    //     var arr = choices.ToArray();
    //     var defaultIndex = Math.Max(0, Array.FindIndex(arr, c => c.Key == "EN"));
    //     BetterOptions.Dropdown(NameSpace, "bilingual_name", Setting.SettingCategory.Video,
    //         defaultIndex, arr,
    //         i => FirstUseMap = i >= 0 && i < arr.Length
    //             ? arr[i].Key
    //             : "EN");
    // }
}