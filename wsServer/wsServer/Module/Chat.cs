using Fleck;
using System;
using System.Threading;

namespace wsServer
{

    public class Chat : WebSocketService
    {
        private static int _num = 0;

        private string _name;

        private string getName()
        {
            //return QueryString.Exists("name")
            //       ? QueryString["name"]
            //       : "anon#" + getNum();
            return null;
        }

        private int getNum()
        {
            return Interlocked.Increment(ref _num);
        }

        public override void OnOpen()
        {
            _name = getName();
        }

        public override void OnMessage(string message)
        {

            var msg = String.Format("{0}: {1}", _name, message);
            Broadcast(msg);
        }

        public override void OnClose()
        {
            var msg = String.Format("{0} got logged off...", _name);
            Broadcast(msg);
        }
    }
}
