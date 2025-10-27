
namespace StairDrafting
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
            this.button_drawStructThreadInPlan = new System.Windows.Forms.Button();
            this.comboBox_lineStyle = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_finishThick = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_drawStructThreadInPlan
            // 
            this.button_drawStructThreadInPlan.Location = new System.Drawing.Point(12, 12);
            this.button_drawStructThreadInPlan.Name = "button_drawStructThreadInPlan";
            this.button_drawStructThreadInPlan.Size = new System.Drawing.Size(171, 23);
            this.button_drawStructThreadInPlan.TabIndex = 0;
            this.button_drawStructThreadInPlan.Text = "绘制平面结构线";
            this.button_drawStructThreadInPlan.UseVisualStyleBackColor = true;
            this.button_drawStructThreadInPlan.Click += new System.EventHandler(this.button_drawStructThreadInPlan_Click);
            // 
            // comboBox_lineStyle
            // 
            this.comboBox_lineStyle.DisplayMember = "Name";
            this.comboBox_lineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_lineStyle.FormattingEnabled = true;
            this.comboBox_lineStyle.Location = new System.Drawing.Point(74, 41);
            this.comboBox_lineStyle.Name = "comboBox_lineStyle";
            this.comboBox_lineStyle.Size = new System.Drawing.Size(109, 20);
            this.comboBox_lineStyle.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "详图线型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "面层厚度";
            // 
            // textBox_finishThick
            // 
            this.textBox_finishThick.Location = new System.Drawing.Point(74, 67);
            this.textBox_finishThick.Name = "textBox_finishThick";
            this.textBox_finishThick.Size = new System.Drawing.Size(109, 21);
            this.textBox_finishThick.TabIndex = 4;
            this.textBox_finishThick.Text = "50";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 102);
            this.Controls.Add(this.textBox_finishThick);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_lineStyle);
            this.Controls.Add(this.button_drawStructThreadInPlan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "楼梯绘制工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_drawStructThreadInPlan;
        private System.Windows.Forms.ComboBox comboBox_lineStyle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_finishThick;
    }
}