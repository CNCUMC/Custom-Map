using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MossLib.Tool;

namespace CustomFungamePack;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("blackmoss.mosslib")]
[HarmonyBefore("blackmoss.mosslib")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "blackmoss.customfungamepack";
    public const string Name = "Custom Fungame Pack";
    public const string Version = "1.0.0";
    
    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);
    internal static Plugin Instance { get; private set; }
    public static ConfigEntry<bool> MoreLogs;

    public void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        
        ModLocale.Initialize(Logger);
        _harmony.PatchAll();
        FungameCheck.Initialize();
        
        MoreLogs = Config.Bind(
            "General",
            "MoreLogs",
            false,
            "Enable more logs");
        Configs.ReloadConfigs();
    }
}