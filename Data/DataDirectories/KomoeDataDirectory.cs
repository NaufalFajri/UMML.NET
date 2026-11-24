using Microsoft.Win32;
using System;
using System.IO;

namespace UMML.Data.DataDirectories
{
    public class KomoeDataDirectory : DataDirectory
    {
        public override string Name => "Komoe (Taiwan)";

        public override bool Exists()
        {
            // 1. Check registry
            string? gameDir = null;
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
                    if (key != null)
                    {
                        var val = key.GetValue("GameInstallPath")?.ToString();
                        if (!string.IsNullOrEmpty(val) && Directory.Exists(val))
                        {
                            gameDir = val;
                            break;
                        }
                    }
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
                        string cand = Path.Combine(drive.RootDirectory.FullName, "komoemumamusume", "komoemumamusume Game");
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
                string persistent = Path.Combine(gameDir, "komoeumamusume_Data", "Persistent");
                if (CheckDirectory(persistent))
                {
                    DataDirectoryPath = persistent;
                    return true;
                }
                if (CheckDirectory(gameDir))
                {
                    DataDirectoryPath = gameDir;
                    return true;
                }
            }

            // 3. Fallback to LocalLow
            string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Komoe", "umamusume");
            if (CheckDirectory(localLow))
            {
                DataDirectoryPath = localLow;
                return true;
            }

            return false;
        }
    }
}
