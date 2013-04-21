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

    public class LightService : WebSocketService
    {
        public static string recently_broadcast = string.Empty;
        public LightService()
        {
            services.register_service("light", this);
        }
        protected override void OnOpen()
        {
            if (LightService.recently_broadcast != string.Empty)
            {
                //this.Broadcast(recently_broadcast);
                this.Send(recently_broadcast);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("Light OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                //Send(cmd.print_string());
                switch (cmd.Name)
                {
                    case "open":
                        Debug.WriteLine(string.Format("{0} 打开智能灯", cmd.Commander));
                        command ncOpen = new command("open", string.Format("{0}打开了灯", cmd.Commander));
                        LightService.recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                        this.Broadcast(LightService.recently_broadcast);
                        break;
                    case "close":
                        Debug.WriteLine(string.Format("{0} 关闭智能灯", cmd.Commander));
                        command ncClose = new command("close", string.Format("{0}关闭了灯", cmd.Commander));
                        LightService.recently_broadcast = JsonConvert.SerializeObject(ncClose);
                        this.Broadcast(LightService.recently_broadcast);
                        //this.Broadcast(JsonConvert.SerializeObject(ncClose));
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }
    }
}
