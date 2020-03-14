using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDA
    {
        public string Supplier { get; set; }
        /// <summary>
        /// 根据主键获取对象
        /// （当数据库表第一行是int自增主键时生成此方法）
        /// </summary>
        public static PDA GetEntityByKeys(string dc)
        {
            string strSql = "SELECT * FROM [PDA] WHERE (Keys=@DC or SN=@DC) and State='启用'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@DC", dc) };

            return DAL.EntityDataHelper.LoadData2Entity<PDA>(strSql, paramters);
        }

        public static List<PDA> GetEntitysAllMore()
        {
            string strSql = "SELECT TOP 100000 a.ID,Keys,SN,Code,State,a.Address,a.SupplierId,b.Name as Supplier FROM [PDA] as a left join [Supplier] as b on a.SupplierId=b.ID";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<PDA>(strSql, paramters);
        }

        public static PDA GetIsByKeys(string Keys)
        {
            string strSql = "SELECT * FROM [PDA] WHERE Keys=@Keys";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Keys", Keys) };

            return DAL.EntityDataHelper.LoadData2Entity<PDA>(strSql, paramters);
        }

        public static List<PDA> SeacherPDA(string keyword)
        {            
            string strSQL = "select * from PDA ";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                strSQL += string.Format("where Keys like '{0}%'", keyword);
            }

            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<PDA>(strSQL, paramters);
        }
    }
}
