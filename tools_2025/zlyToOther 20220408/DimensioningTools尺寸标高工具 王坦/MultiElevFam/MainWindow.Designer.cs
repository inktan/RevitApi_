namespace AutoFillUpLevelHeightAnnotation
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
            this.button_ok = new System.Windows.Forms.Button();
            this.button_pickFamily = new System.Windows.Forms.Button();
            this.textBox_startElevation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_levelHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_numLevel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(12, 155);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(140, 35);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "自动填写标高";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_pickFamily
            // 
            this.button_pickFamily.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_pickFamily.Location = new System.Drawing.Point(12, 11);
            this.button_pickFamily.Name = "button_pickFamily";
            this.button_pickFamily.Size = new System.Drawing.Size(140, 35);
            this.button_pickFamily.TabIndex = 2;
            this.button_pickFamily.Text = "选择标高族";
            this.button_pickFamily.UseVisualStyleBackColor = true;
            this.button_pickFamily.Click += new System.EventHandler(this.button_pickFamily_Click);
            // 
            // textBox_startElevation
            // 
            this.textBox_startElevation.Location = new System.Drawing.Point(69, 87);
            this.textBox_startElevation.Name = "textBox_startElevation";
            this.textBox_startElevation.Size = new System.Drawing.Size(67, 21);
            this.textBox_startElevation.TabIndex = 4;
            this.textBox_startElevation.Text = "0.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "起始高度";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_levelHeight
            // 
            this.textBox_levelHeight.Location = new System.Drawing.Point(69, 114);
            this.textBox_levelHeight.Name = "textBox_levelHeight";
            this.textBox_levelHeight.Size = new System.Drawing.Size(67, 21);
            this.textBox_levelHeight.TabIndex = 6;
            this.textBox_levelHeight.Text = "3.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "层高";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "层数";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_numLevel
            // 
            this.textBox_numLevel.Location = new System.Drawing.Point(69, 60);
            this.textBox_numLevel.Name = "textBox_numLevel";
            this.textBox_numLevel.Size = new System.Drawing.Size(67, 21);
            this.textBox_numLevel.TabIndex = 8;
            this.textBox_numLevel.Text = "16";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(142, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "m";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(141, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "m";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 199);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_numLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_levelHeight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_startElevation);
            this.Controls.Add(this.button_pickFamily);
            this.Controls.Add(this.button_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "自动填写标高";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_pickFamily;
        private System.Windows.Forms.TextBox textBox_startElevation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_levelHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_numLevel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}