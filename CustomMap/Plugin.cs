using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomMap;

[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.custommap";
    public const string Name = "Custom Map";
    public const string Version = "1.0.0";
    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;
        _harmony.PatchAll();

        Logger.LogInfo("Custom Map loaded!");
    }
}