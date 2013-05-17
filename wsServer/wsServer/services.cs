using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wsServer;

namespace ModuleService
{
    public class services
    {
        public static serverForm showStateForm = null;
        public static Dictionary<string, WebSocketService> service_dic = new Dictionary<string, WebSocketService>();

        public static void register_service(string name, WebSocketService wss)
        {
            service_dic[name] = wss;
        }
        public static WebSocketService get_service(string name)
        {
            return service_dic[name];
        }

    }
}
