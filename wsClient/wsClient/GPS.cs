using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;


namespace ModuleService
{

    public class GPS : WebSocketService
    {
        static string registe_name = "gps";
        public GPS()
        {
            services.register_service(registe_name, this);
        }
        protected override void OnOpen(object sender, EventArgs e)
        {
        }

        protected override void OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("GPS OnMessage => {0}", msg));
            //Send("echo -> " + msg);
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
            services.add_log(GPS.raw_data_parser(msg));
        }
        public static void client_OnMessage(object sender, MessageEventArgs e)
        {
            string msg = e.Data;
            Debug.WriteLine(string.Format("wsGPS OnMessage => {0}", msg));
            //这里如果有需要，原始数据应该被解析，之后将有效结果发给客户端
            services.get_service(registe_name).Send(msg);
        }
        static string raw_data_parser(string raw_data)
        {
            return "gps parserd => " + raw_data;
        }
    }
}
