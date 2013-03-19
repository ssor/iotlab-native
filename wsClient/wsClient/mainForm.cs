using System;
using System.Windows.Forms;
using WebSocketSharp;
using System.Diagnostics;
using ModuleCommand;
using Newtonsoft.Json;
using ModuleService;
using WebSocketSharp.Server;

namespace wsClient
{
    public partial class mainForm : Form
    {
        WebSocketServer wssv = null;

        WebSocket wsGPS = null;
        WebSocket wsUHF = null;

        public mainForm()
        {
            InitializeComponent();

            this.initial_web_sockets();
            //this.initial_web_socket_server();
            //var v = services.service_dic;
        }
        public void add_log(string log)
        {
            Action<string> funcInvoke = data =>
            {
                this.txtLog.Text = data + "\r\n" + this.txtLog.Text;
            };

            if (this.txtLog.InvokeRequired)
            {
                this.txtLog.Invoke(funcInvoke, log);
            }
            else
                funcInvoke(log);
        }
        void initial_web_socket_server()
        {
            wssv = new WebSocketServer(4646);
            wssv.AddService<GPS>("/gps");
            //wssv.AddService<UHF>("/uhf");
            wssv.Start();
        }
        void initial_web_sockets()
        {
            //初始化各个模块的websocket连接
            if (wsGPS == null)
            {
                wsGPS = new WebSocket("ws://localhost:4649/gps");
                wsGPS.OnMessage += GPS.client_OnMessage;
                wsGPS.OnError += (sender, e) =>
                    {
                        try
                        {
                            WebSocket wsTemp = wsGPS;
                            wsGPS = null;
                            wsTemp.Close();
                            wsTemp.Dispose();
                        }
                        catch
                        {
                        }
                    };
                try
                {
                    wsGPS.Connect();
                }
                catch
                {
                }
            }
            if (wsUHF == null)
            {

                wsUHF = new WebSocket("ws://localhost:4649/uhf");
                wsUHF.OnMessage += (sender, e) =>
                {
                    Debug.WriteLine(string.Format("wsUHF OnMessage => {0}", e.Data));
                };
                wsUHF.OnError += (sender, e) =>
                {
                    try
                    {
                        WebSocket wsTemp = wsGPS;
                        wsUHF = null;
                        wsTemp.Close();
                        wsTemp.Dispose();
                    }
                    catch
                    {
                    }
                };
                try
                {
                    wsUHF.Connect();
                }
                catch
                {
                }
            }
        }


        private void button1_Click(object s, EventArgs oe)
        {
            if (wsGPS == null)
            {
                this.initial_web_sockets();
            }
            else
            {
                command c = new command("open", "gps");
                //c.Data = "data";
                wsGPS.Send(JsonConvert.SerializeObject(c));
            }
            //using (WebSocket ws = new WebSocket("ws://localhost:4649/GPS"))
            //{
            //    ws.OnOpen += (sender, e) =>
            //    {
            //        ws.Send("Hi, 张!");
            //    };

            //    ws.OnMessage += (sender, e) =>
            //    {
            //        Debug.WriteLine(string.Format("OnMessage => {0}", e.Data));
            //    };

            //    ws.OnError += (sender, e) =>
            //    {
            //        Debug.WriteLine(string.Format("OnError => {0}", e.Message));
            //    };

            //    ws.OnClose += (sender, e) =>
            //    {
            //        Debug.WriteLine(string.Format("OnClose => {0}", e.Data));
            //    };

            //    ws.Connect();

            //Thread.Sleep(500);
            //Debug.WriteLine("\nType \"exit\" to exit.\n");

            //string data;
            //while (true)
            //{
            //    Thread.Sleep(500);

            //    Debug.Write("> ");
            //    data = Console.ReadLine();
            //    if (data == "exit")
            //    //if (data == "exit" || !ws.IsAlive)
            //    {
            //        break;
            //    }

            //    ws.Send(data);
            //}
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (null == wsUHF)
            {
                this.initial_web_sockets();
            }
            else
            {
                command c = new command("open", "uhf");
                wsUHF.Send(JsonConvert.SerializeObject(c));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (wsGPS == null)
            {
                this.initial_web_sockets();
            }
            else
            {
                command c = new command("close", "gps");
                wsGPS.Send(JsonConvert.SerializeObject(c));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (null == wsUHF)
            {
                this.initial_web_sockets();
            }
            else
            {
                command c = new command("close", "uhf");
                wsUHF.Send(JsonConvert.SerializeObject(c));
            }
        }

        private void button5_Click(object _sender, EventArgs _e)
        {
            WebSocket wsEcho = new WebSocket("ws://localhost:4649/Echo");
            wsEcho.OnOpen += (sender, e) =>
            {
                wsEcho.Send("Hi, 张!");
            };
            wsEcho.OnMessage += (sender, e) =>
            {
                Debug.WriteLine(string.Format("wsGPS OnMessage => {0}", e.Data));
            };
            wsEcho.Connect();
        }
    }
}
