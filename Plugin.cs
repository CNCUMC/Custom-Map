using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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

    public void Awake()
    {
        Logger = base.Logger;

        ModLocale.Initialize(Logger);
        _harmony.PatchAll();
        // ModCommand.Initialize(Logger);
        FungameCheck.Initialize();
    }
}