using System;
using System.Diagnostics;

namespace ModuleCommand
{
    public class TargetDeiveName
    {
        public const string ����� = "fan";
        public const string ��� = "engine";
        public const string �̵� = "green_light";
        public const string ��� = "red_light";
        public const string �Ƶ� = "yellow_light";
        public const string GPS = "gps";
        public const string UHF = "uhf";
    }
    public class stateName
    {
        public const string �� = "open";
        public const string �ر� = "close";
    }
    public class command
    {
        public string Name;
        public string Para;
        public string Commander;//����������
        public string TargetDevice;//Ŀ��Ӳ��ģ��
        public string id;//socket���ӵı�ʶ
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
