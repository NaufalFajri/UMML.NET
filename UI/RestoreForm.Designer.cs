namespace UMML.UI
{
    partial class RestoreForm
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
            backupListBox = new ListBox();
            bottomPanel = new Panel();
            statusLabel = new Label();
            progressBar = new ProgressBar();
            buttonsPanel = new FlowLayoutPanel();
            restoreSelectedButton = new Button();
            restoreAllButton = new Button();
            closeButton = new Button();
            bottomPanel.SuspendLayout();
            buttonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // backupListBox
            // 
            backupListBox.Dock = DockStyle.Fill;
            backupListBox.FormattingEnabled = true;
            backupListBox.ItemHeight = 15;
            backupListBox.Location = new Point(10, 10);
            backupListBox.Name = "backupListBox";
            backupListBox.SelectionMode = SelectionMode.MultiExtended;
            backupListBox.Size = new Size(764, 380);
            backupListBox.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.Controls.Add(statusLabel);
            bottomPanel.Controls.Add(progressBar);
            bottomPanel.Controls.Add(buttonsPanel);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(10, 390);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(764, 80);
            bottomPanel.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Location = new Point(0, 65);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 15);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Top;
            progressBar.Location = new Point(0, 42);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(764, 15);
            progressBar.TabIndex = 1;
            // 
            // buttonsPanel
            // 
            buttonsPanel.Controls.Add(restoreSelectedButton);
            buttonsPanel.Controls.Add(restoreAllButton);
            buttonsPanel.Controls.Add(closeButton);
            buttonsPanel.Dock = DockStyle.Top;
            buttonsPanel.Location = new Point(0, 0);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Size = new Size(764, 36);
            buttonsPanel.TabIndex = 0;
            // 
            // restoreSelectedButton
            // 
            restoreSelectedButton.Location = new Point(3, 3);
            restoreSelectedButton.Name = "restoreSelectedButton";
            restoreSelectedButton.Size = new Size(130, 30);
            restoreSelectedButton.TabIndex = 0;
            restoreSelectedButton.Text = "Restore Selected";
            restoreSelectedButton.UseVisualStyleBackColor = true;
            restoreSelectedButton.Click += RestoreSelectedButton_Click;
            // 
            // restoreAllButton
            // 
            restoreAllButton.Location = new Point(139, 3);
            restoreAllButton.Name = "restoreAllButton";
            restoreAllButton.Size = new Size(130, 30);
            restoreAllButton.TabIndex = 1;
            restoreAllButton.Text = "Restore All";
            restoreAllButton.UseVisualStyleBackColor = true;
            restoreAllButton.Click += RestoreAllButton_Click;
            // 
            // closeButton
            // 
            closeButton.Location = new Point(275, 3);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(90, 30);
            closeButton.TabIndex = 2;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButton_Click;
            // 
            // RestoreForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 480);
            Controls.Add(backupListBox);
            Controls.Add(bottomPanel);
            MinimumSize = new Size(600, 400);
            Name = "RestoreForm";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Restore Original Assets";
            Load += RestoreForm_Load;
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            buttonsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListBox backupListBox;
        private Panel bottomPanel;
        private FlowLayoutPanel buttonsPanel;
        private Button restoreSelectedButton;
        private Button restoreAllButton;
        private Button closeButton;
        private ProgressBar progressBar;
        private Label statusLabel;
    }
}
