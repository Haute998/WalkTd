using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;
using WeModels;
using System.Threading;

namespace WeBusiness
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimingEvent
    {
        static System.Threading.Timer timer = null;
        static System.Threading.Timer timer2 = null;
        public TimingEvent()
        {
            // 定时处理IP地址
            timer = new System.Threading.Timer(IPClearEvent, null, 0, 400);

            // 定时处理用户登录状态
            // timer2 = new System.Threading.Timer(UserClearEvent, null, 0, 10000);

            PDAUserMsg.InitPDASystemParam();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IPClearEvent(object obj)
        {
            try
            {
                IPMsg.RemoveIPAddress();
            }
            catch
            { 
                
            }
        }

        public static void UserClearEvent(object obj)
        {
            try
            {
                PDAUserMsg.ClearTimeOutUser();
            }
            catch
            { 
                
            }
        }
    }
}