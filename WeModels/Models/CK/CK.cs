using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.C_UserModel;

namespace WeModels
{
    public class CKSearch:BaseSearch
    {
        /// <summary>
        /// 产品id
        /// </summary>
      
        /// <summary>
        /// 代理名称
        /// </summary>
        public string Name { get; set; }

        public static string StrWhere(CKSearch condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.Name))
            {
                where += string.Format(" and Name like'%{0}%' ", Common.Filter(condition.Name));
            }
            return where;
        }
    }
    public partial class CK
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
                string sql = "select top 1 Name from CK where ID=@ID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ID", id)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                return obj != null ? obj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "CK_GetNameByID_error");
                return string.Empty;
            }
        }
        public static List<CK> GetEntityList()
        {
            string strSql = string.Empty;
         
                strSql = "select * from [CK] ";
            
            System.Data.SqlClient.SqlParameter[] paramters = {
                
            };
            return DAL.EntityDataHelper.FillData2Entities<CK>(strSql, paramters);
        }

    }
}
