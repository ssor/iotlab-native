using ModuleCommand;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Fleck
{
    public interface IServicePlus
    {
        void MCOnMessage(command _cmd);
        void FMSend(command _cmd);
        void MCOpen();
        void OnCloseSocket();
    }
    //管理该路径下的所有链接
    public class WebSocketServiceManager
    {
        public string managerName;
        List<WebSocketService> memberList = new List<WebSocketService>();

        public WebSocketServiceManager(string _name)
        {
            this.managerName = _name;
        }
        public bool addMember(WebSocketService _service)
        {
            if (null == _service) return false;

            WebSocketService service = memberList.Find((_con) =>
            {
                return _service.ID == _con.ID;
            });
            if (null == service)
            {
                this.memberList.Add(_service);
                _service.OnOpen();
                Debug.WriteLine("***** MClient ++ count => " + memberList.Count.ToString());
                return true;
            }
            return false;
        }
        public void Broadcast(string data)
        {
            memberList.ForEach(s => { s.Send(data); });
        }
        public void FMSend(string data, string _id)
        {
            WebSocketService service = memberList.Find((_temp) =>
            {
                return _id == _temp.ID;
            });
            if (null != service)
            {
                //service.Send(data);
                ((IServicePlus)service).FMSend((command)JsonConvert.DeserializeObject(data, typeof(command)));
            }
        }
        public void Open(IWebSocketConnection _socket)
        {
            WebSocketService service = memberList.Find((_temp) =>
            {
                return _temp.ID == _socket.ConnectionInfo.Id.ToString();
            });
            if (service != null)
            {
                ((IServicePlus)service).MCOpen();
            }
        }
        public void removeMember(string _id)
        {
            WebSocketService service = memberList.Find((_temp) =>
            {
                return _id == _temp.ID;
            });
            if (null != service)
            {
                service.OnClose();
                this.memberList.Remove(service);
                Debug.WriteLine("***** MClient -- count => " + memberList.Count.ToString());
            }
        }
        public void MCOnMessage(string message, IWebSocketConnection _socket)
        {
            WebSocketService service = memberList.Find((_temp) =>
            {
                return _temp.ID == _socket.ConnectionInfo.Id.ToString();
            });
            if (service != null)
            {
                command cmd_temp = (command)JsonConvert.DeserializeObject(message, typeof(command));
                cmd_temp.id = _socket.ConnectionInfo.Id.ToString();
                //service.OnMessage(JsonConvert.SerializeObject(cmd_temp));
                ((IServicePlus)service).MCOnMessage(cmd_temp);
            }
            //_socket.Send("private =>" + message);
            //Broadcast("public => " + message);
        }
    }
}
