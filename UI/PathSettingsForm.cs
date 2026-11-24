using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UMML.Data;

namespace UMML.UI
{
    public partial class PathSettingsForm : Form
    {
        private readonly GamePathConfig config;
        private bool isUpdating = false;

        public PathSettingsForm()
        {
            InitializeComponent();
            config = GamePathSettings.Load();
        }

        private void PathSettingsForm_Load(object? sender, EventArgs e)
        {
            platformComboBox.Items.Clear();
            platformComboBox.Items.AddRange(new object[]
            {
                "Steam Global",
                "Steam Japan",
                "DMM",
                "Komoe",
                "Kakao"
            });

            string currentPlat = !string.IsNullOrEmpty(UmaDataHelper.CurrentPlatformName) &&
                                 platformComboBox.Items.Contains(UmaDataHelper.CurrentPlatformName)
                ? UmaDataHelper.CurrentPlatformName
                : config.SelectedPlatform;

            if (!platformComboBox.Items.Contains(currentPlat))
                currentPlat = "Steam Global";

            platformComboBox.SelectedItem = currentPlat;

            keyComboBox.Items.Clear();
            keyComboBox.Items.AddRange(new object[] { "Global", "Japan", "Custom" });

            string currentKey = UmaDataHelper.CurrentKeyMode;
            if (!keyComboBox.Items.Contains(currentKey))
                currentKey = "Global";

            keyComboBox.SelectedItem = currentKey;
            customKeyTextBox.Text = config.CustomKey ?? "";
            customKeyTextBox.Enabled = (currentKey == "Custom");

            LoadPlatformPaths(currentPlat);
        }

        private void PlatformComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (platformComboBox.SelectedItem is string plat)
            {
                LoadPlatformPaths(plat);
            }
        }

        private void LoadPlatformPaths(string platform)
        {
            isUpdating = true;
            try
            {
                var entry = config.GetOrCreatePlatform(platform);
                persistentTextBox.Text = entry.Persistent_dir ?? "";
                gameDirTextBox.Text = entry.Game_dir ?? "";
                UpdateValidationLabels();
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void SaveCurrentPlatformPaths()
        {
            if (platformComboBox.SelectedItem is string plat)
            {
                var entry = config.GetOrCreatePlatform(plat);
                entry.Persistent_dir = persistentTextBox.Text.Trim();
                entry.Game_dir = gameDirTextBox.Text.Trim();
            }
        }

        private void UpdateValidationLabels()
        {
            string p = persistentTextBox.Text.Trim();
            if (string.IsNullOrEmpty(p))
            {
                persistentStatusLabel.Text = "Not configured";
                persistentStatusLabel.ForeColor = Color.Gray;
            }
            else if (!Directory.Exists(p))
            {
                persistentStatusLabel.Text = "Directory does not exist";
                persistentStatusLabel.ForeColor = Color.Red;
            }
            else
            {
                bool hasMeta = File.Exists(Path.Combine(p, "meta"));
                bool hasDat = Directory.Exists(Path.Combine(p, "dat"));

                if (hasMeta && hasDat)
                {
                    persistentStatusLabel.Text = "✓ Valid ('dat' & 'meta' present)";
                    persistentStatusLabel.ForeColor = Color.DarkGreen;
                }
                else if (hasMeta)
                {
                    persistentStatusLabel.Text = "⚠ 'meta' found, but 'dat' folder missing";
                    persistentStatusLabel.ForeColor = Color.DarkOrange;
                }
                else
                {
                    persistentStatusLabel.Text = "✗ 'meta' database file not found in folder";
                    persistentStatusLabel.ForeColor = Color.Red;
                }
            }

            string g = gameDirTextBox.Text.Trim();
            if (string.IsNullOrEmpty(g))
            {
                gameStatusLabel.Text = "Not configured";
                gameStatusLabel.ForeColor = Color.Gray;
            }
            else if (Directory.Exists(g))
            {
                gameStatusLabel.Text = "✓ Directory exists";
                gameStatusLabel.ForeColor = Color.DarkGreen;
            }
            else
            {
                gameStatusLabel.Text = "Directory does not exist";
                gameStatusLabel.ForeColor = Color.Red;
            }
        }

        private void BrowsePersistentButton_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "Select Persistent Data Folder (folder containing 'dat' and 'meta')";
            dialog.UseDescriptionForTitle = true;
            if (Directory.Exists(persistentTextBox.Text))
                dialog.InitialDirectory = persistentTextBox.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                persistentTextBox.Text = dialog.SelectedPath;
                UpdateValidationLabels();
            }
        }

