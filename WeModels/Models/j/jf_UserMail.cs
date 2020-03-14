using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class jf_UserMail
    {
        /// <summary>
        /// 值验证  通过时返回空字符串  否则为错误信息
        /// </summary>
        /// <returns></returns>
        public string veri()
        {
            if (string.IsNullOrWhiteSpace(ContactName))
            {
                return "收货人不能为空";
            }
            if (string.IsNullOrWhiteSpace(ContactMobile))
            {
                return "收货电话不能为空";
            }
            if (string.IsNullOrWhiteSpace(Address))
            {
                return "收货地址不能为空";
            }
            if (Address.Length < 5)
            {
                return "请输入详细的收货地址";
            }

            return string.Empty;
        }
        /// <summary>
        /// 根据用户名获取用户默认收货地址
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static jf_UserMail GetDefaultMailByUser(string username)
        {
            string strSql = "SELECT top 1 * FROM [jf_UserMail] WHERE UserName=@UserName and IsDefault=1 order by id desc";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", username) };

            return DAL.EntityDataHelper.LoadData2Entity<jf_UserMail>(strSql, paramters);
        }

        /// <summary>
        /// 根据客户ID获取邮寄地址
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<jf_UserMail> GetMyMail(string UserName)
        {
            string strSql = "SELECT * FROM [jf_UserMail] where UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.FillData2Entities<jf_UserMail>(strSql, paramters);
        }

        /// <summary>
        /// 取消除某个地址之外的默认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int CancelDefaultNotID(int id)
        {
            string strSql = "UPDATE [jf_UserMail] SET IsDefault=0 WHERE ID<>@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
