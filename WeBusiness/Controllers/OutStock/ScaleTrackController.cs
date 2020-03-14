using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class ScaleTrackController : BaseController
    {
        //
        // GET: /ScaleTrack/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TrackDetail(string SmallCode)
        {
            List<TrackCode> ListTrack = new List<TrackCode>();

            Scale smallTrack = Scale.GetScaleSmallCode(SmallCode);
            List<ScaleOutStoke> smallOutTrackList = ScaleOutStoke.GetSmallCodeList(SmallCode);
            List<ScaleRtnStoke> smallRtnTrackList = ScaleRtnStoke.GetSmallCodeList(SmallCode);

            string ProductNo = "";
            string ProductName = "";
            if (!string.IsNullOrWhiteSpace(smallTrack.ProductNo))
            {
                Product product = Product.GetProductNo(smallTrack.ProductNo);
                ProductNo = product.ProductNumber;
                ProductName = product.ProductName;
            }

            string PDAUserSet = "";
            string AgentUser = "";
            string BoxType = "中箱";
            if (!PDAUserMsg.Param.MiddlePacking) BoxType = "大箱";

            if (!string.IsNullOrWhiteSpace(smallTrack.IntoPDAUser)) PDAUserSet += "'" + smallTrack.IntoPDAUser + "',";
            if (!string.IsNullOrWhiteSpace(smallTrack.LinkBigPDAUser)) PDAUserSet += "'" + smallTrack.LinkBigPDAUser + "',";
            if (!string.IsNullOrWhiteSpace(smallTrack.LinkMidPDAUser)) PDAUserSet += "'" + smallTrack.LinkMidPDAUser + "',";
            if (!string.IsNullOrWhiteSpace(smallTrack.OutPDAUser)) PDAUserSet += "'" + smallTrack.OutPDAUser + "',";

            if (PDAUserSet != "") PDAUserSet = PDAUserSet.Substring(0, PDAUserSet.Length - 1);

            List<PDAUser> pdauser = new List<PDAUser>();
            if (PDAUserSet != "") pdauser = PDAUser.GetUserNameSet(PDAUserSet);

            foreach (ScaleOutStoke outUser in smallOutTrackList)
            {
                AgentUser += "'" + (outUser.Shipper != "总部" ? outUser.Shipper : "") + "',";
                AgentUser += "'" + outUser.Consignee + "',";
            }

            foreach (ScaleRtnStoke rtnUser in smallRtnTrackList)
            {
                if (!string.IsNullOrWhiteSpace(rtnUser.Shipper) && rtnUser.Shipper != "总部") AgentUser += "'" + rtnUser.Shipper + "',";
                if (!string.IsNullOrWhiteSpace(rtnUser.Consignee)) AgentUser += "'" + rtnUser.Consignee + "',";
            }

            if (AgentUser != "") AgentUser = AgentUser.Substring(0, AgentUser.Length - 1);

            List<C_User> UserList = new List<C_User>();

            if (AgentUser != "") UserList = C_User.GetUserSet(AgentUser);

            #region 关联记录

            if (smallTrack.IsLinkMid)
            {
                TrackCode track = new TrackCode();
                track.ITime = smallTrack.LinkMidTime;
                track.STitle = "小标装箱";

                PDAUser PMUser = pdauser.Where(u => u.PUserName == smallTrack.LinkMidPDAUser).FirstOrDefault();

                track.SContent = "操作员：" + smallTrack.LinkMidPDAUser + (PMUser != null ? "-" + PMUser.PRealName : "") + ",将产品码(" + smallTrack.SmallCode + ")装入" + BoxType + "(" + smallTrack.MiddleCode + ")";
                if (PDAUserMsg.Param.IsLinkProduct)
                {
                    track.SContent += ",关联产品为：" + ProductNo + "-" + ProductName;
                }
                ListTrack.Add(track);
            }

            if (smallTrack.IsLinkBig)
            {
                TrackCode track = new TrackCode();
                track.ITime = smallTrack.LinkBigTime;
                track.STitle = "中标装箱";
                PDAUser PBUser = pdauser.Where(u => u.PUserName == smallTrack.LinkBigPDAUser).FirstOrDefault();
                track.SContent = "操作员：" + smallTrack.LinkBigPDAUser + (PBUser != null ? "-" + PBUser.PRealName : "") + ",将中箱码(" + smallTrack.MiddleCode + ")装入大箱(" + smallTrack.BigCode + ")";
                if (PDAUserMsg.Param.IsLinkProduct)
                {
                    track.SContent += ",关联产品为：" + ProductNo + "-" + ProductName;
                }

                ListTrack.Add(track);
            }

            #endregion

            #region 入库记录
            if (smallTrack.IsInto)
            {
                TrackCode track = new TrackCode();
                track.ITime = smallTrack.IntoTime;
                track.STitle = "产品入库";
                PDAUser PIUser = pdauser.Where(u => u.PUserName == smallTrack.IntoPDAUser).FirstOrDefault();
                string BarCode = smallTrack.IsLinkBig ? smallTrack.BigCode : smallTrack.IsLinkMid ? smallTrack.MiddleCode : smallTrack.SmallCode;
                string CodeType="箱码";
                if (BarCode == smallTrack.SmallCode) CodeType = "产品码";
                track.SContent = "操作员：" + smallTrack.IntoPDAUser + (PIUser != null ? "-" + PIUser.PRealName : "") + ",将" + CodeType + "(" + BarCode + ")入库";

                if (PDAUserMsg.Param.IsIntoProduct)
                {
                    track.SContent += ",入库产品为：" + ProductNo + "-" + ProductName;
                }

                ListTrack.Add(track);
            }
            #endregion

            #region 所有出货记录(包含经销商)
            foreach (ScaleOutStoke outcode in smallOutTrackList)
            {
                TrackCode track = new TrackCode();
                track.ITime = outcode.CreateTime;
                track.STitle = (outcode.Shipper == "总部" ? outcode.Shipper : "经销商") + "产品出货";

                C_User OUser=UserList.Where(u => u.UserName == outcode.Consignee).FirstOrDefault();
                C_User OUserShipper = UserList.Where(u => u.UserName == outcode.Shipper).FirstOrDefault();

                track.SContent = outcode.Shipper + (OUserShipper != null ? "-" + OUserShipper.Name : "") + ">出货>" + outcode.Consignee + (OUser != null ? "-" + OUser.Name : "") + "<br>";

                track.SContent += "出货单号：" + outcode.OutOrderNo;
                PDAUser POUser = pdauser.Where(u => u.PUserName == smallTrack.OutPDAUser).FirstOrDefault();
                if (outcode.Shipper == "总部") track.SContent += ",操作员：" + smallTrack.OutPDAUser + "-" + (POUser != null ? "-" + POUser.PRealName : "");

                if (PDAUserMsg.Param.IsOutProduct)
                {
                    track.SContent += ",出货产品为：" + ProductNo + "-" + ProductName;
                }
                ListTrack.Add(track);
            }
            
            #endregion

            #region 退货记录
            foreach (ScaleRtnStoke rtncode in smallRtnTrackList)
            {
                TrackCode track1 = new TrackCode();
                track1.ITime = rtncode.OutTime;
                track1.STitle = (rtncode.Shipper == "总部" ? rtncode.Shipper : "经销商") + "产品出货";
                C_User RUserShipper = UserList.Where(u => u.UserName == rtncode.Shipper).FirstOrDefault();
                C_User RUserConsignee = UserList.Where(u => u.UserName == rtncode.Consignee).FirstOrDefault();

                track1.SContent = rtncode.Shipper + (RUserShipper != null ? "-" + RUserShipper.Name : "") + ">出货>" + rtncode.Consignee + (RUserConsignee != null ? "-" + RUserConsignee.Name : "") + "<br>";

                track1.SContent += "出货单号：" + rtncode.OutOrderNo;
                if (rtncode.Shipper == "总部") track1.SContent += ",操作员：" + smallTrack.OutPDAUser + "-" + pdauser.Where(u => u.PUserName == smallTrack.OutPDAUser).FirstOrDefault().PRealName;
                ListTrack.Add(track1);

                TrackCode track2 = new TrackCode();
                track2.ITime = rtncode.ReturnTime;
                track2.STitle = "产品退货" + (rtncode.Shipper == "总部" ? rtncode.Shipper : "经销商");
                track2.SContent = rtncode.Consignee + (RUserConsignee != null ? "-" + RUserConsignee.Name : "") + ">退货>" + rtncode.Shipper + (RUserShipper != null ? "-" + RUserShipper.Name : "") + "<br>";
                track2.SContent += "退货单号 " + rtncode.OrderNo;
                ListTrack.Add(track2);
            }
            #endregion

            ViewData["BigCode"] = smallTrack.BigCode;
            ViewData["MiddleCode"]=smallTrack.MiddleCode;
            ViewData["SmallCode"] = smallTrack.SmallCode;
            ViewData["CodeTrack"] = ListTrack.OrderBy(m => m.ITime).ToList();
            ViewData["ProductNo"] = ProductNo;
            ViewData["ProductName"] = ProductName;

            return View();
        }

        public ActionResult ExportExcel(BigScaleSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select BigCode,SmallCode,p.ProductName ProductName,s.DatCreate DatCreate,c.UserName UserName,c.Name Name,c.Province,c.City,c.Area from ScaleOutStoke s left join Product p on s.P_ID=p.ProductNumber left join C_User c on s.Consignee=c.UserName left join Supplier s1 on s1.ID=s.GID where 1=1 ");
            
            if (!string.IsNullOrWhiteSpace(condition.SmallCode))
            {
                where.Append(string.Format(" and (ProductName like '%{0}%' or c.UserName like '%{0}%' or c.Name like '%{0}%' or c.Province like '%{0}%' or SmallCode like '%{0}%' or BigCode like '%{0}%' )", condition.SmallCode));
            }
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "外箱条码", "产品条码", "产品名称", "出库时间", "客户编号", "客户名称","省份","城市","乡镇"};
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "条码追踪信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetCodePage(BigScaleSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.SmallCode))
            {
                where += " and SmallCode='" + condition.SmallCode + "'";
            }

            PageJsonModel<Scale> page = new PageJsonModel<Scale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " Scale as a left join C_User as b on a.UserName=b.UserName left join Supplier as c on a.SupplierId=c.ID ";
            page.strSelect = " a.*,b.Name,c.Name as SupplierName ";
            page.strWhere = where;
            page.strOrder = " a.StateID desc ";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
