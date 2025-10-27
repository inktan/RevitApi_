
namespace DimensioningTools
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
            this.button_dimClosestPoint = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button_fakeDim = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_absElevs = new System.Windows.Forms.Button();
            this.button_fakeSpotElevUpdate = new System.Windows.Forms.Button();
            this.button_multiElevFam = new System.Windows.Forms.Button();
            this.button_dimOnFloors = new System.Windows.Forms.Button();
            this.button_windowSpotElevDim = new System.Windows.Forms.Button();
            this.button_multiLevelElev = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_dimClosestPoint
            // 
            this.button_dimClosestPoint.Location = new System.Drawing.Point(6, 49);
            this.button_dimClosestPoint.Name = "button_dimClosestPoint";
            this.button_dimClosestPoint.Size = new System.Drawing.Size(139, 23);
            this.button_dimClosestPoint.TabIndex = 0;
            this.button_dimClosestPoint.Text = "标注最近角点";
            this.button_dimClosestPoint.UseVisualStyleBackColor = true;
            this.button_dimClosestPoint.Click += new System.EventHandler(this.button_dimClosestPoint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button_fakeDim);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button_dimClosestPoint);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(151, 143);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "尺寸标注";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(139, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "假尺寸文字避让";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_fakeDim
            // 
            this.button_fakeDim.Location = new System.Drawing.Point(6, 20);
            this.button_fakeDim.Name = "button_fakeDim";
            this.button_fakeDim.Size = new System.Drawing.Size(139, 23);
            this.button_fakeDim.TabIndex = 6;
            this.button_fakeDim.Text = "假尺寸标注";
            this.button_fakeDim.UseVisualStyleBackColor = true;
            this.button_fakeDim.Click += new System.EventHandler(this.button_fakeDim_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "真尺寸文字避让";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_absElevs);
            this.groupBox2.Controls.Add(this.button_fakeSpotElevUpdate);
            this.groupBox2.Controls.Add(this.button_multiElevFam);
            this.groupBox2.Controls.Add(this.button_dimOnFloors);
            this.groupBox2.Controls.Add(this.button_windowSpotElevDim);
            this.groupBox2.Location = new System.Drawing.Point(12, 165);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 169);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标高标注";
            // 
            // button_absElevs
            // 
            this.button_absElevs.Location = new System.Drawing.Point(6, 136);
            this.button_absElevs.Name = "button_absElevs";
            this.button_absElevs.Size = new System.Drawing.Size(139, 23);
            this.button_absElevs.TabIndex = 4;
            this.button_absElevs.Text = "绝对标高计算";
            this.button_absElevs.UseVisualStyleBackColor = true;
            this.button_absElevs.Click += new System.EventHandler(this.button_absElevs_Click);
            // 
            // button_fakeSpotElevUpdate
            // 
            this.button_fakeSpotElevUpdate.Location = new System.Drawing.Point(6, 78);
            this.button_fakeSpotElevUpdate.Name = "button_fakeSpotElevUpdate";
            this.button_fakeSpotElevUpdate.Size = new System.Drawing.Size(139, 23);
            this.button_fakeSpotElevUpdate.TabIndex = 3;
            this.button_fakeSpotElevUpdate.Text = "假标高计算";
            this.button_fakeSpotElevUpdate.UseVisualStyleBackColor = true;
            this.button_fakeSpotElevUpdate.Click += new System.EventHandler(this.button_fakeSpotElevUpdate_Click);
            // 
            // button_multiElevFam
            // 
            this.button_multiElevFam.Location = new System.Drawing.Point(6, 49);
            this.button_multiElevFam.Name = "button_multiElevFam";
            this.button_multiElevFam.Size = new System.Drawing.Size(139, 23);
            this.button_multiElevFam.TabIndex = 2;
            this.button_multiElevFam.Text = "填写多重标高族";
            this.button_multiElevFam.UseVisualStyleBackColor = true;
            this.button_multiElevFam.Click += new System.EventHandler(this.button_multiElevFam_Click);
            // 
            // button_dimOnFloors
            // 
            this.button_dimOnFloors.Location = new System.Drawing.Point(6, 107);
            this.button_dimOnFloors.Name = "button_dimOnFloors";
            this.button_dimOnFloors.Size = new System.Drawing.Size(139, 23);
            this.button_dimOnFloors.TabIndex = 1;
            this.button_dimOnFloors.Text = "批量楼板标高";
            this.button_dimOnFloors.UseVisualStyleBackColor = true;
            this.button_dimOnFloors.Click += new System.EventHandler(this.button_dimOnFloors_Click);
            // 
            // button_windowSpotElevDim
            // 
            this.button_windowSpotElevDim.Location = new System.Drawing.Point(6, 20);
            this.button_windowSpotElevDim.Name = "button_windowSpotElevDim";
            this.button_windowSpotElevDim.Size = new System.Drawing.Size(139, 23);
            this.button_windowSpotElevDim.TabIndex = 1;
            this.button_windowSpotElevDim.Text = "窗高标注";
            this.button_windowSpotElevDim.UseVisualStyleBackColor = true;
            this.button_windowSpotElevDim.Click += new System.EventHandler(this.button_windowSpotElevDim_Click);
            // 
            // button_multiLevelElev
            // 
            this.button_multiLevelElev.Location = new System.Drawing.Point(198, 237);
            this.button_multiLevelElev.Name = "button_multiLevelElev";
            this.button_multiLevelElev.Size = new System.Drawing.Size(139, 23);
            this.button_multiLevelElev.TabIndex = 2;
            this.button_multiLevelElev.Text = "批量楼层标高";
            this.button_multiLevelElev.UseVisualStyleBackColor = true;
            this.button_multiLevelElev.Click += new System.EventHandler(this.button_multiLevelElev_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(171, 351);
            this.Controls.Add(this.button_multiLevelElev);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "尺寸标高标注";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_dimClosestPoint;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_windowSpotElevDim;
        private System.Windows.Forms.Button button_dimOnFloors;
        private System.Windows.Forms.Button button_multiLevelElev;
        private System.Windows.Forms.Button button_multiElevFam;
        private System.Windows.Forms.Button button_fakeSpotElevUpdate;
        private System.Windows.Forms.Button button_absElevs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_fakeDim;
        private System.Windows.Forms.Button button2;
    }
}