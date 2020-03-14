using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_User
    {
        /// <summary>
        /// 根据ID获取UserName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetUserNameByID(int id)
        {
            try
            {
                string sql = "select top 1 UserName from C_User where ID=@ID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ID", id)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                return obj != null ? obj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "GetUserNameByID_error");
                return string.Empty;
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int EditPwd(string UserName, string newpwd)
        {
            string strSql = "UPDATE [C_User] SET PassWord=@PassWord WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@PassWord",newpwd),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);

            DatPwdChangeByUserName(UserName);

            return cnt;
        }
        public static int GetPhoneCnt(string Phone, string UserName)
        {
            string strSql = "SELECT count(1) FROM [C_User] where Phone=@Phone and UserName!=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters
                = {
        new System.Data.SqlClient.SqlParameter("@Phone", Phone)
        , new System.Data.SqlClient.SqlParameter("@UserName", UserName)
                  };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 审核代理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int AuditByID(int id)
        {
            string strSql = "UPDATE [C_User] SET state='已审核' WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public static string AuditByIDs(int[] cklst)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');

            string strSql = string.Format("UPDATE [C_User] SET state='已审核' ,DatVerify=@DatVerify WHERE ID in ({0});", userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@DatVerify",DateTime.Now.ToString()) };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            if (cnt > 0)
            {
                return "ok";
            }
            else
            {
                return "审核失败";
            }
        }

        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public static string DelByIDs(int[] cklst)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');

            string strSql = string.Format("UPDATE [C_User] SET state='已删除' WHERE ID in ({0});", userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            if (cnt > 0)
            {
                return "ok";
            }
            else
            {
                return "删除失败";
            }
        }

        /// <summary>
        /// 根据用户名获取用户对象
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static C_User GetUserByUserName(string UserName)
        {
            string strSql = "SELECT * FROM [C_User] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }
        public static C_User GetUserBysjUserName(int ID)
        {
            string strSql = "SELECT * FROM [C_User] WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID) };

            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 根据微信号查找用户
        /// </summary>
        /// <param name="wxNo"></param>
        /// <returns></returns>
        public static C_User GetUserBywxNo(string wxNo)
        {
            string strSql = "SELECT * FROM [C_User] WHERE wxNo=@wxNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@wxNo", wxNo) };

            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 微信号账号量
        /// </summary>
        /// <param name="wxNo"></param>
        /// <returns></returns>
        public static int GetWxNoCnt(string wxNo)
        {
            string strSql = "SELECT count(1) FROM [C_User] where wxNo=@wxNo";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@wxNo", wxNo) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        public static int GetPhoneCnt(string Phone)
        {
            string strSql = "SELECT count(1) FROM [C_User] where Phone=@Phone";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Phone", Phone) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }


        public static int GetCardCnt(string Card)
        {
            string strSql = "SELECT count(1) FROM [C_User] where Card=@Card";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Card", Card) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 更新身份证url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CardUrl"></param>
        /// <returns></returns>
        public static int EditCardUrlByID(int id, string CardUrl)
        {
            string strSql = "UPDATE [C_User] SET CardUrl=@CardUrl WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@CardUrl",CardUrl)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static bool IsSysUser(string UserName)
        {
            string SqlStr = "SELECT COUNT(*) FROM [C_User] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@UserName",UserName),
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
    }
}
