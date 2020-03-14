using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.C_UserModel;
using WeModels.Models.OrderModel;

namespace WeModels
{
    public partial class Order
    {
        /// <summary>
        /// 订单查询模型
        /// </summary>
        public class OrderSearch : BaseSearch
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public string keyword { get; set; }
            /// <summary>
            /// 订单编号
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
            /// 订单状态
            /// </summary>
            public string OrderState { get; set; }
            public static string StrWhere(OrderSearch condition)
            {
                string where = string.Empty;
                //订单编号
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
        /// <summary>
        /// 获取用户订单数
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int GetUserOrderCnt(string username)
        {
            try
            {
                string sql = "select count(1) from [Order] where UserName=@UserName";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@UserName", username)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string str = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(str, out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Order_GetUserOrderCnt_error");
                return 0;
            }
        }

        ///// <summary>
        ///// 根据订单编号审核订单 订单发货
        ///// </summary>
        ///// <param name="orderNo"></param>
        ///// <returns></returns>
        //public static string GetUpdateOrderByOrderNo(string orderNo)
        //{

        //    Order order = Order.GetOrderByOrderNo(orderNo);
        //    if (order == null) 
        //    {
        //        return "订单不存在";
        //    }
        //    if (order.SendState == "已发货")
        //    {
        //        return "该订单已经发过货了";
        //    }


        //    #region 以下是返利计算  将获得【userGetMoneys：各代理UserName及获得的金额；rebates：获利记录表集合】

        //    //计算返利
        //    //decimal GetOrderRate = BaseParameters.getRate("GetOrderRate");//拿货返利率
        //    decimal direct_IntroducerRate1 = BaseParameters.getRate("direct_IntroducerRate1");//推荐返利 直属一代返利率	
        //    decimal direct_IntroducerRate2 = BaseParameters.getRate("direct_IntroducerRate2");//推荐返利 直属二代返利率
        //    decimal direct_IntroducerRate3 = BaseParameters.getRate("direct_IntroducerRate3");//推荐返利 直属三代返利率

        //    List<C_UsersRebateMoney> userGetMoneys = new List<C_UsersRebateMoney>();//用于计算总获利额

        //    List<C_UserRebate> rebates = new List<C_UserRebate>();//返利集合

        //    C_UserVM user = C_UserVM.GetVMByUserName(order.UserName);//当前代理

        //    if (user == null)
        //    {
        //        return "找不到该订单的归属人";
        //    }
        //    //**************************【推荐返利】
        //    C_UserVM userIntro = user;//推荐人
        //    for (int i = 1; i <= 3; i++) //循环3个级
        //    {
        //        string thisIntroducer = userIntro.Introducer;
        //        userIntro = C_UserVM.GetVMByUserName(userIntro.Introducer);//推荐人

        //        if (string.IsNullOrWhiteSpace(thisIntroducer) || userIntro == null)
        //        {
        //            break;//没有推荐人则退出
        //        }
        //        if (userIntro.C_UserTypeID == 2) //推荐人如果符合总裁 一个条件则获得推荐返利
        //        {
        //            C_UserRebate rebate = new C_UserRebate();//当前推荐返利

        //            decimal thisRate = 0;
        //            if (i == 1)
        //            {
        //                thisRate = direct_IntroducerRate1;//一级返利率
        //                rebate.Type = "一级返利";
        //                rebate.tjType = 1;
        //            }
        //            else if (i == 2)
        //            {
        //                thisRate = direct_IntroducerRate2;//二级返利率
        //                rebate.Type = "二级返利";
        //                rebate.tjType = 2;
        //            }
        //            else if (i == 3)
        //            {
        //                thisRate = direct_IntroducerRate3;//三级返利率
        //                rebate.Type = "三级返利";
        //                rebate.tjType = 3;
        //            }

        //            C_UserVM userIntro_Chief = C_UserVM.GetVMByID(userIntro.Chief);//推荐人的上级

        //            //返利记录
        //            rebate.Cat = "推荐返利";
        //            rebate.DatCreat = DateTime.Now;
        //            rebate.Issuer = userIntro_Chief.UserName;//返利发放人
        //            rebate.Money = order.SumPrice * thisRate;//返利金额

        //            if (rebate.Money > 0)
        //            {
        //                rebate.R_People = userIntro.UserName;
        //                rebate.State = "未发放";
        //                rebate.OrderNo = order.OrderNo;
        //                rebate.Rate = thisRate.ToString();
        //                rebate.OrderMoney = order.SumPrice;
        //                rebates.Add(rebate);

        //                //获得金额计算
        //                C_UsersRebateMoney userGetMoney = new C_UsersRebateMoney();
        //                userGetMoney.UserName = userIntro.UserName;
        //                userGetMoney.getMoney = rebate.Money;
        //                userGetMoneys.Add(userGetMoney);

        //            }
        //        }

        //    }
        //    #endregion 计算返利结束


        //    using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
        //    {
        //        conn.Open();
        //        System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

