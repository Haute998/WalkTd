using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class MainShow
    {
        /// <summary>
        /// 代理人数
        /// </summary>
        public int CustomerCount { get; set; }
        /// <summary>
        /// 入库条码
        /// </summary>
        public int InScaleCount { get; set; }
        /// <summary>
        /// 出货条码
        /// </summary>
        public int OutScaleCount { get; set; }
        /// <summary>
        /// 剩余条码
        /// </summary>
        public int ScaleCount { get; set; }
        /// <summary>
        /// 能抽奖的条码
        /// </summary>
        public int CanLotteryCount { get; set; }
    }
}
