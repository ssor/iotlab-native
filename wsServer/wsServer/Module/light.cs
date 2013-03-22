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

    public class Light : WebSocketService
    {
        public static string recently_broadcast = string.Empty;
        public Light()
        {
            services.register_service("light", this);
        }
        protected override void OnOpen()
        {
            if (Light.recently_broadcast != string.Empty)
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
                        Debug.WriteLine(string.Format("{0} �����ܵ�", cmd.Commander));
                        command ncOpen = new command("open", string.Format("{0}���˵�", cmd.Commander));
                        Light.recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                        this.Broadcast(Light.recently_broadcast);
                        break;
                    case "close":
                        Debug.WriteLine(string.Format("{0} �ر����ܵ�", cmd.Commander));
                        command ncClose = new command("close", string.Format("{0}�ر��˵�", cmd.Commander));
                        Light.recently_broadcast = JsonConvert.SerializeObject(ncClose);
                        this.Broadcast(Light.recently_broadcast);
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
