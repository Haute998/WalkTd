using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.C_UserModel
{
    public class C_User_TypeVMSearch:BaseSearch
    {
        /// <summary>
        /// 代理类别
        /// </summary>
        public int userTypeID { get; set; }
        public int ID { get; set; }
    }
    public class C_User_TypeVM:C_User
    {
        /// <summary>
        /// 代理级别ID
        /// </summary>
        public int userTypeID { get; set; }
        /// <summary>
        /// 代理级别名称
        /// </summary>
        public string userTypeName { get; set; }
  
    }
}
