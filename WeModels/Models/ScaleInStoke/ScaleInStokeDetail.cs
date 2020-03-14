using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models
{
    public class ScaleInStokeDetail
    {
        public int ID { get; set; }
        public string IntoOrderNo{get;set;}
        public string BigCode{get;set;}
        public string MiddleCode{get;set;}
        public string SmallCode{get;set;}
        public int IntoTime{get;set;}
        public string ProductImg{get;set;}
        public string ProductNumber{get;set;}
        public string ProductName{get;set;}
        public string IntoPDAUser { get; set; }
        public string PRealName { get; set; }
    }
}
