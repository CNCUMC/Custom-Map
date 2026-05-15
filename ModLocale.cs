using BepInEx.Logging;
using MossLib.Base;
using System.Reflection;

namespace CustomFungamePack;

public class ModLocale : ModLocaleBase
{
    private static ModLocale _instance;

    public static void Initialize(ManualLogSource logger)
    {
        if (_instance != null)
            return;
        _instance = new ModLocale();
        _instance.Initialize(logger, Assembly.GetExecutingAssembly());
    }

    public static string GetFormat(string key, params object[] args)
    {
        if (_instance == null)
        {
            return $"[{key}]";
        }

        try
        {
            var result = _instance.GetStringFormatted(key, args);
            return string.IsNullOrEmpty(result) ? $"[{key}]" : result;
        }
        catch
        {
            return $"[{key}]";
        }
    }
}