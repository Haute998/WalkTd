using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.C_UserModel;

namespace WeModels
{
    public class C_UserTypeSearch:BaseSearch
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// 代理名称
        /// </summary>
        public string Name { get; set; }

        public static string StrWhere(C_UserTypeSearch condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.Name))
            {
                where += string.Format(" and C_UserType.Name like'%{0}%' ", Common.Filter(condition.Name));
            }
            return where;
        }
    }
    public partial class C_UserType
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int cnt { get; set; }
        /// <summary>
        /// 根据ID获取名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetNameByID(string id)
        {
            try
            {
                string sql = "select top 1 Name from C_UserType where ID=@ID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ID", id)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                return obj != null ? obj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "C_UserType_GetNameByID_error");
                return string.Empty;
            }
        }
        /// <summary>
        /// 根据GetNameByLever获取名字
        /// </summary>
        /// <param name="Lever"></param>
        /// <returns></returns>
        public static string GetNameByLever(string Lever)
        {
            try
            {
                string sql = "select top 1 Name from C_UserType where Lever=@Lever";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@Lever", Lever)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                return obj != null ? obj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "C_UserType_GetNameByLever_error");
                return string.Empty;
            }
        }
        /// <summary>
        /// 根据等级查找示例
        /// </summary>
        /// <param name="Lever"></param>
        /// <returns></returns>
        public static C_UserType GetEntityByLever(int Lever)
        {
            string strSql = "SELECT * FROM [C_UserType] WHERE Lever=@Lever";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Lever", Lever) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserType>(strSql, paramters);
        }
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static int EditMinOrderPriceByID(int id,decimal price)
        {
            string strSql = "UPDATE [C_UserType] SET MinOrderPrice=@MinOrderPrice WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@MinOrderPrice",price),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 获取最大等级
        /// </summary>
        /// <returns></returns>
        public static int GetMaxLevel()
        {
            try
            {
                string sql = "select isnull(Min(Lever),0) from C_UserType";
                System.Data.SqlClient.SqlParameter[] paramters = null;
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string str = obj != null ? obj.ToString() : string.Empty;
                int level = 0;
                int.TryParse(str,out level);
                return level;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "C_UserType_GetMaxLevel_error");
                return 0;
            }
        }
        /// <summary>
        /// 获取最大级别类型
        /// </summary>
        /// <returns></returns>
        public static C_UserType GetMaxLevel_UserType()
        {
            string strSql = "select * from C_UserType where Lever =(select Min(Lever) from C_UserType)";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.LoadData2Entity<C_UserType>(strSql, paramters);
        }

        /// <summary>
        /// 获取低于某代理的所有级别
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<C_UserType> GetLowerByUserID(int PUserID,int UserID,bool andself=false)
        {
            //C_UserVM Puser = C_UserVM.GetVMByID(PUserID);
            C_UserVM user = C_UserVM.GetVMByID(UserID);
            string strSql = @"select C_UserType.* from C_UserType 
                            where Lever > (select user_type.Lever from C_User left join C_UserType user_type on C_User.C_UserTypeID=user_type.Lever where C_User.ID=@UserID)";

            if (andself == false) 
            {
                strSql += " and Lever<@lowLevel ";
            }
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserID",PUserID),
                new System.Data.SqlClient.SqlParameter("@lowLevel",user!=null?user.userTypeLever:0)
            };

            return DAL.EntityDataHelper.FillData2Entities<C_UserType>(strSql, paramters);
        }

        /// <summary>
        /// 获取低于某代理的所有级别
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<C_UserType_Simple> GetLowerByUserID(int UserTypeID)
        {
            string strSql = @"select * from C_UserType where Lever > (select Lever from C_UserType where ID=@ID)";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@ID",UserTypeID)
            };

            return DAL.EntityDataHelper.FillData2Entities<C_UserType_Simple>(strSql, paramters);
        }

        /// <summary>
        /// 获取高于或等于自己的级别
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<C_UserType> GetC_UserTypeHeighter(int UserID)
        {
            C_UserVM user = C_UserVM.GetVMByID(UserID);
            string strSql = @"select C_UserType.* from C_UserType 
                            where Lever <= (select user_type.Lever from C_User left join C_UserType user_type on C_User.C_UserTypeID=user_type.Lever where C_User.ID=@UserID)";

            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserID",UserID)
            };

            return DAL.EntityDataHelper.FillData2Entities<C_UserType>(strSql, paramters);
        }


        /// <summary>
        /// 状态获取类型列表
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public static List<C_UserType> GetEntityList(string State)
        {
            string strSql = string.Empty;
            if (string.IsNullOrWhiteSpace(State))
            {
                strSql = "select * from [C_UserType] where State=@State  ";
            }
            else
            {
                strSql = "select * from [C_UserType] ";
            }
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@State",State)
            };
            return DAL.EntityDataHelper.FillData2Entities<C_UserType>(strSql, paramters);
        }

        public static bool GetEntittyInt(int Lever)
        {
            string strSql = "select count(*) from [C_UserType] where Lever=@Lever";
            System.Data.SqlClient.SqlParameter[] paramters ={
new System.Data.SqlClient.SqlParameter("@Lever",Lever)
                                                          };
            object count = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return count == null ? true : Convert.ToInt32(count) > 0 ? true : false;

        }
        public static C_UserType GetEntittyC_Type(int Lever)
        {
            string strSql = "select * from [C_UserType] where Lever=@Lever";
            System.Data.SqlClient.SqlParameter[] paramters ={
new System.Data.SqlClient.SqlParameter("@Lever",Lever)    };
            return DAL.EntityDataHelper.LoadData2Entity<C_UserType>(strSql, paramters);

        }
    }

    public class C_UserType_Simple
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
