
namespace SpreadsheetGUI
{
    partial class FileSelector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectButton = new System.Windows.Forms.Button();
            this.NewButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(206, 88);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(75, 23);
            this.SelectButton.TabIndex = 1;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.select_Click);
            // 
            // NewButton
            // 
            this.NewButton.Location = new System.Drawing.Point(206, 158);
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new System.Drawing.Size(75, 23);
            this.NewButton.TabIndex = 2;
            this.NewButton.Text = "Create New ";
            this.NewButton.UseVisualStyleBackColor = true;
            this.NewButton.Click += new System.EventHandler(this.CreateNew_Click);
            // 
            // textBox1
            // 
            this.textBox1.HideSelection = false;
            this.textBox1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox1.Location = new System.Drawing.Point(67, 161);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(123, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Enter Name";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "or";
            // 
            // FileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 269);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NewButton);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.textBox1);
            this.Name = "FileSelector";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Button NewButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}