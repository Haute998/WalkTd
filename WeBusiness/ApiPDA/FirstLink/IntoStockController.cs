using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/IntoStock")]
    public class IntoStockController : ApiBaseController
    {
        [Route("SmallCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult SmallCode(string code, string pno, string orderno = "", int supplierid = 0)
        {
            RequestResult result = new RequestResult();

            try
            {
                // 权限功能控制
                if ((PDAUserMsg.Param.ScanProcess == 1 || PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4) && PDAUserMsg.Param.IsInto && PDAUserMsg.Param.SmallInto)
                {
                    if (supplierid == 0 && PDAUserMsg.Param.IsIntoSupplier)
                    {
                        result.message = "请选择供应商";
                        result.success = false;
                    }
                    else if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (Product.IsSysProduct(pno))
                    {
                        Scale scale = Scale.GetScaleForSmall(code);

                        if (scale != null)
                        {
                            if (scale.IsLinkMid || scale.IsLinkBig)      // 是否已经关联
                            {
                                result.message = "已关联, 无法单标入货";
                                result.success = false;
                            }
                            else
                            {
                                if (scale.IsInto)
                                {
                                    result.message = "重复入库";
                                    result.success = false;
                                }
                                else
                                {
                                    scale.IsInto = true;
                                    scale.IntoPDAUser = PdaUser.PUserName;
                                    scale.ProductNo = pno;
                                    scale.IntoOrderNo = orderno;
                                    scale.SupplierId = supplierid;
                                    scale.IntoTime = CommonFunc.GetNowTimestamp();
                                    scale.StateID = 6;
                                    scale.UpdateByID();

                                    result.message = "成功";
                                    result.success = true;
                                }
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
                        result.message = "产品错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此入库功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("小标入库", "小标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},supplierid:{3}", code, pno, orderno, supplierid), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("小标入库出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        [Route("MiddleCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult MiddleCode(string code, string pno, string orderno = "", int supplierid = 0)
        {
            RequestResult result = new RequestResult();
            try
            {
                if ((PDAUserMsg.Param.ScanProcess == 1 || PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4) && PDAUserMsg.Param.IsInto && PDAUserMsg.Param.MiddleInto)
                {
                    if (supplierid == 0 && PDAUserMsg.Param.IsIntoSupplier)
                    {
                        result.message = "请选择供应商";
                        result.success = false;
                    }
                    else if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (Product.IsSysProduct(pno))
                    {
                        List<Scale> scale = Scale.GetScaleForMiddle(code);

                        if (scale.Count > 0)
                        {
                            if (scale.Where(sca => sca.IsLinkBig == true).Count() > 0)
                            {
                                result.message = "已关联大标, 无法中标入库";
                                result.success = false;
                            }
                            else
                            {
                                if (scale.Where(sca => sca.IsInto == true).Count() > 0)
                                {
                                    result.message = "重复入库";
                                    result.success = false;
                                }
                                else
                                {
                                    Scale scaleDeal = scale.Where(sca => sca.IsInto == false).FirstOrDefault();
                                    scaleDeal.IsInto = true;
                                    scaleDeal.IntoPDAUser = PdaUser.PUserName;
                                    scaleDeal.ProductNo = pno;
                                    scaleDeal.IntoOrderNo = orderno;
                                    scaleDeal.SupplierId = supplierid;
                                    scaleDeal.IntoTime = CommonFunc.GetNowTimestamp();
                                    scaleDeal.StateID = 6;
                                    scaleDeal.MiddleIntoStock();

                                    result.message = "成功";
                                    result.success = true;
                                }
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
                        result.message = "产品错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此入库功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("中标入库", "中标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},supplierid:{3}", code, pno, orderno, supplierid), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("中标入库出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }

        [Route("BigCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult BigCode(string code, string pno, string orderno = "", int supplierid = 0)
        {
            RequestResult result = new RequestResult();
            try
            {
                if ((PDAUserMsg.Param.ScanProcess == 1 || PDAUserMsg.Param.ScanProcess == 2 || PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4) && PDAUserMsg.Param.IsInto && PDAUserMsg.Param.BigInto)
                {
                    if (supplierid == 0 && PDAUserMsg.Param.IsIntoSupplier)
                    {
                        result.message = "请选择供应商";
                        result.success = false;
                    }
                    else if (string.IsNullOrWhiteSpace(pno) && PDAUserMsg.Param.IsOutProduct)
                    {
                        result.message = "请选择产品";
                        result.success = false;
                    }
                    else if (Product.IsSysProduct(pno))
                    {
                        List<Scale> scale = Scale.GetScaleForBig(code);

                        if (scale.Count > 0)
                        {

                            if (scale.Where(sca => sca.IsInto == true).Count() > 0)
                            {
                                result.message = "重复入库";
                                result.success = false;
                            }
                            else
                            {
                                Scale scaleDeal = scale.Where(sca => sca.IsInto == false).FirstOrDefault();
                                scaleDeal.IsInto = true;
                                scaleDeal.IntoPDAUser = PdaUser.PUserName;
                                scaleDeal.ProductNo = pno;
                                scaleDeal.IntoOrderNo = orderno;
                                scaleDeal.SupplierId = supplierid;
                                scaleDeal.IntoTime = CommonFunc.GetNowTimestamp();
                                scaleDeal.StateID = 6;
                                scaleDeal.BigIntoStock();

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
                        result.message = "产品错误";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此入库功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("大标入库", "大标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}, pno:{1}, orderno:{2},supplierid:{3}", code, pno, orderno, supplierid), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("大标入库出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }

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
                    if (!scale.IsInto)
                    {
                        result.message = "未入库";
                        result.success = false;
                    }
                    else if (scale.IsLinkMid || scale.IsLinkBig)      // 是否已经关联
                    {
                        result.message = "已关联";
                        result.success = false;
                    }
                    else if (scale.IsOut)
                    {
                        result.message = "已出货";
                        result.success = false;
                    }
                    else if (scale.StateID == 9)
                    {
                        result.message = "退货无法撤消";
                        result.success = false;
                    }
                    else
                    {
                        scale.IsInto = false;
                        scale.IntoPDAUser = "";// PdaUser.PUserName;
                        scale.IntoTime = 0;// CommonFunc.GetNowTimestamp();
                        scale.ProductNo = "";
                        scale.StateID = scale.IsLinkBig ? 3 : scale.IsLinkMid ? 2 : 0;
                        scale.UpdateByID();

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

                PDALog.Write("撤消入库", "小标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消小标入库出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

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
                    if (scale.Where(sca => sca.IsInto == false).Count() > 0)
                    {
                        result.message = "未入库";
                        result.success = false;
                    }
                    else if (scale.Where(sca => sca.IsLinkBig == true).Count() > 0)
                    {
                        result.message = "已关联";
                        result.success = false;
                    }
                    else if (scale.Where(sca => sca.IsOut == true).Count() > 0)
                    {
                        result.message = "已出货";
                        result.success = false;
                    }
                    else if (scale.Where(sca => sca.StateID == 9).Count() > 0)
                    {
                        result.message = "退货无法撤消";
                        result.success = false;
                    }
                    else
                    {
                        Scale scaleDeal = scale.Where(sca => sca.IsInto == true).FirstOrDefault();
                        scaleDeal.IsInto = false;

                        scaleDeal.IntoPDAUser = "";// PdaUser.PUserName;
                        scaleDeal.IntoTime = 0;// CommonFunc.GetNowTimestamp();
                        scaleDeal.ProductNo = "";
                        //scaleDeal.IntoPDAUser = PdaUser.PUserName;
                        //scaleDeal.IntoTime = CommonFunc.GetNowTimestamp();
                        scaleDeal.StateID = scaleDeal.IsLinkBig ? 3 : scaleDeal.IsLinkMid ? 2 : 0;
                        scaleDeal.RemoveMiddleInto();

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

                PDALog.Write("撤消入库", "中标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消中标入库出错：" + ex.Message, "PDA上传出错");
            }
            
            return result;
        }

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
                    if (scale.Where(sca => sca.IsInto == false).Count() > 0)
                    {
                        result.message = "未入库";
                        result.success = false;
                    }
                    else if (scale.Where(sca => sca.IsOut == true).Count() > 0)
                    {
                        result.message = "已出货";
                        result.success = false;
                    }
                    else if (scale.Where(sca => sca.StateID == 9).Count() > 0)
                    {
                        result.message = "退货无法撤消";
                        result.success = false;
                    }
                    else
                    {
                        Scale scaleDeal = scale.Where(sca => sca.IsInto == true).FirstOrDefault();
                        scaleDeal.IsInto = false;
                        scaleDeal.IntoPDAUser = "";// PdaUser.PUserName;
                        scaleDeal.IntoTime = 0;// CommonFunc.GetNowTimestamp();
                        scaleDeal.ProductNo = "";
                        //scaleDeal.IntoPDAUser = PdaUser.PUserName;
                        //scaleDeal.IntoTime = CommonFunc.GetNowTimestamp();
                        scaleDeal.StateID = scaleDeal.IsLinkBig ? 3 : scaleDeal.IsLinkMid ? 2 : 0;
                        scaleDeal.RemoveBigInto();

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

                PDALog.Write("撤消入库", "大标", code, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("code:{0}", code), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("撤消大标入库出错：" + ex.Message, "PDA上传出错");
            }
            
            return result;
        }

        [Route("RemoveIntoBarCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveIntoBarCode(string barcode)
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
                DAL.Log.Instance.Write("撤消入库出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 入库查询
        /// IntoBarCodeQuery</summary>
        /// <param name="barcode">小标或中标或大标</param>
        /// <returns></returns>
        [Route("IntoBarCodeQuery")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult IntoBarCodeQuery(string barcode)
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

                            if (ScaleList.Where(m => m.MiddleCode == code && (m.IsInto != true || m.StateID != 6)).Count() == 0)
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
                            qscale.Status = scale.IsInto ? "可撤消" : "未入库";
                            qscale.CanRemove = scale.IsInto ? true : false;

                            switch (scale.StateID)
                            {
                                case 0:
                                case 2:
                                case 3:
                                    qscale.Status = "未入库";
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
                DAL.Log.Instance.Write("入库条码查询出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }

    public class IntoQueryScale
    {
        public int Type { get; set; }
        public string BarCode { get; set; }
        public string Status { get; set; }
        public bool CanRemove { get; set; }
    }
}
