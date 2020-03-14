using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SYSAdv
    {
        /// <summary>
        /// 获取首页宣传图
        /// </summary>
        /// <returns></returns>
        public static List<SYSAdv> GetNavAdvs()
        {
            string strSql = "SELECT TOP 100000 * FROM [SYSAdv] where AdvType=1 order by sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSAdv>(strSql, paramters);
        }
        public static SYSAdv GetNavAdvs2()
        {
            string strSql = "SELECT TOP 1 * FROM [SYSAdv] where 1=1";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.LoadData2Entity<SYSAdv>(strSql, paramters);
        }
        /// <summary>
        /// 获取广告
        /// </summary>
        /// <returns></returns>
        public static SYSAdv GetNavNav()
        {
            string strSql = "SELECT TOP 1 * FROM [SYSAdv] where AdvType=2";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.LoadData2Entity<SYSAdv>(strSql, paramters);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="ext"></param>
        public void AddAdv(string ext)
        {
            ID = InsertAndReturnIdentity();
            ImgUrl = "/images/SYSAdv/Adv_" + ID + ext;
            UpdateByID();
        }
    }
}
