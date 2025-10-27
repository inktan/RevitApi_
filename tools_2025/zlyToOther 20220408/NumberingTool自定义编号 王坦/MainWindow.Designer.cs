
namespace NumberingTool
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
            this.textBox_prefix = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_numElems = new System.Windows.Forms.Label();
            this.button_addCurrentSelction = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_surfix = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_paramName = new System.Windows.Forms.ComboBox();
            this.button_numbering = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_prefix
            // 
            this.textBox_prefix.Location = new System.Drawing.Point(47, 113);
            this.textBox_prefix.Name = "textBox_prefix";
            this.textBox_prefix.Size = new System.Drawing.Size(143, 21);
            this.textBox_prefix.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "前缀";
            // 
            // label_numElems
            // 
            this.label_numElems.AutoSize = true;
            this.label_numElems.Location = new System.Drawing.Point(6, 47);
            this.label_numElems.Name = "label_numElems";
            this.label_numElems.Size = new System.Drawing.Size(101, 12);
            this.label_numElems.TabIndex = 2;
            this.label_numElems.Text = " 已添加 0 个图元";
            // 
            // button_addCurrentSelction
            // 
            this.button_addCurrentSelction.Location = new System.Drawing.Point(14, 12);
            this.button_addCurrentSelction.Name = "button_addCurrentSelction";
            this.button_addCurrentSelction.Size = new System.Drawing.Size(176, 23);
            this.button_addCurrentSelction.TabIndex = 3;
            this.button_addCurrentSelction.Text = "添加当前选择";
            this.button_addCurrentSelction.UseVisualStyleBackColor = true;
            this.button_addCurrentSelction.Click += new System.EventHandler(this.button_addCurrentSelction_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "后缀";
            // 
            // textBox_surfix
            // 
            this.textBox_surfix.Location = new System.Drawing.Point(47, 140);
            this.textBox_surfix.Name = "textBox_surfix";
            this.textBox_surfix.Size = new System.Drawing.Size(143, 21);
            this.textBox_surfix.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "参数";
            // 
            // comboBox_paramName
            // 
            this.comboBox_paramName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_paramName.FormattingEnabled = true;
            this.comboBox_paramName.Location = new System.Drawing.Point(47, 76);
            this.comboBox_paramName.Name = "comboBox_paramName";
            this.comboBox_paramName.Size = new System.Drawing.Size(143, 20);
            this.comboBox_paramName.TabIndex = 7;
            // 
            // button_numbering
            // 
            this.button_numbering.Location = new System.Drawing.Point(14, 180);
            this.button_numbering.Name = "button_numbering";
            this.button_numbering.Size = new System.Drawing.Size(176, 23);
            this.button_numbering.TabIndex = 8;
            this.button_numbering.Text = "编号-数字";
            this.button_numbering.UseVisualStyleBackColor = true;
            this.button_numbering.Click += new System.EventHandler(this.button_numbering_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 215);
            this.Controls.Add(this.button_numbering);
            this.Controls.Add(this.comboBox_paramName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_surfix);
            this.Controls.Add(this.button_addCurrentSelction);
            this.Controls.Add(this.label_numElems);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_prefix);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "goa自定义编号";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_prefix;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_numElems;
        private System.Windows.Forms.Button button_addCurrentSelction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_surfix;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_paramName;
        private System.Windows.Forms.Button button_numbering;
    }
}