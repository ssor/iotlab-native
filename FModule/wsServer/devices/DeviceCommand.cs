using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wsServer
{
    public enum enumDeviceCommand
    {
        查询红灯状态, 查询绿灯状态, 查询黄灯状态, 打开红灯, 关闭红灯, 打开绿灯, 关闭绿灯, 打开黄灯, 关闭黄灯, 打开电机, 关闭电机, 打开风扇, 关闭风扇,//。。。
        查询风扇状态,
        查询电机状态, 组网
    }
    public class DeviceCommandManager
    {
        static string 打开红灯命令 = "A5030000000000";
        static string 关闭红灯命令 = "A5020000000000";
        static string 打开绿灯命令 = "A5050000000000";
        static string 关闭绿灯命令 = "A5040000000000";
        static string 打开黄灯命令 = "A5070000000000";
        static string 关闭黄灯命令 = "A5060000000000";
        static string 打开电机命令 = "A5090000000000";
        static string 关闭电机命令 = "A5080000000000";
        static string 打开风扇命令 = "A50B0000000000";
        static string 关闭风扇命令 = "A50A0000000000";
        static string 组网 = "4D41544348";

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
        public static void executeCommand(enumDeviceCommand name, IPEndPoint ipEndPoint, int timeLater)
        {
            if (timeLater > 0)
            {
                Thread.Sleep(timeLater);
            }
            executeCommand(name, ipEndPoint);
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
        public static IDeviceCommand getDeivceCommandWithResponseOf(string res)
        {
            IDeviceCommand idc = DeviceCommandList.Find((cmd) =>
            {
                return cmd.itsMyResponsibility(res);
            });
            return idc;
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
            DeviceCommandList.Add(new cmdCheckEngine());
            DeviceCommandList.Add(new cmdCheckFan());
            DeviceCommandList.Add(new cmdCloseOrOpen(打开红灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(关闭红灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(打开绿灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(关闭绿灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(打开黄灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(关闭黄灯命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(打开电机命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(关闭电机命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(打开风扇命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(关闭风扇命令));
            DeviceCommandList.Add(new cmdCloseOrOpen(组网));

            //读取数据库，匹配命令和名称
        }
        #region helper func
        public static void getResponseData(IAsyncResult ar)
        {
            try
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
            catch (Exception e)
            {
                Debug.WriteLine("getResponseData => " + e.Message);
            }
        }
        public static void sendCommand(IDeviceCommand cmd, IPEndPoint ipEndPoint)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            string strcmd = cmd.getCmd();
            Debug.WriteLine("=> " + strcmd);
            byte[] _byteData = Encoding.UTF8.GetBytes(strcmd);

            clientSocket.BeginSendTo(_byteData, 0, _byteData.Length, SocketFlags.None,
                            ipEndPoint, null, null);
            //clientSocket.BeginSendTo(_byteData, 0, _byteData.Length, SocketFlags.None,
            //    ipEndPoint, getResponseData, new object[] { clientSocket, cmd });
        }
        #endregion
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
        public bool itsMyResponsibility(string res)
        {
            if (res == "A002" || res == "A003")
            {
                return true;
            }
            return false;
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
        public bool itsMyResponsibility(string res)
        {
            if (res == "A004" || res == "A005")
            {
                return true;
            }
            return false;
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
        public bool itsMyResponsibility(string res)
        {
            if (res == "A006" || res == "A007")
            {
                return true;
            }
            return false;
        }
    }
    public class cmdCheckEngine : IDeviceCommand
    {
        string cmd = "5A0480";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }
        public cmdCheckEngine()
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
                case "A008":
                    r = new LightState(name, false);
                    break;
                case "A009":
                    r = new LightState(name, true);
                    break;
            }
            return r;
        }



        public bool itsMyResponsibility(string res)
        {
            if (res == "A008" || res == "A009")
            {
                return true;
            }
            return false;
        }

    }
    public class cmdCheckFan : IDeviceCommand
    {
        string cmd = "5A0580";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }
        public cmdCheckFan()
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
                case "A00A":
                    r = new LightState(name, false);
                    break;
                case "A00B":
                    r = new LightState(name, true);
                    break;
            }
            return r;
        }
        public bool itsMyResponsibility(string res)
        {
            if (res == "A00A" || res == "A00B")
            {
                return true;
            }
            return false;
        }
    }
    public class cmdCloseOrOpen : IDeviceCommand
    {
        string cmd = "5A0580";
        Action<string> callback = null;
        public Action<string> Callback
        {
            set { callback = value; }
            get { return callback; }
        }

        public static cmdCloseOrOpen getCmd(string _cmd)
        {
            return new cmdCloseOrOpen(_cmd);
        }
        public cmdCloseOrOpen(string cmd)
        {
            this.cmd = cmd;
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
            return null;
        }
        public bool itsMyResponsibility(string res)
        {
            return false;
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
