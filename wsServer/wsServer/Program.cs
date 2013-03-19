using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using ModuleService;
using System.Windows.Forms;

namespace wsServer
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            serverForm form = new serverForm();
            services.showStateForm = form;
            Application.Run(form);
        }
    }
}
