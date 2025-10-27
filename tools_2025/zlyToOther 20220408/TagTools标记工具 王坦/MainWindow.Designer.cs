
namespace TagTools
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
            this.button_spaceSelectedTag = new System.Windows.Forms.Button();
            this.button_autoSpaceAllTags = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_findOverlap = new System.Windows.Forms.Button();
            this.button_showHost = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_alignToHostCentroid = new System.Windows.Forms.CheckBox();
            this.comboBox_atPos = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_atPosDist = new System.Windows.Forms.TextBox();
            this.textBox_moveTowardHostBy = new System.Windows.Forms.TextBox();
            this.checkBox_promptChangeAll = new System.Windows.Forms.CheckBox();
            this.button_tagAtPosRelativeToHost = new System.Windows.Forms.Button();
            this.button_moveToHost = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_spaceSelectedTag
            // 
            this.button_spaceSelectedTag.Location = new System.Drawing.Point(7, 50);
            this.button_spaceSelectedTag.Name = "button_spaceSelectedTag";
            this.button_spaceSelectedTag.Size = new System.Drawing.Size(161, 23);
            this.button_spaceSelectedTag.TabIndex = 0;
            this.button_spaceSelectedTag.Text = "选择单个标记避让";
            this.button_spaceSelectedTag.UseVisualStyleBackColor = true;
            this.button_spaceSelectedTag.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_autoSpaceAllTags
            // 
            this.button_autoSpaceAllTags.Location = new System.Drawing.Point(7, 79);
            this.button_autoSpaceAllTags.Name = "button_autoSpaceAllTags";
            this.button_autoSpaceAllTags.Size = new System.Drawing.Size(161, 23);
            this.button_autoSpaceAllTags.TabIndex = 1;
            this.button_autoSpaceAllTags.Text = "自动避让视图中所有";
            this.button_autoSpaceAllTags.UseVisualStyleBackColor = true;
            this.button_autoSpaceAllTags.Click += new System.EventHandler(this.button2_autoSpaceAllTags_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_findOverlap);
            this.groupBox1.Controls.Add(this.button_spaceSelectedTag);
            this.groupBox1.Controls.Add(this.button_autoSpaceAllTags);
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 113);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "标记避让";
            // 
            // button_findOverlap
            // 
            this.button_findOverlap.Location = new System.Drawing.Point(7, 21);
            this.button_findOverlap.Name = "button_findOverlap";
            this.button_findOverlap.Size = new System.Drawing.Size(161, 23);
            this.button_findOverlap.TabIndex = 2;
            this.button_findOverlap.Text = "检查重叠标记";
            this.button_findOverlap.UseVisualStyleBackColor = true;
            this.button_findOverlap.Click += new System.EventHandler(this.button_findOverlap_Click);
            // 
            // button_showHost
            // 
            this.button_showHost.Location = new System.Drawing.Point(7, 20);
            this.button_showHost.Name = "button_showHost";
            this.button_showHost.Size = new System.Drawing.Size(161, 23);
            this.button_showHost.TabIndex = 3;
            this.button_showHost.Text = "主体";
            this.button_showHost.UseVisualStyleBackColor = true;
            this.button_showHost.Click += new System.EventHandler(this.button_showHost_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_showHost);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(177, 51);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标记信息";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_alignToHostCentroid);
            this.groupBox3.Controls.Add(this.comboBox_atPos);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBox_atPosDist);
            this.groupBox3.Controls.Add(this.checkBox_promptChangeAll);
            this.groupBox3.Controls.Add(this.button_tagAtPosRelativeToHost);
            this.groupBox3.Location = new System.Drawing.Point(12, 188);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(177, 123);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "标记位置调整";
            // 
            // checkBox_alignToHostCentroid
            // 
            this.checkBox_alignToHostCentroid.AutoSize = true;
            this.checkBox_alignToHostCentroid.Location = new System.Drawing.Point(7, 77);
            this.checkBox_alignToHostCentroid.Name = "checkBox_alignToHostCentroid";
            this.checkBox_alignToHostCentroid.Size = new System.Drawing.Size(144, 16);
            this.checkBox_alignToHostCentroid.TabIndex = 9;
            this.checkBox_alignToHostCentroid.Text = "对齐主体形心（居中）";
            this.checkBox_alignToHostCentroid.UseVisualStyleBackColor = true;
            // 
            // comboBox_atPos
            // 
            this.comboBox_atPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_atPos.FormattingEnabled = true;
            this.comboBox_atPos.Location = new System.Drawing.Point(7, 51);
            this.comboBox_atPos.Name = "comboBox_atPos";
            this.comboBox_atPos.Size = new System.Drawing.Size(61, 20);
            this.comboBox_atPos.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "mm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(213, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "位移";
            // 
            // textBox_atPosDist
            // 
            this.textBox_atPosDist.Location = new System.Drawing.Point(74, 50);
            this.textBox_atPosDist.Name = "textBox_atPosDist";
            this.textBox_atPosDist.Size = new System.Drawing.Size(70, 21);
            this.textBox_atPosDist.TabIndex = 4;
            this.textBox_atPosDist.Text = "0";
            // 
            // textBox_moveTowardHostBy
            // 
            this.textBox_moveTowardHostBy.Location = new System.Drawing.Point(247, 214);
            this.textBox_moveTowardHostBy.Name = "textBox_moveTowardHostBy";
            this.textBox_moveTowardHostBy.Size = new System.Drawing.Size(102, 21);
            this.textBox_moveTowardHostBy.TabIndex = 3;
            this.textBox_moveTowardHostBy.Text = "0";
            // 
            // checkBox_promptChangeAll
            // 
            this.checkBox_promptChangeAll.AutoSize = true;
            this.checkBox_promptChangeAll.Location = new System.Drawing.Point(6, 99);
            this.checkBox_promptChangeAll.Name = "checkBox_promptChangeAll";
            this.checkBox_promptChangeAll.Size = new System.Drawing.Size(96, 16);
            this.checkBox_promptChangeAll.TabIndex = 2;
            this.checkBox_promptChangeAll.Text = "提示修改所有";
            this.checkBox_promptChangeAll.UseVisualStyleBackColor = true;
            // 
            // button_tagAtPosRelativeToHost
            // 
            this.button_tagAtPosRelativeToHost.Location = new System.Drawing.Point(7, 24);
            this.button_tagAtPosRelativeToHost.Name = "button_tagAtPosRelativeToHost";
            this.button_tagAtPosRelativeToHost.Size = new System.Drawing.Size(161, 23);
            this.button_tagAtPosRelativeToHost.TabIndex = 1;
            this.button_tagAtPosRelativeToHost.Text = "标记位于主体";
            this.button_tagAtPosRelativeToHost.UseVisualStyleBackColor = true;
            this.button_tagAtPosRelativeToHost.Click += new System.EventHandler(this.button_tagAtPosRelativeToHost_Click);
            // 
            // button_moveToHost
            // 
            this.button_moveToHost.Location = new System.Drawing.Point(212, 188);
            this.button_moveToHost.Name = "button_moveToHost";
            this.button_moveToHost.Size = new System.Drawing.Size(161, 23);
            this.button_moveToHost.TabIndex = 0;
            this.button_moveToHost.Text = "标记靠近主体";
            this.button_moveToHost.UseVisualStyleBackColor = true;
            this.button_moveToHost.Click += new System.EventHandler(this.button_snapToHost_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 317);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_moveToHost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_moveTowardHostBy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "goa 标记工具";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_spaceSelectedTag;
        private System.Windows.Forms.Button button_autoSpaceAllTags;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_findOverlap;
        private System.Windows.Forms.Button button_showHost;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_moveToHost;
        private System.Windows.Forms.Button button_tagAtPosRelativeToHost;
        private System.Windows.Forms.CheckBox checkBox_promptChangeAll;
        private System.Windows.Forms.ComboBox comboBox_atPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_atPosDist;
        private System.Windows.Forms.TextBox textBox_moveTowardHostBy;
        private System.Windows.Forms.CheckBox checkBox_alignToHostCentroid;
    }
}