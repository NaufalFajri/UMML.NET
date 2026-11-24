using System;
using System.IO;
using System.Windows.Forms;
using UMML.Modding;
using UMML.UI;

namespace UMML
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--test")
            {
                RunSelfTest();
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void RunSelfTest()
        {
            Console.WriteLine("Running UMML Self-Test...");
            string baseTestDir = @"C:\Users\LENOVO\Documents\_UMPD_ML\UMML\test asset";
            string pfbFile = Path.Combine(baseTestDir, "pfb_bdy1001_00");
            string compFile = Path.Combine(baseTestDir, "comp");
            string encFile = Path.Combine(baseTestDir, "ABM52XFA3AFERJS5PO6NW5X52RIWJDKA");

            // 1. Test CheckIsEncrypted
            bool isEncPfb = ModManager.CheckIsEncrypted(pfbFile, out bool isUnityFSPfb);
            bool isEncComp = ModManager.CheckIsEncrypted(compFile, out bool isUnityFSComp);
            bool isEncEnc = ModManager.CheckIsEncrypted(encFile, out bool isUnityFSEnc);

            Console.WriteLine($"pfb_bdy1001_00: isEnc={isEncPfb}, isUnityFS={isUnityFSPfb}");
            Console.WriteLine($"comp: isEnc={isEncComp}, isUnityFS={isUnityFSComp}");
            Console.WriteLine($"ABM52XFA3AFERJS5PO6NW5X52RIWJDKA: isEnc={isEncEnc}, isUnityFS={isUnityFSEnc}");

            // 2. Test ModPackage loading on raw test asset directory
            var mod = ModPackage.LoadFromDirectory(baseTestDir);
            Console.WriteLine($"ModPackage Title: {mod.Title}");
            Console.WriteLine($"ModPackage Version: {mod.Version}");
            Console.WriteLine($"ModPackage Platform: {mod.Platform}");
            Console.WriteLine($"ModPackage Assets Found: {mod.AssetRelativePaths.Count} -> [{string.Join(", ", mod.AssetRelativePaths)}]");

            // 3. Test ProcessAssetBytes with various compression modes
            Data.ManifestEntry mockEntry = new()
            {
                Name = "3d/chara/body/bdy1001_00/pfb_bdy1001_00",
                HashName = "DUMMYHASH1234567890123456789012",
                EncryptionKey = 0
            };

            // 4. Test FindManifestEntry with internal bundle resolution
            List<Data.ManifestEntry> testEntries =
            [
                new Data.ManifestEntry { Name = "3d/chara/body/bdy1001_00/pfb_bdy1001_00", HashName = "DUMMYHASH1234567890123456789012", EncryptionKey = 123456789 },
                new Data.ManifestEntry { Name = "sound/b/snd_bgm.acb", HashName = "AUDIOHASH1234567890123456789012", EncryptionKey = 0 }
            ];
            var lookup = new ModManager.ManifestLookupIndex(testEntries);

            var resolvedPfb = ModManager.FindManifestEntry(pfbFile, "pfb_bdy1001_00", lookup);
            Console.WriteLine($"Resolve pfb_bdy1001_00 via internal bundle: {(resolvedPfb != null ? resolvedPfb.Name : "NOT FOUND")}");

            foreach (var comp in new[] { ModCompression.Original, ModCompression.LZ4HC, ModCompression.LZ4Fast, ModCompression.LZMA, ModCompression.None })
            {
                try
                {
                    byte[] bytes = ModManager.ProcessAssetBytes(pfbFile, mockEntry, comp);
                    Console.WriteLine($"Compression {comp}: Success, output bytes={bytes.Length}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Compression {comp} FAILED: {ex}");
                }
            }

            Console.WriteLine($"IsGameRunning: {ModManager.IsGameRunning()}");

            // 5. Test Platform Detection, Settings, and Key Decryption
            Console.WriteLine("\n--- Testing Platform Detection & Decryption Keys ---");
            string configPath = Data.GamePathSettings.GetConfigFilePath();
            Console.WriteLine($"GamePath.json path: {configPath}");
            var config = Data.GamePathSettings.Load();
            Console.WriteLine($"Current config SelectedPlatform: {config.SelectedPlatform}, SelectedKey: {config.SelectedKey}");

            var platforms = Data.GamePathSettings.SupportedPlatforms;
            foreach (var plat in platforms)
            {
                var det = Data.PlatformDetector.DetectPlatform(plat);
                Console.WriteLine($"Platform '{plat}': IsValid={det.IsValid}, Persistent='{det.PersistentDirectory}', Game='{det.GameDirectory}'");
            }

            var detectedJp = Data.PlatformDetector.DetectPlatform("Steam Japan");
            if (detectedJp.IsValid && File.Exists(detectedJp.MetaFile))
            {
                var jpTest = Data.UmaDataHelper.TestKey(Data.UmaDataHelper.KeyJapan, detectedJp.MetaFile);
                Console.WriteLine($"Steam Japan Meta Decryption with Japan Key: Success={jpTest.success}, Count={jpTest.count}, Msg={jpTest.message}");

                var gbTest = Data.UmaDataHelper.TestKey(Data.UmaDataHelper.KeyGlobal, detectedJp.MetaFile);
                Console.WriteLine($"Steam Japan Meta Decryption with Global Key (Expected Fail): Success={gbTest.success}, Msg={gbTest.message}");
            }

            Console.WriteLine("UMML Self-Test completed successfully!");
        }
    }
}
