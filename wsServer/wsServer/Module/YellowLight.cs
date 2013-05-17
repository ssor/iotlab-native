using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using Server;
using nsUHF;
using System.Collections.Generic;
using System.Net;
using wsServer;
using Fleck;


namespace ModuleService
{
    public class YellowLightService : WebSocketService
    {
        public static string recently_broadcast = string.Empty;
        command last_command = null;
        public static command last_effective_command = null;
        public YellowLightService(WebSocketServiceManager _manager, IWebSocketConnection socket)
        {
            services.register_service("yellow_light", this);
            this.ID = socket.ConnectionInfo.Id.ToString();
            this._manager = _manager;
            this._websocket = socket;
            this._context = socket.ConnectionInfo;
        }
        public override void OnOpen()
        {
            if (last_effective_command != null)
            {
                recently_broadcast = JsonConvert.SerializeObject(last_effective_command);
                this.Send(recently_broadcast);
            }
            //if (recently_broadcast != string.Empty)
            //{
            //    //this.Broadcast(recently_broadcast); 

            //}
        }
        public override void OnMessage(string msg)
        {
            Debug.WriteLine(string.Format("YellowLight OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                last_command = cmd;
                switch (cmd.Name)
                {
                    case "open":
                        打开灯(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} 试图打开黄灯", cmd.Commander));
                        break;
                    case "close":
                        关闭灯(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} 关闭黄灯", cmd.Commander));
                        break;
                }
                检查灯状态(Program.getRemoteIPEndPoint());
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        public override void OnClose()
        {
            //base.OnClose(e);
        }

        void 关闭灯(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.关闭黄灯, ipEndPoint);

        }
        void 打开灯(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.打开黄灯, ipEndPoint);

        }
        void 检查灯状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询黄灯状态,
               (data) =>
               {
                   Debug.WriteLine("黄灯状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询黄灯状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls && last_command != null)
                       {
                           bool temp = (last_command.Name == "open") ? true : false;
                           if (ls.State == temp)//灯现在的状态和操作目标状态相同，同为关或者开
                           {
                               switch (last_command.Name)
                               {
                                   case "open":
                                       Debug.WriteLine(string.Format("{0} 打开黄灯", last_command.Commander));
                                       command ncOpen = new command("open", string.Format("{0}打开了灯", last_command.Commander));
                                       last_effective_command = ncOpen;
                                       recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                                       this.Broadcast(recently_broadcast);
                                       break;
                                   case "close":
                                       Debug.WriteLine(string.Format("{0} 关闭黄灯", last_command.Commander));
                                       command ncClose = new command("close", string.Format("{0}关闭了灯", last_command.Commander));
                                       last_effective_command = ncClose;
                                       recently_broadcast = JsonConvert.SerializeObject(ncClose);
                                       this.Broadcast(recently_broadcast);
                                       break;
                               }
                           }
                           else//说明设备没有响应，通知客户端
                           {
                               switch (ls.State)
                               {
                                   case true:
                                       command ncOpen = new command("open", "操作失败");
                                       recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                                       this.Send(recently_broadcast);//只通知本人
                                       Debug.WriteLine("关闭黄灯操作失败");
                                       break;
                                   case false:
                                       command ncClose = new command("close", "操作失败");
                                       recently_broadcast = JsonConvert.SerializeObject(ncClose);
                                       this.Send(recently_broadcast);
                                       Debug.WriteLine("打开黄灯操作失败");
                                       break;
                               }
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询黄灯状态, ipEndPoint, 1000);
        }
    }
}
