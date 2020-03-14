using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WeModels
{
    public partial class C_UserStock
    {
        /// <summary>
        /// 读取代理库存
        /// </summary>
        public static List<Stock> GetC_UserEntitysAll(string UserName)
        {
            //string strSql = "SELECT TOP 100000 UserName,ProductNo,Stock FROM [C_UserStock] where UserName=@UserName";

            string strSql = "select * from(select p.ProductName,p.ProductImg as ProductImgUrl,s.Consignee,p.ProductNumber,Count(SmallCode) count " +
                            "from Product as p left join ScaleOutStoke as s on s.ProductNo=p.ProductNumber and s.State<>'禁用' and s.Consignee=@UserName " +
                            "group by s.Consignee,p.ProductNumber,p.ProductName,p.ProductImg)def";

            SqlParameter[] paramters = { new SqlParameter("@UserName", UserName) };
            return DAL.EntityDataHelper.FillData2Entities<Stock>(strSql, paramters);
        }

        /// <summary>
        /// 获取下级代理所有库存统计
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<Stock> GetAgentStockAll(string UserName, string keyword)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where += string.Format(" and Consignee+Name+ProductNo+ProductName like '%{0}%'", Common.Filter(keyword));
            }

            string strSql = "select COUNT(*) count,s.Consignee Consignee, p.ProductName ProductName,p.ProductImg ProductImgUrl,c.Name Name,ProductNumber "+
                            "from ScaleOutStoke  s left join Product p on s.ProductNo=p.ProductNumber left join C_User c on s.Consignee=c.UserName "+
                            "where s.State='启用' and s.Shipper=@UserName " +
                            "group by s.Consignee,s.ProductNo,p.ProductName,p.ProductImg,c.Name,ProductNumber";

            SqlParameter[] paramters = { new SqlParameter("@UserName", UserName) };
            return DAL.EntityDataHelper.FillData2Entities<Stock>(strSql, paramters);
        }
    }
}
