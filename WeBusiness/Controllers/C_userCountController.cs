using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class C_userCountController : BaseController
    {
        //
        // GET: /C_userCount/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowIndex()
        {
            return View();
        }
        public ActionResult GetSaleAgent(SaleSearch condition)
        {
            string where = "and Chief=0 and UserName!='m2000' and c.state='已审核'";
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (c.Name like '%{0}%' or c.Phone like '%{0}%')  ", condition.keyword);
            }
         
            if (condition.C_UserTypeID != 0)
            {
                where += string.Format(" and c.C_UserTypeID ='{0}' ", condition.C_UserTypeID);
            }

            PageJsonModel<C_User> page = new PageJsonModel<C_User>();
            page.pageIndex = condition.pageIndex;
            page.strForm = @" (select c.ID,c.Name,c.UserName,c.Phone,t.Name C_UserTypeName,(select COUNT(*) from [ScaleOutStoke] where  Consignee=c.UserName  and State='启用') count from [C_User] c  left join [C_UserType] t on c.C_UserTypeID=t.Lever  where 1=1  " + where + "  group by c.ID,c.Name,c.UserName,c.Phone,t.Name   ) as show";
            //  page.strForm = @" (select c.ID,c.Name,c.UserName,c.Phone,t.Name C_UserTypeName,count(s.ID) count from [C_User] c left join [ScaleOutStoke] s on c.UserName=s.Consignee  left join [C_UserType] t on c.C_UserTypeID=t.Lever   where s.State='启用'  " + where + " group by c.ID,c.Name,c.UserName,c.Phone,t.Name) as show";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = "";
            page.strOrder = " ID Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("库存明细", "Index")]
        public ActionResult C_userCountDetail(string username)
        {
            ViewData["username"] = username;
            return View();
        }
        public ActionResult GetDetail(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and (ProductName like '%" + condition.keyword + "%' or ProductNumber like '%" + condition.keyword + "%')";
            }       
            return GetPages(condition, where);
        }
        private ActionResult GetPages(Scale condition, string where)
        {
            PageJsonModel<Product> page = new PageJsonModel<Product>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select P.ProductID,p.ProductNumber,P.ProductName,count(s.ID) count from Product p left join ScaleOutStoke s  on s.ProductNo=p.ProductNumber where s.Consignee='" + condition.Name + "' and s.State='启用'  group by P.ProductID,p.ProductNumber,P.ProductName) as Show";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ProductID desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
