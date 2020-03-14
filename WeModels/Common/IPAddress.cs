using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class IPMsg
    {
        public static int IntervalSecond = 500;
        public static List<IPAddress> IPList = new List<IPAddress>();

        /// <summary>
        /// 添加时间
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool AddIPAddress(string IP)
        {
            IPAddress addIp=new IPAddress();
            addIp.IP = IP;
            addIp.Timestamp = CommonFunc.GetNowMTimestamp();

            IPList.Add(addIp);

            return true;
        }

        /// <summary>
        /// 移除超时IP
        /// </summary>
        public static void RemoveIPAddress()
        {
            for (int i = 0; i < IPList.Count; i++)
            {
                double millisend = CommonFunc.GetMTimestamp(DateTime.Now) - IPList[i].Timestamp;
                if (millisend > IntervalSecond)
                {
                    IPList.Remove(IPList[i]);
                }
            }
        }

        /// <summary>
        /// 判断IP是否存在间隔之中
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IsExist(string IP)
        {
            bool IsOK = false;
            for (int i = 0; i < IPList.Count; i++)
            {
                if (IPList[i].IP == IP)
                {
                    IsOK = true;
                    break;
                }
            }

            return IsOK;
        }
    }
    public class IPAddress
    {
        public string IP { get; set; }
        public double Timestamp { get; set; }

    }
}
