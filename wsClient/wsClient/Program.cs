using System;
using System.Windows.Forms;
using ModuleService;

namespace wsClient
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm form = new mainForm();
            services.logForm = form;
            Application.Run(new mainForm());
        }

    }
}
