namespace UMML.UI
{
    partial class MainForm
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
            mainMenuStrip = new MenuStrip();
            platformToolStripMenuItem = new ToolStripMenuItem();
            activePlatformMenuItem = new ToolStripMenuItem();
            activeKeyMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            steamGlobalMenuItem = new ToolStripMenuItem();
            steamJapanMenuItem = new ToolStripMenuItem();
            dmmMenuItem = new ToolStripMenuItem();
            komoeMenuItem = new ToolStripMenuItem();
            kakaoMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            keyGlobalMenuItem = new ToolStripMenuItem();
            keyJapanMenuItem = new ToolStripMenuItem();
            keyCustomMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            pathSettingsMenuItem = new ToolStripMenuItem();
            customPathMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            howDoIInstallModMenuItem = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            gamePathPanel = new Panel();
            changeGamePathButton = new Button();
            keyComboBox = new ComboBox();
            keyLabel = new Label();
            gamePathValueLabel = new Label();
            gamePathLabel = new Label();
            modPathPanel = new Panel();
            helpInstallButton = new Button();
            previewButton = new Button();
            reloadButton = new Button();
            browseButton = new Button();
            modPathTextBox = new TextBox();
            modPathLabel = new Label();
            mainSplitContainer = new SplitContainer();
            leftPanel = new Panel();
            optionsGroupBox = new GroupBox();
            keyDescLabel = new Label();
            optionsKeyComboBox = new ComboBox();
            optionsKeyLabel = new Label();
            compressionDescLabel = new Label();
            compressionComboBox = new ComboBox();
            compressionLabel = new Label();
            modInfoGroupBox = new GroupBox();
            modDescriptionTextBox = new TextBox();
            descLabel = new Label();
            modAssetCountLabel = new Label();
            modPlatformLabel = new Label();
            modAuthorLabel = new Label();
            modVersionLabel = new Label();
            modTitleLabel = new Label();
            rightPanel = new Panel();
            assetsGroupBox = new GroupBox();
            assetListView = new ListView();
            columnHeaderAsset = new ColumnHeader();
            bottomPanel = new Panel();
            statusLabel = new Label();
            progressBar = new ProgressBar();
            actionsPanel = new FlowLayoutPanel();
            installButton = new Button();
            restoreButton = new Button();
            cleanupButton = new Button();
            gamePathPanel.SuspendLayout();
            modPathPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            leftPanel.SuspendLayout();
            optionsGroupBox.SuspendLayout();
            modInfoGroupBox.SuspendLayout();
            rightPanel.SuspendLayout();
            assetsGroupBox.SuspendLayout();
            bottomPanel.SuspendLayout();
            actionsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { platformToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(840, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "mainMenuStrip";
            // 
            // platformToolStripMenuItem
            // 
            platformToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                activePlatformMenuItem,
                activeKeyMenuItem,
                toolStripSeparator1,
                steamGlobalMenuItem,
                steamJapanMenuItem,
                dmmMenuItem,
                komoeMenuItem,
                kakaoMenuItem,
                toolStripSeparator2,
                keyGlobalMenuItem,
                keyJapanMenuItem,
                keyCustomMenuItem,
                toolStripSeparator3,
                pathSettingsMenuItem,
                customPathMenuItem
            });
            platformToolStripMenuItem.Name = "platformToolStripMenuItem";
            platformToolStripMenuItem.Size = new Size(65, 20);
            platformToolStripMenuItem.Text = "Platform";
            // 
            // activePlatformMenuItem
            // 
            activePlatformMenuItem.Enabled = false;
            activePlatformMenuItem.Name = "activePlatformMenuItem";
            activePlatformMenuItem.Size = new Size(215, 22);
            activePlatformMenuItem.Text = "Active: Detecting...";
            // 
            // activeKeyMenuItem
            // 
            activeKeyMenuItem.Enabled = false;
            activeKeyMenuItem.Name = "activeKeyMenuItem";
            activeKeyMenuItem.Size = new Size(215, 22);
            activeKeyMenuItem.Text = "Key: Detecting...";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(212, 6);
            // 
            // steamGlobalMenuItem
            // 
            steamGlobalMenuItem.Name = "steamGlobalMenuItem";
            steamGlobalMenuItem.Size = new Size(215, 22);
            steamGlobalMenuItem.Text = "Steam Global";
            steamGlobalMenuItem.Click += (s, e) => SwitchPlatform("Steam Global");
            // 
            // steamJapanMenuItem
            // 
            steamJapanMenuItem.Name = "steamJapanMenuItem";
            steamJapanMenuItem.Size = new Size(215, 22);
            steamJapanMenuItem.Text = "Steam Japan";
            steamJapanMenuItem.Click += (s, e) => SwitchPlatform("Steam Japan");
            // 
            // dmmMenuItem
            // 
            dmmMenuItem.Name = "dmmMenuItem";
            dmmMenuItem.Size = new Size(215, 22);
            dmmMenuItem.Text = "DMM";
            dmmMenuItem.Click += (s, e) => SwitchPlatform("DMM");
            // 
            // komoeMenuItem
            // 
            komoeMenuItem.Name = "komoeMenuItem";
            komoeMenuItem.Size = new Size(215, 22);
            komoeMenuItem.Text = "Komoe";
            komoeMenuItem.Click += (s, e) => SwitchPlatform("Komoe");
            // 
            // kakaoMenuItem
            // 
            kakaoMenuItem.Name = "kakaoMenuItem";
            kakaoMenuItem.Size = new Size(215, 22);
            kakaoMenuItem.Text = "Kakao";
            kakaoMenuItem.Click += (s, e) => SwitchPlatform("Kakao");
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(212, 6);
            // 
            // keyGlobalMenuItem
            // 
            keyGlobalMenuItem.Name = "keyGlobalMenuItem";
            keyGlobalMenuItem.Size = new Size(215, 22);
            keyGlobalMenuItem.Text = "Key: Global";
            keyGlobalMenuItem.Click += (s, e) => SetKeyMode("Global");
            // 
            // keyJapanMenuItem
            // 
            keyJapanMenuItem.Name = "keyJapanMenuItem";
            keyJapanMenuItem.Size = new Size(215, 22);
            keyJapanMenuItem.Text = "Key: Japan";
            keyJapanMenuItem.Click += (s, e) => SetKeyMode("Japan");
            // 
            // keyCustomMenuItem
            // 
            keyCustomMenuItem.Name = "keyCustomMenuItem";
            keyCustomMenuItem.Size = new Size(215, 22);
            keyCustomMenuItem.Text = "Key: Custom...";
            keyCustomMenuItem.Click += (s, e) => SetKeyMode("Custom");
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(212, 6);
            // 
            // pathSettingsMenuItem
            // 
            pathSettingsMenuItem.Name = "pathSettingsMenuItem";
            pathSettingsMenuItem.Size = new Size(215, 22);
            pathSettingsMenuItem.Text = "Game Path Settings...";
            pathSettingsMenuItem.Click += ChangeGamePathButton_Click;
            // 
            // customPathMenuItem
            // 
            customPathMenuItem.Name = "customPathMenuItem";
            customPathMenuItem.Size = new Size(215, 22);
            customPathMenuItem.Text = "Select Custom Directory...";
            customPathMenuItem.Click += SelectCustomDirectory_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                howDoIInstallModMenuItem,
                aboutMenuItem
            });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // howDoIInstallModMenuItem
            // 
            howDoIInstallModMenuItem.Name = "howDoIInstallModMenuItem";
            howDoIInstallModMenuItem.Size = new Size(199, 22);
            howDoIInstallModMenuItem.Text = "How do I install a mod?";
            howDoIInstallModMenuItem.Click += HelpInstallButton_Click;
            // 
            // aboutMenuItem
            // 
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Size = new Size(199, 22);
            aboutMenuItem.Text = "About UMML";
            aboutMenuItem.Click += AboutMenuItem_Click;
            // 
            // gamePathPanel
            // 
            gamePathPanel.BackColor = SystemColors.ControlLight;
            gamePathPanel.Controls.Add(changeGamePathButton);
            gamePathPanel.Controls.Add(keyComboBox);
            gamePathPanel.Controls.Add(keyLabel);
            gamePathPanel.Controls.Add(gamePathValueLabel);
            gamePathPanel.Controls.Add(gamePathLabel);
            gamePathPanel.Dock = DockStyle.Top;
            gamePathPanel.Location = new Point(0, 24);
            gamePathPanel.Name = "gamePathPanel";
            gamePathPanel.Size = new Size(840, 36);
            gamePathPanel.TabIndex = 0;
            // 
            // changeGamePathButton
            // 
            changeGamePathButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            changeGamePathButton.Location = new Point(718, 6);
            changeGamePathButton.Name = "changeGamePathButton";
            changeGamePathButton.Size = new Size(110, 24);
            changeGamePathButton.TabIndex = 4;
            changeGamePathButton.Text = "Path Settings...";
            changeGamePathButton.UseVisualStyleBackColor = true;
            changeGamePathButton.Click += ChangeGamePathButton_Click;
            // 
            // keyComboBox
            // 
            keyComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            keyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            keyComboBox.FormattingEnabled = true;
            keyComboBox.Items.AddRange(new object[] { "Global", "Japan", "Custom" });
            keyComboBox.Location = new Point(612, 7);
            keyComboBox.Name = "keyComboBox";
            keyComboBox.Size = new Size(95, 23);
            keyComboBox.TabIndex = 3;
            keyComboBox.SelectedIndexChanged += KeyComboBox_SelectedIndexChanged;
            // 
            // keyLabel
            // 
            keyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            keyLabel.AutoSize = true;
            keyLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            keyLabel.Location = new Point(576, 10);
            keyLabel.Name = "keyLabel";
            keyLabel.Size = new Size(31, 15);
            keyLabel.TabIndex = 2;
            keyLabel.Text = "Key:";
            // 
            // gamePathValueLabel
            // 
            gamePathValueLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gamePathValueLabel.AutoEllipsis = true;
            gamePathValueLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            gamePathValueLabel.Location = new Point(85, 10);
            gamePathValueLabel.Name = "gamePathValueLabel";
            gamePathValueLabel.Size = new Size(485, 18);
            gamePathValueLabel.TabIndex = 1;
            gamePathValueLabel.Text = "Detecting...";
            // 
            // gamePathLabel
            // 
            gamePathLabel.AutoSize = true;
            gamePathLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gamePathLabel.Location = new Point(12, 10);
            gamePathLabel.Name = "gamePathLabel";
            gamePathLabel.Size = new Size(64, 15);
            gamePathLabel.TabIndex = 0;
            gamePathLabel.Text = "Data Path:";
            // 
            // modPathPanel
            // 
            modPathPanel.Controls.Add(helpInstallButton);
            modPathPanel.Controls.Add(previewButton);
            modPathPanel.Controls.Add(reloadButton);
            modPathPanel.Controls.Add(browseButton);
            modPathPanel.Controls.Add(modPathTextBox);
            modPathPanel.Controls.Add(modPathLabel);
            modPathPanel.Dock = DockStyle.Top;
            modPathPanel.Location = new Point(0, 60);
            modPathPanel.Name = "modPathPanel";
            modPathPanel.Size = new Size(840, 48);
            modPathPanel.TabIndex = 1;
            // 
            // helpInstallButton
            // 
            helpInstallButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpInstallButton.Location = new Point(782, 10);
            helpInstallButton.Name = "helpInstallButton";
            helpInstallButton.Size = new Size(46, 27);
            helpInstallButton.TabIndex = 5;
            helpInstallButton.Text = "Help";
            helpInstallButton.UseVisualStyleBackColor = true;
            helpInstallButton.Click += HelpInstallButton_Click;
            // 
            // previewButton
            // 
            previewButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            previewButton.Location = new Point(698, 10);
            previewButton.Name = "previewButton";
            previewButton.Size = new Size(78, 27);
            previewButton.TabIndex = 4;
            previewButton.Text = "Preview";
            previewButton.UseVisualStyleBackColor = true;
            previewButton.Click += PreviewButton_Click;
            // 
            // reloadButton
            // 
            reloadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            reloadButton.Location = new Point(626, 10);
            reloadButton.Name = "reloadButton";
            reloadButton.Size = new Size(66, 27);
            reloadButton.TabIndex = 3;
            reloadButton.Text = "Reload";
            reloadButton.UseVisualStyleBackColor = true;
            reloadButton.Click += ReloadButton_Click;
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseButton.Location = new Point(544, 10);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(76, 27);
            browseButton.TabIndex = 2;
            browseButton.Text = "Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += BrowseButton_Click;
            // 
            // modPathTextBox
            // 
            modPathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            modPathTextBox.Location = new Point(90, 12);
            modPathTextBox.Name = "modPathTextBox";
            modPathTextBox.Size = new Size(446, 23);
            modPathTextBox.TabIndex = 1;
            // 
            // modPathLabel
            // 
            modPathLabel.AutoSize = true;
            modPathLabel.Location = new Point(12, 15);
            modPathLabel.Name = "modPathLabel";
            modPathLabel.Size = new Size(70, 15);
            modPathLabel.TabIndex = 0;
            modPathLabel.Text = "Mod Folder:";
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.Location = new Point(0, 108);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(leftPanel);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(rightPanel);
            mainSplitContainer.Size = new Size(840, 427);
            mainSplitContainer.SplitterDistance = 390;
            mainSplitContainer.TabIndex = 2;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(optionsGroupBox);
            leftPanel.Controls.Add(modInfoGroupBox);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(10);
            leftPanel.Size = new Size(390, 427);
            leftPanel.TabIndex = 0;
            // 
            // optionsGroupBox
            // 
            optionsGroupBox.Controls.Add(keyDescLabel);
            optionsGroupBox.Controls.Add(optionsKeyComboBox);
            optionsGroupBox.Controls.Add(optionsKeyLabel);
            optionsGroupBox.Controls.Add(compressionDescLabel);
            optionsGroupBox.Controls.Add(compressionComboBox);
            optionsGroupBox.Controls.Add(compressionLabel);
            optionsGroupBox.Dock = DockStyle.Top;
            optionsGroupBox.Location = new Point(10, 240);
            optionsGroupBox.Name = "optionsGroupBox";
            optionsGroupBox.Size = new Size(370, 150);
            optionsGroupBox.TabIndex = 1;
            optionsGroupBox.TabStop = false;
            optionsGroupBox.Text = "Options";
            // 
            // keyDescLabel
            // 
            keyDescLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            keyDescLabel.Font = new Font("Segoe UI", 8.25F);
            keyDescLabel.ForeColor = SystemColors.GrayText;
            keyDescLabel.Location = new Point(110, 80);
            keyDescLabel.Name = "keyDescLabel";
            keyDescLabel.Size = new Size(247, 18);
            keyDescLabel.TabIndex = 5;
            keyDescLabel.Text = "Global: Steam EN | Japan: Steam JP / DMM";
            // 
            // optionsKeyComboBox
            // 
            optionsKeyComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            optionsKeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            optionsKeyComboBox.FormattingEnabled = true;
            optionsKeyComboBox.Items.AddRange(new object[] { "Global", "Japan", "Custom" });
            optionsKeyComboBox.Location = new Point(110, 55);
            optionsKeyComboBox.Name = "optionsKeyComboBox";
            optionsKeyComboBox.Size = new Size(247, 23);
            optionsKeyComboBox.TabIndex = 4;
            optionsKeyComboBox.SelectedIndexChanged += OptionsKeyComboBox_SelectedIndexChanged;
            // 
            // optionsKeyLabel
            // 
            optionsKeyLabel.AutoSize = true;
            optionsKeyLabel.Location = new Point(12, 58);
            optionsKeyLabel.Name = "optionsKeyLabel";
            optionsKeyLabel.Size = new Size(89, 15);
            optionsKeyLabel.TabIndex = 3;
            optionsKeyLabel.Text = "Decryption Key:";
            // 
            // compressionDescLabel
            // 
            compressionDescLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            compressionDescLabel.Font = new Font("Segoe UI", 8.25F);
            compressionDescLabel.ForeColor = SystemColors.GrayText;
            compressionDescLabel.Location = new Point(12, 105);
            compressionDescLabel.Name = "compressionDescLabel";
            compressionDescLabel.Size = new Size(345, 36);
            compressionDescLabel.TabIndex = 2;
            compressionDescLabel.Text = "Recompression via AssetsTools.NET applied before encryption.";
            // 
            // compressionComboBox
            // 
            compressionComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            compressionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            compressionComboBox.FormattingEnabled = true;
            compressionComboBox.Items.AddRange(new object[] { "LZ4HC (Default)", "LZ4 (Fast)", "LZMA (Smallest)", "Original", "None (Uncompressed)" });
            compressionComboBox.Location = new Point(110, 24);
            compressionComboBox.Name = "compressionComboBox";
            compressionComboBox.Size = new Size(247, 23);
            compressionComboBox.TabIndex = 1;
            // 
            // compressionLabel
            // 
            compressionLabel.AutoSize = true;
            compressionLabel.Location = new Point(12, 27);
            compressionLabel.Name = "compressionLabel";
            compressionLabel.Size = new Size(80, 15);
            compressionLabel.TabIndex = 0;
            compressionLabel.Text = "Compression:";
            // 
            // modInfoGroupBox
            // 
            modInfoGroupBox.Controls.Add(modDescriptionTextBox);
            modInfoGroupBox.Controls.Add(descLabel);
            modInfoGroupBox.Controls.Add(modAssetCountLabel);
            modInfoGroupBox.Controls.Add(modPlatformLabel);
            modInfoGroupBox.Controls.Add(modAuthorLabel);
            modInfoGroupBox.Controls.Add(modVersionLabel);
            modInfoGroupBox.Controls.Add(modTitleLabel);
            modInfoGroupBox.Dock = DockStyle.Top;
            modInfoGroupBox.Location = new Point(10, 10);
            modInfoGroupBox.Name = "modInfoGroupBox";
            modInfoGroupBox.Size = new Size(370, 220);
            modInfoGroupBox.TabIndex = 0;
            modInfoGroupBox.TabStop = false;
            modInfoGroupBox.Text = "Mod Information";
            // 
            // modDescriptionTextBox
            // 
            modDescriptionTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modDescriptionTextBox.Location = new Point(12, 134);
            modDescriptionTextBox.Multiline = true;
            modDescriptionTextBox.Name = "modDescriptionTextBox";
            modDescriptionTextBox.ReadOnly = true;
            modDescriptionTextBox.ScrollBars = ScrollBars.Vertical;
            modDescriptionTextBox.Size = new Size(345, 76);
            modDescriptionTextBox.TabIndex = 6;
            // 
            // descLabel
            // 
            descLabel.AutoSize = true;
            descLabel.Location = new Point(12, 114);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(70, 15);
            descLabel.TabIndex = 5;
            descLabel.Text = "Description:";
            // 
            // modAssetCountLabel
            // 
            modAssetCountLabel.AutoSize = true;
            modAssetCountLabel.Location = new Point(12, 92);
            modAssetCountLabel.Name = "modAssetCountLabel";
            modAssetCountLabel.Size = new Size(75, 15);
            modAssetCountLabel.TabIndex = 4;
            modAssetCountLabel.Text = "Asset Count: -";
            // 
            // modPlatformLabel
            // 
            modPlatformLabel.AutoSize = true;
            modPlatformLabel.Location = new Point(12, 72);
            modPlatformLabel.Name = "modPlatformLabel";
            modPlatformLabel.Size = new Size(61, 15);
            modPlatformLabel.TabIndex = 3;
            modPlatformLabel.Text = "Platform: -";
            // 
            // modAuthorLabel
            // 
            modAuthorLabel.AutoSize = true;
            modAuthorLabel.Location = new Point(12, 52);
            modAuthorLabel.Name = "modAuthorLabel";
            modAuthorLabel.Size = new Size(52, 15);
            modAuthorLabel.TabIndex = 2;
            modAuthorLabel.Text = "Author: -";
            // 
            // modVersionLabel
            // 
            modVersionLabel.AutoSize = true;
            modVersionLabel.Location = new Point(12, 32);
            modVersionLabel.Name = "modVersionLabel";
            modVersionLabel.Size = new Size(53, 15);
            modVersionLabel.TabIndex = 1;
            modVersionLabel.Text = "Version: -";
            // 
            // modTitleLabel
            // 
            modTitleLabel.AutoSize = true;
            modTitleLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            modTitleLabel.Location = new Point(12, 14);
            modTitleLabel.Name = "modTitleLabel";
            modTitleLabel.Size = new Size(111, 17);
            modTitleLabel.TabIndex = 0;
            modTitleLabel.Text = "No Mod Selected";
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(assetsGroupBox);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10);
            rightPanel.Size = new Size(446, 427);
            rightPanel.TabIndex = 0;
            // 
            // assetsGroupBox
            // 
            assetsGroupBox.Controls.Add(assetListView);
            assetsGroupBox.Dock = DockStyle.Fill;
            assetsGroupBox.Location = new Point(10, 10);
            assetsGroupBox.Name = "assetsGroupBox";
            assetsGroupBox.Size = new Size(426, 407);
            assetsGroupBox.TabIndex = 0;
            assetsGroupBox.TabStop = false;
            assetsGroupBox.Text = "Mod Assets";
            // 
            // assetListView
            // 
            assetListView.Columns.AddRange(new ColumnHeader[] { columnHeaderAsset });
            assetListView.Dock = DockStyle.Fill;
            assetListView.FullRowSelect = true;
            assetListView.GridLines = true;
            assetListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            assetListView.Location = new Point(3, 19);
            assetListView.Name = "assetListView";
            assetListView.Size = new Size(420, 385);
            assetListView.TabIndex = 0;
            assetListView.UseCompatibleStateImageBehavior = false;
            assetListView.View = View.Details;
            // 
            // columnHeaderAsset
            // 
            columnHeaderAsset.Text = "Asset Path";
            columnHeaderAsset.Width = 380;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = SystemColors.ControlLight;
            bottomPanel.Controls.Add(statusLabel);
            bottomPanel.Controls.Add(progressBar);
            bottomPanel.Controls.Add(actionsPanel);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 535);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Padding = new Padding(8);
            bottomPanel.Size = new Size(840, 50);
            bottomPanel.TabIndex = 3;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.AutoEllipsis = true;
            statusLabel.Location = new Point(12, 28);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(480, 16);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(12, 10);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(480, 15);
            progressBar.TabIndex = 1;
            // 
            // actionsPanel
            // 
            actionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            actionsPanel.Controls.Add(installButton);
            actionsPanel.Controls.Add(restoreButton);
            actionsPanel.Controls.Add(cleanupButton);
            actionsPanel.FlowDirection = FlowDirection.RightToLeft;
            actionsPanel.Location = new Point(498, 8);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Size = new Size(334, 34);
            actionsPanel.TabIndex = 0;
            // 
            // installButton
            // 
            installButton.BackColor = Color.FromArgb(76, 175, 80);
            installButton.Enabled = false;
            installButton.FlatStyle = FlatStyle.System;
            installButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            installButton.Location = new Point(231, 3);
            installButton.Name = "installButton";
            installButton.Size = new Size(100, 28);
            installButton.TabIndex = 0;
            installButton.Text = "Install Mod";
            installButton.UseVisualStyleBackColor = false;
            installButton.Click += InstallButton_Click;
            // 
            // restoreButton
            // 
            restoreButton.Location = new Point(125, 3);
            restoreButton.Name = "restoreButton";
            restoreButton.Size = new Size(100, 28);
            restoreButton.TabIndex = 1;
            restoreButton.Text = "Restore Assets";
            restoreButton.UseVisualStyleBackColor = true;
            restoreButton.Click += RestoreButton_Click;
            // 
            // cleanupButton
            // 
            cleanupButton.Location = new Point(19, 3);
            cleanupButton.Name = "cleanupButton";
            cleanupButton.Size = new Size(100, 28);
            cleanupButton.TabIndex = 2;
            cleanupButton.Text = "Clean Up...";
            cleanupButton.UseVisualStyleBackColor = true;
            cleanupButton.Click += CleanupButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 585);
            Controls.Add(mainSplitContainer);
            Controls.Add(bottomPanel);
            Controls.Add(modPathPanel);
            Controls.Add(gamePathPanel);
            Controls.Add(mainMenuStrip);
            MainMenuStrip = mainMenuStrip;
            MinimumSize = new Size(720, 500);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Uma Musume Mod Loader (UMML) .NET";
            Load += MainForm_Load;
            gamePathPanel.ResumeLayout(false);
            gamePathPanel.PerformLayout();
            modPathPanel.ResumeLayout(false);
            modPathPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            mainSplitContainer.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            optionsGroupBox.ResumeLayout(false);
            optionsGroupBox.PerformLayout();
            modInfoGroupBox.ResumeLayout(false);
            modInfoGroupBox.PerformLayout();
            rightPanel.ResumeLayout(false);
            assetsGroupBox.ResumeLayout(false);
            bottomPanel.ResumeLayout(false);
            actionsPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem platformToolStripMenuItem;
        private ToolStripMenuItem activePlatformMenuItem;
        private ToolStripMenuItem activeKeyMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem steamGlobalMenuItem;
        private ToolStripMenuItem steamJapanMenuItem;
        private ToolStripMenuItem dmmMenuItem;
        private ToolStripMenuItem komoeMenuItem;
        private ToolStripMenuItem kakaoMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem keyGlobalMenuItem;
        private ToolStripMenuItem keyJapanMenuItem;
        private ToolStripMenuItem keyCustomMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem pathSettingsMenuItem;
        private ToolStripMenuItem customPathMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem howDoIInstallModMenuItem;
        private ToolStripMenuItem aboutMenuItem;
        private Panel gamePathPanel;
        private Label gamePathLabel;
        private Label gamePathValueLabel;
        private Label keyLabel;
        private ComboBox keyComboBox;
        private Button changeGamePathButton;
        private Panel modPathPanel;
        private Label modPathLabel;
        private TextBox modPathTextBox;
        private Button browseButton;
        private Button reloadButton;
        private Button previewButton;
        private Button helpInstallButton;
        private SplitContainer mainSplitContainer;
        private Panel leftPanel;
        private GroupBox modInfoGroupBox;
        private Label modTitleLabel;
        private Label modVersionLabel;
        private Label modAuthorLabel;
        private Label modPlatformLabel;
        private Label modAssetCountLabel;
        private Label descLabel;
        private TextBox modDescriptionTextBox;
        private GroupBox optionsGroupBox;
        private Label compressionLabel;
        private ComboBox compressionComboBox;
        private Label optionsKeyLabel;
        private ComboBox optionsKeyComboBox;
        private Label keyDescLabel;
        private Label compressionDescLabel;
        private Panel rightPanel;
        private GroupBox assetsGroupBox;
        private ListView assetListView;
        private ColumnHeader columnHeaderAsset;
        private Panel bottomPanel;
        private FlowLayoutPanel actionsPanel;
        private Button installButton;
        private Button restoreButton;
        private Button cleanupButton;
        private ProgressBar progressBar;
        private Label statusLabel;
    }
}
