using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/Unboxing")]
    public class UnboxingController : ApiBaseController
    {
        /// <summary>
        /// 小标折箱
        /// </summary>
        /// <param name="barcode">小标或中标</param>
        /// <returns></returns>
        [Route("TearDownSmallAndMiddle")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult TearDownSmallAndMiddle(string barcode)
        {
            return null;
        }

        /// <summary>
        /// 中标拆箱
        /// </summary>
        /// <param name="barcode">中标或大标</param>
        /// <returns></returns>
        public RequestResult TearDownMiddleAndBig(string barcode)
        {   
            return null;
        }

        /// <summary>
        /// 小标折箱查询
        /// </summary>
        /// <param name="barcode">小标或中标</param>
        /// <returns></returns>
        [Route("TearDownSmallQuery")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult TearDownSmallQuery(string barcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<QueryScale> ListQueryScale = new List<QueryScale>();
                List<Scale> ScaleList = Scale.GetMiddleCodeInfo(barcode);

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetSmallCodeInfo(barcode);
                }

                if (ScaleList.Count > 0)
                {
                    foreach (Scale scale in ScaleList)
                    {
                        QueryScale qscale = new QueryScale();
                        qscale.BigCode = scale.BigCode;
                        qscale.MiddleCode = scale.MiddleCode;
                        qscale.SmallCode = scale.SmallCode;
                        qscale.LinkBigDate = CommonFunc.GetStringFromTimestamp(scale.LinkBigTime);
                        qscale.LinkMidDate = CommonFunc.GetStringFromTimestamp(scale.LinkMidTime);
                        qscale.Status = scale.IsLinkMid ? "可拆解" : "未装箱";
                        qscale.CanRemove = scale.IsLinkMid ? true : false;

                        switch (scale.StateID)
                        {
                            case 0:
                                qscale.Status = "未装箱";
                                qscale.CanRemove = false;
                                break;
                            case 4:
                                qscale.Status = "已禁用";
                                qscale.CanRemove = false;
                                break;
                            case 7:
                                qscale.Status = "已出货";
                                qscale.CanRemove = false;
                                break;
                        }

                        ListQueryScale.Add(qscale);
                    }

                    result.data = ListQueryScale;
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
                DAL.Log.Instance.Write("小标折箱查询出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 中标折箱查询
        /// </summary>
        /// <param name="barcode">小标或中标</param>
        /// <returns></returns>
        [Route("TearDownMiddleQuery")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult TearDownMiddleQuery(string barcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<QueryScale> ListQueryScale = new List<QueryScale>();
                List<Scale> ScaleList = Scale.GetBigCodeUnboxingQuery(barcode);

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetMiddleCodeUnboxingQuery(barcode);
                }

                if (ScaleList.Count > 0)
                {
                    foreach (Scale scale in ScaleList)
                    {
                        QueryScale qscale = new QueryScale();
                        qscale.BigCode = scale.BigCode;
                        qscale.MiddleCode = scale.MiddleCode;
                        qscale.SmallCode = scale.SmallCode;
                        //qscale.LinkBigDate = CommonFunc.GetStringFromTimestamp(scale.LinkBigTime);
                        //qscale.LinkMidDate = CommonFunc.GetStringFromTimestamp(scale.LinkMidTime);
                        qscale.SmallQty = scale.SmallQty;
                        qscale.Status = scale.IsLinkBig ? "可拆解" : "未装箱";
                        qscale.CanRemove = scale.IsLinkBig ? true : false;

                        switch (scale.StateID)
                        {
                            case 0:
                                qscale.Status = "未装箱";
                                qscale.CanRemove = false;
                                break;
                            case 2:
                                qscale.Status = "中标未装箱";
                                qscale.CanRemove = false;
                                break;
                            case 4:
                                qscale.Status = "已禁用";
                                qscale.CanRemove = false;
                                break;
                            case 7:
                                qscale.Status = "已出货";
                                qscale.CanRemove = false;
                                break;
                        }

                        bool IsCan = true;
                        for (int k = 0; k < ListQueryScale.Count; k++)
                        {
                            if (ListQueryScale[k].MiddleCode == qscale.MiddleCode)
                            {
                                if (ListQueryScale[k].Status == "已出货" || qscale.Status == "已出货")
                                {
                                    ListQueryScale[k].Status = "部分已出货";
                                    ListQueryScale[k].CanRemove = false;
                                    IsCan = false;
                                }
                            }
                        }

                        if (IsCan) ListQueryScale.Add(qscale);
                    }

                    result.data = ListQueryScale;
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
                DAL.Log.Instance.Write("中标折箱查询出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }
    public class QueryScale
    {
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string SmallCode { get; set; }
        public string LinkMidDate { get; set; }
        public string LinkBigDate { get; set; }
        public string Status { get; set; }
        public bool CanRemove { get; set; }
        public int SmallQty { get; set; }
    }
}
