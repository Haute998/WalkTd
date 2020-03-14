using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SelScale : BaseSearch
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public string keyword { get; set; }

        /// <summary>
        /// 小标
        /// </summary>
        public string SmallCode { get; set; }

        /// <summary>
        /// 大标
        /// </summary>
        public string BigCode { get; set; }

        /// <summary>
        /// 第一次查询时间
        /// </summary>
        public DateTime SelectDate { get; set; }
        public int cnt { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string name { get; set; }

        public int QueryTimes { get; set; }

        public string SalesAddress { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string value { get; set; }

        public static List<SelScale> GetSelScale()
        {
            string strSql = "select province name,CONVERT(varchar,count(id)) value from SelScale where warning='窜货' group by province";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SelScale>(strSql, paramters);
        }
    }

    public partial class View_AntiQuery
    {
        /// <summary>
        /// 查询日期
        /// </summary>
        public DateTime SelectDate {
            get { return CommonFunc.GetDateTimeFromTimestamp(CreateTime); }
        }
    }
}
