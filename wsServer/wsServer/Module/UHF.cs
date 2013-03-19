using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;


namespace ModuleService
{

    public class UHF : WebSocketService
    {
        public UHF()
        {
            services.register_service("uhf", this);
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
                        Debug.WriteLine("´ò¿ªUHF");
                        break;
                    case "close":
                        Debug.WriteLine("¹Ø±ÕUHF");
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
