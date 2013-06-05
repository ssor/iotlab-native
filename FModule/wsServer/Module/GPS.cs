using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using System.Threading;
using Server;
using System.Text;
using Fleck;
using wsServer;


namespace ModuleService
{

    public class GPSService : WebSocketService
    {
        UDPServer updServer;
        NmeaInterpreter GPS;
        NMEA2OSG OSGconv = new NMEA2OSG();
        StringBuilder sbuilder = new StringBuilder();

        public GPSService(WebSocketServiceManager _manager, IWebSocketConnection socket)
        {
            //services.register_service("gps", this);
            this.ID = socket.ConnectionInfo.Id.ToString();
            this._manager = _manager;
            this._websocket = socket;
            this._context = socket.ConnectionInfo;
        }
        public override void OnOpen()
        {
            //打开UDP端口，等待数据传入
            this.updServer = UDPServer.getUDPServer(Program.GPS_UDP_Port);
            updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            updServer.startUDPListening();

            GPS = new NmeaInterpreter();
            GPS.PositionReceived += new NmeaInterpreter.PositionReceivedEventHandler(GPS_PositionReceived);

        }
        private void GPS_PositionReceived(string Lat, string Lon)
        {
            if (OSGconv.ParseNMEA(Lat, Lon, 0))
            {
                GpsPosition pos = new GpsPosition(Convert.ToString(OSGconv.deciLat), Convert.ToString(OSGconv.deciLon));
                string strToSend = JsonConvert.SerializeObject(pos);
                Debug.WriteLine("GPS_PositionReceived => " + strToSend);
                this.Send(strToSend);
                //deleInvokeMapControlPos dele = delegate(string _lat, string _lon)
                //{
                //    this.txtLat.Text = _lat;
                //    this.txtLng.Text = _lon;
                //};
                //if (this.stop_receive == false)
                //{

                //    this.Invoke(dele, (Convert.ToString(OSGconv.deciLat)), (Convert.ToString(OSGconv.deciLon)));
                //}
            }
        }
        void updServer_evtReceived(string inbuff)
        {
            this.sbuilder.Append(inbuff);
            if (sbuilder.Length > 0)
            {
                string temp = sbuilder.ToString();
                int lastIndex = temp.LastIndexOf("$");
                if (lastIndex >= 0 && lastIndex != temp.IndexOf("$"))//至少有两个符号
                {

                    string[] gpsString = temp.Substring(0, lastIndex).Split('$');
                    foreach (string item in gpsString)
                    {
                        Debug.WriteLine("updServer_evtReceived => " + item);
                        GPS.Parse("$" + item);
                    }
                }
            }
        }
        public override void OnMessage(string msg)
        {
            Debug.WriteLine(string.Format("GPS OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                //Send(cmd.print_string());
                switch (cmd.Name)
                {
                    case "open":
                        Debug.WriteLine("打开GPS");
                        //services.add_log("打开GPS");
                        //moduleMannager.reset_module_state("gps", "open");
                        break;
                    case "close":
                        Debug.WriteLine("关闭GPS");
                        //services.add_log("关闭GPS");
                        //moduleMannager.reset_module_state("gps", "close");
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        public override void OnClose()
        {
            this.updServer.evtReceived -= updServer_evtReceived;
            //base.OnClose(e);
        }
    }
}
