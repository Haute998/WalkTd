using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using WeModels;

namespace AgentMobile.WarrantImg
{
    /// <summary>
    /// MyWarrantImg 的摘要说明
    /// </summary>
    public class MyWarrantImg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string id = context.Request["id"].ToString();
            if (string.IsNullOrWhiteSpace(id))
            {
                context.Response.Write("网络出错了！");
            }
            C_User cuser = C_User.GetC_UserByUserName(id);

            if (cuser == null)
            {
                context.Response.Write("没有此用户！");
            }
            if (cuser.state != "已审核")
            {
                context.Response.Write("用户未授权");
            }
            Bitmap MyMap = new Bitmap(HttpContext.Current.Server.MapPath("/images/warrant.jpg"));
            Graphics MyG = Graphics.FromImage(MyMap);
            SizeF sizeFName = MyG.MeasureString(cuser.Name, new Font("微软雅黑", 5));
            int NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            //用户名550 850
            MyG.DrawString(cuser.Name, new Font("微软雅黑", 5), new SolidBrush(Color.Black), new Point(NameWidth, 340));
            sizeFName = MyG.MeasureString(cuser.wxNo, new Font("微软雅黑", 5));
            NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            //用户名UserName 1050  1550
            MyG.DrawString(cuser.wxNo, new Font("微软雅黑", 5), new SolidBrush(Color.Black), new Point(NameWidth, 370));
            ////IcCard
            //MyG.DrawString(cuser.Card, new Font("微软雅黑", 30), new SolidBrush(Color.White), new Point(190, 465));
            //级别
            C_UserType type = C_UserType.GetEntittyC_Type(cuser.C_UserTypeID);
            if (type == null)
            {
                context.Response.Write("数据有误！");
            }
            sizeFName = MyG.MeasureString(type.Name, new Font("微软雅黑", 10));
            NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            MyG.DrawString(type.Name, new Font("微软雅黑", 10), new SolidBrush(Color.Red), new Point(NameWidth, 500));
            //授权
            sizeFName = MyG.MeasureString(cuser.Phone, new Font("微软雅黑", 5));
            NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            MyG.DrawString(cuser.Phone, new Font("微软雅黑", 5), new SolidBrush(Color.Black), new Point(NameWidth, 410));
            sizeFName = MyG.MeasureString(cuser.Card.Substring(0, 4) + "***" + cuser.Card.Substring(12, 4), new Font("微软雅黑", 5));
            NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            //授权
            MyG.DrawString(cuser.Card.Substring(0, 4) + "***" + cuser.Card.Substring(12, 4), new Font("微软雅黑", 5), new SolidBrush(Color.Black), new Point(NameWidth, 440));
            //授权
            sizeFName = MyG.MeasureString(cuser.DatVerify.ToString(), new Font("微软雅黑", 5));
            NameWidth = Convert.ToInt32((MyMap.Size.Width - sizeFName.Width) / 2);
            MyG.DrawString(cuser.DatVerify.ToString(), new Font("微软雅黑", 5), new SolidBrush(Color.Black), new Point(NameWidth, 665));
            ////授权
            //MyG.DrawString(cuser.DatVerify.Day.ToString(), new Font("微软雅黑", 16), new SolidBrush(Color.Black), new Point(1000, 2950));
            context.Response.Clear();
            context.Response.ContentType = "Image/jpg";
            MyMap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            MyMap.Dispose();
            MyG.Dispose();
            context.Response.End();

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}