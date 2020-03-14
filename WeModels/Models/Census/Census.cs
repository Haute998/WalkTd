using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class Census
    {
        public string name { get; set; }
        public int[] data { get; set; }
    }
    public class PieCensus
    {
        public string name { get; set; }
        public decimal y { get; set; }
    }
    public class U_PCensus
    {
        public int Count { get; set; }
        public string name { get; set; }
    }
    
    public class C_CountCensus
    {
        public int Count { get; set; }
        public int C_UserTypeID { get; set; }
        public string name { get; set; }
    }

    public class CustomerCountCensus
    {
        public int Count { get; set; }
        public string name { get; set; }
    }

    public class UserCensus
    {
        public string name { get; set; }
        public string data { get; set; }
    }
}
