using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UMML.Modding;

namespace UMML.UI
{
    public partial class RestoreForm : Form
    {
        private List<BackupItem> backupItems = [];

        public RestoreForm()
        {
            InitializeComponent();
        }

        private void RestoreForm_Load(object? sender, EventArgs e)
        {
            RefreshEntries();
        }

        private void RefreshEntries()
        {
            backupItems = ModManager.GetBackupEntries();
            backupListBox.BeginUpdate();
            backupListBox.Items.Clear();
            foreach (var item in backupItems)
            {
                backupListBox.Items.Add(item);
            }
            backupListBox.EndUpdate();

            statusLabel.Text = $"Found {backupItems.Count} backed-up asset(s).";
            restoreSelectedButton.Enabled = backupItems.Count > 0;
            restoreAllButton.Enabled = backupItems.Count > 0;
        }

        private async void RestoreSelectedButton_Click(object? sender, EventArgs e)
        {
            var selectedIndices = backupListBox.SelectedIndices.Cast<int>().ToList();
            if (selectedIndices.Count == 0)
            {
                MessageBox.Show("No files selected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Restore {selectedIndices.Count} selected file(s)?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            SetBusy(true);
            progressBar.Maximum = selectedIndices.Count;
            progressBar.Value = 0;

            var pathsToRestore = selectedIndices.Select(i => backupItems[i].RelativePath).ToList();

            var progress = new Progress<InstallProgressReport>(report =>
            {
                if (report.Current <= progressBar.Maximum)
                    progressBar.Value = report.Current;
                statusLabel.Text = report.StatusMessage;
            });

            try
            {
                var (restored, cannotRestored) = await ModManager.RestoreFilesAsync(pathsToRestore, progress);
                RefreshEntries();
                MessageBox.Show(
                    $"Restored {restored} selected file(s).\n" +
                    $"{cannotRestored} could not be restored because the original hash no longer exists in the current game data.",
                    "Done",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restore assets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
                progressBar.Value = 0;
            }
        }

        private async void RestoreAllButton_Click(object? sender, EventArgs e)
        {
            if (backupItems.Count == 0) return;

            var confirm = MessageBox.Show(
                "Restore all original assets? This will overwrite all modified files.",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            SetBusy(true);
            progressBar.Maximum = backupItems.Count;
            progressBar.Value = 0;

            var pathsToRestore = backupItems.Select(b => b.RelativePath).ToList();

            var progress = new Progress<InstallProgressReport>(report =>
            {
                if (report.Current <= progressBar.Maximum)
                    progressBar.Value = report.Current;
                statusLabel.Text = report.StatusMessage;
            });

            try
            {
                var (restored, cannotRestored) = await ModManager.RestoreFilesAsync(pathsToRestore, progress);
                RefreshEntries();
                MessageBox.Show(
                    $"Restored {restored} file(s).\n" +
                    $"{cannotRestored} could not be restored because the original hash no longer exists in the current game data.",
                    "Done",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restore assets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
                progressBar.Value = 0;
            }
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void SetBusy(bool busy)
        {
            restoreSelectedButton.Enabled = !busy && backupItems.Count > 0;
            restoreAllButton.Enabled = !busy && backupItems.Count > 0;
            closeButton.Enabled = !busy;
            backupListBox.Enabled = !busy;
        }
    }
}
