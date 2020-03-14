using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class imghelper
    {
        //将base64编码的字符串转为图片并保存
        public static string SaveBase64Image(string source, string savedbname, string savefilename)
        {
            try
            {
                var now = DateTime.Now;
                string strbase64 = source.Substring(source.IndexOf(',') + 1);
                strbase64 = strbase64.Trim('\0');
                byte[] arr = Convert.FromBase64String(strbase64);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    Bitmap bmp = new Bitmap(ms);
                    //if (!Directory.Exists(filePath))
                    //    Log.Debug("没有Directory");
                    //Directory.CreateDirectory(filePath);
                    //新建第二个bitmap类型的bmp2变量。
                    Bitmap bmp2 = new Bitmap(bmp, bmp.Width, bmp.Height);
                    //将第一个bmp拷贝到bmp2中
                    Graphics draw = Graphics.FromImage(bmp2);
                    draw.DrawImage(bmp, 0, 0);
                    draw.Dispose();
                    bmp2.Save(savefilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //bmp.Save("test.bmp", ImageFormat.Bmp);
                    //bmp.Save("test.gif", ImageFormat.Gif);
                    //bmp.Save("test.png", ImageFormat.Png);
                    ms.Close();
                    return savedbname;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool Base64SaveImage(string file_path, string img_string, ref string file_name)
        {
            try
            {
                string[] img_array = img_string.Split(',');
                byte[] arr = Convert.FromBase64String(img_array[1]);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    Bitmap bmp = new Bitmap(ms);
                    if (img_array[0].ToLower() == "data:image/jpeg;base64")
                    {
                        file_name=file_name+".jpg";
                        file_path = file_path + file_name;
                        bmp.Save(file_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else if (img_array[0].ToLower() == "data:image/png;base64")
                    {
                        file_name = file_name + ".png";
                        file_path = file_path + file_name;
                        bmp.Save(file_path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        file_name = "";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
