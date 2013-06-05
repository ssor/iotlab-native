using ModuleCommand;
using ModuleService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck
{
    public class ServiceHost
    {
        public List<WebSocketServiceManager> service_list = new List<WebSocketServiceManager>();

        public WebSocketService NewServiceMap(WebSocketServiceManager _manager, IWebSocketConnection socket)
        {
            WebSocketService service = null;
            switch (_manager.managerName)
            {
                case "/" + TargetDeiveName.GPS:
                    service = new GPSService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.UHF:
                    service = new UHFService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.绿灯:
                    service = new GreenLightService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.红灯:
                    service = new RedLightService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.黄灯:
                    service = new YellowLightService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.电风扇:
                    service = new FanService(_manager, socket);
                    break;
                case "/" + TargetDeiveName.电机:
                    service = new EngineService(_manager, socket);
                    break;
            }
            return service;
        }
        public void OnOpenMCWebSocket(IWebSocketConnection _socket)
        {
            string path = _socket.ConnectionInfo.Path;
            var manager = addConnectionManager(path, service_list);
            if (null != manager)
            {
                WebSocketService service = NewServiceMap(manager, _socket);
                manager.addMember(service);
                manager.Open(_socket);
            }
        }
        public void OnCloseMCWebSocket(IWebSocketConnection _socket)
        {
            var manager = GetWebSocketServiceManager(_socket.ConnectionInfo.Path, service_list);
            if (null != manager)
            {
                manager.removeMember(_socket.ConnectionInfo.Id.ToString());
            }
        }
        public void OnMCMessage(string _message, IWebSocketConnection _socket)
        {
            string path = _socket.ConnectionInfo.Path;

            WebSocketServiceManager manager = GetWebSocketServiceManager(path, service_list);
            if (manager != null)
            {
                manager.MCOnMessage(_message, _socket);
            }
        }
        public void FMSend(string _message)
        {
            command cmd_temp = (command)JsonConvert.DeserializeObject(_message, typeof(command));
            if (cmd_temp != null && cmd_temp.Initializing == "true")
            {
                WebSocketServiceManager.Broadcast2LocalService(cmd_temp);
                return;
            }

            var manager = GetWebSocketServiceManager("/" + cmd_temp.TargetDevice, service_list);
            if (manager != null)
            {


                if (cmd_temp.IfBroadcast == "true")
                {
                    manager.Broadcast(_message);
                }
                else
                {
                    manager.FMSend(_message, cmd_temp.id);
                }
            }
        }
        public WebSocketServiceManager GetWebSocketServiceManager(string name, List<WebSocketServiceManager> groupList)
        {
            WebSocketServiceManager manager = groupList.Find((_group) =>
            {
                return name == _group.managerName;
            });
            return manager;
        }
        public WebSocketServiceManager addConnectionManager(string name, List<WebSocketServiceManager> groupList)
        {
            WebSocketServiceManager group = groupList.Find((_group) =>
            {
                return name == _group.managerName;
            });
            if (group == null)
            {
                WebSocketServiceManager newGroup = new WebSocketServiceManager(name);
                groupList.Add(newGroup);
                return newGroup;
            }
            return group;
        }
    }
}
