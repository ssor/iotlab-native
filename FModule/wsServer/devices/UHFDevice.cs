using ModuleCommand;
using Newtonsoft.Json;
using nsUHF;
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
    public class UHFDevice : IDevice
    {
        Action<ModuleCommand.command> myCallBack = null;
        command myCommand = null;

        UDPServer updServer;
        TDJ_RFIDHelper rfid_helper;

        public void setDevice(ModuleCommand.command cmd, Action<ModuleCommand.command> callback)
        {
            myCallBack = callback;
            myCommand = cmd;

            rfid_helper = new TDJ_RFIDHelper();

            //打开UDP端口，等待数据传入
            this.updServer = UDPServer.getUDPServer(Program.UHF_UDP_Port);
            updServer.evtReceived += new OnReceiveString(updServer_evtReceived);
            updServer.startUDPListening();

            Debug.WriteLine(string.Format("*****  UHFDevice onListening... "));

        }
        public string Name
        {
            get { return TargetDeiveName.UHF; }
        }
        void invokeCallback(command _cmd)
        {
            if (null != myCallBack)
            {
                myCallBack(_cmd);
            }
        }
        void updServer_evtReceived(string str)
        {
            //Debug.WriteLine("UHF => " + str);
            List<TagInfo> list = rfid_helper.ParseDataToTag(str);
            if (list.Count > 0)
            {
                if (myCommand != null)
                {
                    myCommand.Para = list[0].epc;
                    myCommand.IfBroadcast = "true";
                }
                this.invokeCallback(myCommand);
            }
        }

    }
}
