using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    /// <summary>
    /// 搜索条件
    /// </summary>
    public class SYSLogSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        public string Type { get; set; }
        public string FromAPP { get; set; }

    }
    public partial class SYSLog
    {
        public static void add(string logs,string oper,string url,string type,string fromapp) 
        {
            SYSLog log = new SYSLog();
            log.Dat = DateTime.Now;
            log.Logs = logs;
            log.Oper = oper;
            log.Url = url;
            log.Type = type;
            log.FromAPP = fromapp;
            log.InsertAndReturnIdentity();
        }
    }
}
