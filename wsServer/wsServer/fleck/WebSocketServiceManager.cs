using ModuleCommand;
using ModuleService;
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

        /// <summary>
        /// 直接广播到监控端
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(string data)
        {
            memberList.ForEach(s => { s.Send(data); });
        }

        public static void Broadcast2LocalService(command _cmd)
        {
            Debug.WriteLine("*****  初始化状态...");
            //memberList.ForEach(s => { ((IServicePlus)s).FMSend(_cmd); });
            switch (_cmd.TargetDevice)
            {
                case TargetDeiveName.GPS:
                    break;
                case TargetDeiveName.UHF:
                    break;
                case TargetDeiveName.绿灯:
                    GreenLightService.last_effective_command = _cmd;
                    GreenLightService.recently_broadcast = "";
                    Debug.WriteLine(string.Format("*****  绿灯初始状态为 => {0}", _cmd.Name));
                    break;
                case TargetDeiveName.红灯:
                    RedLightService.last_effective_command = _cmd;
                    RedLightService.recently_broadcast = "";
                    Debug.WriteLine(string.Format("*****  红灯初始状态为 => {0}", _cmd.Name));
                    break;
                case TargetDeiveName.黄灯:
                    YellowLightService.last_effective_command = _cmd;
                    YellowLightService.recently_broadcast = "";
                    Debug.WriteLine(string.Format("*****  黄灯初始状态为 => {0}", _cmd.Name));
                    break;
                case TargetDeiveName.电风扇:
                    FanService.last_effective_command = _cmd;
                    FanService.recently_broadcast = "";
                    Debug.WriteLine(string.Format("*****  电风扇初始状态为 => {0}", _cmd.Name));
                    break;
                case TargetDeiveName.电机:
                    EngineService.last_effective_command = _cmd;
                    EngineService.recently_broadcast = "";
                    Debug.WriteLine(string.Format("*****  电机初始状态为 => {0}", _cmd.Name));
                    break;
            }


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
