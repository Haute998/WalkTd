using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class ProductShowController : Controller
    {
        //
        // GET: /ProductShow/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult GetProductPage(MProductSearch condition)
        {
            PageJsonModel<Product> page = new PageJsonModel<Product>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "Product";
            page.strSelect = "* ";
            page.strWhere = " and States='已上架'";
            page.strOrder = "ProductID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProductsDetail(int id)
        {
            Product goods = Product.GetEntityByID(id);
            return View(goods);
        }

    }
}
