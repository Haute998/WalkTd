using System;
using System.Text;

namespace WeModels
{

    public class CommonFunc
    {
        private static char[] NUM =   
        {   
            '0','1','2','3','4','5','6','7','8','9'
        };

        private static char[] ALPHANUM =   
        {   
            '0','1','2','3','4','5','6','7','8','9',  
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
        };

        /// <summary>
        ///  string sRandom = CommonFunc.GenRandonString(8);
        /// </summary>
        /// <param name="_iLength"></param>
        /// <returns></returns>
        public static string GenRandonAlphaNumString(int _iLength)
        {
            StringBuilder sbRandom = new StringBuilder();

            long tick = DateTime.Now.ToUniversalTime().Ticks;
            Random rd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            for (int i = 0; i < _iLength; i++)
            {
                sbRandom.Append(ALPHANUM[rd.Next(0, 36)]);
            }

            return sbRandom.ToString();
        }

        public static string GetRandomNumString(int _iLength)
        {
            StringBuilder sbRandom = new StringBuilder();

            long tick = DateTime.Now.Ticks;
            Random rd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            for (int i = 0; i < _iLength; i++)
            {
                sbRandom.Append(NUM[rd.Next(0, 10)]);
            }

            return sbRandom.ToString();
        }

        /// <summary>
        /// 返回现在自1970年的时间戳，秒
        /// </summary>
        /// <returns></returns>
        public static int GetNowTimestamp()
        {
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtStart = new DateTime(1970, 1, 1);
            return (int)(DateTime.Now.ToUniversalTime() - dtStart).TotalSeconds;
        }

        /// <summary>
        /// 返回现在自1970年的时间戳，毫秒
        /// </summary>
        /// <returns></returns>
        public static double GetNowMTimestamp()
        {
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtStart = new DateTime(1970, 1, 1);
            return (DateTime.Now.ToUniversalTime() - dtStart).TotalMilliseconds;
        }

        /// <summary>
        /// 返回指定时间自1970年的时间戳，秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTimestamp(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //DateTime dtStart = new DateTime(1970, 1, 1);
            return (int)(dt - dtStart).TotalSeconds;
        }

        /// <summary>
        /// 返回指定时间自1970年的时间戳，毫秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetMTimestamp(DateTime dt)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            return (dt - dtStart).TotalMilliseconds;
        }

        /// <summary>
        /// 将时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="iTimestamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromTimestamp(int iTimestamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime TranslateDate = startTime.AddSeconds(iTimestamp);

            return TranslateDate;
        }

        /// <summary>
        /// 将时间戳转换成时间字符串描述
        /// </summary>
        /// <param name="_iTimestamp"></param>
        /// <returns></returns>
        public static string GetStringFromTimestamp(int _iTimestamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime TranslateDate = startTime.AddSeconds(_iTimestamp);

            return TranslateDate.ToString("yyyy-MM-dd hh:mm:ss");
        }

    }
}
