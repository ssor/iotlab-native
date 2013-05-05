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
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using ModuleCommand;

namespace wsServer
{
    public partial class serverForm : Form
    {
        string deviceIP = string.Empty;
        WebSocketServer wssv = null;
        public serverForm()
        {
            InitializeComponent();

            //moduleMannager.moduleList.Add("gps", this.ckbGPS);
            //moduleMannager.moduleList.Add("uhf", this.ckbUHF);

            this.button2.Enabled = false;

            this.deviceIP = Program.GetLocalIP4();

            this.Shown += serverForm_Shown;
        }

        void serverForm_Shown(object sender, EventArgs e)
        {
            //检查设备状态
            检查设备状态(Program.getRemoteIPEndPoint());
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
            wssv.AddWebSocketService<GPSService>("/gps");
            wssv.AddWebSocketService<UHFService>("/uhf");
            wssv.AddWebSocketService<GreenLightService>("/green_light");
            wssv.AddWebSocketService<RedLightService>("/red_light");
            wssv.AddWebSocketService<YellowLightService>("/yellow_light");
            wssv.AddWebSocketService<FanService>("/fan");
            wssv.AddWebSocketService<EngineService>("/engine");
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

        private void button4_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(this.deviceIP);
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 19200);

            检查设备状态(ipEndPoint);
        }
        #region 设备检测
        void 检查设备状态(IPEndPoint ipEndPoint)
        {
            检查绿灯状态(ipEndPoint);
            检查黄灯状态(ipEndPoint);
            检查红灯状态(ipEndPoint);
            查询电机状态(ipEndPoint);
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
                               this.add_log("红灯已经打开");
                               RedLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               this.add_log("红灯已经关闭");
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
                               this.add_log("黄灯已经打开");
                               YellowLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               this.add_log("黄灯已经关闭");
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
                               this.add_log("绿灯已经打开");
                               GreenLightService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               this.add_log("绿灯已经关闭");
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
                               this.add_log("电机已经打开");
                               EngineService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               this.add_log("电机已经关闭");
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
                               this.add_log("风扇已经打开");
                               FanService.last_effective_command = new command("open", "");
                           }
                           else
                           {
                               this.add_log("风扇已经关闭");
                               FanService.last_effective_command = new command("close", "");
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询风扇状态, ipEndPoint);
        }

        #endregion
        private void btnTestCmd_Click(object sender, EventArgs e)
        {
            frmProtocolTest form = new frmProtocolTest();
            form.ShowDialog();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.组网, Program.getRemoteIPEndPoint());
        }



    }
}
