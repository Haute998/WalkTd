using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class PDAParamEasy
    {
        /// <summary>
        /// 
        /// </summary>
        public int LinkWay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLinkProduct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLinkSupplier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IntoWay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIntoProduct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIntoSupplier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OutWay { get; set; }

        public bool IsOutProduct { get; set; }
        public bool MiddlePacking { get; set; }
    }
}
