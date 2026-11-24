using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace UMML.Data
{
    public class DetectedPlatform
    {
        public string Name { get; set; } = "";
        public string PersistentDirectory { get; set; } = "";
        public string GameDirectory { get; set; } = "";
        public string MetaFile => Path.Combine(PersistentDirectory, "meta");
        public string DatDirectory => Path.Combine(PersistentDirectory, "dat");
        public bool HasMeta => File.Exists(MetaFile);
        public bool HasDat => Directory.Exists(DatDirectory);
        public bool IsValid => HasMeta && HasDat;

        public string DefaultKeyMode => Name switch
        {
            "Steam Global" => "Global",
            "Steam Japan" => "Japan",
            "DMM" => "Japan",
            _ => "None"
        };
    }

    public static class PlatformDetector
    {
        private static readonly List<string> CachedSteamLibraries = [];

        public static IReadOnlyList<string> GetSteamLibraries()
        {
            if (CachedSteamLibraries.Count > 0) return CachedSteamLibraries;

            string? steamRoot = GetSteamRootPath();
            List<string> candidateRoots = [];
            if (!string.IsNullOrEmpty(steamRoot) && Directory.Exists(steamRoot))
                candidateRoots.Add(steamRoot);

            candidateRoots.AddRange(new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
                @"C:\Steam",
                @"D:\Steam",
                @"E:\Steam",
                @"D:\SteamLibrary",
                @"E:\SteamLibrary"
            });

            foreach (var root in candidateRoots)
            {
                if (Directory.Exists(root) && !CachedSteamLibraries.Contains(root, StringComparer.OrdinalIgnoreCase))
                {
                    CachedSteamLibraries.Add(root);
                }

                string vdf = Path.Combine(root, "steamapps", "libraryfolders.vdf");
                if (File.Exists(vdf))
                {
                    try
                    {
                        string content = File.ReadAllText(vdf);
                        var matches = Regex.Matches(content, @"""path""\s+""([^""]+)""");
                        foreach (Match m in matches)
                        {
                            string lib = m.Groups[1].Value.Replace(@"\\", @"\");
                            if (Directory.Exists(lib) && !CachedSteamLibraries.Contains(lib, StringComparer.OrdinalIgnoreCase))
                            {
                                CachedSteamLibraries.Add(lib);
                            }
                        }
                    }
                    catch
                    {
                        // ignore malformed vdf
                    }
                }
            }

            return CachedSteamLibraries;
        }

        public static string? GetSteamRootPath()
        {
            string[] possibleKeys =
            [
                @"SOFTWARE\WOW6432Node\Valve\Steam",
                @"SOFTWARE\Valve\Steam"
            ];

            foreach (var keyPath in possibleKeys)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                    var val = key?.GetValue("InstallPath")?.ToString();
                    if (!string.IsNullOrEmpty(val) && Directory.Exists(val))
                        return val;
                }
                catch { }
            }

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                var val = key?.GetValue("SteamPath")?.ToString();
                if (!string.IsNullOrEmpty(val) && Directory.Exists(val))
                    return val;
            }
            catch { }

            return null;
        }

        public static string? FindSteamGamePath(int appId, string defaultInstallDir)
        {
            foreach (var lib in GetSteamLibraries())
            {
                string manifest = Path.Combine(lib, "steamapps", $"appmanifest_{appId}.acf");
                if (File.Exists(manifest))
                {
                    try
                    {
                        string content = File.ReadAllText(manifest);
                        var match = Regex.Match(content, @"""installdir""\s+""([^""]+)""");
                        if (match.Success)
                        {
                            string installDir = match.Groups[1].Value;
                            string full = Path.Combine(lib, "steamapps", "common", installDir);
                            if (Directory.Exists(full)) return full;
                        }
                    }
                    catch { }
                }

                string fallback = Path.Combine(lib, "steamapps", "common", defaultInstallDir);
                if (Directory.Exists(fallback)) return fallback;
            }

            return null;
        }

        public static string? FindDmmGamePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            foreach (var folder in new[] { "dmmgameplayer5", "dmmgameplayer" })
            {
                string configPath = Path.Combine(appData, folder, "dmmgame.cnf");
                if (File.Exists(configPath))
                {
                    try
                    {
                        string configJson = File.ReadAllText(configPath);
                        var json = JsonNode.Parse(configJson);
                        var contents = json?["contents"]?.AsArray();
                        if (contents != null)
                        {
                            foreach (var product in contents)
                            {
                                if (product?["productId"]?.ToString() == "umamusume")
                                {
                                    string? path = product?["detail"]?["path"]?.ToString();
                                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                                        return path;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                string cand = Path.Combine(drive.RootDirectory.FullName, "DMMGames", "umamusume");
                if (Directory.Exists(cand) && File.Exists(Path.Combine(cand, "umamusume.exe")))
                    return cand;
            }

            return null;
        }

        public static string? FindKomoeGamePath()
        {
            string[] subKeys =
            [
                @"Software\komoemumamusume",
                @"SOFTWARE\WOW6432Node\komoemumamusume",
                @"SOFTWARE\komoemumamusume"
            ];

            foreach (var sub in subKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(sub) ?? Registry.LocalMachine.OpenSubKey(sub);
                    var val = key?.GetValue("GameInstallPath")?.ToString();
                    if (!string.IsNullOrEmpty(val) && Directory.Exists(val))
                        return val;
                }
                catch { }
            }

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                string cand = Path.Combine(drive.RootDirectory.FullName, "komoemumamusume", "komoemumamusume Game");
                if (Directory.Exists(cand))
                    return cand;
            }

            return null;
        }

        public static string? FindKakaoGamePath()
        {
            string[] subKeys =
            [
                @"Software\KakaoGames\Umamusume",
                @"SOFTWARE\WOW6432Node\KakaoGames\Umamusume",
                @"SOFTWARE\KakaoGames\Umamusume"
            ];

            foreach (var sub in subKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(sub) ?? Registry.LocalMachine.OpenSubKey(sub);
                    if (key != null)
                    {
                        foreach (var valName in new[] { "InstallPath", "Path", "GamePath" })
                        {
                            var val = key.GetValue(valName)?.ToString();
                            if (!string.IsNullOrEmpty(val) && Directory.Exists(val))
                                return val;
                        }
                    }
                }
                catch { }
            }

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                string cand = Path.Combine(drive.RootDirectory.FullName, "Kakao Games", "Umamusume");
                if (Directory.Exists(cand))
                    return cand;
            }

            return null;
        }

        public static DetectedPlatform DetectPlatform(string platformName)
        {
            string? baseDir = null;
            string? gameDir = null;

            switch (platformName)
            {
                case "Steam Global":
                    {
                        gameDir = FindSteamGamePath(3224770, "UmamusumePrettyDerby");

                        // 1. Persistent inside game folder
                        string? newPersistent = null;
                        if (!string.IsNullOrEmpty(gameDir))
                        {
                            string std = Path.Combine(gameDir, "UmamusumePrettyDerby_Data", "Persistent");
                            if (Directory.Exists(std))
                            {
                                newPersistent = std;
                            }
                            else
                            {
                                try
                                {
                                    var sub = Directory.GetDirectories(gameDir)
                                        .FirstOrDefault(d => d.EndsWith("_Data", StringComparison.OrdinalIgnoreCase));
                                    if (sub != null && Directory.Exists(Path.Combine(sub, "Persistent")))
                                        newPersistent = Path.Combine(sub, "Persistent");
                                }
                                catch { }
                            }
                        }

                        // 2. Persistent in LocalLow
                        string? oldPersistent = null;
                        string cygamesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Cygames");
                        if (Directory.Exists(cygamesDir))
                        {
                            foreach (var subName in new[] { "UmamusumePrettyDerby", "umamusumeprettyderby", "umamusume", "Umamusume" })
                            {
                                string cand = Path.Combine(cygamesDir, subName);
                                if (File.Exists(Path.Combine(cand, "meta")))
                                {
                                    oldPersistent = cand;
                                    break;
                                }
                            }
                            if (oldPersistent == null)
                            {
                                string cand = Path.Combine(cygamesDir, "UmamusumePrettyDerby");
                                if (Directory.Exists(cand)) oldPersistent = cand;
                            }
                        }

                        // Compare timestamps if both have meta
                        bool newHasMeta = !string.IsNullOrEmpty(newPersistent) && File.Exists(Path.Combine(newPersistent, "meta"));
                        bool oldHasMeta = !string.IsNullOrEmpty(oldPersistent) && File.Exists(Path.Combine(oldPersistent, "meta"));

                        if (newHasMeta && oldHasMeta)
                        {
                            try
                            {
                                var newTime = File.GetLastWriteTimeUtc(Path.Combine(newPersistent!, "meta"));
                                var oldTime = File.GetLastWriteTimeUtc(Path.Combine(oldPersistent!, "meta"));
                                baseDir = newTime >= oldTime ? newPersistent : oldPersistent;
                            }
                            catch
                            {
                                baseDir = newPersistent;
                            }
                        }
                        else if (newHasMeta) baseDir = newPersistent;
                        else if (oldHasMeta) baseDir = oldPersistent;
                        else baseDir = newPersistent ?? oldPersistent;

                        break;
                    }

                case "Steam Japan":
                    {
                        gameDir = FindSteamGamePath(3564400, "UmamusumePrettyDerby_Jpn");

                        string? newPersistent = null;
                        if (!string.IsNullOrEmpty(gameDir))
                        {
                            string std = Path.Combine(gameDir, "UmamusumePrettyDerby_Jpn_Data", "Persistent");
                            if (Directory.Exists(std)) newPersistent = std;
                            else
                            {
                                try
                                {
                                    var sub = Directory.GetDirectories(gameDir)
                                        .FirstOrDefault(d => d.EndsWith("_Data", StringComparison.OrdinalIgnoreCase));
                                    if (sub != null && Directory.Exists(Path.Combine(sub, "Persistent")))
                                        newPersistent = Path.Combine(sub, "Persistent");
                                }
                                catch { }
                            }
                        }

                        string? oldPersistent = null;
                        string cygamesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Cygames");
                        if (Directory.Exists(cygamesDir))
                        {
                            foreach (var subName in new[] { "UmamusumePrettyDerby_Jpn", "umamusume" })
                            {
                                string cand = Path.Combine(cygamesDir, subName);
                                if (File.Exists(Path.Combine(cand, "meta")))
                                {
                                    oldPersistent = cand;
                                    break;
                                }
                            }
                        }

                        bool newHasMeta = !string.IsNullOrEmpty(newPersistent) && File.Exists(Path.Combine(newPersistent, "meta"));
                        bool oldHasMeta = !string.IsNullOrEmpty(oldPersistent) && File.Exists(Path.Combine(oldPersistent, "meta"));

                        if (newHasMeta && oldHasMeta)
                        {
                            try
                            {
                                var newTime = File.GetLastWriteTimeUtc(Path.Combine(newPersistent!, "meta"));
                                var oldTime = File.GetLastWriteTimeUtc(Path.Combine(oldPersistent!, "meta"));
                                baseDir = newTime >= oldTime ? newPersistent : oldPersistent;
                            }
                            catch
                            {
                                baseDir = newPersistent;
                            }
                        }
                        else if (newHasMeta) baseDir = newPersistent;
                        else if (oldHasMeta) baseDir = oldPersistent;
                        else baseDir = newPersistent ?? oldPersistent;

                        break;
                    }

                case "DMM":
                    {
                        gameDir = FindDmmGamePath();
                        string? dmmPersistent = !string.IsNullOrEmpty(gameDir)
                            ? Path.Combine(gameDir, "umamusume_Data", "Persistent")
                            : null;

                        string cygamesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Cygames", "umamusume");

                        bool dmmHasMeta = !string.IsNullOrEmpty(dmmPersistent) && File.Exists(Path.Combine(dmmPersistent, "meta"));
                        bool localLowHasMeta = Directory.Exists(cygamesDir) && File.Exists(Path.Combine(cygamesDir, "meta"));

                        if (dmmHasMeta) baseDir = dmmPersistent;
                        else if (localLowHasMeta) baseDir = cygamesDir;
                        else baseDir = dmmPersistent ?? cygamesDir;

                        break;
                    }

                case "Komoe":
                    {
                        gameDir = FindKomoeGamePath();
                        string? p = !string.IsNullOrEmpty(gameDir)
                            ? Path.Combine(gameDir, "komoeumamusume_Data", "Persistent")
                            : null;
                        string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Komoe", "umamusume");

                        if (!string.IsNullOrEmpty(p) && File.Exists(Path.Combine(p, "meta"))) baseDir = p;
                        else if (File.Exists(Path.Combine(localLow, "meta"))) baseDir = localLow;
                        else if (!string.IsNullOrEmpty(gameDir) && File.Exists(Path.Combine(gameDir, "meta"))) baseDir = gameDir;
                        else baseDir = p ?? localLow;

                        break;
                    }

                case "Kakao":
                    {
                        gameDir = FindKakaoGamePath();
                        string? p = null;
                        if (!string.IsNullOrEmpty(gameDir))
                        {
                            foreach (var f in new[] { "umamusume_Data", "Umamusume_Data" })
                            {
                                string cand = Path.Combine(gameDir, f, "Persistent");
                                if (Directory.Exists(cand)) { p = cand; break; }
                            }
                        }
                        string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "KakaoGames", "Umamusume");

                        if (!string.IsNullOrEmpty(p) && File.Exists(Path.Combine(p, "meta"))) baseDir = p;
                        else if (File.Exists(Path.Combine(localLow, "meta"))) baseDir = localLow;
                        else if (!string.IsNullOrEmpty(gameDir) && File.Exists(Path.Combine(gameDir, "meta"))) baseDir = gameDir;
                        else baseDir = p ?? localLow;

                        break;
                    }
            }

            // Apply fallback from GamePath.json
            var (fallbackBase, fallbackGame) = GamePathSettings.FallbackPath(platformName, baseDir, gameDir);

            return new DetectedPlatform
            {
                Name = platformName,
                PersistentDirectory = fallbackBase ?? "",
                GameDirectory = fallbackGame ?? ""
            };
        }

        public static List<DetectedPlatform> ScanAllPlatforms()
        {
            List<DetectedPlatform> results = [];
            foreach (var platName in GamePathSettings.SupportedPlatforms)
            {
                var detected = DetectPlatform(platName);
                if (detected.IsValid)
                {
                    results.Add(detected);
                }
            }
            return results;
        }
    }
}
