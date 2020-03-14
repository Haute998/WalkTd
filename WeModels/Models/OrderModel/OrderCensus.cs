using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class OrderCensus
    {
        public static List<OrderCensusShow> GetLastMonthVal(string where)
        {
            string SqlStr = "SELECT Sum(SumPrice) SumPrice FROM [ORDER] WHERE OrderState='已发货'" + where;
            System.Data.SqlClient.SqlParameter[] SqlParamater = null;
            return DAL.EntityDataHelper.FillData2Entities<OrderCensusShow>(SqlStr, SqlParamater);
        }
    }
    public class OrderCensusShow
    {
        public decimal SumPrice { get; set; }
    }


}
