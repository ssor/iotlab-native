using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace wsServer
{

    public class Echo : WebSocketService
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            Console.WriteLine(string.Format("OnMessage => {0}", msg));
            Send("echo -> " + msg);


            //var msg = QueryString.Exists("name")
            //        ? String.Format("'{0}' returns to {1}", e.Data, QueryString["name"])
            //        : e.Data;

        }
    }
}
