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
    public class GpsPosition
    {
        public string Lat;
        public string Lng;
        public GpsPosition() { }
        public GpsPosition(string lat, string lng)
        {
            this.Lat = lat;
            this.Lng = lng;
        }
    }
    public class GPSService : WebSocketService, IServicePlus
    {
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
            //this.updServer = UDPServer.getUDPServer(Program.GPS_UDP_Port);
            //updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            //updServer.startUDPListening();

            //GPS = new NmeaInterpreter();
            //GPS.PositionReceived += new NmeaInterpreter.PositionReceivedEventHandler(GPS_PositionReceived);

        }
        public void MCOpen()
        {
            command _cmd = new command(stateName.打开, "");
            _cmd.TargetDevice = TargetDeiveName.GPS;
            _cmd.id = this._context.Id.ToString();
            string msg = JsonConvert.SerializeObject(_cmd);
            Debug.WriteLine(string.Format("GPSService OnMessage => {0}", msg));
            FuncModuleManager.OnMessage(msg);
        }
        public void FMSend(command _cmd)
        {
            this.Send(JsonConvert.SerializeObject(_cmd));
        }
        public void MCOnMessage(command _cmd)
        {
            string msg = JsonConvert.SerializeObject(_cmd);
            Debug.WriteLine(string.Format("GPSService OnMessage => {0}", msg));
            FuncModuleManager.OnMessage(msg);
        }
        private void GPS_PositionReceived(string Lat, string Lon)
        {
            if (OSGconv.ParseNMEA(Lat, Lon, 0))
            {
                GpsPosition pos = new GpsPosition(Convert.ToString(OSGconv.deciLat), Convert.ToString(OSGconv.deciLon));
                string strToSend = JsonConvert.SerializeObject(pos);
                Debug.WriteLine("GPSService => " + strToSend);
                this.Send(strToSend);
            }
        }
        //void updServer_evtReceived(string inbuff)
        //{
        //    this.sbuilder.Append(inbuff);
        //    if (sbuilder.Length > 0)
        //    {
        //        string temp = sbuilder.ToString();
        //        int lastIndex = temp.LastIndexOf("$");
        //        if (lastIndex >= 0 && lastIndex != temp.IndexOf("$"))//至少有两个符号
        //        {

        //            string[] gpsString = temp.Substring(0, lastIndex).Split('$');
        //            foreach (string item in gpsString)
        //            {
        //                Debug.WriteLine("updServer_evtReceived => " + item);
        //                GPS.Parse("$" + item);
        //            }
        //        }
        //    }
        //}
        public override void OnMessage(string msg)
        {
            //Debug.WriteLine(string.Format("GPS OnMessage => {0}", msg));
            //try
            //{
            //    command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
            //    Debug.WriteLine(cmd.print_string());
            //    switch (cmd.Name)
            //    {
            //        case "open":
            //            Debug.WriteLine("打开GPS");
            //            break;
            //        case "close":
            //            Debug.WriteLine("关闭GPS");
            //            break;
            //    }
            //}
            //catch
            //{
            //    Debug.WriteLine("parse error!");
            //}
        }
        public override void OnClose()
        {
            //base.OnClose(e);
        }
        public void OnCloseSocket()
        {

        }
    }
}
