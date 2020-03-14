using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL;
using HtmlAgilityPack;
using System.IO;
using System.Web;

namespace WeModels
{
    public class C_UserArticleSearch : BaseSearch
    {
    }
    public partial class C_UserArticle
    {
        public int EditByID()
        {
            string strSql = "UPDATE [C_UserArticle] SET title=@title,Descs=@Descs,contents=@contents,DatEdit=@DatEdit,coverImgUrl=@coverImgUrl WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@title",_title),
                new System.Data.SqlClient.SqlParameter("@Descs",_descs),
                new System.Data.SqlClient.SqlParameter("@contents",_contents),
                new System.Data.SqlClient.SqlParameter("@DatEdit",_datedit),
                 new System.Data.SqlClient.SqlParameter("@coverImgUrl",_coverimgurl),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }


        public static string getwxarticleByUrl(string url, string username, string path)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = wc.DownloadData(url);



                string html = Encoding.UTF8.GetString(pageData);//.ASCII.GetString

                //建立获取网页标题正则表达式  
                String regex = @"<title>.+</title>";

                //返回网页标题  
                String title = Regex.Match(html.ToString(), regex).ToString();
                title = Regex.Replace(title, @"[\""]+", "");
                title = title.Replace("<title>", "").Replace("</title>", "");

                //描述
                Match Desc = Regex.Match(html, "<meta name=\"DESCRIPTION\" content=\"([^<]*)\">", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string strdesc = Desc.Groups[1].Value;



                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(html);

                HtmlNode nodeContent = doc.GetElementbyId("js_content");

                if (nodeContent == null)
                {
                    return "fail|没有抓取到内容呢";
                }

                string js_content = nodeContent.InnerHtml;//内容

                if (string.IsNullOrWhiteSpace(js_content))
                {
                    return "fail|抓取不到内容";
                }


                //文章对象
                C_UserArticle article = new C_UserArticle();
                article.title = title;
                article.Descs = strdesc;
                article.C_UserName = username;
                article.contents = js_content;
                article.DatCreate = DateTime.Now;
                article.DatEdit = DateTime.Now;
                article.ID = article.InsertAndReturnIdentity();


                MatchCollection matches = Regex.Matches(js_content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

                int i = 0;
                foreach (Match match in matches)
                {
                    string imgtag = match.Value.ToString();
                    string imgurl = GetTitleContent(imgtag, "img", "data-src");

                    if (string.IsNullOrWhiteSpace(imgurl))
                    {
                        continue;
                    }

                    string ext = GetTitleContent(imgtag, "img", "data-type");
                    ext = ext == "" ? "png" : ext;

                    string dbUrl = string.Format("/File/mater/{0}/{1}/{2}", username, DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString() + "." + ext));

                    string savename = path + dbUrl;

                    if (i == 0)
                    {
                        article.coverImgUrl = dbUrl;
                    }

                    C_UserMater mater = new C_UserMater();
                    mater.C_UserName = username;
                    mater.Dat = DateTime.Now;
                    mater.MaterType = "img";
                    mater.MaterUrl = dbUrl;
                    mater.ID = mater.InsertAndReturnIdentity();

                    C_UserArticleMater artMater = new C_UserArticleMater();
                    artMater.ArticleID = article.ID;
                    artMater.Dat = DateTime.Now;
                    artMater.MaterID = mater.ID;
                    artMater.ID = artMater.InsertAndReturnIdentity();


                    bool isSave = SavePhotoFromUrl(savename, imgurl);

                    article.contents = article.contents.Replace("data-src=\"" + imgurl + "\"", "src=\"" + WeConfig.wx_domain + dbUrl + "\"");

                    i++;
                }

                article.contents = article.contents.Replace("data-src", "src").Replace("https://v.qq.com/iframe/preview.html", "https://v.qq.com/iframe/player.html");

                article.EditByID();//更新文章


                return "ok|" + article.ID;
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "error_C_UserArticle_getwxarticleByUrl");
                return "fail|什么都没有抓到";
            }

        }
        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public static string GetTitleContent(string str, string title, string attrib)
        {
            try
            {

                string tmpStr = string.Format("<{0}[^>]*?{1}=(['\"\"]?)(?<url>[^'\"\"\\s>]+)\\1[^>]*>", title, attrib); //获取<title>之间内容  

                Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

                string result = TitleMatch.Groups["url"].Value;
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 根据网络url下载图片
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FileName, string Url)
        {
            string path = FileName.SubStringSafe(0, FileName.LastIndexOf('/') + 1);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

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

