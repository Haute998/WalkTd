using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDAFuntion
    {
        public bool IsCheck { get; set; }

        /// <summary>
        /// 读取未禁用的功能权限
        /// </summary>
        public static List<PDAFuntion> GetNotDisabledAll()
        {
            string strSql = "SELECT TOP 100000 ID,FunCode,ParentCode,FunName,CreatDate FROM [PDAFuntion] where Disabled=0";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<PDAFuntion>(strSql, paramters);
        }

        /// <summary>
        /// 禁用功能
        /// </summary>
        /// <param name="funcCode"></param>
        /// <returns></returns>
        public static int SetDisabledFunc(string funcCode)
        {
            string strSql = "UPDATE [PDAFuntion] SET Disabled=1 WHERE FunCode in (" + funcCode + ");UPDATE [PDAUserFunc] set IsEnable=0 where FunCode in (" + funcCode + ");";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 启用功能
        /// </summary>
        /// <param name="funcCode"></param>
        /// <returns></returns>
        public static int SetEnableFunc(string funcCode)
        {
            string strSql = "UPDATE [PDAFuntion] SET Disabled=0 WHERE FunCode in (" + funcCode + ");UPDATE [PDAUserFunc] set IsEnable=1 where FunCode in (" + funcCode + ");";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
