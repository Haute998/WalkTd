using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class RtnStockCensus
    {
        public string OrderNo { get; set; }
        public int ReturnTime{get;set;}
        public string BigCode{get;set;}
        public string MiddleCode{get;set;}
        public string ProductNumber{get;set;}
        public string ProductName{get;set;}
        public string ProductImg{get;set;}
        public string OperaUser{get;set;}
        public string PRealName{get;set;}
        public int SmallCount { get; set; }
        public string Shipper { get; set; }
        public string Name { get; set; }
        public string Consignee { get; set; }

    }
    public class RtnStockCensusShow : BaseSearch
    {
        public string keyword { get; set; }
        public string RtnCK { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }
        public string ProductNumber { get; set; }
        public string OrderNo { get; set; }
    }

    public class RtnStockSeacher : BaseSearch
    {
        public string keyword { get; set; }
        public int ReturnTime { get; set; }
        public string OrderNo { get; set; }
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string ProductNo { get; set; }
        public int RtnWay { get; set; }
        public string BarCode { get; set; }
    }
}
