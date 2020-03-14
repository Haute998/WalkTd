using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class IncreaseAgentController : BaseController
    {
        //
        // GET: /IncreaseAgent/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAgentIncrease(SearchAgentIncrease condition)
        {
            string where = string.Empty;
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and o.Name like '%{0}%'  ", condition.keyword);
            }
            //订单创建时间
            where += string.Format(" and c.DatVerify >='{0}/01 00:00:00' and c.DatVerify<'{1}/01 00:00:00'", Common.Filter(condition.DatCreateMon), DateTime.Parse(Common.Filter(condition.DatCreateMon) + "/01").AddMonths(1).ToString("yyyy-MM"));
            if (condition.C_UserTypeID != 0)
            {
                where += string.Format(" and o.C_UserTypeID ='{0}' ", condition.C_UserTypeID);
            }

            PageJsonModel<AgentIncrease> page = new PageJsonModel<AgentIncrease>();
            page.pageIndex = condition.pageIndex;
            page.strForm = @" ( select Count(c.Chief)+ (select COUNT(*)+1 from C_User where Chief=o.ID and state='已审核' and DatVerify<'" + Common.Filter(condition.DatCreateMon) + "/01 00:00:00') Team, (select COUNT(*)+1 from C_User where Chief=o.ID and state='已审核' and DatVerify<'" + Common.Filter(condition.DatCreateMon) + "/01 00:00:00') Teamcount, Count(c.Chief) counts, Count(c.Chief)/(select COUNT(*)+1.0 from C_User where Chief=o.ID and state='已审核' and DatVerify<'" + Common.Filter(condition.DatCreateMon) + "/01 00:00:00') agentcount,o.Name Name,o.Identifier Identifier from C_User c left join C_User o on c.Chief=o.ID where c.Chief!=0 and c.state='已审核'  " + where + " group by o.Chief,o.Identifier,o.Name,o.ID) as show";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = "";
            page.strOrder = " agentcount Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
