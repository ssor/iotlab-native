using ModuleCommand;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public class FanDevice : IDevice
    {
        Action<ModuleCommand.command> myCallBack = null;
        command myCommand = null;
        public void setDevice(ModuleCommand.command cmd, Action<ModuleCommand.command> callback)
        {
            myCallBack = callback;
            myCommand = cmd;
            switch (cmd.Name)
            {
                case stateName.打开:
                    打开风扇(Program.getRemoteIPEndPoint());
                    break;
                case stateName.关闭:
                    关闭风扇(Program.getRemoteIPEndPoint());
                    break;
            }

        }
        public string Name
        {
            get { return TargetDeiveName.电风扇; }
        }
        void invokeCallback(command _cmd)
        {
            if (null != myCallBack)
            {
                myCallBack(_cmd);
            }
        }
        void 关闭风扇(IPEndPoint ipEndPoint)
        {
            //if (myCommand != null)
            //{
            //    myCommand.Name = stateName.关闭;
            //    //myCommand.Name = stateName.打开;
            //}
            //this.invokeCallback(myCommand);
            //return;

            DeviceCommandManager.executeCommand(enumDeviceCommand.关闭风扇, ipEndPoint);
            检查风扇状态(ipEndPoint);
        }
        void 打开风扇(IPEndPoint ipEndPoint)
        {
            //if (myCommand != null)
            //{
            //    myCommand.Name = stateName.打开;
            //    //myCommand.Name = stateName.关闭;
            //}
            //this.invokeCallback(myCommand);
            //return;


            DeviceCommandManager.executeCommand(enumDeviceCommand.打开风扇, ipEndPoint);
            检查风扇状态(ipEndPoint);
        }
        void 检查风扇状态(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.查询风扇状态,
               (data) =>
               {
                   Debug.WriteLine("风扇状态 => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.查询风扇状态);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls)
                       {
                           switch (ls.State)
                           {
                               case true:
                                   if (myCommand != null)
                                   {
                                       myCommand.Name = stateName.打开;
                                   }
                                   this.invokeCallback(myCommand);
                                   break;
                               case false:
                                   if (myCommand != null)
                                   {
                                       myCommand.Name = stateName.关闭;
                                   }
                                   this.invokeCallback(myCommand);
                                   break;
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.查询风扇状态, ipEndPoint, 1000);
        }
    }
}
