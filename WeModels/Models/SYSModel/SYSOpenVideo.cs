using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSOpenVideoSearch : BaseSearch
    {
        public string Title { get; set; }

        /// <summary>
        /// 时间类型    today/history
        /// </summary>
        public string dayType { get; set; }
    }
    public partial class SYSOpenVideo
    {
        public static bool ToDels(int[] ids, string mappath)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
                SYSOpenVideo oldimgs = SYSOpenVideo.GetEntityByID(i);
                try
                {
                    string delFile = mappath + oldimgs.VideoUrl;
                    System.IO.File.Delete(delFile);
                }
                catch (Exception ex) { }
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("DELETE FROM [SYSOpenVideo] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
    }
}
