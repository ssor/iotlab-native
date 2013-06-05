using System;
using System.Diagnostics;

namespace ModuleCommand
{
    public class TargetDeiveName
    {
        public const string 电风扇 = "fan";
        public const string 电机 = "engine";
        public const string 绿灯 = "green_light";
        public const string 红灯 = "red_light";
        public const string 黄灯 = "yellow_light";
        public const string GPS = "gps";
        public const string UHF = "uhf";
    }
    public class stateName
    {
        public const string 打开 = "open";
        public const string 关闭 = "close";
    }
    public class command
    {
        public string Name;
        public string Para;
        public string Commander;//发起命令者
        public string TargetDevice;//目标硬件模块
        public string id;//socket连接的标识
        public string IfBroadcast = "false";
        public string Initializing = "false";
        public command(string _name, string _para)
        {
            this.Name = _name;
            this.Para = _para;
        }
        public string print_string()
        {
            return string.Format("name => {0}   para => {1}", this.Name, this.Para);
        }
    }
}
