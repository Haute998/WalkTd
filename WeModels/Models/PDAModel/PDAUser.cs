using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDAUser
    {
        public string FuncNameSet { get; set; }

        public string Supplier { get; set; }
        public static PDAUser PDAUserLogin(string PUserName, string Password)
        {
            string strSql = "select * FROM [PDAUser] WHERE PUserName=@PUserName and Password=@Password";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@PUserName", PUserName),
                                                             new System.Data.SqlClient.SqlParameter("@Password", Password)};

            return DAL.EntityDataHelper.LoadData2Entity<PDAUser>(strSql, paramters);
        }
        public static int DatLoginChange(string DeviceCode, string UserName)
        {
            string strSql = "UPDATE [PDAUser] SET LastLoginDCode=@LastLoginDCode,LastLoginTime=@LastLoginTime WHERE PUserName=@PUserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@PUserName",UserName),
                new System.Data.SqlClient.SqlParameter("@LastLoginDCode",DeviceCode),
                new System.Data.SqlClient.SqlParameter("@LastLoginTime",DateTime.Now)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static List<PDAUser> GetEntitysAllMore()
        {
            string strSql = "SELECT TOP 100000 a.ID,PUserName,Password,PRealName,Remark,LastLoginDCode,LastLoginTime,CreatDate,dbo.[GetUserFuncNameSet](a.ID) as FuncNameSet,SupplierID,(case when SupplierID=0 then '总部' else b.Name end) as Supplier " +
                            "FROM [PDAUser] as a left join Supplier as b on a.SupplierID=b.ID ";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<PDAUser>(strSql, paramters);
        }

        public static List<PDAUser> GetUserNameSet(string UserNameSet)
        {
            string strSql = "select * from [PDAUser] where PUserName in(" + UserNameSet + ")";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<PDAUser>(strSql, paramters);
        }
    }
}
