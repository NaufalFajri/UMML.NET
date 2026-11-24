using System;
using System.IO;
using System.Text.Json.Nodes;

namespace UMML.Data.DataDirectories
{
    public class DmmDataDirectory : DataDirectory
    {
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string dmmDirectory = Path.Combine(appData, "dmmgameplayer5");
        private static readonly string dmmConfigPath = Path.Combine(dmmDirectory, "dmmgame.cnf");

        public override string Name => "DMM Games";

        public override bool Exists()
        {
            if (!File.Exists(dmmConfigPath)) return false;

            try
            {
                string configJson = File.ReadAllText(dmmConfigPath);
                JsonNode? json = JsonNode.Parse(configJson);
                JsonArray? contents = json?["contents"]?.AsArray();
                if (contents == null) return false;

                foreach (var product in contents)
                {
                    if (product?["productId"]?.ToString() != "umamusume") continue;
                    string? installDir = product?["detail"]?["path"]?.ToString();
                    if (installDir == null) return false;

                    string dataDir = Path.Combine(installDir, "umamusume_Data", "Persistent");
                    if (CheckDirectory(dataDir))
                    {
                        DataDirectoryPath = dataDir;
                        return true;
                    }
                }
            }
            catch
            {
                // ignore
            }

            return false;
        }
    }
}
