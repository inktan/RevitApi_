namespace FAKE_DIMS
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
            this.button_convert = new System.Windows.Forms.Button();
            this.button_check = new System.Windows.Forms.Button();
            this.button_multiSelect = new System.Windows.Forms.Button();
            this.button_multiViews = new System.Windows.Forms.Button();
            this.button_convertSelected = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_convert
            // 
            this.button_convert.Location = new System.Drawing.Point(12, 12);
            this.button_convert.Name = "button_convert";
            this.button_convert.Size = new System.Drawing.Size(135, 35);
            this.button_convert.TabIndex = 0;
            this.button_convert.Text = "替换当前视图";
            this.button_convert.UseVisualStyleBackColor = true;
            this.button_convert.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_check
            // 
            this.button_check.Location = new System.Drawing.Point(182, 12);
            this.button_check.Name = "button_check";
            this.button_check.Size = new System.Drawing.Size(114, 35);
            this.button_check.TabIndex = 1;
            this.button_check.Text = "检查标注线位置";
            this.button_check.UseVisualStyleBackColor = true;
            this.button_check.Visible = false;
            this.button_check.Click += new System.EventHandler(this.button_check_Click);
            // 
            // button_multiSelect
            // 
            this.button_multiSelect.Location = new System.Drawing.Point(12, 135);
            this.button_multiSelect.Name = "button_multiSelect";
            this.button_multiSelect.Size = new System.Drawing.Size(135, 35);
            this.button_multiSelect.TabIndex = 2;
            this.button_multiSelect.Text = "框选假标注族";
            this.button_multiSelect.UseVisualStyleBackColor = true;
            this.button_multiSelect.Click += new System.EventHandler(this.button_multiSelect_Click);
            // 
            // button_multiViews
            // 
            this.button_multiViews.Location = new System.Drawing.Point(12, 53);
            this.button_multiViews.Name = "button_multiViews";
            this.button_multiViews.Size = new System.Drawing.Size(135, 35);
            this.button_multiViews.TabIndex = 3;
            this.button_multiViews.Text = "替换多个视图";
            this.button_multiViews.UseVisualStyleBackColor = true;
            this.button_multiViews.Click += new System.EventHandler(this.button_multiViews_Click);
            // 
            // button_convertSelected
            // 
            this.button_convertSelected.Location = new System.Drawing.Point(12, 94);
            this.button_convertSelected.Name = "button_convertSelected";
            this.button_convertSelected.Size = new System.Drawing.Size(135, 35);
            this.button_convertSelected.TabIndex = 4;
            this.button_convertSelected.Text = "替换选择的标注";
            this.button_convertSelected.UseVisualStyleBackColor = true;
            this.button_convertSelected.Click += new System.EventHandler(this.button_convertSelected_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(161, 178);
            this.Controls.Add(this.button_convertSelected);
            this.Controls.Add(this.button_multiViews);
            this.Controls.Add(this.button_multiSelect);
            this.Controls.Add(this.button_check);
            this.Controls.Add(this.button_convert);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "goa标注替换";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_convert;
        private System.Windows.Forms.Button button_check;
        private System.Windows.Forms.Button button_multiSelect;
        private System.Windows.Forms.Button button_multiViews;
        private System.Windows.Forms.Button button_convertSelected;
    }
}