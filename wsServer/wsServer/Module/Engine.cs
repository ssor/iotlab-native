using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModuleCommand;
using Server;
using nsUHF;
using System.Collections.Generic;
using System.Net;
using wsServer;


namespace ModuleService
{
    public class EngineService : WebSocketService
    {
        public static string recently_broadcast = string.Empty;
        command last_command = null;
        public static command last_effective_command = null;
        public EngineService()
        {
            services.register_service("engine", this);
        }
        protected override void OnOpen()
        {
            if (last_effective_command != null)
            {
                recently_broadcast = JsonConvert.SerializeObject(last_effective_command);
                this.Send(recently_broadcast);
            }
            //if (recently_broadcast != string.Empty)
            //{
            //    //this.Broadcast(recently_broadcast); 

            //}
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            Debug.WriteLine(string.Format("EngineService OnMessage => {0}", msg));
            try
            {
                command cmd = (command)JsonConvert.DeserializeObject(msg, typeof(command));
                Debug.WriteLine(cmd.print_string());
                last_command = cmd;
                switch (cmd.Name)
                {
                    case "open":
                        �򿪵��(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} ��ͼ�򿪵��", cmd.Commander));
                        break;
                    case "close":
                        �رյ��(Program.getRemoteIPEndPoint());
                        Debug.WriteLine(string.Format("{0} ��ͼ�رյ��", cmd.Commander));
                        break;
                }
                �����״̬(Program.getRemoteIPEndPoint());
            }
            catch
            {
                Debug.WriteLine("parse error!");
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        void �رյ��(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.�رյ��, ipEndPoint);

        }
        void �򿪵��(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.executeCommand(enumDeviceCommand.�򿪵��, ipEndPoint);

        }
        void �����״̬(IPEndPoint ipEndPoint)
        {
            DeviceCommandManager.setCommandCallback(enumDeviceCommand.��ѯ���״̬,
               (data) =>
               {
                   Debug.WriteLine("�Ƶ�״̬ => " + data);
                   IDeviceCommand idc = DeviceCommandManager.getDeivceCommand(enumDeviceCommand.��ѯ���״̬);
                   if (null != idc)
                   {
                       LightState ls = idc.parseResponse(data);
                       if (null != ls && last_command != null)
                       {
                           bool temp = (last_command.Name == "open") ? true : false;
                           if (ls.State == temp)//�����ڵ�״̬�Ͳ���Ŀ��״̬��ͬ��ͬΪ�ػ��߿�
                           {
                               switch (last_command.Name)
                               {
                                   case "open":
                                       Debug.WriteLine(string.Format("{0} �򿪵��", last_command.Commander));
                                       command ncOpen = new command("open", string.Format("{0}���˵��", last_command.Commander));
                                       last_effective_command = ncOpen;
                                       recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                                       this.Broadcast(recently_broadcast);
                                       break;
                                   case "close":
                                       Debug.WriteLine(string.Format("{0} �رյ��", last_command.Commander));
                                       command ncClose = new command("close", string.Format("{0}�ر��˵��", last_command.Commander));
                                       last_effective_command = ncClose;
                                       recently_broadcast = JsonConvert.SerializeObject(ncClose);
                                       this.Broadcast(recently_broadcast);
                                       break;
                               }
                           }
                           else//˵���豸û����Ӧ��֪ͨ�ͻ���
                           {
                               switch (ls.State)
                               {
                                   case true:
                                       command ncOpen = new command("open", "����ʧ��");
                                       recently_broadcast = JsonConvert.SerializeObject(ncOpen);
                                       this.Send(recently_broadcast);//ֻ֪ͨ����
                                       Debug.WriteLine("�رյ������ʧ��");
                                       break;
                                   case false:
                                       command ncClose = new command("close", "����ʧ��");
                                       recently_broadcast = JsonConvert.SerializeObject(ncClose);
                                       this.Send(recently_broadcast);
                                       Debug.WriteLine("�򿪵������ʧ��");
                                       break;
                               }
                           }
                       }
                   }
               });
            DeviceCommandManager.executeCommand(enumDeviceCommand.��ѯ���״̬, ipEndPoint, 1000);
        }
    }
}