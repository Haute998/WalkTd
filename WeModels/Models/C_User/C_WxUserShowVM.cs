using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_WxUserShowVM
    {
        public class Condition : BaseSearch
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public string keyword { get; set; }
            /// <summary>
            /// 客户ID
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 客户微信昵称
            /// </summary>
            public string wx_nickname { get; set; }
            /// <summary>
            /// 微信关注状态
            /// </summary>
            public string wx_subscribeStat { get; set; }
            public string DatCreateB { get; set; }
            /// <summary>
            /// 结束时间
            /// </summary>
            public string DatCreateE { get; set; }

            /// <summary>
            /// 客户上级
            /// </summary>
            public string PaterUser { get; set; }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 客户积分
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// 客户上级
        /// </summary>
        public string PaterUser { get; set; }
        /// <summary>
        /// 客户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 是否可用 (黑名单)
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 客户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 客户手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 客户真实姓名
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// 客户微信号
        /// </summary>
        public string WxNo { get; set; }
        /// <summary>
        /// 客户注册时间
        /// </summary>
        public DateTime DatRegister { get; set; }
        /// <summary>
        /// 客户邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 客户头像
        /// </summary>
        public string PortraitUrl { get; set; }
        /// <summary>
        /// 客户微信openID
        /// </summary>
        public string wx_openid { get; set; }
        /// <summary>
        /// 客户微信昵称
        /// </summary>
        public string wx_nickname { get; set; }
        /// <summary>
        /// 客户微信性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public string wx_sex { get; set; }
        /// <summary>
        /// 客户微信头像
        /// </summary>
        public string wx_headimgurl { get; set; }
        /// <summary>
        /// 公众号对客户的备注
        /// </summary>
        public string wx_remark { get; set; }
        /// <summary>
        /// 公众号对客户的分组ID
        /// </summary>
        public string wx_groupid { get; set; }
        /// <summary>
        /// 是否已关注公众号
        /// </summary>
        public bool wx_subscribe { get; set; }
        /// <summary>
        /// 关注公众号时间
        /// </summary>
        public DateTime wx_subscribe_time { get; set; }
        /// <summary>
        /// 客户微信国籍
        /// </summary>
        public string wx_country { get; set; }
        /// <summary>
        /// 客户微信语言
        /// </summary>
        public string wx_language { get; set; }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public static List<C_WxUserShowVM> GetList(int[] cklst)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');
            string sqlStr = string.Format(@"select * from (select a.ID,a.UserName,a.NickName,a.Mobile,a.TrueName,
                                            a.WxNo,a.DatRegister,a.Email,a.PortraitUrl,wx.openid wx_openid,
                                            wx.nickname wx_nickname,wx.sex wx_sex,wx.headimgurl wx_headimgurl,wx.remark wx_remark,
                                            wx.groupid wx_groupid,wx.subscribe wx_subscribe,wx.subscribe_time wx_subscribe_time,
                                            wx.country wx_country,wx.language wx_language 
                                            from C_WxUser as a left join C_UserWxInfo as wx on a.UserName=wx.C_UserName 
                                            where a.ID in ({0})) as userwxvm ", userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<C_WxUserShowVM>(sqlStr, paramters);
        }
    }
}
