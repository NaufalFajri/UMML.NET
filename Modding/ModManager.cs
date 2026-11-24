using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UMML.Data;

namespace UMML.Modding
{
    public enum ModCompression
    {
        LZ4HC,       // Default (Best compatibility & size balance)
        LZ4Fast,     // LZ4 (Fast)
        LZMA,        // LZMA (Smallest)
        Original,    // Keep existing bundle compression
        None         // Uncompressed
    }

    public class InstallProgressReport
    {
        public int Current { get; set; }
        public int Total { get; set; }
        public string AssetName { get; set; } = "";
        public string StatusMessage { get; set; } = "";
    }

    public class InstallResult
    {
        public int DecodedCount { get; set; }
        public int MissingMetaCount { get; set; }
        public int BackedUpCount { get; set; }
        public List<string> Errors { get; } = [];
    }

    public class BackupItem
    {
        public string RelativePath { get; set; } = "";
        public string Hash { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public override string ToString() => DisplayName;
    }

    public class OrphanAssetInfo
    {
        public string RelativePath { get; set; } = "";
        public string FullPath { get; set; } = "";
        public long Size { get; set; }
    }

    public class OrphanScanResult
    {
        public List<OrphanAssetInfo> OrphanFiles { get; } = [];
        public long TotalBytes { get; set; }

        public string FormattedSize => FormatSize(TotalBytes);

        public static string FormatSize(long bytes)
        {
            double sz = bytes;
            string[] units = ["B", "KB", "MB", "GB", "TB"];
            int unitIdx = 0;
            while (sz >= 1024.0 && unitIdx < units.Length - 1)
            {
                sz /= 1024.0;
                unitIdx++;
            }
            return $"{sz:F1} {units[unitIdx]}";
        }
    }

    public static class ModManager
    {
        public static string BackupDirectory => Path.Combine(UmaDataHelper.UmamusumeDirectory, "dat.backup");

        public static bool CheckIsEncrypted(string filePath, out bool isUnityFS)
        {
            isUnityFS = false;
            try
            {
                using var fs = File.OpenRead(filePath);
                if (fs.Length < 16) return false;

                byte[] magic = new byte[8];
                int read = fs.Read(magic, 0, 8);
                if (read >= 7 && Encoding.ASCII.GetString(magic).StartsWith("UnityFS"))
                {
                    isUnityFS = true;
                    fs.Position = 0;
                    AssetBundleFile bundle = new();
                    bundle.Read(new AssetsFileReader(fs));

                    if ((bundle.BlockAndDirInfo?.DirectoryInfos?.Count ?? 0) == 0)
                        return true;

                    // Verify by reading the first assets file inside the bundle
                    for (int i = 0; i < bundle.BlockAndDirInfo!.DirectoryInfos.Count; i++)
                    {
                        if (bundle.IsAssetsFile(i))
                        {
                            var assetsFile = BundleHelper.LoadAssetFromBundle(bundle, i);
                            if (assetsFile.AssetInfos == null || assetsFile.AssetInfos.Count == 0)
                                return true;

                            // Cleanly read -> Unencrypted plain bundle!
                            return false;
                        }
                    }

                    return false;
                }
            }
            catch
            {
                if (isUnityFS) return true;
            }

            return false;
        }

