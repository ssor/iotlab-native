using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using Server;
using nsUHF;
using System.Collections.Generic;


namespace ModuleService
{

    public class UHF : WebSocketService
    {
        UDPServer updServer;
        TDJ_RFIDHelper rfid_helper;
        public UHF()
        {
            services.register_service("uhf", this);

            rfid_helper = new TDJ_RFIDHelper();

            //打开UDP端口，等待数据传入
            this.updServer = new UDPServer();
            updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            updServer.startUDPListening(3001);
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
        protected override void OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("UHF OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                Send(cmd.print_string());
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
    }
}
