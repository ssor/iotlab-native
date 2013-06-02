using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public interface IDeviceCommand
    {
        //string getName();
        string Name { get; set; }
        Action<string> Callback { get; set; }
        string getCmd();
        void sendCommand(IPEndPoint ipEndPoint);
        void callBack(string data);
        LightState parseResponse(string res);
        bool itsMyResponsibility(string res);
    }
}
