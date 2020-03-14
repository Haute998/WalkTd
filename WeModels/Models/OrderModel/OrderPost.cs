using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.C_UserModel;

namespace WeModels
{
    public partial class OrderPost
    {
        /// <summary>
        /// 根据订单编号获取快递信息
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static OrderPost GetInfoByOrderNo(string OrderNo)
        {
            string strSql = "SELECT top 1 * FROM [OrderPost] WHERE OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@OrderNo", OrderNo) };
            return DAL.EntityDataHelper.LoadData2Entity<OrderPost>(strSql, paramters);
        }

        /// <summary>
        /// 发送错误消息
        /// </summary>
        /// <param name="content"></param>
        private static void senderror(string content)
        {
            new System.Threading.Thread(delegate()
            {
                SYSNotifyMsg.sendSysMsg("error", content);
            }).Start();
        }

        /// <summary>
        /// 订单发货 返回ok为成功 【新发货】
        /// </summary>
        /// <param name="orderPost"></param>
        /// <returns></returns>

    }
}
