using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class j_OrderPost
    {
        /// <summary>
        /// 根据订单号获取快递信息
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static j_OrderPost GetInfoByOrderNo(string OrderNo)
        {
            string strSql = "SELECT top 1 * FROM [j_OrderPost] WHERE OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@OrderNo", OrderNo) };
            return DAL.EntityDataHelper.LoadData2Entity<j_OrderPost>(strSql, paramters);
        }

        /// <summary>
        /// 订单发货 返回ok为成功 【新发货】
        /// </summary>
        /// <param name="orderPost"></param>
        /// <returns></returns>
        public static string SendOrder(j_OrderPost orderPost, string oper)
        {
            jf_lpOrder order = jf_lpOrder.GetOrderByOrderNo(orderPost.OrderNo);
            if (order.OrderState != "待发货")
            {
                return "该订单不能发货";
            }
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    int PostID = orderPost.InsertAndReturnIdentity(tran);

                    string strSql = "UPDATE [jf_lpOrder] SET OrderState='待收货' WHERE OrderNo=@OrderNo;";
                    System.Data.SqlClient.SqlParameter[] paramters ={
                                            new System.Data.SqlClient.SqlParameter("@OrderNo",orderPost.OrderNo)
                                             };
                    int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
                    tran.Commit();
                    jf_lpOrderLog.LogAdd(orderPost.OrderNo, "发货", "订单发货：" + orderPost.PostName + " " + orderPost.PostNo, oper);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "SendOrder_Error");
                    return "发货失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }

            return "ok";
        }
    }
}
