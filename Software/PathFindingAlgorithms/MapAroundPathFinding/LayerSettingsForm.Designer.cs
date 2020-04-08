namespace MapAroundPathFinding
{
    partial class LayerSettingsForm
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
            this.LayersCheckedList = new System.Windows.Forms.CheckedListBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LayersCheckedList
            // 
            this.LayersCheckedList.CheckOnClick = true;
            this.LayersCheckedList.FormattingEnabled = true;
            this.LayersCheckedList.Location = new System.Drawing.Point(12, 12);
            this.LayersCheckedList.Name = "LayersCheckedList";
            this.LayersCheckedList.Size = new System.Drawing.Size(336, 394);
            this.LayersCheckedList.TabIndex = 0;
            this.LayersCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LayersCheckedList_ItemCheck);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(273, 415);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Ok";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LayerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 450);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.LayersCheckedList);
            this.Name = "LayerSettingsForm";
            this.Text = "LayerSettingsForm";
            this.Load += new System.EventHandler(this.LayerSettingsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox LayersCheckedList;
        private System.Windows.Forms.Button SaveButton;
    }
}