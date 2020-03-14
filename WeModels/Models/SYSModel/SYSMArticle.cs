using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSMArticleSearch:BaseSearch
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 时间类型    today/history
        /// </summary>
        public string dayType { get; set; }
    }
    public partial class SYSMArticle
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool ToDels(int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("DELETE FROM [SYSMArticle] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        /// <summary>
        /// 文章内容保存到草稿箱
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public static int SaveTmpByID(int id,string tmp)
        {
            string strSql = "UPDATE [SYSMArticle] SET Tmp=@Tmp WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@Tmp",tmp)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
