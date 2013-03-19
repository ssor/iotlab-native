using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleCommand;

namespace ModuleService
{
    public class moduleMannager
    {
        public static Dictionary<string, CheckBox> moduleList = new Dictionary<string, CheckBox>();

        public static void reset_module_state(string module, string state)
        {
            Action<string, string> funcInvoke = (data, _state) =>
            {
                foreach (CheckBox ckb in moduleMannager.moduleList.Values)
                {
                    ckb.Checked = false;
                }
                CheckBox ckb_taget = moduleMannager.moduleList[data];
                switch (_state)
                {
                    case "open":
                        ckb_taget.Checked = true;
                        //这里通过发送命令控制硬件模块打开
                        //之后可能会有数据自硬件处发送过来
                        open_hardware_interface("gps");
                        break;
                    case "close":
                        ckb_taget.Checked = false;
                        //这里控制硬件关闭模块端口
                        close_hardware_interface("gps");
                        break;
                }
                services.showStateForm.add_log(module + " " + state);
            };
            services.showStateForm.Invoke(funcInvoke, module, state);
        }
        public static void close_hardware_interface(string module)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            _timer = null;
        }

        static void gps_timer_Tick(object sender, EventArgs e)
        {
            string msg = "$GPGGA,092204.999,4250.5589,S,14718.5084,E,1,04,24.4,19.7,M,,,,0000*1F";
            command cmd = new command("data", msg);
            services.get_service("gps").Send(msg);
        }
        public static void open_hardware_interface(string module)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            _timer = new Timer();
            _timer.Interval = 2000;
            switch (module)
            {
                case "gps":
                    _timer.Tick += new EventHandler(gps_timer_Tick);
                    break;
            }
            _timer.Start();
        }

        #region 模拟硬件
        static Timer _timer;

        #endregion
    }
}
