using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_UserCartCalModel
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int totalCnt { get; set; }
        /// <summary>
        /// 总价格
        /// </summary>
        public decimal totalPrice { get; set; }

        public static C_UserCartCalModel GetList(string C_UserName, int UserTypeID)
        {
            string strSql = @"select ISNULL(SUM(C_UserCart.GetCnt),0) totalCnt,
                                     ISNULL(SUM(ISNULL(Product_Lever.Price,Product.Price)*ISNULL(C_UserCart.GetCnt,0)),0) totalPrice
                              from C_UserCart 
                                    left join Product on C_UserCart.GoodsID=Product.ProductID 
                                    left join Product_Lever 
                                    on Product.ProductID=Product_Lever.ProductID 
                                    and Product_Lever.UserTypeID=@UserTypeID
                                    where C_UserCart.C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserTypeID", UserTypeID),
                                                             new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName)};

            return DAL.EntityDataHelper.LoadData2Entity<C_UserCartCalModel>(strSql, paramters);
        }

    }
}
