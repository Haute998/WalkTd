using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_lpOrderShowVM
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ContactMobile { get; set; }
        /// <summary>
        /// 快递名
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string PostNo { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime PostDat { get; set; }
    }
}
