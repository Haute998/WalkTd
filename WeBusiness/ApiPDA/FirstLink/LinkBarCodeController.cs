using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using WeModels;

namespace WeBusiness.ApiPDA.FirstLink
{
    [RoutePrefix("ApiPDA/FirstLink/LinkBarCode")]
    public class LinkBarCodeController : ApiBaseController
    {
        /// <summary>
        /// 小中标关联接口
        /// </summary>
        /// <returns></returns>
        [Route("SmallAndMid")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult SmallAndMid(string scode, string mcode, string orderno = "", string pno = "", int supplierid = 0)
        {
            RequestResult result = new RequestResult();
            try
            {
                // 权限功能控制
                if ((PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4) && PDAUserMsg.Param.IsLink && PDAUserMsg.Param.SmallPacking)
                {
                    if (TB_BMCode.GetBoolBarCode(mcode,1))        // 中标是否存在
                    {
                        Scale scale = Scale.GetScaleForSmall(scode);
                        LinkRtnData RtnInfo = new LinkRtnData();

                        if (scale != null)
                        {
                            if (scale.IsLinkMid || scale.IsLinkBig)      // 是否已经关联
                            {
                                RtnInfo.BoxCode = scale.MiddleCode;
                                RtnInfo.LinkDate = CommonFunc.GetDateTimeFromTimestamp(scale.LinkMidTime);
                                result.data = RtnInfo;
                                result.message = "重复关联";
                                result.success = false;
                            }
                            else if (PDAUserMsg.Param.ScanProcess == 4 && !scale.IsInto)
                            {
                                result.message = "请先入库,再关联装箱";
                                result.success = false;
                            }
                            else
                            {
                                // 先关联模式_如果中标已入库,小标关联也随之入库
                                if (PDAUserMsg.Param.ScanProcess == 3)
                                {
                                    List<Scale> scaleMiddle = Scale.GetScaleForMiddle(mcode);

                                    if (scaleMiddle != null && scaleMiddle.Count() > 0)
                                    {
                                        Scale middle = scaleMiddle.Last();

                                        if (!scale.IsInto && middle.IsInto)
                                        {
                                            scale.IsInto = middle.IsInto;
                                            scale.IntoOrderNo = orderno;
                                            scale.IntoTime = CommonFunc.GetNowTimestamp();
                                            scale.IntoPDAUser = PdaUser.PUserName;
                                            scale.ProductNo = middle.ProductNo;
                                        }
                                        else if (!middle.IsInto && scale.IsInto)
                                        {
                                            result.message = "箱标未入库,无法关联装箱";
                                            result.success = false;
                                            PDALog.Write("小标装箱", "小标", scode + "," + mcode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("scode:{0}, mcode:{1}, orderno:{2},supplierid:{3},pno:{4}", scode, mcode, orderno, supplierid, pno), result.message);
                                            return result;
                                        }
                                    }
                                }

                                Scale bigFrast = Scale.GetMiddleLinkBigScale(mcode);  // 查询中标关联大标

                                scale.IsLinkMid = true;
                                scale.MiddleCode = mcode;
                                scale.LinkMidPDAUser = PdaUser.PUserName;
                                scale.LinkMidOrderNo = orderno;
                                if (supplierid != 0) scale.SupplierId = supplierid;
                                if (!string.IsNullOrWhiteSpace(pno)) scale.ProductNo = pno;
                                scale.LinkMidTime = CommonFunc.GetNowTimestamp();
                                scale.StateID = 2;
                                if (bigFrast != null) { scale.IsLinkBig = true; scale.BigCode = bigFrast.BigCode; scale.StateID = 3; scale.LinkBigTime = CommonFunc.GetNowTimestamp(); }
                                scale.UpdateByID();

                                RtnInfo.BoxCode = mcode;
                                RtnInfo.LinkDate = DateTime.Now;
                                result.data = RtnInfo;
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
                        result.code = 304;
                        result.message = "条码不存在";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此装箱功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }

                PDALog.Write("小标装箱", "小标", scode + "," + mcode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("scode:{0}, mcode:{1}, orderno:{2},supplierid:{3},pno:{4}", scode, mcode, orderno, supplierid, pno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("小标关联出错：" + ex.Message, "PDA上传出错"); 
            }

            return result;
        }

        /// <summary>
        /// 中大标关联接口
        /// </summary>
        /// <returns></returns>
        [Route("MidAndLarge")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult MidAndLarge(string mcode, string lcode, string orderno = "", string pno = "", int supplierid = 0)
        {
            RequestResult result = new RequestResult();
            try
            {
                if ((PDAUserMsg.Param.ScanProcess == 3 || PDAUserMsg.Param.ScanProcess == 4) && PDAUserMsg.Param.IsLink && PDAUserMsg.Param.MiddlePacking)
                {
                    if (TB_BMCode.GetBoolBarCode(lcode,2))                    // 大标是否存在
                    {
                        Scale scale = Scale.GetScaleForMiddle(mcode).FirstOrDefault();
                        LinkRtnData RtnInfo = new LinkRtnData();

                        if (scale != null)
                        {
                            if (scale.IsLinkBig)                            // 是否已经关联
                            {
                                RtnInfo.BoxCode = scale.MiddleCode;
                                RtnInfo.LinkDate = CommonFunc.GetDateTimeFromTimestamp(scale.LinkMidTime);
                                result.data = RtnInfo;
                                result.message = "已关联";
                                result.success = false;
                            }
                            else if (PDAUserMsg.Param.ScanProcess == 4 && !scale.IsInto)
                            {
                                result.message = "请先入库,再关联装箱";
                                result.success = false;
                            }
                            else
                            {
                                // 先关联模式_如果中标已入库,小标关联也随之入库,如果已入库，不再入库。
                                if (PDAUserMsg.Param.ScanProcess == 3)
                                {
                                    List<Scale> scaleBig = Scale.GetScaleForBig(lcode);
                                    if (scaleBig != null && scaleBig.Count() > 0)
                                    {
                                        Scale BigLast = scaleBig.Last();
                                        if (!scale.IsInto && BigLast.IsInto)
                                        {
                                            scale.IsInto = BigLast.IsInto;
                                            scale.IntoOrderNo = orderno;
                                            scale.IntoTime = CommonFunc.GetNowTimestamp();
                                            scale.IntoPDAUser = PdaUser.PUserName;
                                            scale.ProductNo = BigLast.ProductNo;
                                        }
                                        else if (!BigLast.IsInto && scale.IsInto)
                                        {
                                            result.message = "大箱标未入库,无法关联装箱";
                                            result.success = false;
                                            PDALog.Write("中标装箱", "中标", mcode + "," + lcode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("mcode:{0}, lcode:{1}, orderno:{2},supplierid:{3},pno:{4}", mcode, lcode, orderno, supplierid, pno), result.message);
                                            return result;
                                        }
                                    }
                                }

                                scale.IsLinkBig = true;
                                scale.BigCode = lcode;
                                scale.LinkBigPDAUser = PdaUser.PUserName;
                                scale.LinkBigOrderNo = orderno;
                                if (supplierid != 0) scale.SupplierId = supplierid;
                                if (!string.IsNullOrWhiteSpace(pno)) scale.ProductNo = pno;
                                scale.LinkBigTime = CommonFunc.GetNowTimestamp();
                                scale.StateID = 3;
                                scale.UpdateByMiddle();
                                //Scale_Big.InsertScaleBigCode(mcode);        // 关联后插入大标数据表

                                RtnInfo.BoxCode = lcode;
                                RtnInfo.LinkDate = DateTime.Now;
                                result.data = RtnInfo;
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
                        result.code = 304;
                        result.message = "条码不存在";
                        result.success = false;
                    }
                }
                else
                {
                    result.code = 704;
                    result.message = "此装箱功能已被禁用，请在管理后台开启。";
                    result.success = false;
                }
                PDALog.Write("中标装箱", "中标", mcode + "," + lcode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("mcode:{0}, lcode:{1}, orderno:{2},supplierid:{3},pno:{4}", mcode, lcode, orderno, supplierid, pno), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("中标关联出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 检测中标或大标是否存在系统中
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        [Route("CheckMLBarCode")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult CheckMLBarCode(string BarCode, int type = 0)
        {
            RequestResult result = new RequestResult();
            try
            {
                if (TB_BMCode.GetBoolBarCode(BarCode, type))        // 大标或中标是否存在
                {
                    result.message = "正常";
                    result.success = true;

                    if (type == 1)
                    {
                        if (ScaleOutStoke.IsMiddleOutStock(BarCode))
                        {
                            result.code = 304;
                            result.message = "已出货，无法关联";
                            result.success = true;
                        }
                    }
                    else if (type == 2)
                    {
                        if (ScaleOutStoke.IsBigOutStock(BarCode))
                        {
                            result.code = 304;
                            result.message = "已出货，无法关联";
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
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("检测中标或大标出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 解除小标关联接口
        /// </summary>
        /// <returns></returns>
        [Route("RemoveSM")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveSM(string smallcode)
        {
            RequestResult result = new RequestResult();
            try
            {
                Scale scale = Scale.GetScaleForSmall(smallcode);

                if (scale != null)
                {
                    if (scale.IsOut)
                    {
                        result.code = 504;
                        result.message = "已出货";
                        result.success = false;
                    }
                    //else if (scale.IsInto)
                    //{
                    //    result.code = 504;
                    //    result.message = "已入库";
                    //    result.success = false;
                    //}
                    else if (!scale.IsLinkMid && !scale.IsLinkBig)
                    {
                        result.code = 504;
                        result.message = "无关联";
                        result.success = false;
                    }
                    else
                    {
                        scale.BigCode = "";
                        scale.MiddleCode = "";
                        scale.IsLinkBig = false;
                        scale.IsLinkMid = false;
                        scale.LinkBigPDAUser = PdaUser.PUserName;
                        scale.LinkMidPDAUser = PdaUser.PUserName;
                        scale.LinkBigTime = CommonFunc.GetNowTimestamp();
                        scale.LinkMidTime = CommonFunc.GetNowTimestamp();
                        if (PDAUserMsg.Param.ScanProcess == 3) scale.IsInto = false;    // 解除关联后撤消入库
                        if (scale.StateID == 2) scale.StateID = 0;
                        scale.UpdateByID();

                        //Scale_Middle.DeleteScaleMiddleCode(scale.ID);       // 解除小标关联后从中标数据表删除

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

                PDALog.Write("解除小标装箱", "小标", smallcode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("smallcode:{0}", smallcode), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("解除小标关联出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        /// <summary>
        /// 解除中标关联接口
        /// </summary>
        /// <returns></returns>
        [Route("RemoveML")]
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult RemoveML(string middlecode)
        {
            RequestResult result = new RequestResult();
            try
            {
                Scale scale = Scale.GetScaleForMiddle(middlecode).FirstOrDefault();

                if (scale != null)
                {
                    if (scale.IsOut)
                    {
                        result.code = 504;
                        result.message = "已出货";
                        result.success = false;
                    }
                    //else if (scale.IsInto)
                    //{
                    //    result.code = 504;
                    //    result.message = "已入库";
                    //    result.success = false;
                    //}
                    else if (!scale.IsLinkBig)
                    {
                        result.code = 504;
                        result.message = "无关联";
                        result.success = false;
                    }
                    else
                    {
                        scale.BigCode = "";
                        scale.IsLinkBig = false;
                        scale.LinkBigPDAUser = PdaUser.PUserName;
                        scale.LinkBigTime = CommonFunc.GetNowTimestamp();
                        if (PDAUserMsg.Param.ScanProcess == 3) scale.IsInto = false; // 解除关联后撤消入库
                        if (scale.StateID == 3) scale.StateID = scale.IsLinkMid ? 2 : 0;
                        scale.RemoveMiddleAndBigCode();

                        //Scale_Big.DeleteScaleBigCode(middlecode);       // 解除中标关联后从中标数据表删除

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

                PDALog.Write("解除中标装箱", "中标", middlecode, PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("smallcode:{0}", middlecode), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("解除中标关联出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }

    public class LinkRtnData
    {
        public string BoxCode { get; set; }
        public DateTime LinkDate { get; set; }
    }
}
