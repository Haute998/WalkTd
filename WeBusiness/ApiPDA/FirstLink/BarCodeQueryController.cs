using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/BarCodeQuery")]
    public class BarCodeQueryController : ApiBaseController
    {
        [Route("Get")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult Get(string barcode)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Scale> ScaleList = Scale.GetBigCodeInfo(barcode);

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetMiddleCodeInfo(barcode);
                    if (ScaleList.Count == 0)
                    {
                        ScaleList = Scale.GetSmallCodeInfo(barcode);
                    }
                }

                if (ScaleList.Count > 0)
                {
                    result.data = ScaleList;
                    result.message = "成功";
                    result.success = true;
                }
                else
                {
                    result.message = "失败！编码不存在。";
                    result.success = false;
                }
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("中大标验证出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }
}
