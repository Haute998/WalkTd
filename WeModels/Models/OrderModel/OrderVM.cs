using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.OrderModel
{
    public class OrderVM
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public Order order { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        public List<ProductSnap> ProSnapLst { get; set; }
        /// <summary>
        /// 邮寄信息
        /// </summary>
        public C_UserMail mail { get; set; }
        /// <summary>
        /// 快递信息
        /// </summary>
        public OrderPost postInfo { get; set; }

        /// <summary>
        /// 加载订单对象
        /// </summary>
        /// <param name="orderno"></param>
     
    }
}
