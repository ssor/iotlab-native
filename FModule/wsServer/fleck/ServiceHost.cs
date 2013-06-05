using ModuleService;
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
                case "/gps":
                    service = new GPSService(_manager, socket);
                    //service.
                    break;
                case "/uhf":
                    service = new UHFService(_manager, socket);
                    break;
                case "/green_light":
                    service = new GreenLightService(_manager, socket);
                    break;
                case "/red_light":
                    service = new RedLightService(_manager, socket);
                    break;
                case "/yellow_light":
                    service = new YellowLightService(_manager, socket);
                    break;
                case "/fan":
                    service = new FanService(_manager, socket);
                    break;
                case "/engine":
                    service = new EngineService(_manager, socket);
                    break;
            }
            return service;
        }
        public void OnOpenWebSocket(IWebSocketConnection _socket)
        {
            string path = _socket.ConnectionInfo.Path;
            var manager = addConnectionManager(path, service_list);
            WebSocketService service = NewServiceMap(manager, _socket);
            manager.addMember(service);
        }
        public void OnCloseWebSocket(IWebSocketConnection _socket)
        {
            var manager = GetWebSocketServiceManager(_socket.ConnectionInfo.Path, service_list);
            manager.removeMember(_socket.ConnectionInfo.Id.ToString());
        }
        public void OnMessage(string _message, IWebSocketConnection _socket)
        {
            string path = _socket.ConnectionInfo.Path;

            WebSocketServiceManager manager = GetWebSocketServiceManager(path, service_list);
            if (manager != null)
            {
                manager.OnMessage(_message, _socket);
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
