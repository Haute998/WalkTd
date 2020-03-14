using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/Product")]
    public class ProductController : ApiBaseController
    {
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAllList()
        {
            RequestResult result = new RequestResult();
            try
            {
                List<P_Interface> productlist = Product.GetProduct();

                result.data = productlist;
                result.message = "成功";
                result.success = true;

                PDALog.Write("获取产品列表", "获取", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取产品列表出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }

        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUpdateNewList(int Timestamp)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<P_Interface> productlist = Product.GetProduct(Timestamp);

                result.data = productlist;
                result.message = "成功";
                result.success = true;

                PDALog.Write("更新产品列表", "更新", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取最新产品列表出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }

        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult QueryList(string KeyWords)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<P_Interface> productlist = Product.GetProduct(KeyWords);
                result.data = productlist;
                result.message = "成功";
                result.success = true;

                PDALog.Write("查询产品", "查询", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("查询产品列表出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }
}
