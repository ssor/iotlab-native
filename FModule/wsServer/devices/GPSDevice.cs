using ModuleCommand;
using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
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
    public class GPSDevice : IDevice
    {
        Action<ModuleCommand.command> myCallBack = null;
        command myCommand = null;

        UDPServer updServer;
        NmeaInterpreter GPS;
        NMEA2OSG OSGconv = new NMEA2OSG();
        StringBuilder sbuilder = new StringBuilder();

        public void setDevice(ModuleCommand.command cmd, Action<ModuleCommand.command> callback)
        {
            myCallBack = callback;
            myCommand = cmd;

            //打开UDP端口，等待数据传入
            this.updServer = UDPServer.getUDPServer(Program.GPS_UDP_Port);
            updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            updServer.startUDPListening();

            GPS = new NmeaInterpreter();
            GPS.PositionReceived += new NmeaInterpreter.PositionReceivedEventHandler(GPS_PositionReceived);

            Debug.WriteLine(string.Format("*****  GPSService onListening... "));

        }
        public string Name
        {
            get { return TargetDeiveName.GPS; }
        }
        void invokeCallback(command _cmd)
        {
            if (null != myCallBack)
            {
                myCallBack(_cmd);
            }
        }
        private void GPS_PositionReceived(string Lat, string Lon)
        {
            if (OSGconv.ParseNMEA(Lat, Lon, 0))
            {
                GpsPosition pos = new GpsPosition(Convert.ToString(OSGconv.deciLat), Convert.ToString(OSGconv.deciLon));
                string strToSend = JsonConvert.SerializeObject(pos);
                Debug.WriteLine("GPS_PositionReceived => " + strToSend);
                if (myCommand != null)
                {
                    myCommand.Para = strToSend;
                    myCommand.IfBroadcast = "true";
                }
                this.invokeCallback(myCommand);
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

    }
}
