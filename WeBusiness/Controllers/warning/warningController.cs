using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class warningController : BaseController
    {
        //
        // GET: /warning/

       [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

       public ActionResult agentmapfenbu()
       {

           return View();
       }




       public JsonResult GetPage()
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


           List<SelScale> fbs = SelScale.GetSelScale();
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

    }
}
