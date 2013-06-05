using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace Server
{
    public delegate void OnReceiveString(string str);

    public class UDPServer
    {
        public event OnReceiveString evtReceived;
        public static Dictionary<int, UDPServer> UDPServerList = new Dictionary<int, UDPServer>();
        ManualResetEvent Manualstate = new ManualResetEvent(true);
        StringBuilder sbuilder = new StringBuilder();
        Socket serverSocket;
        byte[] byteData = new byte[1024];
        int port = 9001;
        bool RunningState = false;

        public static UDPServer getUDPServer(int port)
        {
            UDPServer server = null;
            if (!UDPServerList.ContainsKey(port))
            {
                server = new UDPServer(port);
                UDPServer.UDPServerList.Add(port, server);
            }
            else
            {
                server = UDPServer.UDPServerList[port];
            }
            return server;
        }

        public UDPServer(int port)
        {
            this.port = port;
        }
        public void stopUDPListening()
        {
            if (serverSocket != null)
            {
                try
                {
                    serverSocket.Close();
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(
                        string.Format("UDPServer.stopUDPListening  ->  = {0}"
                        , ex.Message));
                }
            }
        }
        public void startUDPListening()
        {
            if (this.RunningState)
            {
                return;
            }
            try
            {
                {
                    //We are using UDP sockets
                    serverSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);
                    IPAddress ip = IPAddress.Parse(this.GetLocalIP4());
                    IPEndPoint ipEndPoint = new IPEndPoint(ip, this.port);
                    //Bind this address to the server
                    serverSocket.Bind(ipEndPoint);
                    //防止客户端强行中断造成的异常
                    long IOC_IN = 0x80000000;
                    long IOC_VENDOR = 0x18000000;
                    long SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

                    byte[] optionInValue = { Convert.ToByte(false) };
                    byte[] optionOutValue = new byte[4];
                    serverSocket.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                }

                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                RunningState = true;

                //Start receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);


                //**********************************************************************

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.startUDPListening  -> error = {0}"
                    , ex.Message));
            }
        }
        public void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)ipeSender;

                serverSocket.EndReceiveFrom(ar, ref epSender);

                string strReceived = Encoding.UTF8.GetString(byteData);

                //Debug.WriteLine(
                //    string.Format("UDPServer.OnReceive  -> received = {0}"
                //    , strReceived));

                Array.Clear(byteData, 0, byteData.Length);
                int i = strReceived.IndexOf("\0");
                Manualstate.WaitOne();
                Manualstate.Reset();
                //todo here should deal with the received string
                sbuilder.Append(strReceived.Substring(0, i));
                string str = sbuilder.ToString();
                sbuilder.Remove(0, str.Length);
                HandleEventInNewThread(str);
                Manualstate.Set();

                //Start listening to the message send by the user
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
                    new AsyncCallback(OnReceive), epSender);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.OnReceive  -> error = {0}"
                    , ex.Message));
            }
        }
        //public string GetReceivedString()
        //{
        //    Manualstate.WaitOne();
        //    Manualstate.Reset();
        //    string str = sbuilder.ToString();
        //    sbuilder.Remove(0, str.Length);
        //    return str;
        //}
        void BackgroundThreadWork(object sender, DoWorkEventArgs e)
        {
            string str = (string)e.Argument;
            OnReceiveString evt = this.evtReceived;
            if (evt != null)
            {
                evt(str);
            }
        }
        void HandleEventInNewThread(string _str)
        {
            OnReceiveString evt = this.evtReceived;
            if (evt != null)
            {
                evt(_str);
            }
            //BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            //backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundThreadWork);
            //backgroundWorker1.RunWorkerAsync(_str);
        }
        string GetLocalIP4()
        {
            IPAddress ipAddress = null;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                ipAddress = ipHostInfo.AddressList[i];
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    break;
                }
                else
                {
                    ipAddress = null;
                }
            }
            if (null == ipAddress)
            {
                return null;
            }
            return ipAddress.ToString();
        }
    }
}
