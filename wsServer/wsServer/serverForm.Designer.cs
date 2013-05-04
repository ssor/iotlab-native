namespace wsServer
{
    partial class serverForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbUHF = new System.Windows.Forms.CheckBox();
            this.ckbGPS = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.btnTestCmd = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "启动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "停止";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(123, 37);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(436, 45);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "gps_signal...";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(484, 88);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "发送";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 17);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(537, 121);
            this.txtLog.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLog);
            this.groupBox1.Location = new System.Drawing.Point(16, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(543, 141);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "log记录";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckbUHF);
            this.groupBox2.Controls.Add(this.ckbGPS);
            this.groupBox2.Location = new System.Drawing.Point(19, 143);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(537, 203);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设备状态";
            // 
            // ckbUHF
            // 
            this.ckbUHF.AutoSize = true;
            this.ckbUHF.Location = new System.Drawing.Point(14, 53);
            this.ckbUHF.Name = "ckbUHF";
            this.ckbUHF.Size = new System.Drawing.Size(42, 16);
            this.ckbUHF.TabIndex = 1;
            this.ckbUHF.Text = "UHF";
            this.ckbUHF.UseVisualStyleBackColor = true;
            // 
            // ckbGPS
            // 
            this.ckbGPS.AutoSize = true;
            this.ckbGPS.Location = new System.Drawing.Point(14, 23);
            this.ckbGPS.Name = "ckbGPS";
            this.ckbGPS.Size = new System.Drawing.Size(42, 16);
            this.ckbGPS.TabIndex = 0;
            this.ckbGPS.Text = "GPS";
            this.ckbGPS.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(123, 101);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "测试";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnTestCmd
            // 
            this.btnTestCmd.Location = new System.Drawing.Point(237, 100);
            this.btnTestCmd.Name = "btnTestCmd";
            this.btnTestCmd.Size = new System.Drawing.Size(75, 23);
            this.btnTestCmd.TabIndex = 9;
            this.btnTestCmd.Text = "命令匹配";
            this.btnTestCmd.UseVisualStyleBackColor = true;
            this.btnTestCmd.Click += new System.EventHandler(this.btnTestCmd_Click);
            // 
            // serverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 505);
            this.Controls.Add(this.btnTestCmd);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "serverForm";
            this.Text = "硬件模块管理";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ckbUHF;
        private System.Windows.Forms.CheckBox ckbGPS;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnTestCmd;
    }
}