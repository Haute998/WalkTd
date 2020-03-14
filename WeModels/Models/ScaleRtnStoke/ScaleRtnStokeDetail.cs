using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models
{
    public class ScaleRtnStokeDetail
    {
        public string OrderNo{get;set;}
        public int RtnWay{get;set;}
        public string BarCode{get;set;}
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string SmallCode { get; set; }
        public string ProducctNo{get;set;}
        public int ReturnTime{get;set;}
        public string OperaUser{get;set;}
        public int BarCount{get;set;}
        public string ProductImg{get;set;}
        public string ProductNumber{get;set;}
        public string ProductName{get;set;}
        public string PUserName{get;set;}
        public string PRealName{get;set;}
        public string Name { get; set; }

        public int OutTime { get; set; }
        public string OutOrderNo { get; set; }
    }
}
