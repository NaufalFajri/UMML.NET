using Microsoft.Win32;
using System;
using System.IO;

namespace UMML.Data.DataDirectories
{
    public class KakaoDataDirectory : DataDirectory
    {
        public override string Name => "Kakao (Korea)";

        public override bool Exists()
        {
            // 1. Check registry
            string? gameDir = null;
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
                            {
                                gameDir = val;
                                break;
                            }
                        }
                    }
                    if (gameDir != null) break;
                }
                catch { }
            }

            // 2. Check standard drives
            if (gameDir == null)
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        string cand = Path.Combine(drive.RootDirectory.FullName, "Kakao Games", "Umamusume");
                        if (Directory.Exists(cand))
                        {
                            gameDir = cand;
                            break;
                        }
                    }
                }
            }

            if (gameDir != null)
            {
                foreach (var folder in new[] { "umamusume_Data", "Umamusume_Data" })
                {
                    string persistent = Path.Combine(gameDir, folder, "Persistent");
                    if (CheckDirectory(persistent))
                    {
                        DataDirectoryPath = persistent;
                        return true;
                    }
                }
                if (CheckDirectory(gameDir))
                {
                    DataDirectoryPath = gameDir;
                    return true;
                }
            }

            // 3. Fallback to LocalLow
            string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "KakaoGames", "Umamusume");
            if (CheckDirectory(localLow))
            {
                DataDirectoryPath = localLow;
                return true;
            }

            return false;
        }
    }
}
