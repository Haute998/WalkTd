using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiMobile
{
    [RoutePrefix("ApiMobile/MobileOutStock")]
    public class MobileOutStockController : ApiBaseMobileController
    {
        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult OutStockRecord(int pageindex, int pagesize, string startdate = "", string enddate = "", string keyword = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                int StartTimestamp = startdate != "" ? CommonFunc.GetTimestamp(Convert.ToDateTime(startdate + " 00:00:00")) : 0;
                int EndTimestamp = enddate != "" ? CommonFunc.GetTimestamp(Convert.ToDateTime(enddate + " 23:59:59")) : 0;

                int totalCount = 0;
                List<OutStokeRecord> RecordList=ScaleOutStoke.GetOutStockRecord(MobileUser.UserName, pagesize, pageindex, StartTimestamp, EndTimestamp, keyword, out totalCount);
                for (int i = 0; i < RecordList.Count; i++)
                {
                    RecordList[i].ProductImg = WeConfig.b_domain + RecordList[i].ProductImg;
                }

                result.data = RecordList;
                result.pages = pageindex;
                result.total = totalCount;
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

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult OutStockCount(int pageindex, int pagesize, string startdate = "", string enddate = "", string keyword = "")
        {
            RequestResult result = new RequestResult();
            try
            {   
                int StartTimestamp = startdate != "" ? CommonFunc.GetTimestamp(Convert.ToDateTime(startdate + " 00:00:00")) : 0;
                int EndTimestamp = enddate != "" ? CommonFunc.GetTimestamp(Convert.ToDateTime(enddate + " 23:59:59")) : 0;

                int totalCount = 0;
                List<OutStokeRecord> CountList = ScaleOutStoke.GetOutStockCount(MobileUser.UserName, pagesize, pageindex, StartTimestamp, EndTimestamp, keyword, out totalCount);
                for (int i = 0; i < CountList.Count; i++)
                {
                    CountList[i].ProductImg = WeConfig.b_domain + CountList[i].ProductImg;
                }
                result.data = CountList;
                result.pages = pageindex;
                result.total = totalCount;
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

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult OutStockDetail(int pageindex, int pagesize,string outorderno, string productno, int timestamp, string consignee, string keyword = "")
        {
            RequestResult result = new RequestResult();
            try
            {   
                int totalCount = 0;
                List<ScaleCode_Simple> DetailList = ScaleOutStoke.GetOutStockDetail(MobileUser.UserName, outorderno, productno, pagesize, pageindex, timestamp, consignee, keyword, out totalCount);
                result.data = DetailList;
                result.pages = pageindex;
                result.total = totalCount;
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

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult ScanCode(string barcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<BarCode> CodeList = Scale.GetScanCode(barcode, MobileUser.UserName);

                if (CodeList != null && CodeList.Count > 0)
                {
                    result.data = CodeList;
                    result.message = "正确";
                    result.success = true;
                }
                else
                {
                    result.message = "未找到此条码或不在你的库存中";
                    result.success = false;
                }
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult SmallCodeToOutStock(string outorderno, string agent = "m2000")
        {
            RequestResult result = new RequestResult();
            try
            {
                bool IsOK = true;

                IEnumerable<string> CodeArrayStr;
                Request.Headers.TryGetValues("CodeArrayStr", out CodeArrayStr);

                if (string.IsNullOrWhiteSpace(agent))
                {
                    IsOK = false;
                    result.message = "经销商不能为空";
                    result.success = false;
                }

                if (CodeArrayStr.ToArray() == null || CodeArrayStr.ToArray()[0]=="")
                {
                    IsOK = false;
                    result.message = "正确条码不能为空";
                    result.success = false;
                }

                if (IsOK)
                {
                    string[] codeArray = CodeArrayStr.ToArray()[0].Split(',');
                    string SmallCodeArray = string.Empty;

                    for (int i = 0; i < codeArray.Length; i++)
                    {
                        if (codeArray[i] != "")
                        {
                            if (SmallCodeArray != "") SmallCodeArray += ",";
                            SmallCodeArray += "'" + codeArray[i] + "'";
                        }
                    }

                    if (string.IsNullOrEmpty(SmallCodeArray))
                    {
                        result.message = "出货条码不能为空";
                        result.success = false;
                    }
                    else if (codeArray.Length > 1000)
                    {
                        result.message = "出货条码数量超出最大数量(1000)限制";
                        result.success = false;
                    }
                    else
                    {
                        List<BarCode> SmallCodeList = ScaleOutStoke.GetOutStockID(MobileUser.UserName, SmallCodeArray);

                        string IDSet = "";
                        foreach (BarCode b in SmallCodeList)
                        {
                            if (IDSet != "") IDSet += ",";
                            IDSet += b.ID.ToString();
                        }

                        if (IDSet != "") ScaleOutStoke.ToOutStockAgent(MobileUser.UserName, agent, IDSet, outorderno);

                        result.data = SmallCodeList;
                        result.message = "成功";
                        result.success = true;
                    }
                }
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
