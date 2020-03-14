using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness
{
    [RoutePrefix("ApiPDA/BaseParam")]
    public class BaseParamController : ApiBaseController
    {
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult Get()
        {
            RequestResult result = new RequestResult();

            try
            {
                PDAParam param = PDAParam.GetEntityByID(1);
                List<PDAUserFuncEasy> UserFunc = PDAUserFunc.GetEntitysUserId(PdaUser.ID);
                PDAParamEasy paramEasy = new PDAParamEasy();
                paramEasy.IntoWay = param.IntoWay;
                paramEasy.IsIntoProduct = param.IsIntoProduct;
                paramEasy.IsIntoSupplier = param.IsIntoSupplier;
                paramEasy.IsLinkProduct = param.IsLinkProduct;
                paramEasy.IsLinkSupplier = param.IsLinkSupplier;
                paramEasy.LinkWay = param.LinkWay;
                paramEasy.OutWay = param.OutWay;
                paramEasy.IsOutProduct = param.IsOutProduct;
                paramEasy.MiddlePacking = param.MiddlePacking || param.BigInto || param.BigOut;

                if (!paramEasy.MiddlePacking)
                {
                    for (int i = 0; i < UserFunc.Count(); i++)
                    {
                        switch (UserFunc[i].FunCode)
                        {
                            case "B004":
                                UserFunc[i].FunName = "大标入库";
                                break;
                            case "B007":
                                UserFunc[i].FunName = "大标出货";
                                break;
                            case "B010":
                                UserFunc[i].FunName = "大标退货";
                                break;
                        }
                    }
                }

                MobileBaseParamter BaseParam = new MobileBaseParamter();
                BaseParam.Param = paramEasy;
                BaseParam.FuncList = UserFunc;

                result.data = BaseParam;
                result.message = "成功";
                result.success = true;

            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取PDA基础参数出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        public class MobileBaseParamter
        {
            public PDAParamEasy Param { get; set; }
            public List<PDAUserFuncEasy> FuncList { get; set; }
        }
    }
}
