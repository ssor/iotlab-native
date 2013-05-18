using System;
using System.Diagnostics;

namespace ModuleCommand
{

    public class command
    {
        public string Name;
        public string Para;
        public string Commander;//·¢ÆðÃüÁîÕß

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
