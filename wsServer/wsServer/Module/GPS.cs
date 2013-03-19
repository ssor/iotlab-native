using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using System.Threading;


namespace ModuleService
{

    public class GPS : WebSocketService
    {
        public GPS()
        {
            services.register_service("gps", this);
        }
        protected override void OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("GPS OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                Send(cmd.print_string());
                switch (cmd.Name)
                {
                    case "open":
                        Debug.WriteLine("��GPS");
                        //services.add_log("��GPS");
                        moduleMannager.reset_module_state("gps", "open");
                        break;
                    case "close":
                        Debug.WriteLine("�ر�GPS");
                        //services.add_log("�ر�GPS");
                        moduleMannager.reset_module_state("gps", "close");
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
