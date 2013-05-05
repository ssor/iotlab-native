using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wsServer
{
    public partial class frmProtocolTest : Form
    {
        List<CommandMatch> pList = null;
        bool changed = false;
        public frmProtocolTest()
        {
            InitializeComponent();


            pList = CommandMatchHelper.importCommand();
            if (pList == null)
            {
                pList = new List<CommandMatch>();
            }

            Array array = Enum.GetValues(typeof(enumDeviceCommand));
            for (int i = 0; i < array.Length; i++)
            {
                CommandMatchHelper.addCommand(pList, new CommandMatch(array.GetValue(i).ToString(), ""));
            }
            //CommandMatchHelper.addCommand(pList, new CommandMatch(enumDeviceCommand.检查红灯状态.ToString(), ""));
            //CommandMatchHelper.addCommand(pList, new CommandMatch(enumDeviceCommand.检查绿灯状态.ToString(), ""));
            //CommandMatchHelper.addCommand(pList, new CommandMatch(enumDeviceCommand.检查黄灯状态.ToString(), ""));
            // 。。。

            foreach (CommandMatch c in pList)
            {
                this.cbCmd.Items.Add(c.name);
            }


            this.FormClosing += frmProtocolTest_FormClosing;

            this.cbCmd.SelectedIndexChanged += cbCmd_SelectedIndexChanged;
            if (this.cbCmd.Items.Count > 0)
            {
                this.cbCmd.SelectedIndex = 0;
            }
        }

        void cbCmd_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = (string)this.cbCmd.SelectedItem;
            if (name != null)
            {
                CommandMatch cmd = pList.Find((_cmd) =>
                {
                    return _cmd.name == name;
                });
                if (cmd != null)
                {
                    this.txtCmd.Text = cmd.cmd;
                }
            }
        }

        void frmProtocolTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                DialogResult dr = MessageBox.Show("协议内容已经改变，需要保存吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    CommandMatchHelper.exportCommand(pList);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = (string)this.cbCmd.SelectedItem;
            if (name != null)
            {
                if (CommandMatchHelper.updateCommand(pList, new CommandMatch(name, this.txtCmd.Text)))
                {
                    this.changed = true;
                }
                else
                {
                    MessageBox.Show("保存失败，可能已经存在该命令！");
                }
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string name = (string)this.cbCmd.SelectedItem;
            if (name == null) return;
            string strCmd = this.txtCmd.Text;
            if (strCmd.Length <= 0)
            {
                return;
            }
            this.sendCommand(strCmd);
            this.addLog("发送命令 => " + name + ": " + strCmd);
        }
        #region 内部函数
        void addLog(string log)
        {
            this.txtLog.Text = log + "\r\n" + this.txtLog.Text;
        }
        void getResponseData(IAsyncResult ar)
        {
            Socket clientSocket = (Socket)ar.AsyncState;
            clientSocket.EndSend(ar);
            byte[] byteData = new byte[1024];
            clientSocket.Receive(byteData);

            string strReceived = Encoding.UTF8.GetString(byteData);

            Array.Clear(byteData, 0, byteData.Length);
            int i = strReceived.IndexOf("\0");
            if (i > 0)
            {
                string data = strReceived.Substring(0, i);
                Debug.WriteLine(" Data => SP: " + data);
                //todo here should deal with the received string
                Action<string> func = (_data) =>
                {
                    this.addLog("返回数据 <= " + _data);
                };
                this.Invoke(func, data);
            }
        }
        void sendCommand(string _cmd)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] _byteData = Encoding.UTF8.GetBytes(_cmd);
            IPAddress ip = IPAddress.Parse(Program.GetLocalIP4());
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 19200);

            clientSocket.BeginSendTo(_byteData, 0, _byteData.Length, SocketFlags.None,
                            ipEndPoint, getResponseData, clientSocket);
        }
        #endregion

    }
}
