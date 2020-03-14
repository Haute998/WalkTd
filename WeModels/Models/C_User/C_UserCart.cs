using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_UserCart
    {
        /// <summary>
        /// 根据产品ID获取购物车对象
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        public static C_UserCart GetCartByGoodsID(string C_UserName, int goodsID)
        {
            string strSql = "SELECT top 1 * FROM [C_UserCart] WHERE C_UserName=@C_UserName and GoodsID=@GoodsID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName),
                                                             new System.Data.SqlClient.SqlParameter("@GoodsID", goodsID)};

            return DAL.EntityDataHelper.LoadData2Entity<C_UserCart>(strSql, paramters);
        }
        /// <summary>
        /// 获取当前选择的购物车
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static List<C_UserCart> GetCartByIsCheck(string C_UserName)
        {
            string strSql = "SELECT * FROM [C_UserCart] WHERE IsCheck=1 and C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName) };

            return DAL.EntityDataHelper.FillData2Entities<C_UserCart>(strSql, paramters);
        }


        /// <summary>
        /// 获取购物车数量
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static int GetCartCnt(string C_UserName)
        {
            string StrSq = "SELECT ISNULL(Sum(GetCnt),0) FROM C_UserCart WHERE C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@C_UserName",C_UserName)
            };
            object rtn = DAL.SqlHelper.ExecuteScalar(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }


        /// <summary>
        /// 添加数量根据购物车ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static int AddCntByID(int id, int cnt)
        {
            string strSql = "UPDATE [C_UserCart] SET GetCnt+=@GetCnt WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@GetCnt",cnt),
            };
            int rtn = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return rtn;
        }
        /// <summary>
        /// 添加到购物车
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="GoodsID"></param>
        /// <param name="GetCnt"></param>
        /// <returns></returns>
        public static string AddToCart(string C_UserName, int GoodsID, int GetCnt)
        {
            C_UserCart cart = C_UserCart.GetCartByGoodsID(C_UserName, GoodsID);
            if (cart != null)
            {
                int rtn = C_UserCart.AddCntByID(cart.ID, GetCnt);
                if (rtn > 0)
                {
                    return "ok";
                }
                else
                {
                    return "网络繁忙，购物车挤不进去啦";
                }
            }
            else
            {
                C_UserCart newcart = new C_UserCart();
                newcart.GoodsID = GoodsID;
                newcart.C_UserName = C_UserName;
                newcart.GetCnt = GetCnt;
                newcart.IsCheck = true;
                int rtn = newcart.InsertAndReturnIdentity();
                if (rtn > 0)
                {
                    return "ok";
                }
                else
                {
                    return "网络繁忙，购物车挤不进去啦";
                }
            }
        }

        /// <summary>
        /// 从购物车减少数量
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="GoodID"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static string ReduceFromCart(string C_UserName, int GoodID, int cnt)
        {
            C_UserCart cart = C_UserCart.GetCartByGoodsID(C_UserName, GoodID);
            if (cart != null)
            {
                if (cart.GetCnt <= 1)
                {
                    int rtn = C_UserCart.DeleteByID(cart.ID);
                    if (rtn > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "网络繁忙，刷新试试";
                    }
                }
                else
                {
                    int rtn = C_UserCart.ReduceCntByID(cart.ID, cnt);
                    if (rtn > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "网络繁忙，刷新试试";
                    }
                }
            }
            else
            {
                return "购物车已经没有该产品啦，刷新试试";
            }
        }
        /// <summary>
        /// 修改数量根据购物车ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static int EditCntByID(int id, int cnt)
        {
            string strSql = "UPDATE [C_UserCart] SET GetCnt=@GetCnt WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@GetCnt",cnt),
            };
            int rtn = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return rtn;
        }
        /// <summary>
        /// 从购物车减少数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static int ReduceCntByID(int id, int cnt)
        {
            string strSql = "UPDATE [C_UserCart] SET GetCnt-=@GetCnt WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@GetCnt",cnt),
            };
            int rtn = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return rtn;
        }

        /// <summary>
        /// 从购物车修改产品数量
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="GoodID"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static string EditFromCart(string C_UserName, int GoodsID, int cnt)
        {
            C_UserCart cart = C_UserCart.GetCartByGoodsID(C_UserName, GoodsID);
            if (cart != null)
            {
                int rtn = 0;
                if (cnt > 0)
                {
                    rtn = C_UserCart.EditCntByID(cart.ID, cnt);
                }
                else
                {
                    rtn = C_UserCart.DeleteByID(cart.ID);
                }
                if (rtn > 0)
                {
                    return "ok";
                }
                else
                {
                    return "网络繁忙，刷新试试";
                }
            }
            else
            {
                return "购物车已经没有该产品啦，刷新试试";
            }
        }

        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isCheck"></param>
        /// <returns></returns>
        public static int CheckByID(int id,bool isCheck)
        {
            string strSql = "UPDATE [C_UserCart] SET IsCheck=@IsCheck WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@IsCheck",isCheck)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

    }
}
