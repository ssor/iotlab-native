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
    public class FanService : WebSocketService, IServicePlus
    {
        public static string recently_broadcast = string.Empty;
        command last_command = null;
        public static command last_effective_command = null;
        public FanService(WebSocketServiceManager _manager, IWebSocketConnection socket)
        {
            services.register_service("fan", this);
            this.ID = socket.ConnectionInfo.Id.ToString();
            this._manager = _manager;
            this._websocket = socket;
            this._context = socket.ConnectionInfo;
        }
        public void MCOpen()
        {
            if (last_effective_command != null)
            {
                recently_broadcast = JsonConvert.SerializeObject(last_effective_command);
                this.Send(recently_broadcast);
            }
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
        public void FMSend(command _cmd)
        {
            if (_cmd == null) return;

            if (last_command != null)
            {
                if (last_command.Name == _cmd.Name)
                {
                    switch (last_command.Name)
                    {
                        case stateName.打开:
                            Debug.WriteLine(string.Format("{0} 打开风扇", last_command.Commander));
                            _cmd.Para = string.Format("{0}打开了风扇", last_command.Commander);
                            //command ncOpen = new command("open", string.Format("{0}打开了风扇", last_command.Commander));
                            last_effective_command = _cmd;
                            recently_broadcast = JsonConvert.SerializeObject(_cmd);
                            this.Broadcast(recently_broadcast);
                            break;
                        case stateName.关闭:
                            Debug.WriteLine(string.Format("{0} 关闭风扇", last_command.Commander));
                            _cmd.Para = string.Format("{0}关闭了风扇", last_command.Commander);
                            //command ncClose = new command("close", string.Format("{0}关闭了风扇", last_command.Commander));
                            last_effective_command = _cmd;
                            recently_broadcast = JsonConvert.SerializeObject(_cmd);
                            this.Broadcast(recently_broadcast);
                            break;
                    }
                }
                else//说明设备没有响应，通知客户端
                {
                    _cmd.Para = "操作失败";
                    recently_broadcast = JsonConvert.SerializeObject(_cmd);
                    this.Send(recently_broadcast);//只通知本人
                }
            }

        }
        public void MCOnMessage(command _cmd)
        {
            last_command = _cmd;
            string msg = JsonConvert.SerializeObject(_cmd);
            Debug.WriteLine(string.Format("FanService OnMessage => {0}", msg));
            FuncModuleManager.OnMessage(msg);
        }
        public override void OnMessage(string msg)
        {
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                last_command = cmd;
                switch (cmd.Name)
                {
                    case "open":
                        //打开风扇(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} 试图打开风扇", cmd.Commander));
                        break;
                    case "close":
                        //关闭风扇(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} 试图关闭风扇", cmd.Commander));
                        break;
                }
                //检查风扇状态(Program.getRemoteIPEndPoint());
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
        public void OnCloseSocket()
        { }
        
    }
}
