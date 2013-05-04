using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public enum enumDeviceCommand
    {
        检查红灯状态, 检查绿灯状态, 检查黄灯状态, 打开红灯, 打开绿灯//。。。
    }
    public class DeviceCommandManager
    {
        static List<IDeviceCommand> DeviceCommandList = new List<IDeviceCommand>();

        public static void setCommandCallback(enumDeviceCommand name, Action<string> _callback)
        {
            IDeviceCommand cmd = null;
            cmd = getDeivceCommand(name);
            if (cmd != null)
            {
                cmd.Callback = _callback;
            }
        }
        public static void executeCommand(enumDeviceCommand name, IPEndPoint ipEndPoint)
        {
            IDeviceCommand cmd = null;
            cmd = getDeivceCommand(name);
            if (cmd != null)
            {
                cmd.sendCommand(ipEndPoint);
            }
        }
        public static IDeviceCommand getDeivceCommand(enumDeviceCommand name)
        {
            IDeviceCommand idc = DeviceCommandList.Find((cmd) =>
            {
                return cmd.Name == name.ToString();
            });
            return idc;
        }
        public static void matchCommand(CommandMatch _command)
        {
            if (null == _command) return;
            IDeviceCommand idc = DeviceCommandList.Find((_temp) =>
            {
                return _temp.getCmd() == _command.cmd;
            });
            if (null != idc)
            {
                idc.Name = _command.name;
            }
        }
        public static void matchCommand(List<CommandMatch> _commandList)
        {
            if (null == _commandList) return;
            //根据设备协议匹配名字
            foreach (CommandMatch cmd in _commandList)
            {
                IDeviceCommand idc = DeviceCommandList.Find((_temp) =>
                {
                    return _temp.getCmd() == cmd.cmd;
                });
                if (null != idc)
                {
                    idc.Name = cmd.name;
                }
            }
        }
        public static void initialCommandList()
        {
            DeviceCommandList.Add(new cmdCheckLight1());
            DeviceCommandList.Add(new cmdCheckLight2());
            DeviceCommandList.Add(new cmdCheckLight3());

            //读取数据库，匹配命令和名称
        }
        #region helper func
        public static void getResponseData(IAsyncResult ar)
        {
            //Socket clientSocket = (Socket)ar.AsyncState;
            object[] array = (object[])ar.AsyncState;
            Socket clientSocket = (Socket)array[0];
            clientSocket.EndSend(ar);
            byte[] byteData = new byte[1024];
            clientSocket.Receive(byteData);

            string strReceived = Encoding.UTF8.GetString(byteData);

            Array.Clear(byteData, 0, byteData.Length);
            int i = strReceived.IndexOf("\0");
            if (i > 0)
            {
                string data = strReceived.Substring(0, i);
                Debug.WriteLine(" Data => SP: " + data);
                //todo here should deal with the received string
                IDeviceCommand cmd = (IDeviceCommand)array[1];
                cmd.callBack(data);
            }
        }
        public static void sendCommand(IDeviceCommand cmd, IPEndPoint ipEndPoint)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] _byteData = Encoding.UTF8.GetBytes(cmd.getCmd());

            clientSocket.BeginSendTo(_byteData, 0, _byteData.Length, SocketFlags.None,
                            ipEndPoint, getResponseData, new object[] { clientSocket, cmd });
        }
        #endregion
    }
    public class LightState
    {
       public bool State;
       public string Name;
        public LightState(string _name, bool _state)
        {
            this.Name = _name;
            this.State = _state;
        }
    }
    public class cmdCheckLight1 : IDeviceCommand
    {
        string cmd = "5A0180";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }
        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public cmdCheckLight1()
        {
            //callback = (data) =>
            //{
            //    Debug.WriteLine(name + " => " + data);
            //};
        }
        public void callBack(string data)
        {
            if (this.callback != null)
            {
                this.callback(data);
            }
        }
        public void sendCommand(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.sendCommand(this, ipEndPoint);
        }

        public string getCmd()
        {
            return this.cmd;
        }
        public LightState parseResponse(string res)
        {
            LightState r = null;
            switch (res.ToUpper())
            {
                case "A002":
                    r = new LightState(name, false);
                    break;
                case "A003":
                    r = new LightState(name, true);
                    break;
            }
            return r;
        }
    }
    public class cmdCheckLight2 : IDeviceCommand
    {
        string cmd = "5A0280";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }
        public cmdCheckLight2()
        {
            //callback = (data) =>
            //{
            //    Debug.WriteLine(name + " => " + data);
            //};
        }
        public void callBack(string data)
        {
            if (this.callback != null)
            {
                this.callback(data);
            }
        }
        public void sendCommand(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.sendCommand(this, ipEndPoint);
        }

        public string getCmd()
        {
            return this.cmd;
        }
        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public LightState parseResponse(string res)
        {
            LightState r = null;
            switch (res.ToUpper())
            {
                case "A004":
                    r = new LightState(name, false);
                    break;
                case "A005":
                    r = new LightState(name, true);
                    break;
            }
            return r;
        }
    }
    public class cmdCheckLight3 : IDeviceCommand
    {
        string cmd = "5A0380";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }
        public cmdCheckLight3()
        {
            //callback = (data) =>
            //{
            //    Debug.WriteLine(name + " => " + data);
            //};
        }
        public void callBack(string data)
        {
            if (this.callback != null)
            {
                this.callback(data);
            }
        }
        public void sendCommand(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.sendCommand(this, ipEndPoint);
        }

        public string getCmd()
        {
            return this.cmd;
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public LightState parseResponse(string res)
        {
            LightState r = null;
            switch (res.ToUpper())
            {
                case "A006":
                    r = new LightState(name, false);
                    break;
                case "A007":
                    r = new LightState(name, true);
                    break;
            }
            return r;
        }
    }
    //public class DeviceCommand
    //{
    //    string name = string.Empty;
    //    string cmd = string.Empty;
    //    Action<string> callback = null;

    //    public DeviceCommand(string _cmd, Action<string> _callback)
    //    {
    //        this.cmd = _cmd;
    //        this.callback = _callback;
    //    }
    //    public string getCmd()
    //    {
    //        return this.cmd;
    //    }
    //    public void callBack(string data)
    //    {
    //        if (this.callback != null)
    //        {
    //            this.callback(data);
    //        }
    //    }
    //}
}