        public static bool IsGameRunning()
        {
            string[] gameProcesses =
            [
                "umamusume",
                "umamusumeprettyderby_jpn",
                "umamusumeprettyderby",
                "komoeumamusume"
            ];

            foreach (var procName in gameProcesses)
            {
                try
                {
                    var procs = System.Diagnostics.Process.GetProcessesByName(procName);
                    if (procs.Length > 0)
                    {
                        foreach (var p in procs) p.Dispose();
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        public class ManifestLookupIndex
        {
            private readonly Dictionary<string, ManifestEntry> byHash = new(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<string, ManifestEntry> byName = new(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<string, List<ManifestEntry>> byBaseName = new(StringComparer.OrdinalIgnoreCase);

            public ManifestLookupIndex(IEnumerable<ManifestEntry> entries)
            {
                foreach (var e in entries)
                {
                    if (!string.IsNullOrEmpty(e.HashName))
                        byHash.TryAdd(e.HashName, e);

                    if (!string.IsNullOrEmpty(e.Name))
                        byName.TryAdd(e.Name, e);

                    string baseName = e.BaseName;
                    if (!string.IsNullOrEmpty(baseName))
                    {
                        if (!byBaseName.TryGetValue(baseName, out var list))
                        {
                            list = [];
                            byBaseName[baseName] = list;
                        }
                        list.Add(e);
                    }
                }
            }

            public ManifestEntry? FindByHash(string hash) => byHash.GetValueOrDefault(hash);
            public ManifestEntry? FindByName(string name) => byName.GetValueOrDefault(name);
            public ManifestEntry? FindByMedia(string fname)
            {
                if (byBaseName.TryGetValue(fname, out var list) && list.Count > 0)
                    return list[0];
                return null;
            }
            public ManifestEntry? FindByBaseName(string baseName)
            {
                if (byBaseName.TryGetValue(baseName, out var list) && list.Count > 0)
                    return list[0];
                return null;
            }
        }

        private static bool Is32CharAlphanumeric(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length != 32) return false;
            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }

        public static ManifestEntry? FindManifestEntry(string filePath, string relPath, List<ManifestEntry> entries)
        {
            return FindManifestEntry(filePath, relPath, new ManifestLookupIndex(entries));
        }

        public static ManifestEntry? FindManifestEntry(string filePath, string relPath, ManifestLookupIndex index)
        {
            string fname = Path.GetFileName(filePath);
            string stem = Path.GetFileNameWithoutExtension(filePath);
            string normPath = relPath.Replace('\\', '/');

            // 1. Check 32-char alphanumeric hash
            if (Is32CharAlphanumeric(stem))
            {
                var entry = index.FindByHash(stem);
                if (entry != null) return entry;
            }
            if (Is32CharAlphanumeric(fname))
            {
                var entry = index.FindByHash(fname);
                if (entry != null) return entry;
            }

            // 2. Relative path match (column 'n')
            var pathEntry = index.FindByName(normPath);
            if (pathEntry != null) return pathEntry;

            // 3. Strip common folder prefixes (assets/, pc-s/, dmm/, glo/)
            string[] prefixList = ["assets/", "pc-s/", "dmm/", "glo/"];
            foreach (var pfx in prefixList)
            {
                int idx = normPath.IndexOf(pfx, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    string sub = normPath[(idx + pfx.Length)..];
                    var subEntry = index.FindByName(sub);
                    if (subEntry != null) return subEntry;
                }
            }

            // 4. Audio / Video fallback by basename (.acb, .awb, .usm)
            string ext = Path.GetExtension(fname).ToLowerInvariant();
            if (ext is ".acb" or ".awb" or ".usm")
            {
                var mediaEntry = index.FindByMedia(fname);
                if (mediaEntry != null) return mediaEntry;
            }

            // 5. UnityFS internal AssetBundle object m_Name
            if (CheckIsEncrypted(filePath, out bool isUnityFS) == false && isUnityFS)
            {
                try
                {
                    AssetsManager am = new();
                    using var fs = File.OpenRead(filePath);
                    var bundle = am.LoadBundleFile(fs);
                    foreach (var assetsFileName in bundle.file.GetAllFileNames())
                    {
                        var fileInst = am.LoadAssetsFileFromBundle(bundle, assetsFileName);
                        if (fileInst == null) continue;
                        foreach (var asset in fileInst.file.AssetInfos)
                        {
                            if (asset.TypeId == (int)AssetClassID.AssetBundle)
                            {
                                var baseField = am.GetBaseField(fileInst, asset);
                                string mName = baseField["m_Name"].AsString;
                                if (!string.IsNullOrEmpty(mName))
                                {
                                    // Strip extension only, preserving directory path:
                                    // e.g. "3d/chara/body/bdy1001_00/pfb_bdy1001_00.a" -> "3d/chara/body/bdy1001_00/pfb_bdy1001_00"
                                    string resolvedName = mName;
                                    int lastSlash = Math.Max(resolvedName.LastIndexOf('/'), resolvedName.LastIndexOf('\\'));
                                    int lastDot = resolvedName.LastIndexOf('.');
                                    if (lastDot > lastSlash)
                                    {
                                        resolvedName = resolvedName[..lastDot];
                                    }

                                    var mEntry = index.FindByName(resolvedName) ??
                                                 index.FindByName(mName) ??
                                                 index.FindByBaseName(Path.GetFileNameWithoutExtension(mName));
                                    if (mEntry != null) return mEntry;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static byte[] ProcessAssetBytes(string inputPath, ManifestEntry entry, ModCompression compression)
        {
            bool isEncrypted = CheckIsEncrypted(inputPath, out bool isUnityFS);

            if (isEncrypted)
            {
                // Already Cygames XOR-encrypted: direct copy without double XOR
                return File.ReadAllBytes(inputPath);
            }

            byte[] payload;

            if (isUnityFS && compression != ModCompression.Original)
            {
                using var fs = File.OpenRead(inputPath);
                AssetBundleFile bundle = new();
                bundle.Read(new AssetsFileReader(fs));

                AssetBundleCompressionType targetComp = compression switch
                {
                    ModCompression.LZ4HC => AssetBundleCompressionType.LZ4,
                    ModCompression.LZ4Fast => AssetBundleCompressionType.LZ4Fast,
                    ModCompression.LZMA => AssetBundleCompressionType.LZMA,
                    ModCompression.None => AssetBundleCompressionType.None,
                    _ => AssetBundleCompressionType.LZ4
                };

                using MemoryStream unpackedStream = new();
                if (bundle.DataIsCompressed)
                {
                    bundle.Unpack(new AssetsFileWriter(unpackedStream));
                    unpackedStream.Position = 0;

                    bundle = new AssetBundleFile();
                    bundle.Read(new AssetsFileReader(unpackedStream));
                }

                using MemoryStream packedStream = new();
                bundle.Pack(new AssetsFileWriter(packedStream), targetComp);
                payload = packedStream.ToArray();
            }
            else
            {
                payload = File.ReadAllBytes(inputPath);
            }

            // Apply XOR encryption if meta database requires it
            if (entry.EncryptionKey != 0 && payload.Length > 256)
            {
                AssetCrypto.EncryptData(payload, entry.EncryptionKey);
            }

            return payload;
        }

        public static async Task<InstallResult> InstallModAsync(
            ModPackage mod,
            ModCompression compression,
            IProgress<InstallProgressReport>? progress = null,
            CancellationToken cancellationToken = default)
        {
            InstallResult result = new();
            var manifestEntries = UmaDataHelper.GetManifestEntries();
            var lookupIndex = new ManifestLookupIndex(manifestEntries);

            string datDir = UmaDataHelper.ResourceDirectory;
            string backupDir = BackupDirectory;
            Directory.CreateDirectory(datDir);
            Directory.CreateDirectory(backupDir);

            int total = mod.AssetRelativePaths.Count;
            int processedCount = 0;
            int decodedCount = 0;
            int missingMetaCount = 0;
            int backedUpCount = 0;
            object lockObj = new();

            await Task.Run(() =>
            {
                Parallel.ForEach(mod.AssetRelativePaths, new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, relPath =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string fullInputPath = Path.Combine(mod.BaseAssetPath, relPath);
                    string fileName = Path.GetFileName(fullInputPath);

                    try
                    {
                        var entry = FindManifestEntry(fullInputPath, relPath, lookupIndex);
                        if (entry == null)
                        {
                            Interlocked.Increment(ref missingMetaCount);
                            return;
                        }

                        string hash = entry.HashName;
                        string hashPrefix = hash[..2];
                        string destDir = Path.Combine(datDir, hashPrefix);
                        string destPath = Path.Combine(destDir, hash);

                        string backupSubDir = Path.Combine(backupDir, hashPrefix);
                        string backupPath = Path.Combine(backupSubDir, hash);

                        // Safe one-time backup matching UMML.py
                        if (File.Exists(destPath))
                        {
                            if (!File.Exists(backupPath))
                            {
                                Directory.CreateDirectory(backupSubDir);
                                try
                                {
                                    File.Move(destPath, backupPath); // Backup original game asset
                                    Interlocked.Increment(ref backedUpCount);
                                }
                                catch { }
                            }
                            else
                            {
                                try { File.Delete(destPath); } catch { }
                            }
                        }

                        byte[] processedBytes = ProcessAssetBytes(fullInputPath, entry, compression);

                        Directory.CreateDirectory(destDir);
                        File.WriteAllBytes(destPath, processedBytes);
                        Interlocked.Increment(ref decodedCount);
                    }
                    catch (Exception ex)
                    {
                        lock (lockObj)
                        {
                            result.Errors.Add($"{fileName}: {ex.Message}");
                        }
                    }
                    finally
                    {
                        int current = Interlocked.Increment(ref processedCount);
                        progress?.Report(new InstallProgressReport
                        {
                            Current = current,
                            Total = total,
                            AssetName = fileName,
                            StatusMessage = $"Processed {current} / {total}: {fileName}"
                        });
                    }
                });
            }, cancellationToken);

            result.DecodedCount = decodedCount;
            result.MissingMetaCount = missingMetaCount;
            result.BackedUpCount = backedUpCount;
            return result;
        }

        public static List<BackupItem> GetBackupEntries()
        {
            List<BackupItem> items = [];
            string backupDir = BackupDirectory;

            if (!Directory.Exists(backupDir)) return items;

            var manifestEntries = UmaDataHelper.GetManifestEntries();
            var nameMap = manifestEntries.ToDictionary(e => e.HashName, e => e.Name, StringComparer.OrdinalIgnoreCase);

            var backupFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
            foreach (var bf in backupFiles)
            {
                string rel = Path.GetRelativePath(backupDir, bf).Replace('\\', '/');
                string hash = Path.GetFileName(bf);
                string display = nameMap.TryGetValue(hash, out string? friendly) && !string.IsNullOrEmpty(friendly)
                    ? friendly
                    : $"[Unknown] {hash}";

                items.Add(new BackupItem
                {
                    RelativePath = rel,
                    Hash = hash,
                    DisplayName = display
                });
            }

            items.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
            return items;
        }

        public static async Task<(int Restored, int CannotRestored)> RestoreFilesAsync(
            IEnumerable<string> relativePaths,
            IProgress<InstallProgressReport>? progress = null,
            CancellationToken cancellationToken = default)
        {
            string datDir = UmaDataHelper.ResourceDirectory;
            string backupDir = BackupDirectory;

            var pathList = relativePaths.ToList();
            int total = pathList.Count;
            int restored = 0;
            int cannotRestored = 0;

            await Task.Run(() =>
            {
                for (int i = 0; i < total; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string rel = pathList[i];
                    string backupFile = Path.Combine(backupDir, rel);
                    string datFile = Path.Combine(datDir, rel);

                    progress?.Report(new InstallProgressReport
                    {
                        Current = i + 1,
                        Total = total,
                        AssetName = Path.GetFileName(backupFile),
                        StatusMessage = $"Restoring {i + 1} / {total}: {Path.GetFileName(backupFile)}"
                    });

                    try
                    {
                        string destFolder = Path.GetDirectoryName(datFile)!;
                        Directory.CreateDirectory(destFolder);

                        if (File.Exists(datFile))
                        {
                            File.Delete(datFile);
                            File.Move(backupFile, datFile);
                            CleanupEmptyParentDirs(backupFile, backupDir);
                            restored++;
                        }
                        else
                        {
                            cannotRestored++;
                        }
                    }
                    catch
                    {
                        cannotRestored++;
                    }
                }
            }, cancellationToken);

            return (restored, cannotRestored);
        }

        public static async Task<OrphanScanResult> ScanOrphanAssetsAsync(
            IProgress<InstallProgressReport>? progress = null,
            CancellationToken cancellationToken = default)
        {
            OrphanScanResult scan = new();
            string datDir = UmaDataHelper.ResourceDirectory;

            if (!Directory.Exists(datDir)) return scan;

            var manifestEntries = UmaDataHelper.GetManifestEntries();

            // Safety guard: game normally has tens of thousands of assets
            if (manifestEntries.Count < 500)
            {
                throw new InvalidOperationException(
                    "Meta database returned insufficient valid assets (< 500).\n" +
                    "Clean up was aborted to protect your game installation from data loss.");
            }

            var validHashes = new HashSet<string>(
                manifestEntries
                    .Where(e => !string.IsNullOrEmpty(e.HashName) && e.HashName.Length >= 2)
                    .Select(e => $"{e.HashName[..2]}/{e.HashName}"),
                StringComparer.OrdinalIgnoreCase);

            var existingFiles = Directory.GetFiles(datDir, "*", SearchOption.AllDirectories);
            int total = existingFiles.Length;

            await Task.Run(() =>
            {
                for (int i = 0; i < total; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string fullPath = existingFiles[i];
                    string relPath = Path.GetRelativePath(datDir, fullPath).Replace('\\', '/');

                    var parts = relPath.Split('/');
                    if (parts.Length == 2 && parts[0].Length == 2)
                    {
                        if (!validHashes.Contains(relPath))
                        {
                            long sz = 0;
                            try { sz = new FileInfo(fullPath).Length; } catch { }

                            scan.OrphanFiles.Add(new OrphanAssetInfo
                            {
                                RelativePath = relPath,
                                FullPath = fullPath,
                                Size = sz
                            });
                            scan.TotalBytes += sz;
                        }
                    }

                    if (i % 200 == 0 || i == total - 1)
                    {
                        progress?.Report(new InstallProgressReport
                        {
                            Current = i + 1,
                            Total = total,
                            StatusMessage = $"Scanning dat files {i + 1} / {total}..."
                        });
                    }
                }
            }, cancellationToken);

            return scan;
        }

        public static async Task<(int Deleted, int Failed, long FreedBytes)> DeleteOrphansAsync(
            List<OrphanAssetInfo> orphans,
            IProgress<InstallProgressReport>? progress = null,
            CancellationToken cancellationToken = default)
        {
            int deleted = 0;
            int failed = 0;
            long freedBytes = 0;
            int total = orphans.Count;
            string datDir = UmaDataHelper.ResourceDirectory;

            await Task.Run(() =>
            {
                for (int i = 0; i < total; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var item = orphans[i];
                    try
                    {
                        File.Delete(item.FullPath);
                        deleted++;
                        freedBytes += item.Size;
                        CleanupEmptyParentDirs(item.FullPath, datDir);
                    }
                    catch
                    {
                        failed++;
                    }

                    if (i % 100 == 0 || i == total - 1)
                    {
                        progress?.Report(new InstallProgressReport
                        {
                            Current = i + 1,
                            Total = total,
                            StatusMessage = $"Deleting {i + 1} / {total}..."
                        });
                    }
                }
            }, cancellationToken);

            return (deleted, failed, freedBytes);
        }

        private static void CleanupEmptyParentDirs(string filePath, string stopDir)
        {
            try
            {
                string? parent = Path.GetDirectoryName(filePath);
                if (parent != null && !parent.Equals(stopDir, StringComparison.OrdinalIgnoreCase) && Directory.Exists(parent))
                {
                    if (Directory.GetFiles(parent).Length == 0 && Directory.GetDirectories(parent).Length == 0)
                    {
                        Directory.Delete(parent, false);
                    }
                }
            }
            catch { }
        }
    }
}
