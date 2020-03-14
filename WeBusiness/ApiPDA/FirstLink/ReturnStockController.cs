using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/ReturnStock")]
    public class ReturnStockController : ApiBaseController
    {
        /// <summary>
        /// 小标退货
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("SmallCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult SmallCode(string code, string orderno = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                Scale scale = Scale.GetScaleForSmall(code);

                if (scale != null)
                {
                    if (scale.IsOut)      // 是否已经关联
                    {
                        scale.IsOut = false;
                        scale.StateID = 9;
                        scale.UpdateByID();

                        ScaleRtnStoke scaleRtn = new ScaleRtnStoke();
                        scaleRtn.SmallCode = scale.SmallCode;
                        scaleRtn.OrderNo = orderno;
                        scaleRtn.OperaUser = PdaUser.PUserName;
                        scaleRtn.IsPDAOpera = true;
                        scaleRtn.RtnWay = 1;
                        scale.SmallReturnStock(scaleRtn);

                        result.message = "成功";
                        result.success = true;
                    }
                    else if (scale.StateID == 9)
                    {
                        result.message = "重复退货";
                        result.success = false;
                    }
                    else
                    {
                        result.message = "条码未出货";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }

                PDALog.Write("小标退货", "小标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0},orderno:{1}", code, orderno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("小标退货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 中标退货
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("MiddleCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult MiddleCode(string code, string orderno = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                List<Scale> scale = Scale.GetScaleForMiddle(code);

                if (scale.Count > 0)
                {
                    Scale scaleDeal = scale.Where(sca => (sca.IsOut == true || sca.StateID == 9) && sca.OutWay > 1).FirstOrDefault();
                    if (scaleDeal != null)
                    {
                        if (scaleDeal.IsOut)
                        {
                            ScaleRtnStoke scaleRtn = new ScaleRtnStoke();
                            scaleRtn.MiddleCode = scaleDeal.MiddleCode;
                            scaleRtn.OrderNo = orderno;
                            scaleRtn.OperaUser = PdaUser.PUserName;
                            scaleRtn.IsPDAOpera = true;
                            scaleRtn.RtnWay = 2;
                            scaleDeal.MiddleReturnStock(scaleRtn);

                            result.message = "成功";
                            result.success = true;
                        }
                        else
                        {
                            result.message = "重复退货";
                            result.success = false;
                        }
                    }
                    else
                    {
                        result.message = "条码未出货";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }

                PDALog.Write("中标退货", "中标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0},orderno:{1}", code, orderno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("中标退货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 大标退货
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("BigCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult BigCode(string code, string orderno = "")
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Scale> scale = Scale.GetScaleForBig(code);

                if (scale.Count>0)
                {
                    Scale scaleDeal = scale.Where(sca => (sca.IsOut == true || sca.StateID == 9) && sca.OutWay == 3).FirstOrDefault();

                    if (scaleDeal != null)
                    {
                        if (scaleDeal.IsOut)      // 是否已经出货
                        {
                            ScaleRtnStoke scaleRtn = new ScaleRtnStoke();
                            scaleRtn.BigCode = scaleDeal.BigCode;
                            scaleRtn.OrderNo = orderno;
                            scaleRtn.OperaUser = PdaUser.PUserName;
                            scaleRtn.IsPDAOpera = true;
                            scaleRtn.RtnWay = 3;
                            scaleDeal.BigReturnStock(scaleRtn);

                            result.message = "成功";
                            result.success = true;
                        }
                        else
                        {
                            result.message = "重复退货";
                            result.success = false;
                        }
                    }
                    else
                    {
                        result.message = "条码未出货";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }

                PDALog.Write("大标退货", "大标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0},orderno:{1}", code, orderno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("大标退货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        [Route("OrderNo")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult OrderNo(string orderno)
        {
            RequestResult result = new RequestResult();

            try
            {
                bool IsOK=Scale.CheckOutOrderNo(orderno);
                if (IsOK)
                {
                    Scale scaleDeal = new Scale();

                    ScaleRtnStoke scaleRtn = new ScaleRtnStoke();
                    scaleRtn.OrderNo = orderno;
                    scaleRtn.OperaUser = PdaUser.PUserName;
                    scaleRtn.IsPDAOpera = true;
                    scaleDeal.OrderNoReturnStock(scaleRtn);

                    result.message = "成功";
                    result.success = true;
                }
                else
                {
                    result.message = "未检测到出货订单！";
                    result.success = false;
                }
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("查询出货订单出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }
}
