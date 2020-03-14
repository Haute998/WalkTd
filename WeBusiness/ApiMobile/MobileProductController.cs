using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiMobile
{
    [RoutePrefix("ApiMobile/MobileProduct")]
    public class MobileProductController : ApiBaseMobileController
    {
        [ApiMobileOpenFilter(true)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAllProductList(int pageindex, int pagesize, string keyword = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                int totalCount = 0;
                List<Product> RecordList = Product.GetPageProductList(pagesize, pageindex, keyword, out totalCount);
                List<ProductSimple> ProductList = new List<ProductSimple>();

                foreach (Product p in RecordList)
                {
                    ProductSimple simp = new ProductSimple();
                    simp.ProductNo = p.ProductNumber;
                    simp.ProductName = p.ProductName;
                    simp.ProductImg = WeConfig.b_domain + p.ProductImg;
                    simp.Price = p.kw;

                    ProductList.Add(simp);
                }

                result.data = ProductList;
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

    }
}
