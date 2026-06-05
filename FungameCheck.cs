using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using CustomFungamePack.Loader;
using CustomFungamePack.Patch;

namespace CustomFungamePack;

public static class FungameCheck
{
    private static ManualLogSource _logger;
    private const string LocaleKeyPre = "fungame_check.";
    public static readonly string FungamesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fungames");
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Fungame> Fungames = [];
    public static Fungame CurrentFungame => WorldGenerationPatch.CurrentFungame;
    public static bool HasRunningFungame => CurrentFungame != null;

    public static void Initialize()
    {
        if (_logger != null) return; // 防止重复初始化
        _logger = Plugin.Logger;
        LoadFungameDirectories();
    }

    private static void LoadFungameDirectories()
    {
        var directories = Directory.GetDirectories(FungamesPath);

        _logger.LogInfo($"Read {directories.Length} Fungame folders");

        ValidDirectories.Clear();
        CheckFailDirectories.Clear();
        Fungames.Clear();

        foreach (var fungamesDirectory in directories)
        {
            // 新版结构只需要 fungame.json 作为入口
            var fungameJsonPath = Path.Combine(fungamesDirectory, "fungame.json");
            if (!File.Exists(fungameJsonPath))
            {
                // Warning($"{Path.GetFileName(fungamesDirectory)} Missing fungame.json");
                continue;
            }

            ValidDirectories.Add(fungamesDirectory);
        }

        Fungames.Add(Plugin.TemplateFungame);

        if (ValidDirectories.Count == 0) return;
        _logger.LogInfo($"Valid directories: {ValidDirectories.Count}, loading...");

        var directoriesToValidate = ValidDirectories.ToList();
        foreach (var directory in directoriesToValidate)
        {
            var fungame = FungameDirectoryLoader.LoadFromDirectory(directory);
            if (fungame != null)
            {
                Fungames.Add(fungame);
            }
            else
            {
                UninitializedWarning($"{Path.GetFileName(directory)} Loading failed!");
                CheckFailDirectories.Add(directory);
            }
        }

        if (CheckFailDirectories.Count == 0) return;
        _logger.LogInfo($"Directory validation failed: {CheckFailDirectories.Count}:");
        foreach (var failDirectory in CheckFailDirectories)
        {
            _logger.LogInfo($"- {Path.GetFileName(failDirectory)}");
            ValidDirectories.Remove(failDirectory);
        }
    }

    public static string GetFungamePath(Fungame fungame = null)
    {
        var target = fungame ?? CurrentFungame;
        return string.IsNullOrEmpty(target?.DirectoryPath) ? null : target.DirectoryPath;
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.Log($"{LocaleKeyPre}{key}", args);
    }

    // private static void Warning(string key)
    // {
    //     Log.Warning(key, _logger);
    // }

    private static void UninitializedWarning(string key)
    {
        _logger.LogWarning($"{key}");
    }
}