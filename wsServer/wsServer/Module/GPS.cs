using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using System.Threading;
using Server;
using System.Text;


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
    public class GPSService : WebSocketService
    {
        UDPServer updServer;
        NmeaInterpreter GPS;
        NMEA2OSG OSGconv = new NMEA2OSG();
        StringBuilder sbuilder = new StringBuilder();

        public GPSService()
        {
            //services.register_service("gps", this);
        }
        protected override void OnOpen()
        {
            //��UDP�˿ڣ��ȴ����ݴ���
            this.updServer = UDPServer.getUDPServer(3002);
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
                if (lastIndex >= 0 && lastIndex != temp.IndexOf("$"))//��������������
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
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("GPS OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                //Send(cmd.print_string());
                switch (cmd.Name)
                {
                    case "open":
                        Debug.WriteLine("��GPS");
                        //services.add_log("��GPS");
                        //moduleMannager.reset_module_state("gps", "open");
                        break;
                    case "close":
                        Debug.WriteLine("�ر�GPS");
                        //services.add_log("�ر�GPS");
                        //moduleMannager.reset_module_state("gps", "close");
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            this.updServer.evtReceived -= updServer_evtReceived;
            base.OnClose(e);
        }
    }
}
