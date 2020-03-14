using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class BasePostCode
    {
        /// <summary>
        /// 获取实例（排序）
        /// </summary>
        /// <returns></returns>
        public static List<BasePostCode> GetAllBySort()
        {
            string strSql = "SELECT * FROM [BasePostCode] order by IsHave Desc,Sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<BasePostCode>(strSql, paramters);
        }
        /// <summary>
        /// 获取可用实例（排序）
        /// </summary>
        /// <returns></returns>
        public static List<BasePostCode> GetHaveAllBySort()
        {
            string strSql = "SELECT * FROM [BasePostCode] where IsHave=1 order by IsHave Desc,Sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<BasePostCode>(strSql, paramters);
        }
        /// <summary>
        /// 根据快递公司代号查
        /// </summary>
        /// <param name="PostCode"></param>
        /// <returns></returns>
        public static BasePostCode GetByPostCode(string PostCode)
        {
            string strSql = "SELECT top 1 * FROM [BasePostCode] where PostCode=@PostCode";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@PostCode",PostCode)
            };
            return DAL.EntityDataHelper.LoadData2Entity<BasePostCode>(strSql, paramters);
        }

        /// <summary>
        /// 启用或禁用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ishave"></param>
        /// <returns></returns>
        public static int TohaveByID(int id, bool ishave)
        {
            string strSql = "UPDATE [BasePostCode] SET IsHave=@IsHave WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@IsHave",ishave)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
