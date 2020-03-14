using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_UserSearch : BaseSearch
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        public string username { get; set; }
        /// <summary>
        ///名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 多条件查询
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 外键级别
        /// </summary>
        public int C_UserTypeID { get; set; }
    }
    public class SupplierSearch : BaseSearch
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        ///名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 多条件查询
        /// </summary>
        public string keyword { get; set; }
    }
}
