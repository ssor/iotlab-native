using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebSocketSharp.Server;
using ModuleService;

namespace wsServer
{
    public partial class serverForm : Form
    {
        WebSocketServer wssv = null;
        public serverForm()
        {
            InitializeComponent();

            //moduleMannager.moduleList.Add("gps", this.ckbGPS);
            //moduleMannager.moduleList.Add("uhf", this.ckbUHF);

            this.button2.Enabled = false;
        }

        public void add_log(string log)
        {
            Action<string> funcInvoke = data =>
            {
                this.txtLog.Text = data + "\r\n" + this.txtLog.Text;
            };

            if (this.txtLog.InvokeRequired)
            {
                this.txtLog.Invoke(funcInvoke, log);
            }
            else
                funcInvoke(log);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            wssv = new WebSocketServer(4649);
            wssv.AddWebSocketService<Echo>("/Echo");
            wssv.AddWebSocketService<GPS>("/gps");
            wssv.AddWebSocketService<UHF>("/uhf");
            wssv.AddWebSocketService<Light>("/light");
            wssv.Start();
            this.button2.Enabled = true;
            this.button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            wssv.Stop();
            this.button2.Enabled = false;
            this.button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WebSocketService service = services.get_service("gps");

            service.Send(this.textBox1.Text);
        }
    }
}
