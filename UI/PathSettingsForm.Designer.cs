namespace UMML.UI
{
    partial class PathSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            platformLabel = new System.Windows.Forms.Label();
            platformComboBox = new System.Windows.Forms.ComboBox();
            pathsGroupBox = new System.Windows.Forms.GroupBox();
            autoDetectButton = new System.Windows.Forms.Button();
            gameStatusLabel = new System.Windows.Forms.Label();
            browseGameButton = new System.Windows.Forms.Button();
            gameDirTextBox = new System.Windows.Forms.TextBox();
            gameDirLabel = new System.Windows.Forms.Label();
            persistentStatusLabel = new System.Windows.Forms.Label();
            browsePersistentButton = new System.Windows.Forms.Button();
            persistentTextBox = new System.Windows.Forms.TextBox();
            persistentLabel = new System.Windows.Forms.Label();
            keyGroupBox = new System.Windows.Forms.GroupBox();
            keyDescLabel = new System.Windows.Forms.Label();
            testKeyButton = new System.Windows.Forms.Button();
            customKeyTextBox = new System.Windows.Forms.TextBox();
            customKeyLabel = new System.Windows.Forms.Label();
            keyComboBox = new System.Windows.Forms.ComboBox();
            keyLabel = new System.Windows.Forms.Label();
            saveButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            pathsGroupBox.SuspendLayout();
            keyGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // platformLabel
            // 
            platformLabel.AutoSize = true;
            platformLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            platformLabel.Location = new System.Drawing.Point(16, 16);
            platformLabel.Name = "platformLabel";
            platformLabel.Size = new System.Drawing.Size(95, 15);
            platformLabel.TabIndex = 0;
            platformLabel.Text = "Target Platform:";
            // 
            // platformComboBox
            // 
            platformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            platformComboBox.FormattingEnabled = true;
            platformComboBox.Location = new System.Drawing.Point(120, 13);
            platformComboBox.Name = "platformComboBox";
            platformComboBox.Size = new System.Drawing.Size(260, 23);
            platformComboBox.TabIndex = 1;
            platformComboBox.SelectedIndexChanged += PlatformComboBox_SelectedIndexChanged;
            // 
            // pathsGroupBox
            // 
            pathsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pathsGroupBox.Controls.Add(autoDetectButton);
            pathsGroupBox.Controls.Add(gameStatusLabel);
            pathsGroupBox.Controls.Add(browseGameButton);
            pathsGroupBox.Controls.Add(gameDirTextBox);
            pathsGroupBox.Controls.Add(gameDirLabel);
            pathsGroupBox.Controls.Add(persistentStatusLabel);
            pathsGroupBox.Controls.Add(browsePersistentButton);
            pathsGroupBox.Controls.Add(persistentTextBox);
            pathsGroupBox.Controls.Add(persistentLabel);
            pathsGroupBox.Location = new System.Drawing.Point(16, 50);
            pathsGroupBox.Name = "pathsGroupBox";
            pathsGroupBox.Size = new System.Drawing.Size(620, 185);
            pathsGroupBox.TabIndex = 2;
            pathsGroupBox.TabStop = false;
            pathsGroupBox.Text = "Platform Directories (GamePath.json)";
            // 
            // autoDetectButton
            // 
            autoDetectButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            autoDetectButton.Location = new System.Drawing.Point(440, 148);
            autoDetectButton.Name = "autoDetectButton";
            autoDetectButton.Size = new System.Drawing.Size(166, 26);
            autoDetectButton.TabIndex = 8;
            autoDetectButton.Text = "Auto-Detect This Platform";
            autoDetectButton.UseVisualStyleBackColor = true;
            autoDetectButton.Click += AutoDetectButton_Click;
            // 
            // gameStatusLabel
            // 
            gameStatusLabel.AutoSize = true;
            gameStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            gameStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            gameStatusLabel.Location = new System.Drawing.Point(15, 135);
            gameStatusLabel.Name = "gameStatusLabel";
            gameStatusLabel.Size = new System.Drawing.Size(88, 13);
            gameStatusLabel.TabIndex = 7;
            gameStatusLabel.Text = "Not configured";
            // 
            // browseGameButton
            // 
            browseGameButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            browseGameButton.Location = new System.Drawing.Point(531, 107);
            browseGameButton.Name = "browseGameButton";
            browseGameButton.Size = new System.Drawing.Size(75, 25);
            browseGameButton.TabIndex = 6;
            browseGameButton.Text = "Browse...";
            browseGameButton.UseVisualStyleBackColor = true;
            browseGameButton.Click += BrowseGameButton_Click;
            // 
            // gameDirTextBox
            // 
            gameDirTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gameDirTextBox.Location = new System.Drawing.Point(15, 108);
            gameDirTextBox.Name = "gameDirTextBox";
            gameDirTextBox.Size = new System.Drawing.Size(510, 23);
            gameDirTextBox.TabIndex = 5;
            gameDirTextBox.TextChanged += GameDirTextBox_TextChanged;
            // 
            // gameDirLabel
            // 
            gameDirLabel.AutoSize = true;
            gameDirLabel.Location = new System.Drawing.Point(15, 90);
            gameDirLabel.Name = "gameDirLabel";
            gameDirLabel.Size = new System.Drawing.Size(262, 15);
            gameDirLabel.TabIndex = 4;
            gameDirLabel.Text = "Game Directory (Contains executable / Game_dir):";
            // 
            // persistentStatusLabel
            // 
            persistentStatusLabel.AutoSize = true;
            persistentStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            persistentStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            persistentStatusLabel.Location = new System.Drawing.Point(15, 70);
            persistentStatusLabel.Name = "persistentStatusLabel";
            persistentStatusLabel.Size = new System.Drawing.Size(88, 13);
            persistentStatusLabel.TabIndex = 3;
            persistentStatusLabel.Text = "Not configured";
            // 
            // browsePersistentButton
            // 
            browsePersistentButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            browsePersistentButton.Location = new System.Drawing.Point(531, 42);
            browsePersistentButton.Name = "browsePersistentButton";
            browsePersistentButton.Size = new System.Drawing.Size(75, 25);
            browsePersistentButton.TabIndex = 2;
            browsePersistentButton.Text = "Browse...";
            browsePersistentButton.UseVisualStyleBackColor = true;
            browsePersistentButton.Click += BrowsePersistentButton_Click;
            // 
            // persistentTextBox
            // 
            persistentTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            persistentTextBox.Location = new System.Drawing.Point(15, 43);
            persistentTextBox.Name = "persistentTextBox";
            persistentTextBox.Size = new System.Drawing.Size(510, 23);
            persistentTextBox.TabIndex = 1;
            persistentTextBox.TextChanged += PersistentTextBox_TextChanged;
            // 
            // persistentLabel
            // 
            persistentLabel.AutoSize = true;
            persistentLabel.Location = new System.Drawing.Point(15, 25);
            persistentLabel.Name = "persistentLabel";
            persistentLabel.Size = new System.Drawing.Size(325, 15);
            persistentLabel.TabIndex = 0;
            persistentLabel.Text = "Persistent / Data Directory (Contains 'dat' and 'meta' database):";
            // 
            // keyGroupBox
            // 
            keyGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            keyGroupBox.Controls.Add(keyDescLabel);
            keyGroupBox.Controls.Add(testKeyButton);
            keyGroupBox.Controls.Add(customKeyTextBox);
            keyGroupBox.Controls.Add(customKeyLabel);
            keyGroupBox.Controls.Add(keyComboBox);
            keyGroupBox.Controls.Add(keyLabel);
            keyGroupBox.Location = new System.Drawing.Point(16, 245);
            keyGroupBox.Name = "keyGroupBox";
            keyGroupBox.Size = new System.Drawing.Size(620, 130);
            keyGroupBox.TabIndex = 3;
            keyGroupBox.TabStop = false;
            keyGroupBox.Text = "Meta Decryption Key";
            // 
            // keyDescLabel
            // 
            keyDescLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            keyDescLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            keyDescLabel.Location = new System.Drawing.Point(15, 96);
            keyDescLabel.Name = "keyDescLabel";
            keyDescLabel.Size = new System.Drawing.Size(460, 25);
            keyDescLabel.TabIndex = 5;
            keyDescLabel.Text = "Global: Steam Global | Japan: Steam Japan / DMM | Custom: User-specified hex key";
            // 
            // testKeyButton
            // 
            testKeyButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            testKeyButton.Location = new System.Drawing.Point(495, 23);
            testKeyButton.Name = "testKeyButton";
            testKeyButton.Size = new System.Drawing.Size(111, 26);
            testKeyButton.TabIndex = 4;
            testKeyButton.Text = "Test Key";
            testKeyButton.UseVisualStyleBackColor = true;
            testKeyButton.Click += TestKeyButton_Click;
            // 
            // customKeyTextBox
            // 
            customKeyTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            customKeyTextBox.Font = new System.Drawing.Font("Consolas", 8.5F);
            customKeyTextBox.Location = new System.Drawing.Point(120, 60);
            customKeyTextBox.Name = "customKeyTextBox";
            customKeyTextBox.PlaceholderText = "Enter custom 64-character hex key (or leave empty if unencrypted)";
            customKeyTextBox.Size = new System.Drawing.Size(486, 21);
            customKeyTextBox.TabIndex = 3;
            // 
            // customKeyLabel
            // 
            customKeyLabel.AutoSize = true;
            customKeyLabel.Location = new System.Drawing.Point(15, 63);
            customKeyLabel.Name = "customKeyLabel";
            customKeyLabel.Size = new System.Drawing.Size(95, 15);
            customKeyLabel.TabIndex = 2;
            customKeyLabel.Text = "Custom Hex Key:";
            // 
            // keyComboBox
            // 
            keyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            keyComboBox.FormattingEnabled = true;
            keyComboBox.Items.AddRange(new object[] { "Global", "Japan", "Custom" });
            keyComboBox.Location = new System.Drawing.Point(120, 25);
            keyComboBox.Name = "keyComboBox";
            keyComboBox.Size = new System.Drawing.Size(180, 23);
            keyComboBox.TabIndex = 1;
            keyComboBox.SelectedIndexChanged += KeyComboBox_SelectedIndexChanged;
            // 
            // keyLabel
            // 
            keyLabel.AutoSize = true;
            keyLabel.Location = new System.Drawing.Point(15, 28);
            keyLabel.Name = "keyLabel";
            keyLabel.Size = new System.Drawing.Size(89, 15);
            keyLabel.TabIndex = 0;
            keyLabel.Text = "Dropdown Key:";
            // 
            // saveButton
            // 
            saveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            saveButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            saveButton.Location = new System.Drawing.Point(436, 390);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(105, 28);
            saveButton.TabIndex = 4;
            saveButton.Text = "Save && Apply";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(547, 390);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(89, 28);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // PathSettingsForm
            // 
            AcceptButton = saveButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(652, 430);
            Controls.Add(cancelButton);
            Controls.Add(saveButton);
            Controls.Add(keyGroupBox);
            Controls.Add(pathsGroupBox);
            Controls.Add(platformComboBox);
            Controls.Add(platformLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PathSettingsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Game Path & Key Settings";
            Load += PathSettingsForm_Load;
            pathsGroupBox.ResumeLayout(false);
            pathsGroupBox.PerformLayout();
            keyGroupBox.ResumeLayout(false);
            keyGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label platformLabel;
        private System.Windows.Forms.ComboBox platformComboBox;
        private System.Windows.Forms.GroupBox pathsGroupBox;
        private System.Windows.Forms.Label persistentLabel;
        private System.Windows.Forms.TextBox persistentTextBox;
        private System.Windows.Forms.Button browsePersistentButton;
        private System.Windows.Forms.Label persistentStatusLabel;
        private System.Windows.Forms.Label gameDirLabel;
        private System.Windows.Forms.TextBox gameDirTextBox;
        private System.Windows.Forms.Button browseGameButton;
        private System.Windows.Forms.Label gameStatusLabel;
        private System.Windows.Forms.Button autoDetectButton;
        private System.Windows.Forms.GroupBox keyGroupBox;
        private System.Windows.Forms.Label keyLabel;
        private System.Windows.Forms.ComboBox keyComboBox;
        private System.Windows.Forms.Label customKeyLabel;
        private System.Windows.Forms.TextBox customKeyTextBox;
        private System.Windows.Forms.Button testKeyButton;
        private System.Windows.Forms.Label keyDescLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
