using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using System.Text.RegularExpressions;


namespace nsUHF
{
    public enum emuTagInfoFormat
    {
        标准, 简化, 无
    }
    /// <summary>
    /// 将标签信息解析后通过此类中转处理
    /// </summary>
    public class TagInfo
    {
        public int readCount = 0;
        public string antennaID = string.Empty;
        public string tagType = string.Empty;
        public string epc = string.Empty;
        public string getTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //public int milliSecond = DateTime.Now.Millisecond;

        public long milliSecond = 0;
        public TagInfo() { }
        public TagInfo(string _epc, string _ant, string _count)
        {
            this.epc = _epc;
            this.antennaID = _ant;
            try
            {
                this.readCount = int.Parse(_count);
            }
            catch
            {

            }
        }
        public string toString()
        {
            string str = string.Empty;
            str = string.Format("ant -> {0} | count -> {1} | epc -> {2}",
                                this.antennaID, this.readCount, this.epc);

            return str;
        }


    }
    //板状读写器操作类
    //实时获取标签信息
    public class TDJ_RFIDHelper
    {
        public bool bBusy = false;

        StringBuilder stringBuilder = new StringBuilder();
        ManualResetEvent resetState = new ManualResetEvent(true);

        public TDJ_RFIDHelper()
        {
        }

        //接收串口或者其它方式接收到的标签信息，
        public List<TagInfo> ParseDataToTag(string data)
        {
            List<TagInfo> listR = new List<TagInfo>();
            if (data == null || data.Length <= 0)
            {
                return listR;
            }
            this.stringBuilder.Append(data);
            string temp1 = this.stringBuilder.ToString();
            string match_string = @"Disc:\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}, Last:\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}, Count:(?<count>\d{5}), Ant:(?<ant>\d{2}), Type:\d{2}, Tag:(?<epc>[0-9A-F]{24})";
            MatchCollection mc = Regex.Matches(temp1, match_string);
            foreach (Match m in mc)
            {
                string strCmd = m.ToString();
                string epc = m.Groups["epc"].Value;
                string ant = m.Groups["ant"].Value;
                string count = m.Groups["count"].Value;
                TagInfo ti = new TagInfo(epc, ant, count);
                listR.Add(ti);
                this.stringBuilder.Replace(strCmd, "");
            }

            this.stringBuilder.Replace("\r\n", "");
            this.stringBuilder.Replace("    ", "");
            return listR;
        }


    }
}
