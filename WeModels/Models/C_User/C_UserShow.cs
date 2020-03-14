using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_UserShow
    {
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 微信账号
        /// </summary>
        public string wxNo { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string Card { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 授权编号
        /// </summary>
        public string Introducer { get; set; }
        /// <summary>
        /// 级别名称
        /// </summary>
        public string LevelName { get; set; }
        /// <summary>
        /// 上级级别
        /// </summary>
        public string ChiefLevel { get; set; }
        /// <summary>
        /// 上级姓名
        /// </summary>
        public string ChiefName { get; set; }
        /// <summary>
        /// 上级电话
        /// </summary>
        public string ChiefPhone { get; set; }
        /// <summary>
        /// 上级外键
        /// </summary>
        public int Chief { get; set; }
        /// <summary>
        /// 级别外键
        /// </summary>
        public int C_UserTypeID { get; set; }
        /// <summary>
        /// 图像
        /// </summary>
        public string PortraitUrl { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime DatCreate { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime DatVerify { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string CardUrl { get; set; }
        /// <summary>
        /// 推荐人姓名
        /// </summary>
        public string IName { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        public string Identifier { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string Area { get; set; }
        public string WxQRCode { get; set; }
    }
}