        //        try
        //        {
        //            string strSql = "UPDATE [Order] SET OrderState='已发货',DatSend=@DatSend,SendState='已发货' WHERE OrderNo=@OrderNo;";
        //            System.Data.SqlClient.SqlParameter[] paramters ={
        //                                    new System.Data.SqlClient.SqlParameter("@OrderNo",orderNo),
        //                                        new System.Data.SqlClient.SqlParameter("@DatSend",DateTime.Now)
        //                                     };
        //            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);




        //            #region 以下是返利信息提交

        //            //代理的余额增加
        //            foreach (C_UsersRebateMoney userGetMoney in userGetMoneys)
        //            {
        //                C_User.addMoneyByUserName(tran, userGetMoney.UserName, userGetMoney.getMoney);
        //            }

        //            //返利记录增加
        //            foreach (C_UserRebate rebate in rebates)
        //            {
        //                rebate.InsertAndReturnIdentity(tran);
        //            }

        //            #endregion 返利信息提交结束



        //            tran.Commit();
        //            OrderLog.LogAdd(orderNo, "发货", "订单被发货", "mobile");
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            DAL.Log.Instance.Write(ex.ToString(), "SendOrder_Error");
        //            return "发货失败";
        //        }
        //        finally
        //        {
        //            tran.Dispose();
        //            conn.Close();
        //        }
        //    }
        //    return "ok";
        //}
        /// <summary>
        /// 根据订单编号审核订单 订单发货
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        

        /// <summary>
        /// 获取订单数量
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static int GetOrderCount(string orderNo)
        {
            string SqlStr = "SELECT Sum(GetCnt) FROM ProductSnap WHERE OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@OrderNo", orderNo) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));
        }
        /// <summary>
        /// 根据订单编号获取订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static Order GetOrderByOrderNo(string orderNo)
        {
            string strSql = "SELECT * FROM [Order] WHERE OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@OrderNo", orderNo) };
            return DAL.EntityDataHelper.LoadData2Entity<Order>(strSql, paramters);
        }

        /// <summary>
        /// 取消订单  ok为成功
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static int DelOrder(string OrderNo)
        {
            string SqlStr = "UPDATE [Order] SET OrderState='已删除' where OrderNo=@OrderNo";
            System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@OrderNo", OrderNo) };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, SqlParameter);
        }
        /// <summary>
        /// 获取未审核订单(红点)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetC_UserCircles(string UserName)
        {
            string strSql = "SELECT Count(*) FROM [Order] WHERE ParentUser=@ID and AuditState='未审核' and OrderState='待审核'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", UserName) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }

        /// <summary>
        /// 获取未发货订单(红点)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetOrder(string UserName)
        {
            string strSql = "SELECT Count(*) FROM [Order] WHERE ParentUser=@ID and AuditState='已审核' and SendState='未发货'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", UserName) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }

        /// <summary>
        /// 获总部所有未出货订单
        /// </summary>
        /// <returns></returns>
        public static List<CustomerOrder> GetHQNotOrderAll()
        {
            string strSql = "SELECT TOP 100000 a.OrderNo,c.ProductName,c.ProductNumber,b.GetCnt as Qty "+
                            "FROM [Order] as a left join ProductSnap as b on a.OrderNo=b.OrderNo left join Product as c on b.ProductID=c.ProductID "+
                            "where ParentUser='' and AuditState='已审核' and SendState='未发货' and c.ProductID is not null ";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<CustomerOrder>(strSql, paramters);
        }

        /// <summary>
        /// 获取总部分页订单数据
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static List<CustomerOrder> GetHQPageNotOrderAll(int PageIndex, int PageSize, string OrderNo, out int totalCount)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(OrderNo))
            {
                where = @" OrderNo='" + OrderNo + "'";
            }

            string strSql = "exec dbo.Common_PageList 'View_GetHQOrder','*','OrderNo',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };

            List<CustomerOrder> OrderList = DAL.EntityDataHelper.FillData2Entities<CustomerOrder>(strSql, paramters);
            if (!string.IsNullOrWhiteSpace(OrderNo))
            {
                where = @" where OrderNo='" + OrderNo + "'";
            }
            strSql = "select count(*) from View_GetHQOrder "+ where;
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;
        }

    }
    /// <summary>
    /// 订单查询模型
    /// </summary>
    public class OrderSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }
        public string Name { get; set; }
        public string NName { get; set; }
        /// <summary>
        /// 订单创建时间开始时间
        /// </summary>
        public string DatCreateB { get; set; }
        /// <summary>
        /// 订单创建时间结束时间
        /// </summary>
        public string DatCreateE { get; set; }
        /// <summary>
        /// 订单状态（待审核/待发货/已发货/已取消）
        /// </summary>
        public string OrderState { get; set; }
        /// <summary>
        /// 审核状态  未审核/已审核
        /// </summary>
        public string AuditState { get; set; }
        public static string StrWhere(OrderSearch condition)
        {
            string where = string.Empty;
            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (OrderNo like '%{0}%' or OrderState like '%{0}%' or OrderName like '%{0}%' or C_User.Name like '%{0}%') ", condition.keyword);
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


            if (!string.IsNullOrWhiteSpace(condition.OrderState))
            {
                where += string.Format(" and OrderState ='{0}' ", Common.Filter(condition.OrderState));
            }
            return where;
        }
    }

    public class CustomerOrder
    {
        public string OrderNo { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
