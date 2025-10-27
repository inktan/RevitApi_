
namespace CoordinationInfo
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
            this.button_setAll = new System.Windows.Forms.Button();
            this.button_setPicked = new System.Windows.Forms.Button();
            this.button_pickLevels = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_elemInLinkId = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_setAll
            // 
            this.button_setAll.Location = new System.Drawing.Point(6, 49);
            this.button_setAll.Name = "button_setAll";
            this.button_setAll.Size = new System.Drawing.Size(175, 23);
            this.button_setAll.TabIndex = 0;
            this.button_setAll.Text = "设置模型中全部";
            this.button_setAll.UseVisualStyleBackColor = true;
            this.button_setAll.Click += new System.EventHandler(this.button_setAll_Click);
            // 
            // button_setPicked
            // 
            this.button_setPicked.Location = new System.Drawing.Point(6, 78);
            this.button_setPicked.Name = "button_setPicked";
            this.button_setPicked.Size = new System.Drawing.Size(175, 23);
            this.button_setPicked.TabIndex = 1;
            this.button_setPicked.Text = "设置选择图元";
            this.button_setPicked.UseVisualStyleBackColor = true;
            this.button_setPicked.Click += new System.EventHandler(this.button_setPicked_Click);
            // 
            // button_pickLevels
            // 
            this.button_pickLevels.Location = new System.Drawing.Point(6, 20);
            this.button_pickLevels.Name = "button_pickLevels";
            this.button_pickLevels.Size = new System.Drawing.Size(175, 23);
            this.button_pickLevels.TabIndex = 2;
            this.button_pickLevels.Text = "选择楼层";
            this.button_pickLevels.UseVisualStyleBackColor = true;
            this.button_pickLevels.Click += new System.EventHandler(this.button_pickLevels_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_pickLevels);
            this.groupBox1.Controls.Add(this.button_setAll);
            this.groupBox1.Controls.Add(this.button_setPicked);
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 112);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "协作楼层";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_elemInLinkId);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 53);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "链接";
            // 
            // button_elemInLinkId
            // 
            this.button_elemInLinkId.Location = new System.Drawing.Point(6, 20);
            this.button_elemInLinkId.Name = "button_elemInLinkId";
            this.button_elemInLinkId.Size = new System.Drawing.Size(175, 23);
            this.button_elemInLinkId.TabIndex = 3;
            this.button_elemInLinkId.Text = "选择楼层";
            this.button_elemInLinkId.UseVisualStyleBackColor = true;
            this.button_elemInLinkId.Click += new System.EventHandler(this.button_elemInLinkId_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 193);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "goa 构件协作信息";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_setAll;
        private System.Windows.Forms.Button button_setPicked;
        private System.Windows.Forms.Button button_pickLevels;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_elemInLinkId;
    }
}