using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class InStockScaleShow
    {
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string SmallCode { get; set; }
        public string AntiCode { get; set; }
        public int IntoTime { get; set; }
        public string B_UserID { get; set; }
        public string IntoOrderNo { get; set; }
        public string keyword { get; set; }
        public string ProductImg { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public decimal kw { get; set; }
        public static InStockScaleShow GetSmallScaleListcode(string code)
        {
            string SqlStr = "SELECT Product.ProductName,Product.kw kw FROM ScaleInStoke left join Product on ScaleInStoke.P_ID=Product.ProductNumber WHERE AntiCode=@AntiCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@AntiCode",code)
             };
            return DAL.EntityDataHelper.LoadData2Entity<InStockScaleShow>(SqlStr, Parameter);
        }
    }
}
