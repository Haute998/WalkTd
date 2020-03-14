using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Supplier
    {
        public static List<Supplier> GetEntitysAll1()
        {
            string strSql = "SELECT * FROM [Supplier]";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<Supplier>(strSql, paramters);
        }
    }
}
