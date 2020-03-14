using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
   public class Stock
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string  ProductName { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImgUrl { get; set; }
        /// <summary>
        /// 产品数量
        /// </summary>
        public int count { get; set; }
        public string Consignee { get; set; }
        public string ProductNumber { get; set; }
    }
}
