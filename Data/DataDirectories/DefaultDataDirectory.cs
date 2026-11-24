using System;
using System.IO;

namespace UMML.Data.DataDirectories
{
    public class DefaultDataDirectory : DataDirectory
    {
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string cygamesDirectory = Path.Combine(appData + "Low", "Cygames");
        private static readonly string umamusumeDirectory = Path.Combine(cygamesDirectory, "umamusume");
        private static readonly string umamusumeJpnDirectory = Path.Combine(cygamesDirectory, "UmamusumePrettyDerby_Jpn");
        private static readonly string umamusumeGlobalDirectory = Path.Combine(cygamesDirectory, "UmamusumePrettyDerby");

        public override string Name => "Default (AppData)";

        public override bool Exists()
        {
            string[] dataDirectories =
            [
                umamusumeDirectory,
                umamusumeJpnDirectory,
                umamusumeGlobalDirectory,
                Path.Combine(appData + "Low", "Komoe", "umamusume"),
                Path.Combine(appData + "Low", "KakaoGames", "Umamusume")
            ];

            foreach (var dataDirectory in dataDirectories)
            {
                if (CheckDirectory(dataDirectory))
                {
                    DataDirectoryPath = dataDirectory;
                    return true;
                }
            }

            return false;
        }
    }
}
