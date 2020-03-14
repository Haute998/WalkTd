using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.ProductModel
{
    public class LevelPriceVM : C_UserType
    {
        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// Product_Lever 主键
        /// </summary>
        public int ProL_ID { get; set; }
    }
}
