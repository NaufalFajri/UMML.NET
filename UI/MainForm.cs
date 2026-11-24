using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UMML.Data;
using UMML.Modding;

namespace UMML.UI
{
    public partial class MainForm : Form
    {
        private ModPackage? currentMod;
        private bool isKeyUpdating = false;

        public MainForm()
        {
            InitializeComponent();
            compressionComboBox.SelectedIndex = 0; // LZ4HC (Default)

            // Enable Drag and Drop
            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            InitializeGameDirectory();
        }

        private void InitializeGameDirectory()
        {
            var config = GamePathSettings.Load();
            string targetPlat = !string.IsNullOrEmpty(config.SelectedPlatform) ? config.SelectedPlatform : "Steam Global";
            string targetKey = !string.IsNullOrEmpty(config.SelectedKey) ? config.SelectedKey : "Global";

            UmaDataHelper.CustomKey = config.CustomKey ?? "";
            UmaDataHelper.CurrentKeyMode = targetKey;

            if (!UmaDataHelper.TrySwitchPlatform(targetPlat, out string path))
            {
                var detected = UmaDataHelper.ScanPlatforms();
                if (detected.Count > 0)
                {
                    var first = detected.First();
                    UmaDataHelper.TrySwitchPlatform(first.Name, out path);
                }
            }

            UpdateUIFromDataHelper();
        }

        private void UpdateUIFromDataHelper()
        {
            isKeyUpdating = true;
            try
            {
                string plat = UmaDataHelper.CurrentPlatformName;
                string path = UmaDataHelper.UmamusumeDirectory;
                string keyMode = UmaDataHelper.CurrentKeyMode;

                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    gamePathValueLabel.Text = $"[{plat}] {path}";
                    activePlatformMenuItem.Text = $"Active: {plat}";
                }
                else
                {
                    gamePathValueLabel.Text = "Not configured. Click 'Path Settings...' to configure.";
                    activePlatformMenuItem.Text = "Active: None";
                }

                activeKeyMenuItem.Text = $"Key: {keyMode}";
                keyGlobalMenuItem.Checked = (keyMode == "Global");
                keyJapanMenuItem.Checked = (keyMode == "Japan");
                keyCustomMenuItem.Checked = (keyMode == "Custom");

                keyComboBox.SelectedItem = keyMode;
                optionsKeyComboBox.SelectedItem = keyMode;

                keyDescLabel.Text = keyMode switch
                {
                    "Global" => "Key: Global (Steam Global)",
                    "Japan" => "Key: Japan (Steam Japan / DMM)",
                    "Custom" => string.IsNullOrEmpty(UmaDataHelper.CustomKey)
                        ? "Key: Custom (Not configured)"
                        : $"Key: Custom ({UmaDataHelper.CustomKey[..Math.Min(16, UmaDataHelper.CustomKey.Length)]}...)",
                    _ => "Key: Global"
                };
            }
            finally
            {
                isKeyUpdating = false;
            }
        }

