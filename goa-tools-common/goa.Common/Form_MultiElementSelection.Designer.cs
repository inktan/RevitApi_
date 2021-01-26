namespace goa.Common
{
    partial class Form_MultiElementSelection
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
            this.button1 = new System.Windows.Forms.Button();
            this.button_selAll = new System.Windows.Forms.Button();
            this.button_selNone = new System.Windows.Forms.Button();
            this.button_reverseSel = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(49, 328);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 30);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "确定";
            this.button_ok.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(130, 327);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button_selAll
            // 
            this.button_selAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_selAll.Location = new System.Drawing.Point(12, 292);
            this.button_selAll.Name = "button_selAll";
            this.button_selAll.Size = new System.Drawing.Size(75, 25);
            this.button_selAll.TabIndex = 3;
            this.button_selAll.Text = "全选";
            this.button_selAll.UseVisualStyleBackColor = true;
            this.button_selAll.Click += new System.EventHandler(this.button_selAll_Click);
            // 
            // button_selNone
            // 
            this.button_selNone.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_selNone.Location = new System.Drawing.Point(93, 292);
            this.button_selNone.Name = "button_selNone";
            this.button_selNone.Size = new System.Drawing.Size(75, 25);
            this.button_selNone.TabIndex = 4;
            this.button_selNone.Text = "不选";
            this.button_selNone.UseVisualStyleBackColor = true;
            this.button_selNone.Click += new System.EventHandler(this.button_selNone_Click);
            // 
            // button_reverseSel
            // 
            this.button_reverseSel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_reverseSel.Location = new System.Drawing.Point(174, 292);
            this.button_reverseSel.Name = "button_reverseSel";
            this.button_reverseSel.Size = new System.Drawing.Size(75, 25);
            this.button_reverseSel.TabIndex = 5;
            this.button_reverseSel.Text = "反选";
            this.button_reverseSel.UseVisualStyleBackColor = true;
            this.button_reverseSel.Click += new System.EventHandler(this.button_reverseSel_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(237, 268);
            this.listBox1.TabIndex = 6;
            // 
            // Form_MultiElementSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 368);
            this.ControlBox = false;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button_reverseSel);
            this.Controls.Add(this.button_selNone);
            this.Controls.Add(this.button_selAll);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_ok);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_MultiElementSelection";
            this.ShowIcon = false;
            this.Text = "选择图元";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_selAll;
        private System.Windows.Forms.Button button_selNone;
        private System.Windows.Forms.Button button_reverseSel;
        private System.Windows.Forms.ListBox listBox1;
    }
}