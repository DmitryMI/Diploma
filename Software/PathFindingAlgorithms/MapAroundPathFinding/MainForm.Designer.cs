namespace MapAroundPathFinding
{
    partial class MainForm
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
            this.OpenMapButton = new System.Windows.Forms.Button();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ErrorLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.LayerSettings = new System.Windows.Forms.Button();
            this.GetCellMapButton = new System.Windows.Forms.Button();
            this.HpaTestingButton = new System.Windows.Forms.Button();
            this.MapAroundControl = new MapAround.UI.WinForms.MapControl();
            this.TestUserYButton = new System.Windows.Forms.Button();
            this.MainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapAroundControl)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenMapButton
            // 
            this.OpenMapButton.Location = new System.Drawing.Point(1017, 12);
            this.OpenMapButton.Name = "OpenMapButton";
            this.OpenMapButton.Size = new System.Drawing.Size(152, 23);
            this.OpenMapButton.TabIndex = 1;
            this.OpenMapButton.Text = "Открыть карту";
            this.OpenMapButton.UseVisualStyleBackColor = true;
            this.OpenMapButton.Click += new System.EventHandler(this.OpenMapButton_Click);
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorLabel});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 567);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(1181, 22);
            this.MainStatusStrip.TabIndex = 2;
            this.MainStatusStrip.Text = "statusStrip1";
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.ForeColor = System.Drawing.Color.Green;
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(23, 17);
            this.ErrorLabel.Text = "OK";
            // 
            // LayerSettings
            // 
            this.LayerSettings.Location = new System.Drawing.Point(1017, 41);
            this.LayerSettings.Name = "LayerSettings";
            this.LayerSettings.Size = new System.Drawing.Size(152, 23);
            this.LayerSettings.TabIndex = 3;
            this.LayerSettings.Text = "Настройки слоев";
            this.LayerSettings.UseVisualStyleBackColor = true;
            this.LayerSettings.Click += new System.EventHandler(this.LayerSettings_Click);
            // 
            // GetCellMapButton
            // 
            this.GetCellMapButton.Location = new System.Drawing.Point(1017, 70);
            this.GetCellMapButton.Name = "GetCellMapButton";
            this.GetCellMapButton.Size = new System.Drawing.Size(152, 23);
            this.GetCellMapButton.TabIndex = 5;
            this.GetCellMapButton.Text = "Дискретизация";
            this.GetCellMapButton.UseVisualStyleBackColor = true;
            this.GetCellMapButton.Click += new System.EventHandler(this.GetCellMapButton_Click);
            // 
            // HpaTestingButton
            // 
            this.HpaTestingButton.Location = new System.Drawing.Point(1017, 99);
            this.HpaTestingButton.Name = "HpaTestingButton";
            this.HpaTestingButton.Size = new System.Drawing.Size(152, 23);
            this.HpaTestingButton.TabIndex = 6;
            this.HpaTestingButton.Text = "HPA*";
            this.HpaTestingButton.UseVisualStyleBackColor = true;
            this.HpaTestingButton.Click += new System.EventHandler(this.HpaTestingButton_Click);
            // 
            // MapAroundControl
            // 
            this.MapAroundControl.AlignmentWhileZooming = true;
            this.MapAroundControl.Animation = false;
            this.MapAroundControl.AnimationTime = 400;
            this.MapAroundControl.BackColor = System.Drawing.Color.White;
            this.MapAroundControl.DragMode = MapAround.UI.WinForms.MapControl.DraggingMode.Pan;
            this.MapAroundControl.DragThreshold = 1;
            this.MapAroundControl.Editor = null;
            this.MapAroundControl.IsDragging = false;
            this.MapAroundControl.Location = new System.Drawing.Point(12, 12);
            this.MapAroundControl.Map = null;
            this.MapAroundControl.MouseWheelZooming = true;
            this.MapAroundControl.Name = "MapAroundControl";
            this.MapAroundControl.SelectionMargin = 3;
            this.MapAroundControl.SelectionRectangleColor = System.Drawing.SystemColors.Highlight;
            this.MapAroundControl.Size = new System.Drawing.Size(999, 552);
            this.MapAroundControl.TabIndex = 0;
            this.MapAroundControl.Text = "mapControl1";
            this.MapAroundControl.ZoomPercent = 60;
            this.MapAroundControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapAroundControl_MouseDown);
            this.MapAroundControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapAroundControl_MouseUp);
            // 
            // TestUserYButton
            // 
            this.TestUserYButton.Location = new System.Drawing.Point(1017, 128);
            this.TestUserYButton.Name = "TestUserYButton";
            this.TestUserYButton.Size = new System.Drawing.Size(152, 23);
            this.TestUserYButton.TabIndex = 7;
            this.TestUserYButton.Text = "Y TEST";
            this.TestUserYButton.UseVisualStyleBackColor = true;
            this.TestUserYButton.Click += new System.EventHandler(this.TestUserYButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 589);
            this.Controls.Add(this.TestUserYButton);
            this.Controls.Add(this.HpaTestingButton);
            this.Controls.Add(this.GetCellMapButton);
            this.Controls.Add(this.LayerSettings);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.OpenMapButton);
            this.Controls.Add(this.MapAroundControl);
            this.Name = "MainForm";
            this.Text = "Поиск кратчайшего безопасного пути";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapAroundControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapAround.UI.WinForms.MapControl MapAroundControl;
        private System.Windows.Forms.Button OpenMapButton;
        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ErrorLabel;
        private System.Windows.Forms.Button LayerSettings;
        private System.Windows.Forms.Button GetCellMapButton;
        private System.Windows.Forms.Button HpaTestingButton;
        private System.Windows.Forms.Button TestUserYButton;
    }
}