using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models.C_UserModel;
using Newtonsoft.Json;

namespace WeBusiness.Controllers
{
    public class MainController : BaseController
    {   
        public ActionResult Index()
        {
            //B_UserRoles BU = B_UserRoles.GetByUserName(CurrentUser.UserName);
            //if (CurrentUser.UserName != WeConfig.robot)
            //{
            //    ViewData["BR"] = B_Role.GetEntityByID(BU.RoleID);
            //}

            List<BaseMain> List = B_UserDesktopAuth.GetEntitysMainUserNames(CurrentUser.UserName);
            string bm = "";
            for (int i = 0; i < List.Count; i++)
            {
                bm = bm + List[i].MainID + ",";
            }

            List<BaseMain> Listbase = BaseMain.GetEntitysAll();
            string BB = "";
            for (int i = 0; i < Listbase.Count; i++)
            {
                BB = BB + Listbase[i].MainID + ",";
            }

            ViewData["BM"] = bm;
            ViewData["Z_BM"] = BB;

            ViewData["UserName"] = CurrentUser.UserName;
            ViewData["T_UserName"] = WeConfig.robot;
            ViewData["dj"] = CurrentUser.ID;
            MainShow main = Scale.GetScaleCount();
            return View(main);
        }
        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns></returns>
        public JsonResult getview(string url) 
        {

            List<TableViewConfig> configs = TableViewConfig.GetEntitys(url, CurrentUser.UserName);
            configs = configs.OrderByDescending(m=>m.Sort).ToList();
            //List<string> configsStr = configs.Select(item => item.ShowName ).ToList();

            return Json(configs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存视图
        /// </summary>
        /// <param name="ShowNames"></param>
        /// <param name="TableUrl"></param>
        /// <returns></returns>
        public ContentResult saveview(string[] ShowNames, string TableUrl)
        {
            TableViewConfig.Delete(TableUrl, CurrentUser.UserName);
            foreach(string ShowName in ShowNames)
            {

                //TableViewConfig oldconfig = TableViewConfig.GetEntity(TableUrl, CurrentUser.UserName, ShowName);
                //if (oldconfig == null) 
                //{
                  TableViewConfig config = new TableViewConfig();
                  config.ShowName = ShowName;
                  config.TableUrl = TableUrl;
                  config.Sort = ShowName.IndexOf(ShowName);
                  config.UserName = CurrentUser.UserName;
                  config.InsertAndReturnIdentity();
                //}
            }

            return Content("ok");
        }

        [B_MenuRightsTag("查看")]
        public ActionResult MyInfo()
        {
            B_User user = B_User.GetB_UserByUserName(CurrentUser.UserName);
            return View(user);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult EditPwd()
        {
            return View();
        }
        //[B_MenuRightsTag("修改密码", "Index")]
        public ActionResult toEditPwd(string oldpwd, string pwd, string confirmpwd)
        {
            B_User user = B_User.GetB_UserByUserName(CurrentUser.UserName);
            if (user.UserName == pwd)
            {
                return Content("账号和密码不能一致");
            }
            if (pwd.Length < 6)
            {
                return Content("密码长度必须高于6位");
            }
            if (user.PassWord != oldpwd)
            {
                return Content("旧密码错误");
            }
            if (pwd != confirmpwd)
            {
                return Content("两次输入密码不一致");
            }

            int rtn = B_User.EditPwdByUserName(pwd, CurrentUser.UserName);
            if (rtn > 0)
            {
                SYSLog.add("修改后台密码从[" + oldpwd + "]修改为[" + pwd + "]", "后台用户" + CurrentUser.UserName + "(" + CurrentUser.Name + ")", CurrentURL,"修改密码","电脑端后台");
                return Content("ok");
            }
            return Content("修改密码失败");
        }

        public ActionResult CusomerCensus()
        {
            string value = C_User.GetC_UserCountCennsus();
            if (string.IsNullOrWhiteSpace(value))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SalesOrder(int Year)
        {
            ScaleOutStoke ordersale = new ScaleOutStoke();
            return Content(ordersale.GetSalesOrder(Year));
        }
        public ActionResult CustomerPieCensus()
        {
            string value = C_User.GetPieC_UserCountCensus();
            if (string.IsNullOrWhiteSpace(value))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerPieCensus2()
        {
            
            string value = C_UserAdvice1.GetPieC_UserCountCensus();
            if (string.IsNullOrWhiteSpace(value))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerPie_cxySex_Census(string type)
        {
            string value = C_User.GetPieCustomerCountCensus(type);
            if (string.IsNullOrWhiteSpace(value))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult agentmapfenbu()
        {

            return View();
        }

        public JsonResult getagentmapfenbu()
        {
            string fileStr = "";
            List<string> json = new List<string>();
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory;
                if (!System.IO.Directory.Exists(filePath + "BaseFile\\")) { System.IO.Directory.CreateDirectory(filePath + "BaseFile\\"); }
                filePath = filePath + "BaseFile\\";
                //读取文件
                StreamReader sr = new StreamReader(filePath + "provincedata.json", Encoding.UTF8);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    fileStr += (line.ToString());
                }
                sr.Close();

                json = JsonConvert.DeserializeObject<List<string>>(fileStr);
            }
            catch (Exception ex) { }


            List<C_User_fb> fbs = C_User_fb.Getfbs();
            for (int i = 0; i < fbs.Count; i++)
            {
                var newname = json.FirstOrDefault(m => fbs[i].name.Contains(m));
                if (string.IsNullOrWhiteSpace(newname) == false)
                {
                    fbs[i].name = newname;
                }
            }

            return Json(fbs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult C_ConsumerFenbu()
        {
            return View();
        }

        public ActionResult C_ConsumercxyFenbu()
        {
            return View();
        }

        public JsonResult getC_Consumermapfenbu(string type)
        {
            string fileStr = "";
            List<string> json = new List<string>();
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory;
                if (!System.IO.Directory.Exists(filePath + "BaseFile\\")) { System.IO.Directory.CreateDirectory(filePath + "BaseFile\\"); }
                filePath = filePath + "BaseFile\\";
                //读取文件
                StreamReader sr = new StreamReader(filePath + "provincedata.json", Encoding.UTF8);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    fileStr += (line.ToString());
                }
                sr.Close();

                json = JsonConvert.DeserializeObject<List<string>>(fileStr);
            }
            catch { }

            List<C_Consumer_fb> fbs = C_Consumer_fb.Getfbs(type);
            for (int i = 0; i < fbs.Count; i++)
            {
                var newname = json.FirstOrDefault(m => fbs[i].name.Contains(m));
                if (string.IsNullOrWhiteSpace(newname) == false)
                {
                    fbs[i].name = newname;
                }
            }

            return Json(fbs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult agentmap() 
        {
            
            return View();
        }

        public JsonResult get_agents()
        {
            List<C_User> users = C_User.GetEntitysAll();
            return Json(users,JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_agents_page(BaseSearch condition)
        {
            PageJsonModel<C_UserVM> page = new PageJsonModel<C_UserVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_User left join [C_UserType] on C_User.C_UserTypeID=[C_UserType].Lever";
            page.strSelect = " C_User.*,[C_UserType].Name userTypeName  ";
            page.strWhere ="";

            page.strOrder = "C_User.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Close(string TextID)
        {
            if (CurrentUser.UserName != WeConfig.robot)
            {
                BaseMain BM = B_UserDesktopAuth.GetEntitysByStringMainID(TextID);
                if (B_UserDesktopAuth.RemoveUserDesktopAuth(BM.ID, CurrentUser.UserName))
                {
                    return Content("ok");
                }
                else
                {
                    return Content("隐藏失败！");
                }
            }
            return Content("隐藏失败！");
        }

        [B_MenuRightsTag("查看")]
        public ActionResult DesktopSet()
        {
            if (CurrentUser.UserName == WeConfig.robot)
            {
                ViewBag.MenuRights = BaseMain.GetEntitysAll();
            }
            else
            {
                ViewBag.MenuRights = B_UserDesktopAuth.GetUserDesktopAuth(CurrentUser.RoleID,CurrentUser.UserName);
            }

            List<B_UserDesktopAuth> roRightsList = B_UserDesktopAuth.GetEntitysMainRoleID(CurrentUser.RoleID, CurrentUser.UserName);
            HashSet<int> setRights = new HashSet<int>();
            foreach (B_UserDesktopAuth roRights in roRightsList)
            {
                setRights.Add(roRights.MainID);
            }

            ViewBag.HashRights = setRights;
            ViewData["UserName"] = CurrentUser.UserName;
            ViewData["C_UserName"] = WeConfig.robot;

            return View();
        }

        public ContentResult ToSetUserDesktop(string DesktopIDSet)
        {
            if (B_Role.SetUserDesktopAuth(CurrentUser.RoleID, CurrentUser.UserName, DesktopIDSet))
            {
                B_MenuRights.ClearHashMenuRights();
            }
            else
            {
                return Content("分配角色权限失败！");
            }

            return Content("ok");
        }
    }
}
