using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleService;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using ModuleCommand;
using com.google.zxing.common;
using com.google.zxing;
using System.Threading;
using Fleck;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace wsServer
{
    public partial class serverForm : Form
    {
        string deviceIP = string.Empty;
        //WebSocketServer wssv = null;
        string FMServerUrl = "ws://localhost:4650";
        string MCServerUrl = "ws://localhost:4649";
        WebSocketServer MCserver = null;
        WebSocketServer FMserver = null;//功能模块服务
        ServiceHost host = new ServiceHost();
        //WebSocket ws = null;
        public serverForm()
        {
            InitializeComponent();

            //moduleMannager.moduleList.Add("gps", this.ckbGPS);
            //moduleMannager.moduleList.Add("uhf", this.ckbUHF);

            this.button2.Enabled = false;

            this.deviceIP = Program.GetLocalIP4();
            this.txtIP.Text = this.deviceIP;
            this.Shown += serverForm_Shown;
        }

        void serverForm_Shown(object sender, EventArgs e)
        {
            //string content = "http://" + this.deviceIP + ":9901/index.php";
            //this.txtQRcode.Text = content;
            //setQRcode(content);

        }
        //void setQRcode(string content)
        //{
        //    int heigth = this.pictureBox1.Height;
        //    int width = this.pictureBox1.Width;
        //    BarcodeFormat format = BarcodeFormat.QR_CODE;

        //    ByteMatrix byteMatrix = new MultiFormatWriter().encode(content, format, width, heigth);
        //    Bitmap bitmap = toBitmap(byteMatrix);
        //    pictureBox1.Image = bitmap;
        //}
        public Bitmap toBitmap(ByteMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix.get_Renamed(x, y) != -1 ? ColorTranslator.FromHtml("0xFF000000") : ColorTranslator.FromHtml("0xFFFFFFFF"));
                }
            }
            return bmap;
        }
        public void add_log(string log)
        {
            Action<string> funcInvoke = data =>
            {
                this.txtLog.Text = data + "\r\n" + this.txtLog.Text;
            };
            this.Invoke(funcInvoke, log);
            //if (this.txtLog.InvokeRequired)
            //{
            //    this.txtLog.Invoke(funcInvoke, log);
            //}
            //else
            //    funcInvoke(log);
        }
        private void button1_Click(object sender1, EventArgs e1)
        {
            try
            {
                MCserver = new WebSocketServer(MCServerUrl);
                MCserver.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        host.OnOpenMCWebSocket(socket);
                    };
                    socket.OnClose = () =>
                    {
                        host.OnCloseMCWebSocket(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        host.OnMCMessage(message, socket);
                    };
                });


                FMserver = new WebSocketServer(FMServerUrl);
                FMserver.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        FuncModuleManager.addClient(socket);
                    };
                    socket.OnClose = () =>
                    {
                        FuncModuleManager.removeClient(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        Debug.WriteLine("*****  FM Client Message  => " + message);
                        host.FMSend(message);
                    };
                    socket.OnError = (error) =>
                    {
                        Debug.WriteLine("OnError => " + error.Data);
                        FuncModuleManager.removeClient(socket);
                    };
                });

                //this.initial_udp_server(Program.inputPort);
                //检查设备状态(Program.getRemoteIPEndPoint(), 3000);

                this.button2.Enabled = true;
                this.button1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //wssv.Stop();
            MCserver.Dispose();
            MCserver = null;
            //if (serverSocket != null)
            //{
            //    serverSocket.Close();
            //    serverSocket = null;
            //}
            this.button2.Enabled = false;
            this.button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(this.deviceIP);
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 19200);

            检查设备状态(ipEndPoint, 2000);
        }
        private void btnTestCmd_Click(object sender, EventArgs e)
        {
            frmProtocolTest form = new frmProtocolTest();
            form.ShowDialog();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.组网, Program.getRemoteIPEndPoint());
        }

        private void btnResetQrcode_Click(object sender, EventArgs e)
        {
            //if (txtQRcode.Text == string.Empty)
            //{
            //    return;
            //}
            //else
            //{
            //    setQRcode(txtQRcode.Text);
            //}
        }

        #region 设备检测
        void 检查设备状态(IPEndPoint ipEndPoint, int timeLater)
        {
            检查绿灯状态(ipEndPoint);

            Thread.Sleep(timeLater);
            检查黄灯状态(ipEndPoint);

            Thread.Sleep(timeLater);
            检查红灯状态(ipEndPoint);

            Thread.Sleep(timeLater);
            查询电机状态(ipEndPoint);

            Thread.Sleep(timeLater);
            查询风扇状态(ipEndPoint);
        }
        void 检查红灯状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询红灯状态,
               (data) =>
               {
                   Debug.WriteLine("红灯状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询红灯状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           if (ls.State)
                           {
                               string log = "红灯已经打开";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               RedLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               string log = "红灯已经关闭";
                               this.add_log(log);
                               Debug.WriteLine(log);

                               RedLightService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询红灯状态, ipEndPoint);
        }
        void 检查黄灯状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询黄灯状态,
               (data) =>
               {
                   Debug.WriteLine("黄灯状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询黄灯状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           if (ls.State)
                           {
                               string log = "黄灯已经打开";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               YellowLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {

                               string log = "黄灯已经关闭";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               YellowLightService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询黄灯状态, ipEndPoint);
        }
        void 检查绿灯状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询绿灯状态,
               (data) =>
               {
                   Debug.WriteLine("绿灯状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询绿灯状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           if (ls.State)
                           {
                               string log = "绿灯已经打开";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               GreenLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               string log = "绿灯已经关闭";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               GreenLightService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询绿灯状态, ipEndPoint);
        }
        void 查询电机状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询电机状态,
               (data) =>
               {
                   Debug.WriteLine("电机状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询电机状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           if (ls.State)
                           {
                               string log = "电机已经打开";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               EngineService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               string log = "电机已经关闭";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               EngineService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询电机状态, ipEndPoint);
        }
        void 查询风扇状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询风扇状态,
               (data) =>
               {
                   Debug.WriteLine("风扇状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询风扇状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           if (ls.State)
                           {
                               string log = "风扇已经打开";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               FanService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               string log = "风扇已经关闭";
                               this.add_log(log);
                               Debug.WriteLine(log);
                               FanService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询风扇状态, ipEndPoint);
        }

        #endregion


        #region UDP 监听
        //Socket serverSocket = null; //The main server socket
        //byte[] byteData = new byte[1024];

        //void initial_udp_server(int port)
        //{
        //    try
        //    {
        //        serverSocket = new Socket(AddressFamily.InterNetwork,
        //                    SocketType.Dgram, ProtocolType.Udp);
        //        IPAddress ip = IPAddress.Parse(Program.GetLocalIP4());
        //        IPEndPoint ipEndPoint = new IPEndPoint(ip, port);
        //        //Bind this address to the server
        //        serverSocket.Bind(ipEndPoint);
        //        //防止客户端强行中断造成的异常
        //        long IOC_IN = 0x80000000;
        //        long IOC_VENDOR = 0x18000000;
        //        long SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

        //        byte[] optionInValue = { Convert.ToByte(false) };
        //        byte[] optionOutValue = new byte[4];
        //        serverSocket.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);

        //        IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
        //        //The epSender identifies the incoming clients
        //        EndPoint epSender = (EndPoint)ipeSender;

        //        //Start receiving data
        //        serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
        //            SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }
        //}
        //private void OnReceive(IAsyncResult ar)
        //{

        //    try
        //    {
        //        IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
        //        EndPoint epSender = (EndPoint)ipeSender;

        //        serverSocket.EndReceiveFrom(ar, ref epSender);
        //        StringBuilder builder = new StringBuilder();
        //        if (true)//16进制
        //        {
        //            //依次的拼接出16进制字符串
        //            foreach (byte b in byteData)
        //            {
        //                if (b > 0)
        //                {
        //                    builder.Append(b.ToString("X2"));
        //                }
        //            }

        //        }
        //        else
        //        {
        //            //直接按ASCII规则转换成字符串
        //            builder.Append(Encoding.ASCII.GetString(byteData));
        //        }

        //        Array.Clear(byteData, 0, byteData.Length);
        //        string strReceived = builder.ToString();
        //        //int i = strReceived.IndexOf("\0");
        //        if (strReceived.Length > 0)
        //        {
        //            IDeviceCommand cmd = DeviceCommandManager.getDeivceCommandWithResponseOf(strReceived);
        //            if (cmd != null)
        //            {
        //                cmd.callBack(strReceived);
        //            }
        //        }

        //        //Start listening to the message send by the user
        //        serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
        //            new AsyncCallback(OnReceive), epSender);

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(
        //            string.Format("UDPServer.OnReceive  -> error = {0}"
        //            , ex.Message));
        //    }
        //}
        #endregion

    }
}
