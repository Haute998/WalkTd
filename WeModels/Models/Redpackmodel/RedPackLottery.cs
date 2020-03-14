using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class RedPackLottery
    {
        public static RedPackLottery GetEntityByUserName(string UserName)
        {
            string strSql = "SELECT ID,Code,UserName,Addtime FROM [RedPackLottery] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<RedPackLottery>(strSql, paramters);
        }
        public static RedPackLottery GetEntityByUserName2(string UserName)
        {
            string strSql = "SELECT RedPackLottery.ID,RedPackLottery.Code,RedPackLottery.UserName,RedPackLottery.Addtime " +
                            "FROM [RedPackLottery] left join C_WxUser on RedPackLottery.UserName=C_WxUser.UserName " +
                            "WHERE RedPackLottery.UserName=@UserName and C_WxUser.Email<>'T' ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<RedPackLottery>(strSql, paramters);
        }
    }
}
