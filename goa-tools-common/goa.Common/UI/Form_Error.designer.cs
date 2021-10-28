namespace goa.Common
{
    partial class Form_Error
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
            this.label_message = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_errorMessage = new System.Windows.Forms.TextBox();
            this.button_close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_message
            // 
            this.label_message.Location = new System.Drawing.Point(12, 9);
            this.label_message.Name = "label_message";
            this.label_message.Size = new System.Drawing.Size(315, 46);
            this.label_message.TabIndex = 0;
            this.label_message.Text = "插件遇到错误，已终止运行的任务，未对模型造成损害。\r\n\r\n请保存错误信息并联系开发者获得帮助。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "错误信息";
            // 
            // textBox_errorMessage
            // 
            this.textBox_errorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_errorMessage.Location = new System.Drawing.Point(14, 91);
            this.textBox_errorMessage.Multiline = true;
            this.textBox_errorMessage.Name = "textBox_errorMessage";
            this.textBox_errorMessage.ReadOnly = true;
            this.textBox_errorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_errorMessage.Size = new System.Drawing.Size(307, 116);
            this.textBox_errorMessage.TabIndex = 2;
            this.textBox_errorMessage.Text = "\r\n";
            // 
            // button_close
            // 
            this.button_close.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_close.Location = new System.Drawing.Point(122, 220);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 3;
            this.button_close.Text = "关闭";
            this.button_close.UseVisualStyleBackColor = true;
            // 
            // Form_Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 275);
            this.ControlBox = false;
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.textBox_errorMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_message);
            this.MinimumSize = new System.Drawing.Size(351, 291);
            this.Name = "Form_Error";
            this.ShowIcon = false;
            this.Text = "错误";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_message;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_errorMessage;
        private System.Windows.Forms.Button button_close;
    }
}