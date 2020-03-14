using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class BaseSearch
    {
        /// <summary>
        /// 页码  1开始
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 排序  a desc
        /// </summary>
        public string orderby { get; set; }

        /// <summary>
        /// 获取sql  自己领悟
        /// </summary>
        /// <param name="datname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getDatSqlStr(string datname,string type) 
        {
            string sql = "";
            switch (type)
            {
                case "本月":
                    sql = @" and DatCreat>=dateadd(day,1-day(getdate()),convert(varchar,getdate(),112))  
                             and DatCreat<dateadd(month,1,dateadd(day,1-day(getdate()),convert(varchar,getdate(),112))) ";
                    break;
                default: 
                    sql="";
                    break;
            }
            return sql;
        }

    }
    public class BaseInSearch:BaseSearch
    {
        public int ID { get; set; }
        public string keyword { get; set; }
    }

    public class BaseDateSearch : BaseSearch
    {
        public int ID { get; set; }
        public string keyword { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }
    }
}
