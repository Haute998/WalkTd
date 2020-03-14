using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class MyAgentsInfoController : BaseController
    {
        //
        // GET: /MyAgentsInfo/

        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }
        public ActionResult AgentsInfoIndex()
        {
            return View();
        }
        public ActionResult AgentstuiIndex()
        {
            return View();
        }
        public ActionResult AgentstjIndex()
        {
            return View();
        }
        public ActionResult GetC_Userhuantj(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            string where1 = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Phone like '%" + condition.keyword + "%'";
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where1 += string.Format(" and C_UserAdvice.DatCreate >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where1 += string.Format(" and C_UserAdvice.DatCreate <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }
            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            string strForm = "";
            strForm = @"C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN";
            if (!string.IsNullOrWhiteSpace(where1))
            {
                strForm += where1;
            }
            page.strForm = strForm;
            page.strSelect = " C_UserAdvice.Phone Phone,count(SN) sl ";
            page.strWhere = string.Format(" and ScaleOutStoke.Consignee='{0}' and C_UserAdvice.state2='已审核' and C_UserAdvice.state='换货' group by Phone ", CurrentUser.UserName) + where;
            page.strOrder = "Phone";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetC_Usertuitj(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
           
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and ProductName like '%" + condition.keyword + "%'";
            }
    
            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN left join Product on ScaleOutStoke.P_ID=Product.ProductNumber";
            page.strSelect = " Product.ProductName ProductName,count(SN) sl,C_UserAdvice.state state";
            page.strWhere = string.Format(" and ScaleOutStoke.Consignee='{0}' and C_UserAdvice.state2='已审核' and C_UserAdvice.OrderNo='' group by ProductName ", CurrentUser.UserName) + where;
            page.strOrder = "ProductName";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetC_Usertuitj2(ScaleOutStokeShow condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and ProductName like '%" + condition.keyword + "%'";
            }

            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN left join Product on ScaleOutStoke.P_ID=Product.ProductNumber left join C_User on ScaleOutStoke.Consignee=C_User.UserName ";
            page.strSelect = " Product.ProductName ProductName,C_UserAdvice.State,ScaleOutStoke.Consignee ,count(C_User.Name) sl,ScaleOutStoke.Shipper,C_User.Name XName ";
            page.strWhere = string.Format(" and ScaleOutStoke.Shipper='{0}' and C_UserAdvice.state2='已审核' and C_UserAdvice.OrderNo is null group by ProductName,C_UserAdvice.State,ScaleOutStoke.Consignee,ScaleOutStoke.Shipper,C_User.Name ", CurrentUser.UserName) + where;
            page.strOrder = "ProductName";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult AuditByIDtuihuan(ScaleOutStokeShow condition)
        //{



        //    string strSql = string.Format("update C_UserAdvice set state='已审核' where  (select Product.ProductName ProductName,C_UserAdvice.State,ScaleOutStoke.Consignee,count(SN) sl,ScaleOutStoke.Shipper from C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN left join Product on ScaleOutStoke.P_ID=Product.ProductNumber where  ScaleOutStoke.Shipper='" + CurrentUser.UserName + "' and C_UserAdvice.state2='已审核' group by ProductName,C_UserAdvice.State,ScaleOutStoke.Consignee,ScaleOutStoke.Shipper);");
        //    System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@DatCreate", DateTime.Now.ToString()) };
        //    int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
        //    return cnt;
        //}
        public ActionResult GetUserTypes(BaseInSearch condition) 
        {
            PageJsonModel<C_UserType> page = new PageJsonModel<C_UserType>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" C_UserType left join C_User on C_User.C_UserTypeID=[C_UserType].Lever and C_User.Chief={0} and C_User.state='已审核' ", condition.ID);
            page.strSelect = " C_UserType.ID,C_UserType.Name,count(C_User.ID) cnt  ";
            page.strWhere = "  group by C_UserType.ID,C_UserType.Name ";
            page.strOrder = "C_UserType.ID";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取下级
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult getsubs(C_User_TypeVMSearch condition) 
        {
            PageJsonModel<C_User_TypeVM> page = new PageJsonModel<C_User_TypeVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_User left join [C_UserType] on C_User.C_UserTypeID=[C_UserType].Lever ";
            page.strSelect = " C_User.*,[C_UserType].ID userTypeID,[C_UserType].Name userTypeName ";
            page.strWhere = string.Format(" and C_User.Chief={0} and [C_UserType].ID={1} and C_User.state='已审核' ", condition.ID, condition.userTypeID);
            page.strOrder = " C_User.ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 审核管理
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentsNoAudit() 
        {
            return View();
        }
        /// <summary>
        /// 获取待审核的代理人
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadAgentsNoAudit(C_UserVMSearch condition)
        {
            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN";
            page.strSelect = " C_UserAdvice.* ";
            page.strWhere = string.Format(" and ScaleOutStoke.Consignee='{0}' and C_UserAdvice.state2='未审核' ", CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.keyword)) 
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and (ScaleOutStoke.Consignee like '%{0}%' or C_UserAdvice.Phone like '%{0}%' or C_UserAdvice.State like '%{0}%'  ) ", condition.keyword);
            }

            page.strOrder = "C_UserAdvice.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadAgentsNoAudit2(C_UserVMSearch condition)
        {
            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN";
            page.strSelect = " C_UserAdvice.* ";
            page.strWhere = string.Format(" and ScaleOutStoke.Consignee='{0}' and C_UserAdvice.state2='已审核' ", CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and (ScaleOutStoke.Consignee like '%{0}%' or C_UserAdvice.Phone like '%{0}%' or C_UserAdvice.State like '%{0}%' ) ", condition.keyword);
            }

            page.strOrder = "C_UserAdvice.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取未审核的数量
        /// </summary>
        /// <returns></returns>
        public ActionResult getNoAuditCnt() 
        {
            int cnt=C_UserVM.GetNoAuditCnt(CurrentUser.ID);
            return Content(cnt.ToString());
        }
        /// <summary>
        /// 升级审核列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UpgradeNoAudit()
        {
            return View();
        }
        /// <summary>
        /// 读取升级审核列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadUpgradeNoAudit(C_UserUpGradeVMSearch condition)
        {
            PageJsonModel<C_UserUpGradeVM> page = new PageJsonModel<C_UserUpGradeVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @" C_UserUpGrade 
                           left join C_User on C_UserUpGrade.UserName=C_User.UserName    
                           left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever
                           left join C_UserType newType on C_UserUpGrade.NewUserTypeID=newType.Lever";
            page.strSelect = " C_UserUpGrade.*,C_User.Name,C_User.wxNo,C_User.Phone,C_UserType.Name OldUserTypeName,newType.Name NewUserTypeName  ";
            page.strWhere = string.Format(" and C_UserUpGrade.ParentUser='{0}' and C_UserUpGrade.AuditStat='未审核' ", CurrentUser.UserName);
            
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and (C_User.Name like '%{0}%' or C_User.Phone like '%{0}%' or C_User.wxNo='{0}' or OldUserTypeName='{0}' ) ", condition.keyword);
            }

            page.strOrder = "C_UserUpGrade.ID Asc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 审核升级
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public ActionResult toAuditUpgrade(int[] cklst) 
        {
            if (cklst == null || cklst.Length <= 0)
            {
                return Content("请勾选你要审核的人");
            }
            string rtn = C_UserUpGradeVM.AuditByIDs(cklst);
            return Content(rtn);
        }

        //public static string AuditByIDtuihuan()
        //{
        //public ActionResult AuditByIDtuihuan(ScaleOutStokeShow condition)
        //    {
        //        string sql = "select Product.ProductName ProductName,count(SN) sl from C_UserAdvice left join ScaleOutStoke on ScaleOutStoke.SmallCode=C_UserAdvice.SN left join Product on ScaleOutStoke.P_ID=Product.ProductNumber where C_UserAdvice.state2='已审核' AND ScaleOutStoke.Consignee='"+CurrentUser.UserName+"' and C_UserAdvice.OrderNo is NULL group by ProductName ";
        //    System.Data.SqlClient.SqlParameter[] paramters1 = null;
        //    List<C_UserAdvice> c_UserAdvice = EntityDataHelper.FillData2Entities<C_UserAdvice>(sql, paramters1);

        //    string ordero = DateTime.Now.ToString("yyyymmddss");
        //    string strSql = string.Format("update C_UserAdvice set OrderNo='" + ordero + "' where SN in (select smallcode from ScaleOutStoke  where Consignee='" + CurrentUser.UserName + "');");
        //    System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@DatCreate", DateTime.Now.ToString()) };
        //    int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
        //    if (cnt > 0)
        //    {
        //        return "ok";
        //    }
        //    else
        //    {
        //        return "审核失败";
        //    }
        //}
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public ActionResult AuditByIDs(int[] cklst) 
        {
            if (cklst == null || cklst.Length <= 0)
            {
                return Content("请勾选你要审核的人");
            }
            
            string rtn = C_UserAdvice1.AuditByIDs(cklst);
            return Content(rtn);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public ActionResult DelByIDs(int[] cklst)
        {
            if (cklst == null || cklst.Length <= 0)
            {
                return Content("请勾选你要删除的维修信息");
            }
            string rtn = C_UserAdvice1.DelByIDs(cklst);
            return Content(rtn);
        }

        /// <summary>
        /// 代理资料详情
        /// </summary>
        /// <returns></returns>
        public ActionResult agentDetail(int id) 
        {
            ViewData["user"] = C_UserVM.GetVMByID(id);
            return View();
        }
        /// <summary>
        /// 升级详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpgradeDetail(int id) 
        {
            C_UserUpGradeVM models= C_UserUpGradeVM.getVMByID(id);
            if (models.ParentUser != CurrentUser.UserName) 
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "找不到路啦" });
            }
            return View(models);
        }

        public ActionResult toAuditUpgrade_a(int id)
        {
            if (id <= 0)
            {
                return Content("您要审核什么？");
            }
            int[] cklst = new int[] { id};
            string rtn = C_UserUpGradeVM.AuditByIDs(cklst);
            return Content(rtn);
        }
        /// <summary>
        /// 跨级申请
        /// </summary>
        /// <returns></returns>
        public ActionResult UpgradeNoAuditAgent()
        {
            return View();
        }
        /// <summary>
        /// 跨级申请
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadUpgradeNoAuditAgent(C_UserUpGradeVMSearch condition)
        {
            PageJsonModel<C_UserUpGradeVM> page = new PageJsonModel<C_UserUpGradeVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @" C_UserUpGrade 
                           left join C_User on C_UserUpGrade.UserName=C_User.UserName    
                           left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever
                           left join C_UserType newType on C_UserUpGrade.NewUserTypeID=newType.Lever";
            page.strSelect = " C_UserUpGrade.*,C_User.Name,C_User.wxNo,C_User.Phone,C_UserType.Name OldUserTypeName,newType.Name NewUserTypeName  ";
            page.strWhere = string.Format(" and C_UserUpGrade.OldParentUser='{0}' and OldParentUser!=ParentUser and C_UserUpGrade.AuditStat='未审核' ", CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and (C_User.Name like '%{0}%' or C_User.Phone like '%{0}%' or C_User.wxNo='{0}' or OldUserTypeName='{0}' ) ", condition.keyword);
            }

            page.strOrder = "C_UserUpGrade.ID Asc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 我的意向代理  待完善
        /// </summary>
        /// <returns></returns>
        public ActionResult My_yixiang() 
        {
            C_UserVM user = C_UserVM.GetVMByUserName(CurrentUser.UserName);
            ViewData["user"] = user;
            return View();
        }


        public ActionResult LoadMy_yixiang(AgentIntentionSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Name like '%{0}%' or Phone like '%{0}%')", condition.keyword);
            }

            where += string.Format(" and ChiefUser='{0}' ", CurrentUser.UserName);
          

            return GetMy_yixiangs(condition, where);
        }
        private ActionResult GetMy_yixiangs(AgentIntentionSearch condition, string where)
        {
            PageJsonModel<AgentIntention> page = new PageJsonModel<AgentIntention>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@" AgentIntention ");
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 我的代理
        /// </summary>
        /// <returns></returns>
        public ActionResult MyAgentHome() 
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }




    }
}
