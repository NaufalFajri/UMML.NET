using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace UMML.Modding
{
    public class ModPackage
    {
        public string Title { get; set; } = "";
        public string Version { get; set; } = "1.0";
        public string Author { get; set; } = "Unknown";
        public string Description { get; set; } = "";
        public string Platform { get; set; } = "Standard Asset Structure";
        public string DirectoryPath { get; set; } = "";
        public string BaseAssetPath { get; set; } = "";
        public bool IsStandardMod { get; set; } = false;
        public string? FilterFolder { get; set; } = null;
        public List<string> AssetRelativePaths { get; } = [];

        public static ModPackage LoadFromDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Mod directory not found: {folderPath}");

            ModPackage package = new()
            {
                DirectoryPath = folderPath,
                Title = Path.GetFileName(folderPath),
                BaseAssetPath = folderPath
            };

            string jsonSettings = Path.Combine(folderPath, "setting.json");
            string yamlSettings = Path.Combine(folderPath, "setting.yaml");
            string ymlSettings = Path.Combine(folderPath, "setting.yml");

            if (File.Exists(jsonSettings))
            {
                package.IsStandardMod = true;
                try
                {
                    string text = File.ReadAllText(jsonSettings);
                    JsonNode? json = JsonNode.Parse(text);
                    if (json != null)
                    {
                        package.Title = json["title"]?.ToString() ?? json["name"]?.ToString() ?? package.Title;
                        package.Version = json["mod_version"]?.ToString() ?? json["version"]?.ToString() ?? "1.0";
                        package.Author = json["author"]?.ToString() ?? "Unknown";

                        var descNode = json["description"];
                        if (descNode is JsonArray arr)
                        {
                            var lines = arr.Select(x => x?.ToString() ?? "").ToList();
                            package.Description = string.Join(Environment.NewLine, lines);
                        }
                        else
                        {
                            package.Description = descNode?.ToString() ?? "";
                        }

                        package.Platform = json["platform"]?.ToString() ?? "Standard Asset Structure";
                    }
                }
                catch { }
            }
            else if (File.Exists(yamlSettings) || File.Exists(ymlSettings))
            {
                package.IsStandardMod = true;
                string targetYaml = File.Exists(yamlSettings) ? yamlSettings : ymlSettings;
                try
                {
                    string[] lines = File.ReadAllLines(targetYaml);
                    List<string> descLines = [];
                    bool inDescList = false;

                    foreach (var rawLine in lines)
                    {
                        string line = rawLine.TrimEnd();
                        string trimmed = line.Trim();

                        if (inDescList)
                        {
                            if (trimmed.StartsWith('-'))
                            {
                                descLines.Add(trimmed[1..].Trim().Trim('"', '\''));
                                continue;
                            }
                            else if (line.StartsWith("  ") || line.StartsWith('\t'))
                            {
                                descLines.Add(trimmed.Trim('"', '\''));
                                continue;
                            }
                            else
                            {
                                inDescList = false;
                            }
                        }

                        int colonIdx = line.IndexOf(':');
                        if (colonIdx <= 0) continue;
                        string key = line[..colonIdx].Trim().ToLowerInvariant();
                        string val = line[(colonIdx + 1)..].Trim().Trim('"', '\'');
                        switch (key)
                        {
                            case "title":
                            case "name":
                                if (!string.IsNullOrEmpty(val)) package.Title = val;
                                break;
                            case "mod_version":
                            case "version":
                                if (!string.IsNullOrEmpty(val)) package.Version = val;
                                break;
                            case "author":
                                if (!string.IsNullOrEmpty(val)) package.Author = val;
                                break;
                            case "description":
                                if (string.IsNullOrEmpty(val))
                                {
                                    inDescList = true;
                                }
                                else
                                {
                                    package.Description = val;
                                }
                                break;
                            case "platform":
                                if (!string.IsNullOrEmpty(val)) package.Platform = val;
                                break;
                        }
                    }

                    if (descLines.Count > 0)
                    {
                        package.Description = string.Join(Environment.NewLine, descLines);
                    }
                }
                catch { }
            }
            else
            {
                package.IsStandardMod = false;
            }

            // Determine base asset source and platform filters
            if (package.IsStandardMod)
            {
                string assetsSub = Path.Combine(folderPath, "assets");
                if (Directory.Exists(assetsSub))
                {
                    package.BaseAssetPath = assetsSub;
                }
                package.FilterFolder = null;
            }
            else
            {
                package.BaseAssetPath = folderPath;
                string folderName = Path.GetFileName(folderPath);
                if (string.IsNullOrEmpty(folderName)) folderName = "Custom Mod";

                package.Title = $"Custom Mod: {folderName}";
                package.Version = "Format: Raw / Custom Mod (Non-UMML)";
                package.FilterFolder = DetectPlatformFilter(folderPath, out string detectedPlatform);
                package.Platform = detectedPlatform;
            }

            // Enumerate and validate assets
            var allFiles = Directory.GetFiles(package.BaseAssetPath, "*", SearchOption.AllDirectories);
            string[] ignoredFiles = ["setting.yml", "setting.json", "meta.json", "mod_pack.png", "desktop.ini", ".ds_store", "thumbs.db"];
            string[] ignoredExtensions = [".txt", ".md", ".png", ".jpg", ".jpeg", ".zip", ".bat", ".py"];

            foreach (var file in allFiles)
            {
                string fileName = Path.GetFileName(file);
                string fileNameLower = fileName.ToLowerInvariant();
                string ext = Path.GetExtension(file).ToLowerInvariant();
                string stem = Path.GetFileNameWithoutExtension(file);

                if (fileName.StartsWith('.') || ignoredFiles.Contains(fileNameLower) || ignoredExtensions.Contains(ext))
                    continue;

                string relPath = Path.GetRelativePath(package.BaseAssetPath, file).Replace('\\', '/');
                var pathParts = relPath.Split('/');

                // Ignore files in preview folder
                if (pathParts.Contains("preview", StringComparer.OrdinalIgnoreCase))
                    continue;

                // Apply filter_folder if specified (e.g. PC-s, DMM, GLO)
                if (!string.IsNullOrEmpty(package.FilterFolder))
                {
                    if (!pathParts.Contains(package.FilterFolder, StringComparer.OrdinalIgnoreCase))
                        continue;
                }

                // Upfront asset file validation matching UMML.py:
                // 1. Audio / Video (.acb, .awb, .usm)
                if (ext is ".acb" or ".awb" or ".usm")
                {
                    package.AssetRelativePaths.Add(relPath);
                    continue;
                }

                // 2. 32-char alphanumeric hash
                if ((fileName.Length == 32 && IsAlphanumeric(fileName)) || (stem.Length == 32 && IsAlphanumeric(stem)))
                {
                    package.AssetRelativePaths.Add(relPath);
                    continue;
                }

                // 3. Binary magic bytes: UnityFS, @UTF, AFS2, CRID
                try
                {
                    using var fs = File.OpenRead(file);
                    if (fs.Length >= 4)
                    {
                        byte[] magic = new byte[16];
                        int read = fs.Read(magic, 0, 16);
                        string magicAscii = Encoding.ASCII.GetString(magic, 0, read);
                        if (magicAscii.StartsWith("UnityFS") ||
                            magicAscii.StartsWith("@UTF") ||
                            magicAscii.StartsWith("AFS2") ||
                            magicAscii.StartsWith("CRID"))
                        {
                            package.AssetRelativePaths.Add(relPath);
                        }
                    }
                }
                catch { }
            }

            if (!package.IsStandardMod)
            {
                List<string> infoLines =
                [
                    $"Path: {folderPath}",
                    $"Total files detected: {package.AssetRelativePaths.Count}",
                    ""
                ];

                if (!string.IsNullOrEmpty(package.FilterFolder))
                {
                    infoLines.Add($"Detected Platform: {package.Platform}");
                    if (package.FilterFolder.Equals("PC-s", StringComparison.OrdinalIgnoreCase))
                    {
                        infoLines.Add("-> 'PC-s' detected: Raw assets will be encrypted automatically.");
                    }
                    else
                    {
                        infoLines.Add("-> Platform folder detected: UMML will check encryption upon install.");
                    }
                }
                else
                {
                    infoLines.Add("Platform: Standard Asset Structure");
                    infoLines.Add("-> UMML will check encryption upon install.");
                }

                infoLines.Add("");
                infoLines.Add("Ready to install. Click 'Install Mod' below to proceed.");
                package.Description = string.Join(Environment.NewLine, infoLines);
            }

            return package;
        }

        private static string? DetectPlatformFilter(string baseFolder, out string platformName)
        {
            var files = Directory.GetFiles(baseFolder, "*", SearchOption.AllDirectories);
            bool hasPcs = false;
            bool hasDmm = false;
            bool hasGlo = false;

            foreach (var f in files)
            {
                string rel = Path.GetRelativePath(baseFolder, f).Replace('\\', '/');
                var parts = rel.Split('/');
                if (parts.Contains("PC-s", StringComparer.OrdinalIgnoreCase)) hasPcs = true;
                if (parts.Contains("DMM", StringComparer.OrdinalIgnoreCase)) hasDmm = true;
                if (parts.Contains("GLO", StringComparer.OrdinalIgnoreCase)) hasGlo = true;
            }

            if (hasPcs)
            {
                platformName = "PC-s (Auto-encrypt)";
                return "PC-s";
            }
            if (hasDmm)
            {
                platformName = "DMM";
                return "DMM";
            }
            if (hasGlo)
            {
                platformName = "GLO";
                return "GLO";
            }

            platformName = "Standard Asset Structure";
            return null;
        }

        private static bool IsAlphanumeric(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }
    }
}
