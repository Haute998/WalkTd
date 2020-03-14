using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class ProductPriceVM:Product
    {
        /// <summary>
        /// 代理价格
        /// </summary>
        public decimal Price_agent { get; set; }

        /// <summary>
        /// 获取产品价格信息
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="UserTypeID"></param>
        /// <returns></returns>
        public static ProductPriceVM getInfoByProductID(int ProductID, int UserTypeID) 
        {
            string strSql = @"select Product.*,ISNULL(Product_Lever.Price,Product.Price) Price_agent
                              from Product 
                              left join Product_Lever on Product.ProductID=Product_Lever.ProductID 
                              and Product_Lever.UserTypeID=@UserTypeID where Product.ProductID=@ProductID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserTypeID", UserTypeID),
                                                             new System.Data.SqlClient.SqlParameter("@ProductID", ProductID)};

            return DAL.EntityDataHelper.LoadData2Entity<ProductPriceVM>(strSql, paramters);
        }
    }
}
