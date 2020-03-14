using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SAntiFakeSelSearch : BaseSearch
    {
        /// <summary>
        /// 多条件查询
        /// </summary>
        public string keyword { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string DatCreateB { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string DatCreateE { get; set; }



        public static string StrWhere(SAntiFakeSelSearch condition)
        {
            string where = string.Empty;
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (o.OrderNo like '%{0}%' or s.Name like '%{0}%' or w.WorderNo like '%{0}%') ", condition.keyword);
            }
            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and AddingTime >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and AddingTime <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }
         
            return where;
        }
    }

    public partial class SAntiFakeSel
    {
        public string Name { get; set; }

        public string OrderNo { get; set; }

        public static List<Order> GetNoID()
        {
            string strSql = "select * from [Order] where id not in(select BatchNumber from Worder)";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<Order>(strSql, paramters);
        }
    }
}
