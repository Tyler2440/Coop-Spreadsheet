using SS;

namespace SpreadsheetGUI
{
    partial class SpreadsheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param> 
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CellInfoPanel = new System.Windows.Forms.Panel();
            this.CellContentText = new System.Windows.Forms.TextBox();
            this.ContentLabel = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.CellLabel = new System.Windows.Forms.Label();
            this.CellValueText = new System.Windows.Forms.TextBox();
            this.CellNameText = new System.Windows.Forms.TextBox();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.AddressText = new System.Windows.Forms.TextBox();
            this.JoinButton = new System.Windows.Forms.Button();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.CellInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menuStrip1.Size = new System.Drawing.Size(750, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // CellInfoPanel
            // 
            this.CellInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CellInfoPanel.AutoSize = true;
            this.CellInfoPanel.Controls.Add(this.CellContentText);
            this.CellInfoPanel.Controls.Add(this.ContentLabel);
            this.CellInfoPanel.Controls.Add(this.ValueLabel);
            this.CellInfoPanel.Controls.Add(this.CellLabel);
            this.CellInfoPanel.Controls.Add(this.CellValueText);
            this.CellInfoPanel.Controls.Add(this.CellNameText);
            this.CellInfoPanel.Location = new System.Drawing.Point(0, 434);
            this.CellInfoPanel.Name = "CellInfoPanel";
            this.CellInfoPanel.Size = new System.Drawing.Size(732, 27);
            this.CellInfoPanel.TabIndex = 2;
            // 
            // CellContentText
            // 
            this.CellContentText.Location = new System.Drawing.Point(352, 4);
            this.CellContentText.Name = "CellContentText";
            this.CellContentText.Size = new System.Drawing.Size(100, 20);
            this.CellContentText.TabIndex = 5;
            this.CellContentText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CellContentText_KeyPress);
            // 
            // ContentLabel
            // 
            this.ContentLabel.AutoSize = true;
            this.ContentLabel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContentLabel.Location = new System.Drawing.Point(289, 5);
            this.ContentLabel.Name = "ContentLabel";
            this.ContentLabel.Size = new System.Drawing.Size(57, 17);
            this.ContentLabel.TabIndex = 4;
            this.ContentLabel.Text = "Content:";
            this.ContentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueLabel.Location = new System.Drawing.Point(120, 5);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(44, 17);
            this.ValueLabel.TabIndex = 3;
            this.ValueLabel.Text = "Value:";
            this.ValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CellLabel
            // 
            this.CellLabel.AutoSize = true;
            this.CellLabel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CellLabel.Location = new System.Drawing.Point(14, 5);
            this.CellLabel.Name = "CellLabel";
            this.CellLabel.Size = new System.Drawing.Size(34, 17);
            this.CellLabel.TabIndex = 2;
            this.CellLabel.Text = "Cell:";
            this.CellLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CellValueText
            // 
            this.CellValueText.Enabled = false;
            this.CellValueText.Location = new System.Drawing.Point(170, 4);
            this.CellValueText.Name = "CellValueText";
            this.CellValueText.Size = new System.Drawing.Size(100, 20);
            this.CellValueText.TabIndex = 1;
            // 
            // CellNameText
            // 
            this.CellNameText.Enabled = false;
            this.CellNameText.Location = new System.Drawing.Point(54, 3);
            this.CellNameText.Name = "CellNameText";
            this.CellNameText.Size = new System.Drawing.Size(38, 20);
            this.CellNameText.TabIndex = 0;
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 24);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(750, 455);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // ServerLabel
            // 
            this.ServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerLabel.Location = new System.Drawing.Point(7, 3);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(49, 18);
            this.ServerLabel.TabIndex = 3;
            this.ServerLabel.Text = "Server";
            // 
            // AddressText
            // 
            this.AddressText.Location = new System.Drawing.Point(54, 3);
            this.AddressText.Name = "AddressText";
            this.AddressText.Size = new System.Drawing.Size(100, 20);
            this.AddressText.TabIndex = 4;
            // 
            // JoinButton
            // 
            this.JoinButton.Location = new System.Drawing.Point(352, 0);
            this.JoinButton.Name = "JoinButton";
            this.JoinButton.Size = new System.Drawing.Size(75, 23);
            this.JoinButton.TabIndex = 5;
            this.JoinButton.Text = "Join";
            this.JoinButton.UseVisualStyleBackColor = true;
            this.JoinButton.Click += new System.EventHandler(this.JoinButton_Click);
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(228, 3);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(100, 20);
            this.UsernameBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(167, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Username";
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 479);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UsernameBox);
            this.Controls.Add(this.JoinButton);
            this.Controls.Add(this.AddressText);
            this.Controls.Add(this.ServerLabel);
            this.Controls.Add(this.CellInfoPanel);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetForm";
            this.Text = "Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetForm_FormClosing);
            this.Load += new System.EventHandler(this.SpreadsheetForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.CellInfoPanel.ResumeLayout(false);
            this.CellInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #region Windows Form Designer generated code

        #endregion

        private SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel CellInfoPanel;
        private System.Windows.Forms.TextBox CellContentText;
        private System.Windows.Forms.Label ContentLabel;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.Label CellLabel;
        private System.Windows.Forms.TextBox CellValueText;
        private System.Windows.Forms.TextBox CellNameText;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.TextBox AddressText;
        private System.Windows.Forms.Button JoinButton;
        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label label1;
    }
}

