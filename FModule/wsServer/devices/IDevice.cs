using ModuleCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public interface IDevice
    {
        //void openDevice(command cmd, Action<command> callback);
        //void closeDevice(command cmd, Action<command> callback);
        void setDevice(ModuleCommand.command cmd, Action<ModuleCommand.command> callback);
        string Name { get; }
    }
}
