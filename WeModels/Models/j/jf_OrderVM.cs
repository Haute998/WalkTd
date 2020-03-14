using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_OrderVM
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public jf_lpOrder order { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public List<jf_GoodsSnap> gSnapLst { get; set; }
        /// <summary>
        /// 邮寄信息
        /// </summary>
        public jf_UserMail mail { get; set; }
        /// <summary>
        /// 快递信息
        /// </summary>
        public j_OrderPost postInfo { get; set; }

        /// <summary>
        /// 加载订单对象
        /// </summary>
        /// <param name="orderno"></param>
        public void LoadOrder(string orderno)
        {
            order = jf_lpOrder.GetOrderByOrderNo(orderno);
            gSnapLst = EntityDataHelper.FillData2Entities<jf_GoodsSnap>(string.Format(@"select * from jf_GoodsSnap where OrderNo='{0}'", orderno));
            mail = jf_UserMail.GetEntityByID(order.MailID);
            postInfo = j_OrderPost.GetInfoByOrderNo(orderno);
        }
    }
}
