using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class C_UserController : BaseController
    {
        /// <summary>
        /// 直属代理
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult DirectlyIndex()
        {
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "DirectlyIndex")]
        public ActionResult ExportExcel(C_UserSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select UserName,Name,wxNo,Phone,Province,City,Area,WxQRCode from [C_User] where 1=1 ");
            where.Append("and state='已审核'  and Chief=0  ");
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where.Append(string.Format(" and (Name like '%{0}%' or Phone like '%{0}%' or Identifier like '%{0}%' or LevelName like '%{0}%')", condition.keyword));
            }
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "经销商编号", "经销商名称", "联系人", "联系方式", "省份", "市区", "城镇", "详细地址" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "直属客户信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        [B_MenuRightsTag("添加", "DirectlyIndex")]
        public ActionResult DirectlyIndex_Add()
        {
            return View();
        }

        [B_MenuRightsTag("添加", "DirectlyIndex")]

        public ActionResult DirectlyIndex_toAdd(C_User para)
        {
            C_UserType usertype = C_UserType.GetMaxLevel_UserType();

            para.state = "已审核";
            para.Chief = 0;
            para.IsValid = true;
            var UType = Request.Params["C_UserTypeID"].Split(',')[1] == "" ? 0 : Convert.ToInt32(Request.Params["C_UserTypeID"].Split(',')[1]);
            if (UType > 1)
            {
                para.C_UserTypeID = UType;
            }
            else
            {
                para.C_UserTypeID = usertype.Lever;
            }

            // return Content("用户等级:" + UType);

            para.DatCreate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(para.Name))
            {
                return Content("姓名不能为空");
            }
            if (string.IsNullOrWhiteSpace(para.Phone))
            {
                return Content("手机号不能为空");
            }
            if (para.Phone.Length != 11)
            {
                return Content("请输入正确的手机号码");
            }
            if (para.Province == "请选择")
            {
                return Content("请选择省份");
            }
            if (para.City == "请选择")
            {
                return Content("请选择城市");
            }
            if (para.Area == "请选择")
            {
                return Content("请选择地区");
            }
            if (C_User.GetPhoneCnt(para.Phone) > 0)
            {
                return Content("手机号码重复！");
            }

            HttpPostedFileBase upfile = Request.Files["Img"];
            if (upfile != null)
            {
                if (upfile.ContentLength >= (5242880))
                {
                    return Content("fail|请上传5M以内的文件！");
                }

                string imgName = DateTime.Now.ToString("yyyy-MM-dd-HH-ss") + DateTime.Now.Ticks;
                string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
                para.CardUrl = "/File/C_User/" + CurrentUser.UserName + "_" + imgName + ext;

                try
                {
                    if (!Directory.Exists(Server.MapPath("~/File/C_User/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/File/C_User/"));
                    }
                    upfile.SaveAs(Server.MapPath(para.CardUrl));
                }
                catch (Exception ex)
                {
                    DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
                }
            }
            else
            {
                para.CardUrl = "";
            }
            para.PassWord = para.Phone.Substring(para.Phone.Length - 6, 6);


            //if (C_User.GetC_UserByUserName(para.UserName) != null)
            //{
            //    return Content("用户编号已存在，不可重复添加！");
            //}


            para.ID = para.InsertAndReturnIdentity();

            /*自动添加用户编号*/
            int idLen = para.ID.ToString().Length;
            string idDQ = para.ID.ToString();
            if (idLen < 5)
            {
                string zero = new string('0', 5 - idLen);
                idDQ = zero + idDQ;
            }
            para.UserName = usertype.TypeCode + idDQ;
            para.Identifier = usertype.TypeCode + idDQ;

            if (para.UpdateByID() > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("保存出错");
            }


            //if (para.InsertAndReturnIdentity() > 0)
            //{
            //    return Content("ok");
            //}
            //return Content("添加出错");
        }

        #region
        //public ActionResult DirectlyIndex_toAdd(C_User para)
        //{
        //    C_UserType usertype = C_UserType.GetMaxLevel_UserType();

        //    para.state = "已审核";
        //    para.Chief = 0;
        //    para.C_UserTypeID = usertype.Lever;

        //    para.DatCreate = DateTime.Now;
        //    if (string.IsNullOrWhiteSpace(para.Name))
        //    {
        //        return Content("姓名不能为空");
        //    }
        //    if (string.IsNullOrWhiteSpace(para.Phone))
        //    {
        //        return Content("手机号不能为空");
        //    }
        //    if (para.Phone.Length != 11)
        //    {
        //        return Content("请输入正确的手机号码");
        //    }
        //    if (para.Province == "请选择")
        //    {
        //        return Content("请选择省份");
        //    }
        //    if (para.City == "请选择")
        //    {
        //        return Content("请选择城市");
        //    }
        //    if (para.Area == "请选择")
        //    {
        //        return Content("请选择地区");
        //    }

        //    HttpPostedFileBase upfile = Request.Files["Img"];
        //    if (upfile != null)
        //    {
        //        if (upfile.ContentLength >= (5242880))
        //        {
        //            return Content("fail|请上传5M以内的文件！");
        //        }

        //        string imgName = DateTime.Now.ToString("yyyy-MM-dd-HH-ss") + DateTime.Now.Ticks;
        //        string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
        //        para.CardUrl = "/File/C_User/" + CurrentUser.UserName + "_" + imgName + ext;

        //        try
        //        {
        //            if (!Directory.Exists(Server.MapPath("~/File/C_User/")))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/File/C_User/"));
        //            }
        //             upfile.SaveAs(Server.MapPath(para.CardUrl));
        //        }
        //        catch (Exception ex)
        //        {
        //            DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
        //        }
        //    }
        //    else
        //    {
        //        para.CardUrl = "";
        //    }
        //    para.PassWord = para.Phone.Substring(para.Phone.Length - 6, 6);
        //    para.ID = para.InsertAndReturnIdentity();

        //    int idLen = para.ID.ToString().Length;
        //    string idDQ = para.ID.ToString();
        //    if (idLen < 5)
        //    {
        //        string zero = new string('0', 5 - idLen);
        //        idDQ = zero + idDQ;
        //    }
        //    para.UserName = usertype.TypeCode + idDQ;
        //    para.Identifier = usertype.TypeCode + idDQ;

        //    if (para.UpdateByID() > 0)
        //    {
        //        return Content("ok");
        //    }
        //    return Content("添加出错");
        //}
        #endregion

        [B_MenuRightsTag("导入", "DirectlyIndex")]
        public ActionResult inportgo()
        {

            return View();
        }

        [B_MenuRightsTag("导入", "DirectlyIndex")]

        public ActionResult DirectlyIndex_inport()
        {
            C_UserType usertype = C_UserType.GetMaxLevel_UserType();



            int idLen = C_User.GetTopUseID().ToString().Length;
            string idDQ = (C_User.GetTopUseID() + 1).ToString();
            if (idLen < 5)
            {
                string zero = new string('0', 5 - idLen);
                idDQ = zero + idDQ;
            }
            string msg = "";
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/Codetxt/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Codetxt/"));
                }
                if (file.ContentLength == 0 || file == null)
                {
                    msg = "上传的文件没有内容！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }
                if (file.ContentLength > 3242880)
                {
                    msg = "上传的文件不能超过3MB！！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }
                if (ext != ".txt")
                {
                    msg = "上传文件格式不对！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }
                string DatNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                file.SaveAs(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
                StreamReader sr = new StreamReader(Server.MapPath("~/Codetxt/" + DatNow + ".txt"), Encoding.Default);
                String line;
                var i = 0;
                int idDQ_num = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    if (i == 1)
                    {
                        continue;
                    }
                    try
                    {


                        string[] list = line.ToString().Split(',');
                        if (string.IsNullOrWhiteSpace(list[0]) || string.IsNullOrWhiteSpace(list[1]))
                        {
                            continue;
                        }
                        if (C_User.GetPhoneCnt(list[1]) > 0 || C_User.GetNameCnt(list[0]) > 0)
                        {
                            SYSLog.add("导入失败(数据重复)：手机号-" + list[1] + "||经销商名称-" + list[0], "电脑端后台用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")", CurrentURL, "导入用户", "电脑后台");
                            DAL.Log.Instance.Write("数据重复：手机号-" + list[1] + "||经销商名称-" + list[0], "导入用户失败");
                            continue;
                        }
                        /* var ls = idDQ.Length;
                         if (Convert.ToInt32(idDQ)==idDQ_num)
                         {
                             idDQ = (idDQ_num + 1).ToString();
                             idDQ_num = Convert.ToInt32(idDQ);
                         }
                         else
                         {
                             idDQ_num = Convert.ToInt32(idDQ);0
                         }
                         if (ls > idDQ.ToString().Length)
                         {
                             int length1 = ls - idDQ.ToString().Length;
                             if (length1 == 5) idDQ = "00000" + idDQ;
                             if (length1 == 4) idDQ = "0000" + idDQ;
                             if (length1 == 3) idDQ = "000" + idDQ;
                             if (length1 == 2) idDQ = "00" + idDQ;
                             if (length1 == 1) idDQ = "0" + idDQ;
                         }*/
                        C_User para = new C_User();
                        para.state = "已审核";
                        para.Chief = 0;
                        para.C_UserTypeID = 1;
                        para.UserName = usertype.TypeCode + (C_User.GetTopUseID() + 1 + 10000);
                        //经销商名称 联系人 联系方式 省份 市区 城镇 详细地址

                        para.Name = list[0];
                        para.Phone = list[2];
                        para.PassWord = para.Phone.Substring(para.Phone.Length - 6, 6);
                        para.Card = "";
                        para.Identifier = usertype.TypeCode + (C_User.GetTopUseID() + 1 + 10000);
                        para.wxNo = list[1];
                        if (list.Count() > 4) para.Province = string.IsNullOrWhiteSpace(list[3]) ? "" : list[4];
                        if (list.Count() > 5) para.City = string.IsNullOrWhiteSpace(list[4]) ? "" : list[5];
                        if (list.Count() > 6) para.Area = string.IsNullOrWhiteSpace(list[5]) ? "" : list[6];
                        if (list.Count() > 7) para.WxQRCode = string.IsNullOrWhiteSpace(list[6]) ? "" : list[7];
                        para.DatCreate = DateTime.Now;
                        if (string.IsNullOrWhiteSpace(para.UserName))
                        {
                            return Content("经销商编号不能为空");
                        }
                        if (string.IsNullOrWhiteSpace(para.Name))
                        {
                            return Content("姓名不能为空");
                        }
                        if (string.IsNullOrWhiteSpace(para.Identifier))
                        {
                            return Content("授权编号不能为空");
                        }
                        if (RepeatHelper.NoRepeat("C_User", "Identifier", para.Identifier, para.ID) > 0)
                        {
                            return Content("授权编号已存在");
                        }



                        para.InsertAndReturnIdentity();
                    }
                    catch (Exception e)
                    {
                        DAL.Log.Instance.Write(e.Message, "导入用户失败");
                        continue;
                    }
                }
                sr.Close();
                System.IO.File.Delete(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
                msg = "导入成功！";
                TempData["ToIndex_err"] = msg;
                return View("DirectlyIndex");
            }
            catch
            {
                msg = "导入失败,文件太大！！";
                TempData["ToIndex_err"] = msg;
                return View("DirectlyIndex");
            }

        }

        //public ActionResult DirectlyIndex_inport()
        //{
        //    string msg = "";
        //    try
        //    {
        //        var file = Request.Files[0];
        //        string path = Request.MapPath("~/");
        //        string ext = Path.GetExtension(file.FileName);//获得文件扩展名
        //        if (!Directory.Exists(Server.MapPath("~/Codetxt/")))
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/Codetxt/"));
        //        }
        //        if (file.ContentLength == 0 || file == null)
        //        {
        //            msg = "上传的文件没有内容！";
        //            TempData["ToIndex_err"] = msg;
        //            return View();
        //        }
        //        if (file.ContentLength > 3242880)
        //        {
        //            msg = "上传的文件不能超过3MB！！";
        //            TempData["ToIndex_err"] = msg;
        //            return View();
        //        }
        //        if (ext != ".txt")
        //        {
        //            msg = "上传文件格式不对！";
        //            TempData["ToIndex_err"] = msg;
        //            return View();
        //        }
        //        string DatNow = DateTime.Now.ToString("yyyyMMddHHmmss");
        //        file.SaveAs(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
        //        StreamReader sr = new StreamReader(Server.MapPath("~/Codetxt/" + DatNow + ".txt"), Encoding.Default);
        //        String line;
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            string[] list = line.ToString().Split(' ');
        //            C_User para = new C_User();
        //            para.state = "已审核";
        //            para.Chief = 0;
        //            para.C_UserTypeID = 1;
        //            para.UserName = "p" + (C_User.GetTopUseID() + 1 + 1000);

        //            para.Name = list[0];
        //            para.Phone = list[1];
        //            para.PassWord = list[2];
        //            para.Card = list[3];
        //            para.Identifier = list[4];
        //            para.wxNo = list[5];

        //            if (string.IsNullOrWhiteSpace(para.Name))
        //            {
        //                return Content("姓名不能为空");
        //            }
        //            if (string.IsNullOrWhiteSpace(para.Identifier))
        //            {
        //                return Content("授权编号不能为空");
        //            }
        //            if (RepeatHelper.NoRepeat("C_User", "Identifier", para.Identifier, para.ID) > 0)
        //            {
        //                return Content("授权编号已存在");
        //            }



        //            para.InsertAndReturnIdentity();
        //        }
        //        sr.Close();
        //        System.IO.File.Delete(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
        //        msg = "导入成功！";
        //        TempData["ToIndex_err"] = msg;
        //        return View("DirectlyIndex");
        //    }
        //    catch
        //    {
        //        msg = "导入失败,文件太大！！";
        //        TempData["ToIndex_err"] = msg;
        //        return View("DirectlyIndex");
        //    }

        //}

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private string StrWhere(C_UserSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Name like '%{0}%' or Phone like '%{0}%' or Identifier like '%{0}%' or LevelName like '%{0}%' or (Province+City+Area) like '%{0}%')", condition.keyword);
            }
            return where;
        }
        /// <summary>
        /// 直属查询条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>

        public ActionResult GetPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已审核'  and Chief=0 ";
            return GetPages(condition, where);
        }

        /// <summary>
        /// 删除到回收站
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("删除", "DirectlyIndex")]
        public ActionResult GetUserDel(int ID)
        {
            C_User cuser = C_User.GetEntityByID(ID);
            C_User.GetUpdateChief(cuser.Chief, ID);
            int rtn = C_User.GetUpdateStateDel(ID);
            return Content(rtn > 0 ? "ok" : "删除出错了！！");
        }
        /// <summary>
        /// 彻底删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("彻底删除", "DirectlyIndex")]
        public ActionResult GetUserDelete(int ID)
        {
            C_User cuser = C_User.GetEntityByID(ID);
            C_User.GetUpdateChief(cuser.Chief, ID);
            int rtn = C_User.DeleteByID(ID);
            return Content(rtn > 0 ? "ok" : "删除出错了！！");
        }
        /// <summary>
        /// 拉成黑名单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("拉黑", "DirectlyIndex")]
        public ActionResult GetUserBlack(int ID)
        {
            C_User cuser = C_User.GetEntityByID(ID);
            C_User.GetUpdateChief(cuser.Chief, ID);
            int rtn = C_User.GetUpdateStateBlack(ID);
            return Content(rtn > 0 ? "ok" : "拉黑出错了！！");
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("修改", "DirectlyIndex")]
        public ActionResult DirectlyEditIndex(int ID)
        {
            C_User cUser = C_User.GetEntityByID(ID);
            return View(cUser);
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="cUser"></param>
        /// <returns></returns>
        [B_MenuRightsTag("修改", "DirectlyEditIndex")]
        public ActionResult GetDirectlyEdit(C_User cUser)
        {
            if (string.IsNullOrWhiteSpace(cUser.Name))
            {
                return Content("名称不能为空！！");
            }
            if (string.IsNullOrWhiteSpace(cUser.Phone))
            {
                return Content("电话不能为空！！");
            }
            Regex PhoneReg = new Regex("[0-9]{11,11}");
            if (!PhoneReg.IsMatch(cUser.Phone))
            {
                return Content("电话格式不对！！");
            }

            if (cUser.Province == "请选择")
            {
                return Content("请选择省份");
            }
            if (cUser.City == "请选择")
            {
                return Content("请选择城市");
            }
            if (cUser.Area == "请选择")
            {
                return Content("请选择地区");
            }

            //身份证正则表达式(15位)    
            // isIDCard1=/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/;    
            //身份证正则表达式(18位)      
            // isIDCard2=/^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$/; 
            //Regex CardReg = new Regex(@"[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}");
            //if (!CardReg.IsMatch(cUser.Card))
            //{
            //    return Content("身份证格式不对！！");
            //}
            C_User olduser = C_User.GetEntityByID(cUser.ID);
            olduser.Name = cUser.Name;
            olduser.Phone = cUser.Phone;
            olduser.wxNo = cUser.wxNo;
            olduser.Card = cUser.Card;
            olduser.C_UserTypeID = cUser.C_UserTypeID;
            olduser.Province = cUser.Province;
            olduser.City = cUser.City;
            olduser.Area = cUser.Area;
            olduser.WxQRCode = cUser.WxQRCode;
            HttpPostedFileBase upfile = Request.Files["Img"];
            if (upfile != null)
            {

                //判断文件大小是否符合要求  
                if (upfile.ContentLength >= (5242880))
                {
                    return Content("fail|请上传5M以内的文件！");
                }
                string imgName = DateTime.Now.ToString("yyyy-MM-dd-HH-ss") + DateTime.Now.Ticks;
                string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
                try
                {
                    if (string.IsNullOrWhiteSpace(olduser.CardUrl) == false)
                    {
                        string delFile = Server.MapPath("~") + olduser.CardUrl;
                        System.IO.File.Delete(delFile);
                    }
                }
                catch (Exception ex)
                {
                    DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
                }
                olduser.CardUrl = "/File/C_User/" + CurrentUser.UserName + "_" + imgName + ext;

                try
                {
                    if (!Directory.Exists(Server.MapPath("~/File/C_User/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/File/C_User/"));
                    }
                    upfile.SaveAs(Server.MapPath(olduser.CardUrl));
                }
                catch (Exception ex)
                {
                    DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
                }
            }
            else
            {
                olduser.CardUrl = olduser.CardUrl;
            }

            int rtn = olduser.UpdateByID();


            C_User.DatPwdChangeByUserName(olduser.UserName);

            return Content(rtn > 0 ? "ok" : "编辑出错了！！");
        }
        /// <summary>
        /// 转移下级代理
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("转移", "DirectlyIndex")]
        public ActionResult GetUserChiefIndex(int ID)
        {
            C_User cUser = C_User.GetEntityByID(ID);
            return View(cUser);
        }
        /// <summary>
        /// 修改上级
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("修改上级", "DirectlyIndex")]
        public ActionResult GetChangeUserChiefIndex(int ID)
        {
            C_User cUser = C_User.GetEntityByID(ID);
            ViewBag.User = cUser;
            return View();
        }
        /// <summary>
        /// 查看直属下级代理信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看直属下级", "DirectlyIndex")]
        public ActionResult UserIDVerifyIndex(int ID)
        {
            C_User cUser = C_User.GetEntityByID(ID);
            return View(cUser);
        }

        public ActionResult UserIDVerifyPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已审核' and Chief=" + condition.ID;
            return GetPages(condition, where);
        }
        /// <summary>
        /// 转移下级代理条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetChiefPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已审核'  and  ID!=" + condition.ID + " and C_UserTypeID<=" + condition.C_UserTypeID;
            return GetPages(condition, where);
        }
        /// <summary>
        /// 转移下级代理条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetOldChiefPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已审核'  and  ID!=" + condition.ID + " and C_UserTypeID<" + condition.C_UserTypeID;
            return GetPages(condition, where);
        }
        /// <summary>
        /// 转移下级代理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OldId"></param>
        /// <returns></returns>
        [B_MenuRightsTag("转移", "GetUserChiefIndex")]
        public ActionResult GetChangeChief(int id, int OldId)
        {
            int count = C_User.GetChiefInt(OldId);
            if (count <= 0)
            {
                return Content("您没有下级,无法转移！！");
            }
            int rtn = C_User.GetUpdateChief(id, OldId);
            return Content(rtn > 0 ? "ok" : "出错了！！");
        }
        /// <summary>
        /// 修改上级代理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OldId"></param>
        /// <returns></returns>
        [B_MenuRightsTag("转移", "GetChangeUserChiefIndex")]
        public ActionResult GetChangeOldChief(int id, int OldId)
        {
            int rtn = C_User.GetUpdateOldChief(id, OldId);
            return Content(rtn > 0 ? "ok" : "出错了！！");
        }
        /// <summary>
        /// 转移到总部
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OldId"></param>
        /// <returns></returns>
        [B_MenuRightsTag("转移", "GetUserChiefIndex")]
        public ActionResult GetChangeChiefBy(int id, int OldId)
        {
            int count = C_User.GetChiefInt(OldId);
            if (count <= 0)
            {
                return Content("您没有下级,无法转移！！");
            }
            int rtn = C_User.GetUpdateChief(id, OldId);
            return Content(rtn > 0 ? "ok" : "出错了！！");
        }
        /// <summary>
        /// 直属授权审核
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult DirectlyVerifyIndex()
        {
            return View();
        }
        /// <summary>
        /// 未审核条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetNoVerifyPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='未审核' and Chief=0";
            return GetPages(condition, where);
        }
        /// <summary>
        /// 直属审核
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("审核", "DirectlyVerifyIndex")]
        public ActionResult GetVerify(int ID)
        {
            C_User cUser = C_User.GetEntityByID(ID);
            cUser.state = "已审核";
            cUser.DatVerify = DateTime.Now;
            int rtn = cUser.UpdateByID();
            return Content(rtn > 0 ? "ok" : "出错了！！");
        }
        /// <summary>
        /// 黑名单
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult BlackIndex()
        {
            return View();
        }
        /// <summary>
        /// 修改成白名单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        //[B_MenuRightsTag("白名单", "BlackIndex")]
        public ActionResult GetNoBlack(int ID)
        {
            int rtn = C_User.GetUpdateStateWhite(ID);
            return Content(rtn > 0 ? "ok" : "出错了！！");
        }
        /// <summary>
        /// 黑名单条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetBlackPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='黑名单' ";
            return GetPages(condition, where);
        }
        /// <summary>
        /// 回收站
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult DelUserIndex()
        {
            return View();
        }
        /// <summary>
        /// 回收站条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetUserDelPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已删除' ";
            return GetPages(condition, where);
        }
        private ActionResult GetPages(C_UserSearch condition, string where)
        {
            PageJsonModel<C_UserShow> page = new PageJsonModel<C_UserShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"(select c.*,t.Name LevelName,I.Name IName from C_User as  c left join C_UserType as t on t.Lever=c.C_UserTypeID left join C_User I on c.Introducer=I.UserName) as C_UserShow";
            page.strSelect = " * ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ID desc";
            }

            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetLeverPage(int ID)
        {
            C_User c_user = C_User.GetEntityByID(ID);
            return View(c_user);
        }
        [B_MenuRightsTag("修改上级", "GetChangeUserChiefIndex")]
        public ActionResult GetUpdateLever(C_User cUser)
        {
            C_User c_user = C_User.GetEntityByID(cUser.ID);

            string oldC_UserTypeName = C_UserType.GetNameByLever(c_user.C_UserTypeID.ToString());


            c_user.C_UserTypeID = cUser.C_UserTypeID;


            string newC_UserTypeName = C_UserType.GetNameByLever(c_user.C_UserTypeID.ToString());

            int rtn = c_user.UpdateByID();

            if (rtn > 0)
            {

                SYSLog.add("将经销商[" + c_user.Name + "]的等级从[" + oldC_UserTypeName + "]修改为[" + newC_UserTypeName + "]", "电脑端后台用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")", CurrentURL, "修改经销商级别", "电脑后台");
            }

            return Content(rtn > 0 ? "ok" : "出错了");
        }

        public ActionResult CustomerMsg(string ID)
        {
            C_User cuser = C_User.GetC_UserByUserName(ID);
            return View(cuser);
        }
    }
}
