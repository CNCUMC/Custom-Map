using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomFungamePack;

[BepInPlugin(Guid, Name, "1.0.0")]
[BepInDependency("blackmoss.mosslib")]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    internal const string Guid = "blackmoss.customfungamepack";
    internal const string Name = "Custom Fungame Pack";
    private readonly Harmony _harmony = new(Guid);

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Plugin Instance { get; private set; } = null!;

    public void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        ModLocale.Initialize(Logger);
        _harmony.PatchAll();
        // ModCommand.Initialize(Logger);
        FungameCheck.Initialize();
    }
}