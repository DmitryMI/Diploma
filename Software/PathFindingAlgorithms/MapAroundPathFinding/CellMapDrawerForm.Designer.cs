namespace MapAroundPathFinding
{
    partial class CellMapDrawerForm
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
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.FindPathButton = new System.Windows.Forms.Button();
            this.PreferencesPanel = new System.Windows.Forms.Panel();
            this.AlgorithmSelectorBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.PreferencesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PictureBox
            // 
            this.PictureBox.BackColor = System.Drawing.Color.White;
            this.PictureBox.Location = new System.Drawing.Point(12, 12);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(1000, 550);
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
            this.PictureBox.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // FindPathButton
            // 
            this.FindPathButton.Location = new System.Drawing.Point(3, 3);
            this.FindPathButton.Name = "FindPathButton";
            this.FindPathButton.Size = new System.Drawing.Size(118, 23);
            this.FindPathButton.TabIndex = 1;
            this.FindPathButton.Text = "Проложить путь";
            this.FindPathButton.UseVisualStyleBackColor = true;
            this.FindPathButton.Click += new System.EventHandler(this.FindPathButton_Click);
            // 
            // PreferencesPanel
            // 
            this.PreferencesPanel.Controls.Add(this.AlgorithmSelectorBox);
            this.PreferencesPanel.Controls.Add(this.FindPathButton);
            this.PreferencesPanel.Location = new System.Drawing.Point(1019, 12);
            this.PreferencesPanel.Name = "PreferencesPanel";
            this.PreferencesPanel.Size = new System.Drawing.Size(124, 240);
            this.PreferencesPanel.TabIndex = 2;
            // 
            // AlgorithmSelectorBox
            // 
            this.AlgorithmSelectorBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AlgorithmSelectorBox.FormattingEnabled = true;
            this.AlgorithmSelectorBox.Location = new System.Drawing.Point(3, 32);
            this.AlgorithmSelectorBox.Name = "AlgorithmSelectorBox";
            this.AlgorithmSelectorBox.Size = new System.Drawing.Size(118, 21);
            this.AlgorithmSelectorBox.TabIndex = 2;
            // 
            // CellMapDrawerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 571);
            this.Controls.Add(this.PreferencesPanel);
            this.Controls.Add(this.PictureBox);
            this.Name = "CellMapDrawerForm";
            this.Text = "CellMapDrawer";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.PreferencesPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.Button FindPathButton;
        private System.Windows.Forms.Panel PreferencesPanel;
        private System.Windows.Forms.ComboBox AlgorithmSelectorBox;
    }
}