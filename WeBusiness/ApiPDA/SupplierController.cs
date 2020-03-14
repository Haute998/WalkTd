using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/Supplier")]
    public class SupplierController : ApiController
    {
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAllList()
        {
            RequestResult result = new RequestResult();
            try
            {
                List<Supplier> SupplierList = Supplier.GetEntitysAll();

                result.data = SupplierList;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取所有供应商出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }
    }
}
