namespace SHET_NEW
{
    partial class Form1
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
            this.comboBox_titleblocks = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_excel = new System.Windows.Forms.Button();
            this.button_create = new System.Windows.Forms.Button();
            this.textBox_excel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox_titleblocks
            // 
            this.comboBox_titleblocks.DisplayMember = "Name";
            this.comboBox_titleblocks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_titleblocks.FormattingEnabled = true;
            this.comboBox_titleblocks.Location = new System.Drawing.Point(68, 6);
            this.comboBox_titleblocks.Name = "comboBox_titleblocks";
            this.comboBox_titleblocks.Size = new System.Drawing.Size(100, 20);
            this.comboBox_titleblocks.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "图框";
            // 
            // button_excel
            // 
            this.button_excel.Location = new System.Drawing.Point(12, 61);
            this.button_excel.Name = "button_excel";
            this.button_excel.Size = new System.Drawing.Size(50, 23);
            this.button_excel.TabIndex = 2;
            this.button_excel.Text = "选择";
            this.button_excel.UseVisualStyleBackColor = true;
            this.button_excel.Click += new System.EventHandler(this.button_excel_Click);
            // 
            // button_create
            // 
            this.button_create.Location = new System.Drawing.Point(12, 100);
            this.button_create.Name = "button_create";
            this.button_create.Size = new System.Drawing.Size(154, 30);
            this.button_create.TabIndex = 3;
            this.button_create.Text = "批量创建";
            this.button_create.UseVisualStyleBackColor = true;
            this.button_create.Click += new System.EventHandler(this.button_create_Click);
            // 
            // textBox_excel
            // 
            this.textBox_excel.Enabled = false;
            this.textBox_excel.Location = new System.Drawing.Point(68, 63);
            this.textBox_excel.Name = "textBox_excel";
            this.textBox_excel.Size = new System.Drawing.Size(100, 21);
            this.textBox_excel.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Excel 文件";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(185, 140);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_excel);
            this.Controls.Add(this.button_create);
            this.Controls.Add(this.button_excel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_titleblocks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "批量创建图纸";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_titleblocks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_excel;
        private System.Windows.Forms.Button button_create;
        private System.Windows.Forms.TextBox textBox_excel;
        private System.Windows.Forms.Label label2;
    }
}