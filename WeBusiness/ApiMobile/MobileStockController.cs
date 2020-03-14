using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;
using WeModels.WxModel;
using WeModels.Models.C_UserModel;

namespace WeBusiness.ApiMobile
{
    [RoutePrefix("ApiMobile/MobileStock")]
    public class MobileStockController : ApiBaseMobileController
    {
        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUserStock()
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Stock> UserStockList = C_UserStock.GetC_UserEntitysAll(MobileUser.UserName);

                for (int i = 0; i < UserStockList.Count; i++)
                {
                    UserStockList[i].ProductImgUrl = WeConfig.b_domain + UserStockList[i].ProductImgUrl;
                }

                result.data = UserStockList;
                result.message = "成功";
                result.success = true;
            }
            catch(Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAgentStock(string keyword="")
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Stock> AgentStockList = C_UserStock.GetAgentStockAll(MobileUser.UserName, keyword);

                for (int i = 0; i < AgentStockList.Count; i++)
                {
                    AgentStockList[i].ProductImgUrl = WeConfig.b_domain + AgentStockList[i].ProductImgUrl;
                }

                result.data = AgentStockList;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }

            return result;
        }
    }
}
