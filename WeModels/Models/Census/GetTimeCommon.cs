using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    class GetTimeCommon
    {
        /// <summary>
        /// 获取今年Min时间
        /// </summary>
        /// <param name="Year">年</param>
        /// <returns></returns>
        public static DateTime GetTheYearBeginTime(int Year)
        {
            return DateTime.Parse(Year + "-01-01");
        }

        /// <summary>
        /// 获取今年Max时间
        /// </summary>
        /// <param name="Year">年</param>
        /// <returns></returns>
        public static DateTime GetTheYearEndTime(int Year)
        {
            return DateTime.Parse(Year+1 + "-01-01");
        }
    }
}
