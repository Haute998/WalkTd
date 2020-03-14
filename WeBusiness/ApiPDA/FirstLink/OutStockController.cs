using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/OutStock")]
    public class OutStockController : ApiBaseController
    {
        /// <summary>
        /// 小标出货
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("SmallCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult SmallCode(string code, string username, string orderno = "", string pno = "")
        {
            RequestResult result = new RequestResult();

            try
            {

                if (PDAUserMsg.Param.IsOut && PDAUserMsg.Param.SmallOut)
                {
                    if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (C_User.IsSysUser(username))
                    {
                        Scale scale = Scale.GetScaleForSmall(code);

                        if (scale != null)
                        {
                            if (scale.IsOut)
                            {
                                result.message = "重复出货";
                                result.success = false;
                            }
                            else if (!scale.IsInto && PDAUserMsg.Param.IsInto && (PDAUserMsg.Param.ScanProcess == 1 || PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4))
                            {
                                result.message = "未入库";
                                result.success = false;
                            }
                            else if (!string.IsNullOrWhiteSpace(scale.ProductNo) && !string.IsNullOrWhiteSpace(pno) && pno != scale.ProductNo)
                            {
                                result.message = "出货产品错误";
                                result.success = false;
                            }
                            else
                            {
                                scale.IsOut = true;
                                scale.OutPDAUser = PdaUser.PUserName;
                                scale.UserName = username;
                                scale.OutOrderNo = orderno;
                                if (!string.IsNullOrWhiteSpace(pno)) scale.ProductNo = pno;
                                scale.OutTime = CommonFunc.GetNowTimestamp();
                                scale.OutWay = 1;
                                scale.StateID = 7;
                                scale.UpdateByID();

                                ScaleOutStoke scaleOut = new ScaleOutStoke();
                                scaleOut.BigCode = scale.BigCode;
                                scaleOut.SmallCode = scale.SmallCode;
                                scaleOut.MiddleCode = scale.MiddleCode;
                                scaleOut.AntiCode = scale.AntiCode;
                                scaleOut.Shipper = "总部";
                                scaleOut.ProductNo = scale.ProductNo;
                                scaleOut.Consignee = scale.UserName;
                                scaleOut.State = "启用";
                                scaleOut.CreateTime = CommonFunc.GetNowTimestamp();
                                scaleOut.OutOrderNo = orderno;
                                scaleOut.OutWay = 1;
                                scaleOut.InsertAndReturnIdentity();

                                result.message = "成功";
                                result.success = true;
                            }
                        }
                        else
                        {
                            result.code = 304;
                            result.message = "条码不存在";
                            result.success = false;
                        }
                    }
                    else
                    {
                        result.code = 504;
                        result.message = "客户错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此出货功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("小标出货", "小标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},username:{3}", code, pno, orderno, username), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("小标出货错误：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 中标出库
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("MiddleCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult MiddleCode(string code, string username, string orderno = "",string pno="")
        {
            RequestResult result = new RequestResult();
            try
            {
                if (PDAUserMsg.Param.IsOut && PDAUserMsg.Param.MiddleOut)
                {
                    if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (C_User.IsSysUser(username))
                    {
                        List<Scale> scale = Scale.GetScaleForMiddle(code);

                        if (scale.Count > 0)
                        {
                            if (scale.Where(sca => sca.IsOut == false).Count() == 0)
                            {
                                result.message = "重复出货";
                                result.success = false;
                            }
                            else if (scale.Where(sca => sca.IsInto == true).Count() == 0 && PDAUserMsg.Param.IsInto && (PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4))
                            {
                                result.message = "未入库";
                                result.success = false;
                            }
                            else if (!string.IsNullOrWhiteSpace(scale[0].ProductNo) && !string.IsNullOrWhiteSpace(pno) && pno != scale[0].ProductNo)
                            {
                                result.message = "出货产品错误";
                                result.success = false;
                            }
                            else
                            {
                                Scale scaleDeal = scale.Where(sca => sca.IsOut == false).FirstOrDefault();
                                scaleDeal.IsOut = true;
                                scaleDeal.OutPDAUser = PdaUser.PUserName;
                                scaleDeal.UserName = username;
                                scaleDeal.OutOrderNo = orderno;
                                if (!string.IsNullOrWhiteSpace(pno)) scaleDeal.ProductNo = pno;
                                scaleDeal.OutTime = CommonFunc.GetNowTimestamp();
                                scaleDeal.StateID = 7;
                                scaleDeal.OutWay = 2;
                                scaleDeal.MiddleOutStock();

                                result.message = "成功";
                                result.success = true;
                            }
                        }
                        else
                        {
                            result.code = 304;
                            result.message = "条码不存在";
                            result.success = false;
                        }
                    }
                    else
                    {
                        result.code = 504;
                        result.message = "客户错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此出货功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("中标出货", "中标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},username:{3}", code, pno, orderno, username), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("中标出货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 大标出库
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("BigCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult BigCode(string code, string username, string orderno = "", string pno = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                if (PDAUserMsg.Param.IsOut && PDAUserMsg.Param.MiddleOut)
                {
                    if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (C_User.IsSysUser(username))
                    {
                        List<Scale> scale = Scale.GetScaleForBig(code);

                        if (scale.Count > 0)
                        {
                            if (scale.Where(sca => sca.IsOut == false).Count() == 0)
                            {
                                result.message = "重复出货";
                                result.success = false;
                            }
                            else if (scale.Where(sca => sca.IsInto == true).Count() == 0 && PDAUserMsg.Param.IsInto && (PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4))
                            {
                                result.message = "未入库";
                                result.success = false;
                            }
                            else if (!string.IsNullOrWhiteSpace(scale[0].ProductNo) && !string.IsNullOrWhiteSpace(pno) && pno != scale[0].ProductNo)
                            {
                                result.message = "出货产品错误";
                                result.success = false;
                            }
                            else
                            {
                                Scale scaleDeal = scale.Where(sca => sca.IsOut == false).FirstOrDefault();
                                scaleDeal.IsOut = true;
                                scaleDeal.OutPDAUser = PdaUser.PUserName;
                                scaleDeal.UserName = username;
                                scaleDeal.OutOrderNo = orderno;
                                if (!string.IsNullOrWhiteSpace(pno)) scaleDeal.ProductNo = pno;
                                scaleDeal.OutTime = CommonFunc.GetNowTimestamp();
                                scaleDeal.StateID = 7;
                                scaleDeal.OutWay = 3;
                                scaleDeal.BigOutStock();

                                result.message = "成功";
                                result.success = true;
                            }
                        }
                        else
                        {
                            result.code = 304;
                            result.message = "条码不存在";
                            result.success = false;
                        }
                    }
                    else
                    {
                        result.code = 504;
                        result.message = "客户错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此出货功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("大标出货", "大标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},username:{3}", code, pno, orderno, username), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("大标出货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 移除小标出货
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("RemoveSmallCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveSmallCode(string code)
        {
            RequestResult result = new RequestResult();
            try
            {
                Scale scale = Scale.GetScaleForSmall(code);

                if (scale != null)
                {
                    if (!scale.IsOut)
                    {
                        result.message = "未出货";
                        result.success = false;
                    }
                    else if (ScaleOutStoke.IsSubOutStock_Small(code))
                    {
                        result.message = "此条码下级已出货";
                        result.success = false;
                    }
                    else
                    {
                        scale.IsOut = false;
                        scale.OutPDAUser = "";// PdaUser.PUserName;
                        scale.OutTime = 0;// CommonFunc.GetNowTimestamp();
                        scale.UserName = "";
                        scale.StateID = 6;
                        scale.UpdateByID();
                        ScaleOutStoke.DeleteOutScaleRtnState(code);     // 移除出货表记录

                        result.message = "成功";
                        result.success = true;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }

                PDALog.Write("撤消出货", "小标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消小标出货出错：" + ex.Message, "PDA上传出错");
            }
            
            return result;
        }

        /// <summary>
        /// 移除中标出货
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("RemoveMiddleCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveMiddleCode(string code)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<Scale> scale = Scale.GetScaleForMiddle(code);

                if (scale.Count > 0)
                {
                    if (scale.Where(sca => sca.IsOut == true && sca.OutWay == 2).Count() == 0)
                    {
                        result.message = "未使用中标出货";
                        result.success = false;
                    }
                    else if (ScaleOutStoke.IsSubOutStock_Middle(code))
                    {
                        result.message = "此条码下级已出货";
                        result.success = false;
                    }
                    else
                    {
                        Scale scaleDeal = scale.Where(sca => sca.IsOut == true && sca.OutWay == 2).FirstOrDefault();
                        scaleDeal.IsOut = false;
                        scaleDeal.OutPDAUser = "";// PdaUser.PUserName;
                        scaleDeal.OutTime = 0;// CommonFunc.GetNowTimestamp();
                        scaleDeal.UserName = "";
                        scaleDeal.StateID = 6;
                        scaleDeal.RemoveMiddleOut();

                        result.message = "成功";
                        result.success = true;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消中标出货出错：" + ex.Message, "PDA上传出错");
            }

            PDALog.Write("撤消出货", "中标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            return result;
        }

        /// <summary>
        /// 移除大标出货
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("RemoveBigCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveBigCode(string code)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<Scale> scale = Scale.GetScaleForBig(code);

                if (scale.Count > 0)
                {
                    if (scale.Where(sca => sca.IsOut == true && sca.OutWay == 3).Count() == 0)
                    {
                        result.message = "未使用大标出货";
                        result.success = false;
                    }
                    else if (ScaleOutStoke.IsSubOutStock_BigCode(code))
                    {
                        result.message = "此条码下级已出货";
                        result.success = false;
                    }
                    else
                    {
                        Scale scaleDeal = scale.Where(sca => sca.IsOut == true && sca.OutWay == 3).FirstOrDefault();
                        scaleDeal.IsOut = false;
                        scaleDeal.OutPDAUser = "";// PdaUser.PUserName;
                        scaleDeal.OutTime = 0;// CommonFunc.GetNowTimestamp();
                        scaleDeal.UserName = "";
                        scaleDeal.StateID = 6;
                        scaleDeal.RemoveBigOut();

                        result.message = "成功";
                        result.success = true;
                    }
                }
                else
                {
                    result.code = 304;
                    result.message = "条码不存在";
                    result.success = false;
                }

                PDALog.Write("撤消出货", "大标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消大标出货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        [Route("RemoveOutBarCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveOutBarCode(string barcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                int type = 3;
                List<Scale> ScaleList = Scale.GetBigCodeInfo(barcode);

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetMiddleCodeInfo(barcode);
                    type = 2;
                }

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetSmallCodeInfo(barcode);
                    type = 1;
                }

                if (ScaleList.Count > 0)
                {
                    switch (type)
                    {
                        case 1:
                            result = RemoveSmallCode(barcode);
                            result.data = "小标";
                            break;
                        case 2:
                            result = RemoveMiddleCode(barcode);
                            if (PDAUserMsg.Param.MiddlePacking) result.data = "中标";
                            else result.data = "大标";
                            break;
                        case 3:
                            result = RemoveBigCode(barcode);
                            result.data = "大标";
                            break;
                    }
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
                DAL.Log.Instance.Write("撤消条码出货出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 出货查询
        /// IntoBarCodeQuery</summary>
        /// <param name="barcode">小标或中标或大标</param>
        /// <returns></returns>
        [Route("OutBarCodeQuery")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult OutBarCodeQuery(string barcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<IntoQueryScale> ListQueryScale = new List<IntoQueryScale>();
                int type = 2;
                List<Scale> ScaleList = Scale.GetBigCodeInfo(barcode);

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetMiddleCodeInfo(barcode);
                    type = 1;
                }

                if (ScaleList.Count == 0)
                {
                    ScaleList = Scale.GetSmallCodeInfo(barcode);
                    type = 1;
                }

                if (ScaleList.Count > 0)
                {
                    if (type == 2)
                    {
                        List<string> BarCodeList = new List<string>();
                        foreach (Scale scale in ScaleList)
                        {
                            if (BarCodeList.Where(m => m == scale.MiddleCode).Count() == 0)
                            {
                                BarCodeList.Add(scale.MiddleCode);
                            }
                        }

                        foreach (string code in BarCodeList)
                        {
                            IntoQueryScale qscale = new IntoQueryScale();
                            qscale.Type = type;
                            qscale.BarCode = code;

                            if (ScaleList.Where(m => m.MiddleCode == code && (m.IsOut != true || m.StateID != 7)).Count() == 0)
                            {
                                qscale.Status = "可撤消";
                                qscale.CanRemove = true;
                            }
                            else
                            {
                                qscale.Status = "不可撤消";
                                qscale.CanRemove = false;
                            }

                            ListQueryScale.Add(qscale);
                        }
                    }
                    else
                    {
                        foreach (Scale scale in ScaleList)
                        {
                            IntoQueryScale qscale = new IntoQueryScale();
                            qscale.Type = type;
                            qscale.BarCode = scale.SmallCode;
                            qscale.Status = scale.IsOut ? "可撤消" : "未出货";
                            qscale.CanRemove = scale.IsOut ? true : false;

                            switch (scale.StateID)
                            {
                                case 0:
                                case 2:
                                case 3:
                                case 6:
                                    qscale.Status = "未出货";
                                    qscale.CanRemove = false;
                                    break;
                                case 4:
                                    qscale.Status = "已禁用";
                                    qscale.CanRemove = false;
                                    break;
                                case 9:
                                    qscale.Status = "已退货";
                                    qscale.CanRemove = false;
                                    break;
                            }

                            ListQueryScale.Add(qscale);
                        }
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
                DAL.Log.Instance.Write("出货条码查询出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }
}
