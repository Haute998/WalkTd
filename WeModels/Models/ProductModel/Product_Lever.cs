using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Product_Lever
    {
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static int EditPriceByID(int id, decimal Price)
        {
            string strSql = "UPDATE [Product_Lever] SET Price=@Price WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@Price",Price)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int EditPriceByProductIDAndUserTypeID(int ProductID, int UserTypeID, decimal Price)
        {
            string strSql = "UPDATE [Product_Lever] SET Price=@Price WHERE ProductID=@ProductID and UserTypeID=@UserTypeID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ProductID", ProductID),
                new System.Data.SqlClient.SqlParameter("@UserTypeID", UserTypeID),
                new System.Data.SqlClient.SqlParameter("@Price",Price),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="UserTypeID"></param>
        /// <returns></returns>
        public static Product_Lever GetByProductIDAndUserTypeID(int ProductID, int UserTypeID)
        {
            string strSql = "SELECT top 1 * FROM [Product_Lever] WHERE ProductID=@ProductID and UserTypeID=@UserTypeID order by ID desc";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ProductID", ProductID),
                                                                 new System.Data.SqlClient.SqlParameter("@UserTypeID", UserTypeID)
                                                             };

            return DAL.EntityDataHelper.LoadData2Entity<Product_Lever>(strSql, paramters);
        }
    }
}
