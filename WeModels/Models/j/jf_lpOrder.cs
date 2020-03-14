using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_lpOrderSearch : BaseSearch 
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单创建时间开始时间
        /// </summary>
        public string DatCreateB { get; set; }
        /// <summary>
        /// 订单创建时间结束时间
        /// </summary>
        public string DatCreateE { get; set; }
        /// <summary>
        /// 订单支付时间开始时间
        /// </summary>
        public string DatPayB { get; set; }
        /// <summary>
        /// 订单支付时间结束时间
        /// </summary>
        public string DatPayE { get; set; }

        /// <summary>
        /// 消费者/促销员
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderState { get; set; }
        public static string StrWhere(jf_lpOrderSearch condition)
        {
            string where = string.Empty;
            //订单号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (OrderNo like '%{0}%' or OrderState like '%{0}%' or OrderName like '%{0}%') ", condition.keyword);
            }
            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and DatCreate >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and DatCreate <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }

            //订单支付时间
            if (!string.IsNullOrWhiteSpace(condition.DatPayB))
            {
                where += string.Format(" and DatPay >='{0} 00:00:00' ", Common.Filter(condition.DatPayB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatPayE))
            {
                where += string.Format(" and DatPay <'{0} 23:59:59' ", Common.Filter(condition.DatPayE));
            }

            if (!string.IsNullOrWhiteSpace(condition.OrderState))
            {
                where += string.Format(" and OrderState ='{0}' ", Common.Filter(condition.OrderState));
            }
            return where;
        }
    }
    public partial class jf_lpOrder
    {
        /// <summary>
        /// 根据订单号获取订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static jf_lpOrder GetOrderByOrderNo(string orderNo)
        {
            string strSql = "SELECT * FROM [jf_lpOrder] WHERE OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@OrderNo", orderNo) };
            return DAL.EntityDataHelper.LoadData2Entity<jf_lpOrder>(strSql, paramters);
        }
        /// <summary>
        /// 积分支付  返回 ok 为成功
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="payIntegral"></param>
        /// <returns></returns>
        public static string jf_lpOrderPay_jf(string orderNo)
        {
            jf_lpOrder order = jf_lpOrder.GetOrderByOrderNo(orderNo);
            if (order == null)
            {
                return "兑换失败，订单不存在！";
            }
            if (order.OrderState != "待支付")
            {
                return "订单不能重复兑换";
            }
            int payIntegral = order.SumIntegral;

            string payUserName = order.UserName;//支付人

            C_Consumer user = C_Consumer.GetEntityByUserName(order.UserName);
            if (user != null)
            {
                payUserName = user.UserName;
            }
            else
            {
                return "订单所属的人不存在";
            }
            if (user.jf<order.SumIntegral)
            {
                return "兑换失败，您的积分不足！";
            }


            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //改变订单状态
                    string strSql = "UPDATE [jf_lpOrder] SET OrderState='待发货',PayState='已支付',PayIntegral=@PayIntegral,DatPay=@DatPay,PayUserName=@PayUserName WHERE OrderNo=@OrderNo";
                    System.Data.SqlClient.SqlParameter[] paramters ={
                                new System.Data.SqlClient.SqlParameter("@PayIntegral",payIntegral),
                                new System.Data.SqlClient.SqlParameter("@DatPay",DateTime.Now),
                                new System.Data.SqlClient.SqlParameter("@OrderNo",orderNo),
                                new System.Data.SqlClient.SqlParameter("@PayUserName",payUserName)
                            };
                    DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);

                    //积分变动
                    strSql = "update C_Consumer set jf-=@jf where UserName=@UserName";
                    System.Data.SqlClient.SqlParameter[] paramters4 ={
                         new System.Data.SqlClient.SqlParameter("@jf",payIntegral),
                         new System.Data.SqlClient.SqlParameter("@UserName",user.UserName)};
                    DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters4);


                    j_jflog log = new j_jflog();
                    log.Code = "";
                    log.Dat = DateTime.Now;
                    log.jf = payIntegral;
                    log.logContents = user.UserName + "使用" + payIntegral + "积分兑换礼品";
                    log.Type = "兑换";
                    log.UserName = user.UserName;
                    log.UserType = user.Type;
                    log.InsertAndReturnIdentity(tran);

                    C_Consumer.addjfused(user.UserName, payIntegral,tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "jf_lpOrderPay_jf_error");
                    return "支付失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            //微信通知
            new System.Threading.Thread(delegate()
            {
                SYSNotifyMsg.sendSysMsg("auditOrder", string.Format("礼品订单【{0}】支付成功，请尽快处理", order.OrderNo));
            }).Start();

            return "ok";
        }


        /// <summary>
        /// 订单确认收货
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static string ReceiptOrder(string OrderNo)
        {
            jf_OrderVM order = new jf_OrderVM();
            order.LoadOrder(OrderNo);
            if (order.order == null)
            {
                return "订单不存在";
            }
            if (order.gSnapLst == null || order.gSnapLst.Count <= 0)
            {
                return "商品不存在";
            }
            if (order.order.OrderState != "待收货")
            {
                return "该订单" + order.order.OrderState;
            }



            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //改变订单状态
                    string strSql = "UPDATE [jf_lpOrder] SET OrderState='交易完成',DatFinish=@DatFinish WHERE OrderNo=@OrderNo;";
                    System.Data.SqlClient.SqlParameter[] paramters ={
                           new System.Data.SqlClient.SqlParameter("@DatFinish",DateTime.Now),
                           new System.Data.SqlClient.SqlParameter("@OrderNo",OrderNo)
                       };
                    DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);






                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "ReceiptOrder_error");
                    return "收货失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return "ok";
        }
        /// <summary>
        /// 取消订单  ok为成功
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static string CancelOrder(string OrderNo)
        {
            jf_OrderVM order = new jf_OrderVM();
            order.LoadOrder(OrderNo);
            if (order.order == null)
            {
                return "订单不存在";
            }
            if (order.gSnapLst == null || order.gSnapLst.Count <= 0)
            {
                return "商品不存在";
            }
            if (order.order.OrderState != "待支付")
            {
                return "该订单" + order.order.OrderState;
            }
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //改变订单状态
                    string strSql = "UPDATE [jf_lpOrder] SET OrderState='已取消' WHERE OrderNo=@OrderNo;";
                    System.Data.SqlClient.SqlParameter[] paramters ={
                           new System.Data.SqlClient.SqlParameter("@OrderNo",OrderNo)
                       };
                    int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);

                    foreach (jf_GoodsSnap gsnap in order.gSnapLst)
                    {
                        cnt = jf_Goods.BackQuantityByID(tran, gsnap.GoodsID, gsnap.GetCnt);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "CancelOrder_Error");
                    return "取消失败";
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
