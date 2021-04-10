namespace MapAroundPathFinding
{
    partial class HpaTestForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.AddObstacle = new System.Windows.Forms.Button();
            this.RemoveObstaclesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 500);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(518, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(100, 23);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Запустить HPA*";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // AddObstacle
            // 
            this.AddObstacle.Location = new System.Drawing.Point(518, 41);
            this.AddObstacle.Name = "AddObstacle";
            this.AddObstacle.Size = new System.Drawing.Size(100, 43);
            this.AddObstacle.TabIndex = 2;
            this.AddObstacle.Text = "Добавить препятствие";
            this.AddObstacle.UseVisualStyleBackColor = true;
            this.AddObstacle.Click += new System.EventHandler(this.AddObstacle_Click);
            // 
            // RemoveObstaclesButton
            // 
            this.RemoveObstaclesButton.Location = new System.Drawing.Point(518, 90);
            this.RemoveObstaclesButton.Name = "RemoveObstaclesButton";
            this.RemoveObstaclesButton.Size = new System.Drawing.Size(100, 43);
            this.RemoveObstaclesButton.TabIndex = 3;
            this.RemoveObstaclesButton.Text = "Убрать препятствия";
            this.RemoveObstaclesButton.UseVisualStyleBackColor = true;
            this.RemoveObstaclesButton.Click += new System.EventHandler(this.RemoveObstaclesButton_Click);
            // 
            // HpaTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 527);
            this.Controls.Add(this.RemoveObstaclesButton);
            this.Controls.Add(this.AddObstacle);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "HpaTestForm";
            this.Text = "HpaTestForm";
            this.Load += new System.EventHandler(this.HpaTestForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button AddObstacle;
        private System.Windows.Forms.Button RemoveObstaclesButton;
    }
}