namespace TOPO_ANLS
{
    partial class MainWindow
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
            this.button_elev = new System.Windows.Forms.Button();
            this.button_slope = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_elev
            // 
            this.button_elev.Location = new System.Drawing.Point(12, 12);
            this.button_elev.Name = "button_elev";
            this.button_elev.Size = new System.Drawing.Size(100, 35);
            this.button_elev.TabIndex = 0;
            this.button_elev.Text = "高程分析";
            this.button_elev.UseVisualStyleBackColor = true;
            this.button_elev.Click += new System.EventHandler(this.button_elev_Click);
            // 
            // button_slope
            // 
            this.button_slope.Location = new System.Drawing.Point(12, 53);
            this.button_slope.Name = "button_slope";
            this.button_slope.Size = new System.Drawing.Size(100, 35);
            this.button_slope.TabIndex = 1;
            this.button_slope.Text = "坡度分析";
            this.button_slope.UseVisualStyleBackColor = true;
            this.button_slope.Click += new System.EventHandler(this.button_slope_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(125, 100);
            this.Controls.Add(this.button_slope);
            this.Controls.Add(this.button_elev);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "坡地分析";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_elev;
        private System.Windows.Forms.Button button_slope;
    }
}