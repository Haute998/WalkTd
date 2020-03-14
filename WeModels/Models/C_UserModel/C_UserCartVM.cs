using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.C_UserModel
{
    public class C_UserCartVM : C_UserCart
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductNumber { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 产品主图
        /// </summary>
        public string ProductImg { get; set; }
        /// <summary>
        /// 产品状态  已上架/未上架
        /// </summary>
        public string States { get; set; }

        /// <summary>
        /// 产品价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 代理价格
        /// </summary>
        public decimal Price_agent { get; set; }

        /// <summary>
        /// 读取客户的购物车
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static List<C_UserCartVM> getCarts(int UserTypeID, string C_UserName)
        {
            string strSql = @"SELECT top 100 C_UserCart.*,Product.ProductNumber,Product.ProductName,
                              Product.ProductImg,Product.Price,Product.States,ISNULL(Product_Lever.Price,Product.Price) Price_agent
                FROM C_UserCart left join Product on C_UserCart.GoodsID=Product.ProductID
                     left join Product_Lever on Product.ProductID=Product_Lever.ProductID and Product_Lever.UserTypeID=@UserTypeID
                where C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserTypeID", UserTypeID),
                                                             new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName)};
            return DAL.EntityDataHelper.FillData2Entities<C_UserCartVM>(strSql, paramters);
        }
    }
}
