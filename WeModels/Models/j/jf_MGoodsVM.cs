using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_MGoodsVM
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        public jf_Goods Goods { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int cnt { get; set; }

        public void LoadGoodsVMByGoodsID(int id)
        {
            Goods = jf_Goods.GetEntityByID(id);
        }
    }
}
