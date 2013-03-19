using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using wsClient;

namespace ModuleService
{
    public class services
    {
        public static mainForm logForm = null;
        public static Dictionary<string, WebSocketService> service_dic = new Dictionary<string, WebSocketService>();
        public static void register_service(string name, WebSocketService wss)
        {
            service_dic[name] = wss;
        }
        public static WebSocketService get_service(string name)
        {
            return service_dic[name];
        }
        public static void add_log(string log)
        {
            if (logForm != null)
            {
                logForm.add_log(log);
            }
        }
    }
}
