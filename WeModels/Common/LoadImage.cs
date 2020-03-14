using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.ModelService.Common
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//将该类设置为com可访问
    public class LoadImage
    {

        /// <summary>
        /// 从Url保存图片到本地
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FileName, string Url)
        {
            bool value = false;
            WebResponse response = null;
            Stream stream = null;

            try
            {
                WebRequest request = WebRequest.Create(Url);
                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    value = SaveBinaryFile(response, FileName);
                }

            }
            catch (Exception err)
            {
                string aa = err.ToString();
            }
            return value;
        }

       private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                //if (!File.Exists(FileName))
                //{

                    Stream outStream = System.IO.File.Create(FileName);
                    Stream inStream = response.GetResponseStream();

                    int l;
                    do
                    {
                        l = inStream.Read(buffer, 0, buffer.Length);
                        if (l > 0)
                            outStream.Write(buffer, 0, l);
                    }
                    while (l > 0);

                    outStream.Close();
                    inStream.Close();
                //}
            }
            catch
            {
                Value = false;
            }

            return Value;
        }
    }




}
