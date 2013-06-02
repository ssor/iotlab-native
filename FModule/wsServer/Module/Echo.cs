using Fleck;
using System;

namespace wsServer
{

    public class Echo : WebSocketService
    {
        public override void OnMessage(string msg)
        {
            Console.WriteLine(string.Format("OnMessage => {0}", msg));
            Send("echo -> " + msg);


            //var msg = QueryString.Exists("name")
            //        ? String.Format("'{0}' returns to {1}", e.Data, QueryString["name"])
            //        : e.Data;

        }
    }
}
