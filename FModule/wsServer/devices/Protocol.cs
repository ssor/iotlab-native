using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public class CommandMatchHelper
    {

        public static bool updateCommand(List<CommandMatch> list, CommandMatch cmd)
        {
            //首先要保证协议内同一个命令不能两次出现
            bool b = list.Exists((_cmd) =>
                {
                    return cmd.cmd == _cmd.cmd;
                });
            if (!b)
            {
                CommandMatch temp = list.Find((_cmd) =>
                {
                    return cmd.name == _cmd.name;
                });
                if (temp != null)
                {
                    temp.cmd = cmd.cmd;
                    DeviceCommandManager.matchCommand(cmd);
                    return true;
                }
            }

            return false;
        }
        //往列表里加入一个命令，如果有名字相同的则不再添加
        public static void addCommand(List<CommandMatch> list, CommandMatch cmd)
        {
            bool b = list.Exists((_cmd) =>
            {
                return _cmd.name == cmd.name;
            });
            if (!b)
            {
                list.Add(cmd);
            }
        }
        public static List<CommandMatch> importCommand()
        {
            try
            {
                string strReadFilePath3 = @"./db.txt";
                StreamReader srReadFile3 = new StreamReader(strReadFilePath3);
                string Command = srReadFile3.ReadToEnd();
                srReadFile3.Close();
                List<CommandMatch> List = (List<CommandMatch>)JsonConvert.DeserializeObject<List<CommandMatch>>(Command);
                return List;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public static void exportCommand(List<CommandMatch> list)
        {
            try
            {
                string data = JsonConvert.SerializeObject(list);
                string strReadFilePath1 = @"./db.txt";
                StreamWriter srWriteFile1 = new StreamWriter(strReadFilePath1);
                srWriteFile1.Write(data);
                srWriteFile1.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
    public class CommandMatch
    {
        public string name;
        public string cmd;

        public CommandMatch(string _name, string _cmd)
        {
            this.name = _name;
            this.cmd = _cmd;
        }
    }
}
