using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SYSMNotify
    {
        /// <summary>
        /// 获取包含排序
        /// </summary>
        /// <returns></returns>
        public static List<SYSMNotify> GetBySort()
        {
            string strSql = "SELECT * FROM [SYSMNotify] order by Sort desc";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSMNotify>(strSql, paramters);
        }
        /// <summary>
        /// 获取前5个公告
        /// </summary>
        /// <returns></returns>
        public static List<SYSMNotify> GetTop5BySort()
        {
            string strSql = "SELECT top 5 * FROM [SYSMNotify] order by Sort desc";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSMNotify>(strSql, paramters);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = "UPDATE [SYSMNotify] SET Title=@Title,Url=@Url,DatEdit=@DatEdit,OperEdit=@OperEdit,Sort=@Sort WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@Title",_title),
                new System.Data.SqlClient.SqlParameter("@Url",_url),
                new System.Data.SqlClient.SqlParameter("@DatEdit",DateTime.Now),
                new System.Data.SqlClient.SqlParameter("@OperEdit",_operedit),
                new System.Data.SqlClient.SqlParameter("@Sort",_sort),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
