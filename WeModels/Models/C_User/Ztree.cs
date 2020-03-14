using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class Ztree
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool isParent { get; set; }
    }
    public class RebateZree
    {
        public string id { get; set; }

        public string name { get; set; }

        public bool isParent { get; set; }
    }

    public class SearchZtree : C_UserShow
    {
        public int ChiefCount { get; set; }
    }
}
