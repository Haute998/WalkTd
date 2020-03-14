using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class OutStockCensus
    {
        /// </summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string P_ImgUrl { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string C_Name { get; set; }
        public int CreateTime{get;set;}
        public string OutOrderNo{get;set;}
        public string BigCode{get;set;}
        public string MiddleCode { get; set; }
    }
    public class OutStockCensusShow : BaseSearch
    {
        public string keyword { get; set; }
        public string OutCK { get; set; }
        public string CK { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }
        public string OutOrderNo { get; set; }
        public string ProductNumber { get; set; }
    }
}
