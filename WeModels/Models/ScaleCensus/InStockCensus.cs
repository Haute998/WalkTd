using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class InStockCensus 
    {
        public string IntoOrderNo { get; set; }
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string ProductNumber { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string P_ImgUrl { get; set; }
        public int IntoTime { get; set; }
    }
    public class InStockCensusShow : BaseSearch
    {
        public string IntoOrderNo { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public int O_ID { get; set; }
        public string CK { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }
        public string keyword { get; set; }

    }


}
