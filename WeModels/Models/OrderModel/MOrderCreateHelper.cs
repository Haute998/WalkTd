using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.OrderModel
{
    public class MOrderCreateHelper
    {
        /// <summary>
        /// 订单对象
        /// </summary>
        public Order order { get; set; }
        /// <summary>
        /// 商品集合
        /// </summary>
        public List<Product> goodsLst { get; set; }

        /// <summary>
        /// 下单的购物车  下单后会赋值
        /// </summary>
        public List<C_UserCart> carts { get; set; }

        /// <summary>
        /// 订单创建
        /// </summary>
        /// <param name="mid">商品编号，直接购买单个商品时必填</param>
        /// <param name="mcnt_i">商品数量，直接购买单个商品时必填</param>
        /// <param name="mailid">收货地址编号</param>
        /// <param name="type">类型：mail/cart 即：商品/购物车</param>
        /// <param name="user">用户</param>
        /// <param name="Remark">买家备注</param>
        /// <returns></returns>
 


        /// <summary>
        /// 创建订单  在此生成订单编号
        /// </summary>
        /// <returns></returns>
       
    }
}
