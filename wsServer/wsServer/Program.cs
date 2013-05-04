using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using ModuleService;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace wsServer
{
    class Program
    {
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
