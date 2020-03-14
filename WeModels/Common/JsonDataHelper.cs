using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.ModelService.Common
{
    public class JsonDataHelper
    {
        /// <summary>
        /// JsonData是否包含某字段
        /// </summary>
        /// <param name="jd">JsonData</param>
        /// <param name="name">字段</param>
        /// <returns></returns>
        public static bool IsContains(JsonData jd,string name) 
        {
            return ((IDictionary)jd).Contains(name);
        }
        /// <summary>
        /// 获取JsonData值，没有则返回""
        /// </summary>
        /// <param name="jd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetValue(JsonData jd, string name)
        {
            try 
            {
            if (((IDictionary)jd).Contains(name) == true) 
            {
                return jd[name].ToString();
            }
            return string.Empty;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
