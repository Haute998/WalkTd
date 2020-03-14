using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WeModels
{
    public partial class C_WxUser
    {
        /// <summary>
        /// 根据用户名获取用户对象
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static C_WxUser GetUserByUserName(string UserName)
        {
            string strSql = "SELECT * FROM [C_WxUser] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_WxUser>(strSql, paramters);
        }

        /// <summary>
        /// 查询出直属于总部用户
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>

        private static List<C_WxUser> GetOptionzTreeMenu(string PaterUser)
        {
            string strSql = "SELECT * FROM [C_WxUser] WHERE PaterUser=@PaterUser";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@PaterUser", string.IsNullOrWhiteSpace(PaterUser) ? "" : PaterUser) };
            return DAL.EntityDataHelper.FillData2Entities<C_WxUser>(strSql, paramters);
        }

        public static int GetOptionUser(string GrandpaUser)
        {
            string strSql = "SELECT * FROM [C_WxUser] WHERE GrandpaUser=@GrandpaUser";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@GrandpaUser", string.IsNullOrWhiteSpace(GrandpaUser) ? "" : GrandpaUser) };
            return DAL.EntityDataHelper.FillData2Entities<C_WxUser>(strSql, paramters).Count;
        }

        public static int GetOptionUsers(string PaterUser)
        {
            return GetOptionzTreeMenu(PaterUser).Count;
        }

     
        /// <summary>
        /// 更新客户邀请信息
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="GrandpaUser"></param>
        /// <param name="PaterUser"></param>
        /// <returns></returns>
        public static int UpdateInviteByUserName(string UserName, string TooGrandpaUser, string GrandpaUser, string PaterUser)
        {
            string strSql = "UPDATE [C_WxUser] SET GrandpaUser=@GrandpaUser,PaterUser=@PaterUser,TooGrandpaUser=@TooGrandpaUser WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@GrandpaUser",GrandpaUser),
                new System.Data.SqlClient.SqlParameter("@PaterUser",PaterUser),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@TooGrandpaUser",TooGrandpaUser)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 更新最后抽奖时间
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="DatLastPrizeDraw"></param>
        /// <returns></returns>
        public static int UpdateDatLastPrizeDrawByUserName(string UserName, DateTime DatLastPrizeDraw)
        {
            string strSql = "UPDATE [C_WxUser] SET DatLastPrizeDraw=@DatLastPrizeDraw WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@DatLastPrizeDraw",DatLastPrizeDraw),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 增加一次抽奖次数
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int AddOnePrizeDrawCntByUserName(string UserName)
        {
            string strSql = "UPDATE [C_WxUser] SET PrizeDrawCnt+=1 WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 修改抽奖次数
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PrizeDrawCnt"></param>
        /// <returns></returns>
        public static int EditPrizeDrawCntByUserName(string UserName, int PrizeDrawCnt)
        {
            string strSql = "UPDATE [C_WxUser] SET PrizeDrawCnt=@PrizeDrawCnt WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@PrizeDrawCnt",PrizeDrawCnt),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 获取当前最大的用户ID
        /// </summary>
        /// <returns></returns>
        public static int GetTopUseID()
        {
            string strSql = "SELECT ISNULL(MAX(ID),1000) FROM [C_WxUser]";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 根据用户名密码获取用户
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static C_WxUser GetUserByPwd(string UserName, string password)
        {
            string strSql = "SELECT * FROM [C_WxUser] WHERE UserName=@UserName and Password=@Password";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@AdminName", UserName),
                                                             new System.Data.SqlClient.SqlParameter("@Password", password)};

            return DAL.EntityDataHelper.LoadData2Entity<C_WxUser>(strSql, paramters);
        }

        /// <summary>
        /// 获取小伙伴数
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int GetPartnersCount(string UserName)
        {
            string strSql = "SELECT count(1) FROM [C_WxUser] where GrandpaUser=@UserName or PaterUser=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 获取小伙伴总订单数
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int MyPartnersOrderCount(string UserName)
        {
            string strSql = @"select COUNT(1) as allorder from MOrder 
                                left join C_WxUser on MOrder.UserName=C_WxUser.UserName
                                where (C_WxUser.GrandpaUser=@UserName or C_WxUser.PaterUser=@UserName) 
                                and OrderState='交易完成' ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 拉黑客户或取消拉黑
        /// </summary>
        /// <param name="cklst">客户主键集合</param>
        /// 是否拉黑
        /// <returns></returns>
        public static string DeFriend(int[] cklst, bool isde)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');


            string strSql = string.Format("UPDATE [C_WxUser] SET IsValid={0} WHERE C_WxUser.ID in ({1});", isde ? 0 : 1, userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            if (cnt > 0)
            {
                return "ok";
            }
            else
            {
                return "拉黑失败";
            }

        }

        /// <summary>
        /// 获取用户邀请码 返回邀请码url
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
 

        /// <summary>
        /// 进行邀请行为
        /// </summary>
        /// <param name="username">当前客户</param>
        /// <param name="fromUsername">来自谁的邀请</param>
        /// <returns></returns>
    


        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="Mobile"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int EditMobileByUserName(string Mobile, string UserName)
        {
            string strSql = "UPDATE [C_WxUser] SET Mobile=@Mobile WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@Mobile",Mobile),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public bool UpdateUserEmail(string UserName)
        {
            string strSql = "UPDATE [C_WxUser] SET Email='T' WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
               new System.Data.SqlClient.SqlParameter("@UserName", UserName)
              
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }

        public bool UpdateUserqx(string UserName)
        {
            string strSql = "UPDATE [C_WxUser] SET Email='' WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
               new System.Data.SqlClient.SqlParameter("@UserName", UserName)
              
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }
    }
}
