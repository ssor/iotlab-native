using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModuleService;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace wsServer
{
    class Program
    {
        public static int inputPort = 19201;
        public static int outputPort = 19200;
        public static int UHF_UDP_Port = 3001;
        public static int GPS_UDP_Port = 3002;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            DeviceCommandManager.initialCommandList();
            List<CommandMatch> cmdList = CommandMatchHelper.importCommand();
            DeviceCommandManager.matchCommand(cmdList);

            serverForm form = new serverForm();
            services.showStateForm = form;
            //frmProtocolTest form = new frmProtocolTest();
            Application.Run(form);
        }
        public static IPEndPoint getRemoteIPEndPoint()
        {
            IPAddress ip = IPAddress.Parse(GetLocalIP4());
            IPEndPoint ipEndPoint = new IPEndPoint(ip, outputPort);
            return ipEndPoint;
        }
        public static string GetLocalIP4()
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
