using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UMML.Data
{
    public static class UmaDataHelper
    {
        public const string KeyGlobal = "c753a5e8f5f78294f7fef57df4a14ffbf9a896cea1d4e09947e0d904e7fde8eaf0";
        public const string KeyJapan = "9c2bab97bcf8c0c4f1a9ea7881a213f6c9ebf9d8d4c6a8e43ce5a259bde7e9fd";

        private static string activeKey = "";
        private static string currentKeyMode = "Global"; // "Global", "Japan", "Custom"
        private static string customKey = "";

        private static bool isMetaFileEncrypted = false;
        private static List<ManifestEntry>? manifestEntries;

        static UmaDataHelper()
        {
            try
            {
                Batteries_V2.Init();
            }
            catch { }
        }

        public static string UmamusumeDirectory { get; set; } = "";
        public static string ResourceDirectory => Path.Combine(UmamusumeDirectory, "dat");
        public static string MetaFile => Path.Combine(UmamusumeDirectory, "meta");
        public static string CurrentPlatformName { get; set; } = "Auto-detected";

        public static string CurrentKeyMode
        {
            get => currentKeyMode;
            set
            {
                if (currentKeyMode != value)
                {
                    currentKeyMode = value;
                    activeKey = "";
                    ReloadManifest();
                }
            }
        }

        public static string CustomKey
        {
            get => customKey;
            set
            {
                if (customKey != value)
                {
                    customKey = value;
                    if (currentKeyMode == "Custom")
                    {
                        activeKey = "";
                        ReloadManifest();
                    }
                }
            }
        }

        public static string ActiveKey => activeKey;
        public static bool IsMetaFileEncrypted => isMetaFileEncrypted;

        public static string GetKeyForMode(string mode)
        {
            return mode switch
            {
                "Global" => KeyGlobal,
                "Japan" => KeyJapan,
                "Custom" => customKey,
                _ => KeyGlobal
            };
        }

        public static List<DetectedPlatform> ScanPlatforms()
        {
            return PlatformDetector.ScanAllPlatforms();
        }

        public static bool TrySwitchPlatform(string platformName, out string directoryPath)
        {
            directoryPath = "";
            var detected = PlatformDetector.DetectPlatform(platformName);

            if (detected.IsValid || (Directory.Exists(detected.PersistentDirectory) && File.Exists(Path.Combine(detected.PersistentDirectory, "meta"))))
            {
                directoryPath = detected.PersistentDirectory;
                UmamusumeDirectory = directoryPath;
                CurrentPlatformName = platformName;

                // Sync default key for platform if not custom
                if (currentKeyMode != "Custom")
                {
                    if (detected.DefaultKeyMode != "None")
                        CurrentKeyMode = detected.DefaultKeyMode;
                }

                activeKey = "";
                Initialize();
                ReloadManifest();

                // Save to GamePathSettings
                var config = GamePathSettings.Current;
                config.SelectedPlatform = platformName;
                config.SelectedKey = currentKeyMode;
                var entry = config.GetOrCreatePlatform(platformName);
                entry.Persistent_dir = directoryPath;
                if (!string.IsNullOrEmpty(detected.GameDirectory))
                    entry.Game_dir = detected.GameDirectory;
                GamePathSettings.Save(config);

                return true;
            }

            return false;
        }

        public static bool CheckDirectory()
        {
            return CheckDirectory(UmamusumeDirectory);
        }

        public static bool CheckDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (!Directory.Exists(path)) return false;
            if (!File.Exists(Path.Combine(path, "meta"))) return false;

            return true;
        }

        public static void Initialize()
        {
            if (File.Exists(MetaFile))
            {
                try
                {
                    using FileStream fileStream = new(MetaFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    if (fileStream.Length >= 4)
                    {
                        using BinaryReader reader = new(fileStream);
                        // Standard unencrypted SQLite starts with "SQLi" (0x694C5153 in little endian)
                        isMetaFileEncrypted = reader.ReadUInt32() != 0x694C5153;
                    }
                    else
                    {
                        isMetaFileEncrypted = false;
                    }
                }
                catch
                {
                    isMetaFileEncrypted = true;
                }
            }
            else
            {
                isMetaFileEncrypted = false;
            }

            Batteries_V2.Init();
        }

        public static string GetPath(ManifestEntry? entry)
        {
            if (entry is not null)
            {
                string path = Path.Combine(ResourceDirectory, entry.HashName[..2], entry.HashName);
                return File.Exists(path) ? path : "";
            }
            return "";
        }

        public static List<ManifestEntry> GetManifestEntries(Func<ManifestEntry, bool>? condition = null)
        {
            if (manifestEntries == null)
            {
                using var conn = OpenDatabase(MetaFile, isMetaFileEncrypted);
                manifestEntries = GetRows<ManifestEntry>(conn);
            }

            if (condition is not null)
                return [.. manifestEntries.Where(condition)];
            else
                return manifestEntries;
        }

        public static void ReloadManifest()
        {
            manifestEntries = null;
            activeKey = "";
        }

        public static (bool success, int count, string message) TestKey(string keyHex, string? metaPath = null)
        {
            metaPath ??= MetaFile;
            if (!File.Exists(metaPath))
                return (false, 0, "Meta file does not exist at the specified path.");

            try
            {
                // Check if unencrypted
                using (var fs = new FileStream(metaPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length >= 4)
                    {
                        using var br = new BinaryReader(fs);
                        if (br.ReadUInt32() == 0x694C5153)
                        {
                            using var plainConn = new SQLiteConnection(metaPath, SQLiteOpenFlags.ReadOnly);
                            int rowCount = plainConn.ExecuteScalar<int>("SELECT count(*) FROM a;");
                            return (true, rowCount, "Meta is unencrypted SQLite. Successfully read!");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(keyHex))
                    return (false, 0, "Meta file is encrypted, but the provided key is empty.");

                using SQLiteConnection conn = OpenEncryptedDatabase(metaPath, keyHex.Trim());
                int count = conn.ExecuteScalar<int>("SELECT count(*) FROM a;");
                return (true, count, $"Success! Decrypted meta and read {count} asset entries.");
            }
            catch (Exception ex)
            {
                return (false, 0, $"Decryption failed: {ex.Message}");
            }
        }

        private static SQLiteConnection OpenDatabase(string databaseFile, bool encrypted = false)
        {
            if (!File.Exists(databaseFile))
                throw new FileNotFoundException($"Meta database not found at '{databaseFile}'.");

            if (!encrypted)
            {
                try
                {
                    var test = new SQLiteConnection(databaseFile, SQLiteOpenFlags.ReadWrite);
                    _ = test.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master;");
                    return test;
                }
                catch
                {
                    // Fall through to encrypted attempt
                }
            }

            // Encrypted database
            string primaryKey = GetKeyForMode(currentKeyMode);
            List<string> candidateKeys = [];

            if (!string.IsNullOrWhiteSpace(primaryKey))
                candidateKeys.Add(primaryKey.Trim());

            // Add standard fallbacks if primary fails
            if (!candidateKeys.Contains(KeyGlobal, StringComparer.OrdinalIgnoreCase))
                candidateKeys.Add(KeyGlobal);
            if (!candidateKeys.Contains(KeyJapan, StringComparer.OrdinalIgnoreCase))
                candidateKeys.Add(KeyJapan);

            Exception? lastException = null;
            foreach (string checkKey in candidateKeys)
            {
                try
                {
                    SQLiteConnection conn = OpenEncryptedDatabase(databaseFile, checkKey);
                    _ = conn.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master;");
                    activeKey = checkKey;
                    return conn;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            throw new InvalidOperationException(
                $"Unable to open encrypted meta database with the active key (Mode: {currentKeyMode}).\n" +
                $"Please verify your Platform and Key settings.",
                lastException);
        }

        private static SQLiteConnection OpenEncryptedDatabase(string databaseFile, string hexKey)
        {
            SQLiteConnection connection = new(databaseFile, SQLiteOpenFlags.ReadWrite);
            try
            {
                connection.ExecuteScalar<string>("pragma cipher = 'chacha20';");

                byte[] keyBytes = Convert.FromHexString(hexKey);
                int result = raw.sqlite3_key(connection.Handle, keyBytes);
                if (result != raw.SQLITE_OK)
                {
                    throw new InvalidOperationException($"sqlite3_key failed with result {result}.");
                }

                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        private static List<T> GetRows<T>(SQLiteConnection connection, Func<T, bool>? condition = null) where T : new()
        {
            try
            {
                TableQuery<T> table = connection.Table<T>();
                if (condition is not null)
                    return [.. table.Where(condition)];
                else
                    return [.. table];
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
