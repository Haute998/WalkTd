using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/Order")]
    public class OrderController : ApiBaseController
    {
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAllList(int pageindex, int pagesize, string orderno = "")
        {

            RequestResult result = new RequestResult();
            try
            {
                int totalCount = 0;
                List<CustomerOrder> ListOrder = Order.GetHQPageNotOrderAll(pageindex, pagesize, orderno, out totalCount);

                result.data = ListOrder;
                result.pages = pageindex;
                result.total = totalCount;
                result.message = "成功";
                result.success = true;

                PDALog.Write("获取所有订单", "获取", "", PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("pageindex:{0},pagesize:{1},orderno:{2}", pageindex, pagesize, orderno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("客户查询出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }
    }
}
