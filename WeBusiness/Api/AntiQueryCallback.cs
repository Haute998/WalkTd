using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.Api
{
    /// <summary>
    /// 防伪查询回调接口
    /// </summary>
    [RoutePrefix("Api/AntiQueryCallback")]
    public class AntiQueryCallbackController : ApiBaseController
    {
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult InsertQuery(int FID, string AntiCode, string IP, string Province, string City, string Address, string Country="")
        {
            DAL.Log.Instance.Write("FID:" + FID.ToString() + ",AntiCode:" + AntiCode + ",IP:" + IP + ",Country:" + Country + ",Province:" + Province + ",City:" + City + ",Address:" + Address, "防伪查询回调参数");

            RequestResult result = new RequestResult();

            try
            {
                SelScale selscale = new SelScale();

                ScaleOutStoke Stoke = ScaleOutStoke.GetSmallScaleListcode(AntiCode);

                if (Stoke == null)
                {
                    Scale scale = Scale.GetEntityByAntiCode(AntiCode);
                    if (scale==null)
                    {
                        selscale.warning = "错误";
                        result.message = "错误";
                        result.success = true;
                    }
                    else if (!scale.IsOut)
                    {
                        selscale.warning = "未出货";
                        result.message = "未出货";
                        result.success = true;
                    }
                    else
                    {
                        selscale.warning = "异常";
                        result.message = "异常";
                        result.success = true;
                    }
                }
                else if (Province == "未知" || City == "未知")
                {
                    selscale.warning = "异常";
                    result.message = "异常";
                    result.success = true;
                }
                else
                {
                    C_User user = new C_User();
                    if (Stoke.Consignee == "m2000")
                    {
                        //查到出货人信息
                        user = C_User.GetC_UserByUserName(Stoke.Shipper);
                        if (user != null)
                        {
                            if (user.Province != Province)
                            {
                                selscale.warning = "窜货";
                            }
                            else
                            {
                                selscale.warning = "正常";
                            }
                        }
                        else {
                            selscale.warning = "异常";
                        }
                    }
                    else
                    {
                        user = C_User.GetC_UserByUserName(Stoke.Consignee);
                        if (user != null)
                        {
                            if (user.Province != Province)
                            {
                                selscale.warning = "窜货";
                            }
                            else
                            {
                                selscale.warning = "正常";
                            }
                        }
                        else {
                            selscale.warning = "异常";
                        }
                    }

                    result.message = "成功";
                    result.success = true;
                }

                selscale.warning = string.IsNullOrWhiteSpace(selscale.warning) ? "未知" : selscale.warning;
                selscale.Country = Country;
                selscale.province = Province;
                selscale.city = City;
                selscale.IP = IP;
                selscale.FID = FID;
                selscale.AntiCode = AntiCode;
                selscale.Address = Address;
                selscale.CreateTime = CommonFunc.GetNowTimestamp();
                selscale.InsertAndReturnIdentity();
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.Message,"防伪查询回调出错");
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }

            return result;
        }

        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult ChkAntiCodeOut(string AntiCode)
        {
            RequestResult result = new RequestResult();

            bool IsOk = ScaleOut_Anti.IsAntiCodeOut(AntiCode);

            if (IsOk)
            {
                result.message = "已出库";
                result.success = true;
            }
            else
            {
                result.message = "未出库";
                result.success = false;
            }

            return result;
        }
    }
}