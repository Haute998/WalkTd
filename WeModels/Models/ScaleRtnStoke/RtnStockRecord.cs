using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class RtnStockRecord
    {
        public int Qty { get; set; }
        public string Shipper { get; set; }
        public string OrderNo { get; set; }
        public string Name { get; set; }
        public int ReturnTime { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string ProductNumber { get; set; }
        public string Consignee { get; set; }
    }
}
