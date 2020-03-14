using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.OrderModel;

namespace WeModels.WxModel
{
    public class WxTempMsg
    {
        //以下行业   IT科技 - 互联网|电子商务


        /// <summary>
        /// 服务完成通知   OPENTM401114907
        /// </summary>
        /// <param name="username"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="dat"></param>
        /// <param name="remark"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SendForServiceFinish(string username,string title,string type, DateTime dat,string remark,string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return "error:标题不能为空";
                }
                if (string.IsNullOrWhiteSpace(type))
                {
                    return "error:服务类型不能为空";
                }
                C_UserWxInfo wxinfo = C_UserWxInfo.GetInfoByC_UserName(username);
                if (wxinfo == null)
                {
                    return "error:客户不存在或不是微信用户";
                }
                List<dataParam> datas = new List<dataParam>();
                dataParam datafirst = new dataParam();
                datafirst.name = "first";
                datafirst.value = title;
                datafirst.color = "#3897ff";
                datas.Add(datafirst);

                //服务类型
                dataParam keyword1 = new dataParam();
                keyword1.name = "keyword1";
                keyword1.value = type;
                datas.Add(keyword1);

                //时间
                dataParam keyword2 = new dataParam();
                keyword2.name = "keyword2";
                keyword2.value = dat.ToString("yyyy年MM月dd日 HH:mm");
                datas.Add(keyword2);

                //备注
                dataParam dataRemark = new dataParam();
                dataRemark.name = "remark";
                dataRemark.value = remark;
                datas.Add(dataRemark);
                string templateID = BaseParameters.GetVal("服务完成通知");
                SendTempMsgRtn rtn = new SendTempMsgRtn();
                rtn = SendTempMsg(wxinfo.openid, templateID, url, datas);

                if (rtn.errcode == "0" && rtn.errmsg.ToLower() == "ok")
                {
                    return "ok";
                }
                else
                {
                    Log.Instance.Write("errcode:" + rtn.errcode + ",errmsg:" + rtn.errmsg, "SendForServiceFinish_Error");
                    return "error:" + rtn.errmsg;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "SendForServiceFinish_Error");
                return "error:异常";
            }

        }





        /// <summary>
        /// 小伙伴被您邀请成功的提醒
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="dat"></param>
        /// <returns></returns>
        public static string SendForNewPartner(string C_UserName,DateTime dat)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(C_UserName))
                {
                    return "error:客户编号不能为空";
                }
                C_UserWxInfo WXInfo = C_UserWxInfo.GetInfoByC_UserName(C_UserName);
                if (WXInfo == null)
                {
                    return "error:该客户不存在或没有微信";
                }
                List<dataParam> datas = new List<dataParam>();
                dataParam datafirst = new dataParam();
                datafirst.name = "first";
                datafirst.value = "您的团队加入了新的成员";
                datafirst.color = "#3897ff";
                datas.Add(datafirst);

                //姓名
                dataParam keyword1 = new dataParam();
                keyword1.name = "keyword1";
                keyword1.value = WXInfo.nickname;
                datas.Add(keyword1);

                //时间
                dataParam keyword2 = new dataParam();
                keyword2.name = "keyword2";
                keyword2.value = dat.ToString("yyyy年MM月dd日 HH:mm");
                datas.Add(keyword2);

                dataParam dataRemark = new dataParam();
                dataRemark.name = "remark";
                dataRemark.value = "恭喜！您的朋友" + WXInfo.nickname + "成功加入您的团队！";
                datas.Add(dataRemark);

                string templateID = BaseParameters.GetVal("成员加入提醒");
                string domainName = WeConfig.wx_domain;
                string url = domainName + "/MyCenter/MyPartners";

                SendTempMsgRtn rtn = new SendTempMsgRtn();
                rtn = SendTempMsg(WXInfo.openid, templateID, url, datas);
                if (rtn.errcode == "0" && rtn.errmsg.ToLower() == "ok")
                {
                    return "ok";
                }
                else
                {
                    Log.Instance.Write("errcode:" + rtn.errcode + ",errmsg:" + rtn.errmsg, "SendForNewPartner_Error");
                    return "error:" + rtn.errmsg;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "SendForNewPartner_Error");
                return "error:异常";
            }

        }






        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="toOpenID"></param>
        /// <param name="templateID"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SendTempMsgRtn SendTempMsg(string toOpenID, string templateID, string url, List<dataParam> data)
        {
            string dataStr = "";

            foreach (var item in data)
            {
                dataStr += "\"" + item.name + "\": {\"value\": \"" + item.value + "\",\"color\": \"" + item.color + "\"},";
            }
            dataStr = dataStr.TrimEnd(',');

            string jsonParam = "{\"touser\": \"" + toOpenID + "\",\"template_id\": \"" + templateID + "\",\"url\": \"" + url + "\",\"data\": {" + dataStr + "}}";
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            return JsonConvert.DeserializeObject<SendTempMsgRtn>(VariousApi.SendTempMsg(jsonParam));

        }

        /// <summary>
        /// data参数
        /// </summary>
        public class dataParam
        {
            /// <summary>
            /// 参数名
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 参数值
            /// </summary>
            public string value { get; set; }
            /// <summary>
            /// 字体颜色
            /// </summary>
            public string color { get; set; }
        }

        /// <summary>
        /// 发送结果
        /// </summary>
        public class SendTempMsgRtn
        {
            /// <summary>
            /// 成功返回0
            /// </summary>
            public string errcode { get; set; }
            /// <summary>
            /// 成功返回ok
            /// </summary>
            public string errmsg { get; set; }
            public string msgid { get; set; }
        }
    }
}
