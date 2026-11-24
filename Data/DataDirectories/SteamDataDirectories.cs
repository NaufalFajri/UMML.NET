using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UMML.Data.DataDirectories
{
    internal static class SteamLibraries
    {
        private static readonly List<string> CachedLibraries = [];

        public static IReadOnlyList<string> GetLibraries()
        {
            if (CachedLibraries.Count > 0) return CachedLibraries;

            List<string> candidateSteamRoots =
            [
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
                @"C:\Steam",
                @"D:\Steam",
                @"E:\Steam",
                @"D:\SteamLibrary",
                @"E:\SteamLibrary"
            ];

            foreach (var root in candidateSteamRoots)
            {
                if (Directory.Exists(root) && !CachedLibraries.Contains(root, StringComparer.OrdinalIgnoreCase))
                {
                    CachedLibraries.Add(root);
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
                            if (Directory.Exists(lib) && !CachedLibraries.Contains(lib, StringComparer.OrdinalIgnoreCase))
                            {
                                CachedLibraries.Add(lib);
                            }
                        }
                    }
                    catch
                    {
                        // ignore malformed vdf
                    }
                }
            }

            return CachedLibraries;
        }
    }

    public class SteamJapanDataDirectory : DataDirectory
    {
        public override string Name => "Steam (Japan)";

        public override bool Exists()
        {
            foreach (var lib in SteamLibraries.GetLibraries())
            {
                string path = Path.Combine(lib, "steamapps", "common", "UmamusumePrettyDerby_Jpn", "UmamusumePrettyDerby_Jpn_Data", "Persistent");
                if (CheckDirectory(path))
                {
                    DataDirectoryPath = path;
                    return true;
                }
            }
            return false;
        }
    }

    public class SteamGlobalDataDirectory : DataDirectory
    {
        public override string Name => "Steam (Global)";

        public override bool Exists()
        {
            foreach (var lib in SteamLibraries.GetLibraries())
            {
                string path = Path.Combine(lib, "steamapps", "common", "UmamusumePrettyDerby", "UmamusumePrettyDerby_Data", "Persistent");
                if (CheckDirectory(path))
                {
                    DataDirectoryPath = path;
                    return true;
                }
            }
            return false;
        }
    }
}
