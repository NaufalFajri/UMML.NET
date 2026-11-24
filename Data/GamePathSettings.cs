using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace UMML.Data
{
    public class PlatformPathEntry
    {
        public string Persistent_dir { get; set; } = "";
        public string Game_dir { get; set; } = "";
    }

    public class GamePathConfig
    {
        public string SelectedPlatform { get; set; } = "Steam Global";
        public string SelectedKey { get; set; } = "Global"; // "Global", "Japan", "Custom"
        public string CustomKey { get; set; } = "";
        public Dictionary<string, PlatformPathEntry> Platforms { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public PlatformPathEntry GetOrCreatePlatform(string platform)
        {
            if (!Platforms.TryGetValue(platform, out var entry))
            {
                entry = new PlatformPathEntry();
                Platforms[platform] = entry;
            }
            return entry;
        }
    }

    public static class GamePathSettings
    {
        private static string? resolvedConfigPath;
        private static GamePathConfig? currentConfig;

        public static string[] SupportedPlatforms =>
        [
            "Steam Global",
            "Steam Japan",
            "DMM",
            "Komoe",
            "Kakao"
        ];

        public static GamePathConfig Current => currentConfig ??= Load();

        public static string GetConfigFilePath()
        {
            if (!string.IsNullOrEmpty(resolvedConfigPath) && File.Exists(resolvedConfigPath))
                return resolvedConfigPath;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentDir = Directory.GetCurrentDirectory();

            string[] directCandidates =
            [
                Path.Combine(baseDir, "UMML_Data", "GamePath.json"),
                Path.Combine(baseDir, "GamePath.json"),
                Path.Combine(currentDir, "UMML_Data", "GamePath.json"),
                Path.Combine(currentDir, "GamePath.json")
            ];

            foreach (var cand in directCandidates)
            {
                if (File.Exists(cand))
                {
                    resolvedConfigPath = cand;
                    return cand;
                }
            }

            // Search parent directories
            foreach (var startDir in new[] { baseDir, currentDir })
            {
                var dirInfo = new DirectoryInfo(startDir);
                while (dirInfo != null && dirInfo.Parent != null)
                {
                    dirInfo = dirInfo.Parent;
                    string check1 = Path.Combine(dirInfo.FullName, "UMML_Data", "GamePath.json");
                    if (File.Exists(check1))
                    {
                        resolvedConfigPath = check1;
                        return check1;
                    }
                    string check2 = Path.Combine(dirInfo.FullName, "GamePath.json");
                    if (File.Exists(check2))
                    {
                        resolvedConfigPath = check2;
                        return check2;
                    }
                }
            }

            // Default save location
            string defaultFolder = Path.Combine(baseDir, "UMML_Data");
            resolvedConfigPath = Path.Combine(defaultFolder, "GamePath.json");
            return resolvedConfigPath;
        }

        public static GamePathConfig Load()
        {
            var config = new GamePathConfig();
            string filePath = GetConfigFilePath();

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonText = File.ReadAllText(filePath);
                    var jsonNode = JsonNode.Parse(jsonText);
                    if (jsonNode is JsonObject rootObj)
                    {
                        if (rootObj.TryGetPropertyValue("SelectedPlatform", out var selPlat) && selPlat != null)
                            config.SelectedPlatform = selPlat.ToString();

                        if (rootObj.TryGetPropertyValue("SelectedKey", out var selKey) && selKey != null)
                            config.SelectedKey = selKey.ToString();

                        if (rootObj.TryGetPropertyValue("CustomKey", out var custKey) && custKey != null)
                            config.CustomKey = custKey.ToString();

                        foreach (var kvp in rootObj)
                        {
                            if (kvp.Value is JsonObject platObj)
                            {
                                var entry = new PlatformPathEntry();
                                if (platObj.TryGetPropertyValue("Persistent_dir", out var pDir) && pDir != null)
                                    entry.Persistent_dir = NormalizePath(pDir.ToString());

                                if (platObj.TryGetPropertyValue("Game_dir", out var gDir) && gDir != null)
                                    entry.Game_dir = NormalizePath(gDir.ToString());

                                config.Platforms[kvp.Key] = entry;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load GamePath.json: {ex.Message}");
                }
            }

            currentConfig = config;
            return config;
        }

        public static void Save(GamePathConfig? config = null)
        {
            config ??= Current;
            string filePath = GetConfigFilePath();

            try
            {
                string? dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var rootObj = new JsonObject();

                // Save platforms
                foreach (var kvp in config.Platforms)
                {
                    var platObj = new JsonObject
                    {
                        ["Persistent_dir"] = kvp.Value.Persistent_dir ?? "",
                        ["Game_dir"] = kvp.Value.Game_dir ?? ""
                    };
                    rootObj[kvp.Key] = platObj;
                }

                rootObj["SelectedPlatform"] = config.SelectedPlatform ?? "";
                rootObj["SelectedKey"] = config.SelectedKey ?? "Global";
                rootObj["CustomKey"] = config.CustomKey ?? "";

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonOutput = rootObj.ToJsonString(options);
                File.WriteAllText(filePath, jsonOutput);
                resolvedConfigPath = filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save GamePath.json: {ex.Message}");
            }
        }

        public static (string? persistentDir, string? gameDir) FallbackPath(string platform, string? baseDir = null, string? gameDir = null)
        {
            var config = Current;
            if (config.Platforms.TryGetValue(platform, out var entry))
            {
                bool baseHasMeta = !string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir) && File.Exists(Path.Combine(baseDir, "meta"));
                if (string.IsNullOrEmpty(baseDir) || !Directory.Exists(baseDir) || !baseHasMeta)
                {
                    string saved = NormalizePath(entry.Persistent_dir);
                    if (!string.IsNullOrEmpty(saved) && Directory.Exists(saved))
                    {
                        bool savedHasMeta = File.Exists(Path.Combine(saved, "meta"));
                        if (string.IsNullOrEmpty(baseDir) || !Directory.Exists(baseDir) || savedHasMeta)
                        {
                            baseDir = saved;
                        }
                    }
                }

                if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir))
                {
                    string savedGame = NormalizePath(entry.Game_dir);
                    if (!string.IsNullOrEmpty(savedGame) && Directory.Exists(savedGame))
                    {
                        gameDir = savedGame;
                    }
                }
            }

            return (baseDir, gameDir);
        }

        public static string NormalizePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            path = path.Trim().Trim('"', '\'');
            path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }
    }
}