        private void BrowseGameButton_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "Select Game Installation Directory";
            dialog.UseDescriptionForTitle = true;
            if (Directory.Exists(gameDirTextBox.Text))
                dialog.InitialDirectory = gameDirTextBox.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gameDirTextBox.Text = dialog.SelectedPath;
                UpdateValidationLabels();
            }
        }

        private void AutoDetectButton_Click(object? sender, EventArgs e)
        {
            if (platformComboBox.SelectedItem is string plat)
            {
                var detected = PlatformDetector.DetectPlatform(plat);
                persistentTextBox.Text = detected.PersistentDirectory;
                gameDirTextBox.Text = detected.GameDirectory;
                UpdateValidationLabels();

                if (detected.IsValid)
                {
                    MessageBox.Show(
                        $"Auto-detection successful for {plat}!\n\n" +
                        $"Persistent: {detected.PersistentDirectory}\n" +
                        $"Game Dir: {detected.GameDirectory}",
                        "Auto-Detection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Auto-detection did not find complete files for {plat}.\n" +
                        "You can locate the folders manually using 'Browse...'.",
                        "Auto-Detection Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }

        private void KeyComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string mode = keyComboBox.SelectedItem?.ToString() ?? "Global";
            customKeyTextBox.Enabled = (mode == "Custom");
        }

        private void TestKeyButton_Click(object? sender, EventArgs e)
        {
            string p = persistentTextBox.Text.Trim();
            string metaPath = Path.Combine(p, "meta");

            if (!File.Exists(metaPath))
            {
                MessageBox.Show(
                    "Cannot test key because the 'meta' file does not exist at:\n" + metaPath,
                    "Meta Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string mode = keyComboBox.SelectedItem?.ToString() ?? "Global";
            string testKeyHex = mode switch
            {
                "Global" => UmaDataHelper.KeyGlobal,
                "Japan" => UmaDataHelper.KeyJapan,
                "Custom" => customKeyTextBox.Text.Trim(),
                _ => UmaDataHelper.KeyGlobal
            };

            var (success, count, message) = UmaDataHelper.TestKey(testKeyHex, metaPath);

            if (success)
            {
                MessageBox.Show(
                    $"Key test succeeded!\n\n{message}\n(Key Mode: {mode})",
                    "Key Valid",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Key test failed!\n\n{message}\n\nPlease verify that the selected key matches your game version/region.",
                    "Key Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PersistentTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (!isUpdating) UpdateValidationLabels();
        }

        private void GameDirTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (!isUpdating) UpdateValidationLabels();
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            SaveCurrentPlatformPaths();

            string selectedPlat = platformComboBox.SelectedItem?.ToString() ?? "Steam Global";
            string selectedKey = keyComboBox.SelectedItem?.ToString() ?? "Global";
            string custKey = customKeyTextBox.Text.Trim();

            config.SelectedPlatform = selectedPlat;
            config.SelectedKey = selectedKey;
            config.CustomKey = custKey;

            GamePathSettings.Save(config);

            // Apply to UmaDataHelper
            UmaDataHelper.CurrentPlatformName = selectedPlat;
            UmaDataHelper.CustomKey = custKey;
            UmaDataHelper.CurrentKeyMode = selectedKey;

            var activeEntry = config.GetOrCreatePlatform(selectedPlat);
            if (!string.IsNullOrEmpty(activeEntry.Persistent_dir))
            {
                UmaDataHelper.UmamusumeDirectory = activeEntry.Persistent_dir;
                UmaDataHelper.Initialize();
                UmaDataHelper.ReloadManifest();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