        private void KeyComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isKeyUpdating) return;
            string selected = keyComboBox.SelectedItem?.ToString() ?? "Global";
            SetKeyMode(selected);
        }

        private void OptionsKeyComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isKeyUpdating) return;
            string selected = optionsKeyComboBox.SelectedItem?.ToString() ?? "Global";
            SetKeyMode(selected);
        }

        private void SetKeyMode(string keyMode)
        {
            if (keyMode == "Custom" && string.IsNullOrWhiteSpace(UmaDataHelper.CustomKey))
            {
                var res = MessageBox.Show(
                    "Custom encryption key is not set.\n\nWould you like to open Path & Key Settings to enter it?",
                    "Custom Key Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (res == DialogResult.Yes)
                {
                    OpenPathSettings();
                    return;
                }
            }

            UmaDataHelper.CurrentKeyMode = keyMode;

            var config = GamePathSettings.Current;
            config.SelectedKey = keyMode;
            GamePathSettings.Save(config);

            UpdateUIFromDataHelper();
            statusLabel.Text = $"Active decryption key set to {keyMode}.";
        }

        private void SwitchPlatform(string platformName)
        {
            if (UmaDataHelper.TrySwitchPlatform(platformName, out string newPath))
            {
                UpdateUIFromDataHelper();
                statusLabel.Text = $"Switched platform to {platformName} (Key: {UmaDataHelper.CurrentKeyMode}).";
                MessageBox.Show(
                    $"Successfully switched to {platformName}.\n\nPath:\n{newPath}\nKey Mode: {UmaDataHelper.CurrentKeyMode}",
                    "Platform Changed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                var res = MessageBox.Show(
                    $"Platform '{platformName}' was not found at its standard locations.\n\n" +
                    "Would you like to open Path Settings to locate its folders manually?",
                    "Platform Not Detected",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                {
                    OpenPathSettings();
                }
            }
        }

        private void ChangeGamePathButton_Click(object? sender, EventArgs e)
        {
            OpenPathSettings();
        }

        private void OpenPathSettings()
        {
            using PathSettingsForm form = new();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                UpdateUIFromDataHelper();
                statusLabel.Text = "Game paths and keys updated.";
            }
        }

        private void SelectCustomDirectory_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "Select Umamusume Data Directory (containing 'dat' and 'meta')";
            dialog.UseDescriptionForTitle = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selected = dialog.SelectedPath;
                if (UmaDataHelper.CheckDirectory(selected))
                {
                    UmaDataHelper.UmamusumeDirectory = selected;
                    UmaDataHelper.CurrentPlatformName = "Custom";
                    UmaDataHelper.Initialize();
                    UmaDataHelper.ReloadManifest();

                    var config = GamePathSettings.Current;
                    config.SelectedPlatform = "Custom";
                    var entry = config.GetOrCreatePlatform("Custom");
                    entry.Persistent_dir = selected;
                    GamePathSettings.Save(config);

                    UpdateUIFromDataHelper();
                    statusLabel.Text = "Custom data directory applied.";
                }
                else
                {
                    MessageBox.Show(
                        "Selected directory is not a valid Umamusume data directory.\nIt must contain the 'dat' folder and 'meta' database file.",
                        "Invalid Directory",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }

        private void PreviewButton_Click(object? sender, EventArgs e)
        {
            string modPath = modPathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(modPath) || !Directory.Exists(modPath))
            {
                MessageBox.Show("Please select a valid mod folder first.", "No Mod", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string previewDir = Path.Combine(modPath, "preview");
            if (!Directory.Exists(previewDir))
            {
                MessageBox.Show("No 'preview' folder found in the selected mod.", "No Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] allowedExts = [".png", ".jpg", ".jpeg", ".bmp"];
            var images = Directory.GetFiles(previewDir)
                .Where(f => allowedExts.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(Path.GetFileName(f), @"\d+");
                    return match.Success && int.TryParse(match.Value, out int n) ? n : int.MaxValue;
                })
                .ThenBy(f => f)
                .ToList();

            if (images.Count == 0)
            {
                MessageBox.Show("No preview images found.", "No Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(images[0]) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HelpInstallButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Installing Mods in UMML:\n\n" +
                "1. Click 'Browse' to select any mod folder.\n" +
                "   (Supports standard UMML mods, raw asset folders, and PC-s mods)\n\n" +
                "2. UMML will automatically detect the mod structure and platform.\n\n" +
                "3. Select your game Platform & Key from the top bar or Options:\n" +
                "   - Global: for Steam Global\n" +
                "   - Japan: for Steam Japan or DMM\n" +
                "   - Custom: for custom hex key\n\n" +
                "4. Click 'Install Mod' to install with ultra-fast parallel compression.",
                "How do I install a mod?",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void AboutMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "UMML (Uma Musume Mod Loader) .NET 8 Standalone\n" +
                "Version 2.1.0\n\n" +
                "Features:\n" +
                "- High-performance parallel AssetsTools.NET compression\n" +
                "- Multi-platform detection (Steam Global, Steam Japan, DMM, Komoe, Kakao)\n" +
                "- Dropdown Decryption Key (Global, Japan, Custom)\n" +
                "- Per-platform GamePath.json settings configuration\n" +
                "- Automatic XOR encryption, safe backups, and clean-up.",
                "About UMML",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (files.Length > 0 && Directory.Exists(files[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (files.Length > 0 && Directory.Exists(files[0]))
                {
                    modPathTextBox.Text = files[0];
                    LoadMod(files[0]);
                }
            }
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "Select Mod Folder";
            dialog.UseDescriptionForTitle = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                modPathTextBox.Text = dialog.SelectedPath;
                LoadMod(dialog.SelectedPath);
            }
        }

        private void ReloadButton_Click(object? sender, EventArgs e)
        {
            string path = modPathTextBox.Text.Trim();
            if (Directory.Exists(path))
            {
                LoadMod(path);
            }
            else
            {
                MessageBox.Show("Specified folder path does not exist.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void LoadMod(string folderPath)
        {
            try
            {
                currentMod = ModPackage.LoadFromDirectory(folderPath);

                modTitleLabel.Text = currentMod.Title;
                modVersionLabel.Text = $"Version: {currentMod.Version}";
                modAuthorLabel.Text = $"Author: {currentMod.Author}";
                modPlatformLabel.Text = $"Platform: {currentMod.Platform}";
                modAssetCountLabel.Text = $"Asset Count: {currentMod.AssetRelativePaths.Count}";
                modDescriptionTextBox.Text = currentMod.Description;

                assetListView.BeginUpdate();
                assetListView.Items.Clear();
                foreach (var relPath in currentMod.AssetRelativePaths)
                {
                    assetListView.Items.Add(new ListViewItem(relPath));
                }
                assetListView.EndUpdate();

                installButton.Enabled = currentMod.AssetRelativePaths.Count > 0;
                statusLabel.Text = $"Loaded {currentMod.AssetRelativePaths.Count} asset(s) from mod.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load mod: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                installButton.Enabled = false;
            }
        }

        private ModCompression GetSelectedCompression()
        {
            return compressionComboBox.SelectedIndex switch
            {
                0 => ModCompression.LZ4HC,
                1 => ModCompression.LZ4Fast,
                2 => ModCompression.LZMA,
                3 => ModCompression.Original,
                4 => ModCompression.None,
                _ => ModCompression.LZ4HC
            };
        }

        private void SetControlsEnabled(bool enabled)
        {
            browseButton.Enabled = enabled;
            reloadButton.Enabled = enabled;
            previewButton.Enabled = enabled;
            helpInstallButton.Enabled = enabled;
            installButton.Enabled = enabled && currentMod != null && currentMod.AssetRelativePaths.Count > 0;
            restoreButton.Enabled = enabled;
            cleanupButton.Enabled = enabled;
            compressionComboBox.Enabled = enabled;
            keyComboBox.Enabled = enabled;
            optionsKeyComboBox.Enabled = enabled;
            changeGamePathButton.Enabled = enabled;
            platformToolStripMenuItem.Enabled = enabled;
        }

        private async void InstallButton_Click(object? sender, EventArgs e)
        {
            if (currentMod == null || currentMod.AssetRelativePaths.Count == 0)
            {
                MessageBox.Show("Please select a valid mod folder first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!UmaDataHelper.CheckDirectory())
            {
                MessageBox.Show("Game data folder or meta database file not found.\nPlease configure paths in 'Path Settings...'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ModManager.IsGameRunning())
            {
                var warn = MessageBox.Show(
                    "The game appears to be currently running.\n\n" +
                    "Installing assets while the game is running may cause errors or crashes.\n\n" +
                    "Do you want to continue anyway?",
                    "Game Running Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (warn != DialogResult.Yes) return;
            }

            string compChoice = compressionComboBox.SelectedItem?.ToString() ?? "LZ4HC (Default)";
            var confirm = MessageBox.Show(
                $"Install mod '{currentMod.Title}' now?\n\nCompression: {compChoice}\nTarget: {UmaDataHelper.CurrentPlatformName}\nKey: {UmaDataHelper.CurrentKeyMode}",
                "Confirm Installation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            SetControlsEnabled(false);
            progressBar.Value = 0;
            progressBar.Maximum = currentMod.AssetRelativePaths.Count;

            var progress = new Progress<InstallProgressReport>(report =>
            {
                if (report.Current <= progressBar.Maximum)
                    progressBar.Value = report.Current;
                statusLabel.Text = report.StatusMessage;
            });

            try
            {
                var compression = GetSelectedCompression();
                var result = await ModManager.InstallModAsync(currentMod, compression, progress);

                progressBar.Value = progressBar.Maximum;
                statusLabel.Text = $"{result.DecodedCount} asset(s) processed. {result.MissingMetaCount} skipped.";

                if (result.DecodedCount == 0 && result.MissingMetaCount == 0)
                {
                    MessageBox.Show("No valid assets found in the mod folder.", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show(
                    $"{result.DecodedCount} asset(s) processed and installed successfully!\n" +
                    $"{result.MissingMetaCount} asset(s) were not found in meta database.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install assets:\n{ex.Message}", "Install Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Installation failed.";
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private void RestoreButton_Click(object? sender, EventArgs e)
        {
            if (ModManager.IsGameRunning())
            {
                var warn = MessageBox.Show(
                    "The game appears to be currently running.\n\n" +
                    "Restoring assets while the game is running may cause errors or crashes.\n\n" +
                    "Do you want to continue anyway?",
                    "Game Running Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (warn != DialogResult.Yes) return;
            }

            if (!Directory.Exists(ModManager.BackupDirectory))
            {
                MessageBox.Show("No backup files found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var entries = ModManager.GetBackupEntries();
            if (entries.Count == 0)
            {
                MessageBox.Show("No backup files found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using RestoreForm restoreForm = new();
            restoreForm.ShowDialog(this);
        }

        private async void CleanupButton_Click(object? sender, EventArgs e)
        {
            if (ModManager.IsGameRunning())
            {
                MessageBox.Show(
                    "The game appears to be currently running.\n\n" +
                    "Please close the game before performing asset clean up.",
                    "Game Running Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!UmaDataHelper.CheckDirectory())
            {
                MessageBox.Show("Game data folder or meta database not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetControlsEnabled(false);
            progressBar.Style = ProgressBarStyle.Marquee;
            statusLabel.Text = "Scanning assets...";

            var scanProgress = new Progress<InstallProgressReport>(report =>
            {
                statusLabel.Text = report.StatusMessage;
            });

            OrphanScanResult scanResult;
            try
            {
                scanResult = await ModManager.ScanOrphanAssetsAsync(scanProgress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Clean Up Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Clean up aborted.";
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                SetControlsEnabled(true);
                return;
            }

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;

            if (scanResult.OrphanFiles.Count == 0)
            {
                MessageBox.Show("No unused assets were found.", "Clean Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Ready";
                SetControlsEnabled(true);
                return;
            }

            var confirm = MessageBox.Show(
                $"Found {scanResult.OrphanFiles.Count} unused asset(s) (~{scanResult.FormattedSize}).\n\n" +
                "These files are no longer referenced by the current meta database\n" +
                "(usually leftovers from game updates or removed mods).\n\n" +
                "Permanently delete them?",
                "Clean Up Assets",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                statusLabel.Text = "Ready";
                SetControlsEnabled(true);
                return;
            }

            progressBar.Maximum = scanResult.OrphanFiles.Count;
            progressBar.Value = 0;

            var deleteProgress = new Progress<InstallProgressReport>(report =>
            {
                if (report.Current <= progressBar.Maximum)
                    progressBar.Value = report.Current;
                statusLabel.Text = report.StatusMessage;
            });

            try
            {
                var (deleted, failed, freedBytes) = await ModManager.DeleteOrphansAsync(scanResult.OrphanFiles, deleteProgress);
                string freedStr = OrphanScanResult.FormatSize(freedBytes);

                statusLabel.Text = $"Clean up finished! Deleted: {deleted} (~{freedStr} freed)";
                MessageBox.Show(
                    $"Clean up finished!\n\n" +
                    $"Deleted: {deleted} asset(s) (~{freedStr} freed)\n" +
                    $"Failed: {failed} asset(s)",
                    "Done",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed during cleanup:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Cleanup error.";
            }
            finally
            {
                progressBar.Value = 0;
                SetControlsEnabled(true);
            }
        }
    }
}
