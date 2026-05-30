namespace CustomFungamePack;

public static class Configs
{
    public static bool MoreLogs;
    public static bool StartGameUseFungame;
    public static string FirstUseFungame;

    public static void ReloadConfigs()
    {
        MoreLogs = Plugin.MoreLogs.Value;
        StartGameUseFungame = Plugin.StartGameUseFungame.Value;
        FirstUseFungame = Plugin.FirstUseFungame.Value;
    }
}