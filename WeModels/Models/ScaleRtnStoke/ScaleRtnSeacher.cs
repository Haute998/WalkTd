using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class ScaleRtnSeacher : BaseSearch
    {
        public string keyword { get; set; }

        public string OrderNo { get; set; }
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string SmallCode { get; set; }
        public string ProducctNo { get; set; }
        public int ReturnTime { get; set; }
        public string Consignee { get; set; }
    }
}
