using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using Server;
using nsUHF;
using System.Collections.Generic;
using Fleck;
using wsServer;


namespace ModuleService
{

    public class UHFService : WebSocketService
    {
        UDPServer updServer;
        TDJ_RFIDHelper rfid_helper;
        public UHFService(WebSocketServiceManager _manager, IWebSocketConnection socket)
        {
            services.register_service("uhf", this);
            this.ID = socket.ConnectionInfo.Id.ToString();
            this._manager = _manager;
            this._websocket = socket;
            this._context = socket.ConnectionInfo;

            rfid_helper = new TDJ_RFIDHelper();

            //打开UDP端口，等待数据传入
            this.updServer = UDPServer.getUDPServer(Program.UHF_UDP_Port);
            updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            updServer.startUDPListening();
        }

        void updServer_evtReceived(string str)
        {
            //Debug.WriteLine("UHF => " + str);
            List<TagInfo> list = rfid_helper.ParseDataToTag(str);
            if (list.Count > 0)
            {
                this.Send(list[0].epc);
            }
        }
        public override void OnMessage(string msg)
        {
            Debug.WriteLine(string.Format("UHF OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                //Send(cmd.print_string());
                switch (cmd.Name)
                {
                    case "open":
                        Debug.WriteLine("打开UHF");
                        break;
                    case "close":
                        Debug.WriteLine("关闭UHF");
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        public override void OnClose()
        {
            this.updServer.evtReceived -= updServer_evtReceived;
            //base.OnClose(e);
        }
    }
}
