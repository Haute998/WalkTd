using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public  class C_Consumer_fb
    {
        public string name { get; set; }
        public string value { get; set; }
        public static List<C_Consumer_fb> Getfbs(string type)
        {
            string strSql = "select province name,CONVERT(varchar,count(id)) value from C_Consumer where [Type]='" + type + "' group by province";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<C_Consumer_fb>(strSql, paramters);
        }
    }
}
