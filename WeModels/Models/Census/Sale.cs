using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class Sale
    {
        /// <summary>
        /// 授权编号
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 总销售量
        /// </summary>
        public decimal SumPrice { get; set; }
        public int sumcount { get; set; }
        public string Consignee { get; set; }
        
        
    }
    public class SaleSearch : BaseSearch
    {
        /// <summary>
        /// 多条件
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        public int C_UserTypeID { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string DatCreateB { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string DatCreateE { get; set; }
    }
    public class SaleInCrease
    {
        /// <summary>
        /// 授权编号
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 总销售量
        /// </summary>
        public decimal OldSumPrice { get; set; }
        /// <summary>
        /// 总销售量
        /// </summary>
        public decimal TheMonthSumPrice { get; set; }
        /// <summary>
        /// 总销售量
        /// </summary>
        public decimal Increase { get; set; }
        /// <summary>
        /// 总销售量
        /// </summary>
        public decimal SumPrice { get; set; }
    }
}
