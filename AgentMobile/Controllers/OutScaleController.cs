using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.OrderModel;

namespace AgentMobile.Controllers
{
    public class OutScaleController : BaseController
    {
        //
        // GET: /OutScale/
        /// <summary>
        /// 选择订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 选择代理
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentIndex()
        {
            return View();
        }

        /// <summary>
        /// 订单扫描页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderIDIndex(string ID)
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }
            Order order = Order.GetOrderByOrderNo(ID);


            List<BasePostCode> PostCodes = BasePostCode.GetAllBySort();
            PostCodes = PostCodes.FindAll(m => m.IsHave).ToList();
            ViewData["PostCodes"] = PostCodes;

            return View(order);
        }
        /// <summary>
        /// 扫描给代理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgentIndex(string ID)
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }
            C_User Cuser = C_User.GetC_UserByUserName(ID);
            return View(Cuser);
        }

        public ActionResult ScanOutStock()
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }

            string orderno=DateTime.Now.ToString("yyMMddHHmmssf");
            ViewData["OrderNo"] = orderno;

            return View();
        }

        public ActionResult RetailOutStock()
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }

            string orderno = DateTime.Now.ToString("yyMMddHHmmssf");
            ViewData["OrderNo"] = orderno;

            return View();
        }
        /// <summary>
        /// 加载发货代理
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadOutScaleC_User(C_UserSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and ( UserName+Name like '%" + condition.keyword + "%' or Province like '%" + condition.keyword + "%' or City like '%" + condition.keyword + "%')";
            }
            PageJsonModel<C_UserShow> page = new PageJsonModel<C_UserShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "( select c.PortraitUrl PortraitUrl,c.UserName UserName,c.ID ID, c.Name Name,c.Phone Phone,c.Province Province,c.City City,c.C_UserTypeID C_UserTypeID,c.DatCreate DatCreate,c.DatVerify DatVerify,c.state state,c.Identifier Identifier,c.Chief Chief,t.Name LevelName from C_User as  c left join C_UserType as t on t.Lever=c.C_UserTypeID ) as C_UserShow";
            page.strSelect = "* ";
            page.strWhere = " and Chief=" + CurrentUser.ID + where; // " or UserName='m2000'"
            page.strOrder = "ID ";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 加载发货订单
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadOutScaleOrder(OrderSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and OrderNo like '%" + condition.keyword + "%'";
            }
            PageJsonModel<Order> page = new PageJsonModel<Order>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order]";
            page.strSelect = "* ";
            page.strWhere = " and OrderState='待发货' and AuditState='已审核' and ParentUser='" + CurrentUser.UserName + "'" + where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBoolOutScale(string ID)
        {
            RequestResult result = new RequestResult();

            if (string.IsNullOrWhiteSpace(ID) || ID.Trim().Length < 6)
            {
                result.message = "条码错误，请检查后试。";
                result.success = false;
            }
            else
            {
                List<BarCode> CodeList = Scale.GetScanCode(ID, CurrentUser.UserName);

                if (CodeList != null && CodeList.Count > 0)
                {
                    result.data = CodeList;
                    result.message = "正确";
                    result.success = true;
                }
                else
                {
                    result.message = "未找到此条码或不在你的库存中";
                    result.success = false;
                }
            }

            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUpdateC_UserOutScale(string Agent, string Scale,string OutOrderNo)
        {
            RequestResult result = new RequestResult();
            bool IsOK = true;

            if (string.IsNullOrWhiteSpace(Agent))
            {
                IsOK = false;
                result.message = "姓名不能为空";
                result.success = false;
            }
            if (string.IsNullOrWhiteSpace(Scale))
            {
                IsOK = false;
                result.message = "正确条码不能为空";
                result.success = false;
            }

            if (IsOK)
            {
                List<BarCode> SmallCodeList = ScaleOutStoke.GetOutStockID(CurrentUser.UserName, Scale);

                string IDSet = "";
                foreach (BarCode b in SmallCodeList)
                {
                    if (IDSet != "") IDSet += ",";
                    IDSet += b.ID.ToString();
                }

                if (IDSet != "") ScaleOutStoke.ToOutStockAgent(CurrentUser.UserName, Agent, IDSet, OutOrderNo);

                result.data = SmallCodeList;
                result.message = "成功";
                result.success = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOutScaleDetailIndex()
        {
            return View(CurrentUser);
        }
        /// <summary>
        /// 出货详情
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetC_UserOutScaleDetail(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name+OutOrderNo+ProductName like '%" + condition.keyword + "%'";
            }
            if (!string.IsNullOrEmpty(condition.DatCreateB))
            {
                where += string.Format(" and CreateTime>={0}", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrEmpty(condition.DatCreateE))
            {
                where += string.Format(" and CreateTime<={0}", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            PageJsonModel<ScaleOutStokeShow> page = new PageJsonModel<ScaleOutStokeShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " (select COUNT(*) Qty,s.Shipper, s.ProductNo as ProductNumber,c.Name,s.CreateTime,s.OutOrderNo,p.ProductName,p.ProductImg,Consignee " +
                            "from ScaleOutStoke s left join C_User c on s.Consignee=c.UserName left join Product as p on s.ProductNo=p.ProductNumber " +
                            "group by s.ProductNo ,c.Name,s.CreateTime,s.Shipper,s.OutOrderNo,p.ProductName,p.ProductImg,Consignee ) as Detail ";
            page.strSelect = "* ";
            page.strWhere = " and Shipper='" + CurrentUser.UserName + "'" + where;
            page.strOrder = "CreateTime,OutOrderNo desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 出货祥情页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOutDetailIndex(string OutOrderNo, string ProductNo, string Consignee,int IDateTime)
        {
            ViewData["OutOrderNo"] = OutOrderNo;
            ViewData["ProductNo"] = ProductNo;
            ViewData["Consignee"] = Consignee;
            ViewData["IDateTime"] = IDateTime;
            return View();
        }
        public ActionResult GetOuttjIndex(string ID)
        {
            ViewData["OrderNo"] = ID;
            return View();
        }
        public ActionResult GetC_UserScaleDetail(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and isnull(BigCode,'')+isnull(MiddleCode,'')+SmallCode like '%" + condition.keyword + "%'";
            }

            PageJsonModel<OutScaleDetail> page = new PageJsonModel<OutScaleDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            
            page.strForm = "ScaleOutStoke";
            page.strSelect = "BigCode,MiddleCode,SmallCode";
            page.strWhere = string.Format(" and Shipper='{0}' and OutOrderNo='{1}' and ProductNo='{2}' and CreateTime={3} and Consignee='{4}'", CurrentUser.UserName, condition.OutOrderNo, condition.ProductNumber, condition.CreateTime, condition.Consignee) + where;
            page.strOrder = "SmallCode asc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetC_UserScaletj(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and ProductNo+ProductName like '%" + condition.keyword + "%'";
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and CreateTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and CreateTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }
            PageJsonModel<OutScaleDetail> page = new PageJsonModel<OutScaleDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;

            page.strForm = "(select count(SmallCode) as Qty,s.Shipper,s.ProductNo,p.ProductName,p.ProductImg " +
                            "from ScaleOutStoke s left join Product p on s.ProductNo=p.ProductNumber " +
                            "where Shipper='" + CurrentUser.UserName + "'" + where +
                            "group by s.Shipper,s.ProductNo,p.ProductName,ProductImg) as Detail";

            page.strSelect = "* ";
            page.strOrder = "Qty desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
