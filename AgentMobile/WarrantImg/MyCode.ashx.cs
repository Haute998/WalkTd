using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using ThoughtWorks.QRCode.Codec;
using WeModels;

namespace AgentMobile.WarrantImg
{
    /// <summary>
    /// MyCode 的摘要说明
    /// </summary>
    public class MyCode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = DESEncrypt.DesDecrypt(context.Request["id"].Replace(' ', '+'));
            QRCodeEncoder endocder = new QRCodeEncoder();
            //二维码背景颜色
            endocder.QRCodeBackgroundColor = System.Drawing.Color.White;
            //二维码编码方式
            endocder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //每个小方格的宽度
            endocder.QRCodeScale = 10;
            //二维码版本号
            endocder.QRCodeVersion = 5;
            //纠错等级
            endocder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            //将json川做成二维码
            Bitmap bitmap = endocder.Encode(id);
            Bitmap MyMap = new Bitmap(HttpContext.Current.Server.MapPath("/images/MyCode.jpg"));
            Graphics MyG = Graphics.FromImage(MyMap);

            MyG.DrawImage(bitmap, 150, MyMap.Width / 4, MyMap.Width / 2, MyMap.Width / 2 - MyMap.Width / 10);
            context.Response.Clear();
            context.Response.ContentType = "Image/jpg";
            MyMap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            MyMap.Dispose();
            MyG.Dispose();
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