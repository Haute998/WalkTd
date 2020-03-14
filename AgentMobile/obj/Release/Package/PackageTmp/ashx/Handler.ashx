<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Web.Services;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class Handler : SystemBase
{   
    [WebMethod]
    public static string GetQueryCode(string QueryCode)
    {   
        CodeQuery query = new CodeQuery();
        string IPAddress = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
        ModelQueryParam result = GetAddress(IPAddress);
        result.SCode = QueryCode;
        result.IpAddress = IPAddress;
        string RetrunContent = query.QueryBarCode(result);
        return RetrunContent;
    }

    public class JsonParser
    {   
        public string province; 
        public string city; 
    }

    private static ModelQueryParam GetAddress(string IPAddress)
    {
        ModelQueryParam queryParam = new ModelQueryParam();
        queryParam.Area = "未知";
        string strContent = HttpPost("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js&ip=" + IPAddress);
        if (strContent == "-2")
        {
            queryParam.Area = "本机地址";
        }
        else
        {   
            strContent = strContent.Split(new char[] { '=' })[1];
            strContent = strContent.Substring(0, strContent.Length - 1);
            strContent = "[" + strContent + "]";
            JArray ja = JArray.Parse(strContent);
            JObject o = (JObject)ja[0];
            queryParam.Area = o["province"].ToString() + " " + o["city"].ToString();
            queryParam.Province = o["province"].ToString();
            queryParam.City = o["city"].ToString();
        }

        return queryParam;
    }

    public static string HttpPost(string Url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "GET";
        request.ContentType = "application/json";
        
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return Unicode2String(retString);
    }

    public static string Unicode2String(string source)
    {
        return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                     source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
    }  


    //private static string GetAddress(string IPAddress)
    //{
    //    string strContent = HttpGet("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js&ip=" + IPAddress);
        
    //    strContent=strContent.Split(new char[]{'='})[1];
    //    strContent = strContent.Substring(0, strContent.Length - 1);
    //    strContent = "[" + strContent + "]";
    //    JArray ja = JArray.Parse(strContent);
    //    JObject o = (JObject)ja[0];
    //    string Address = o["province"].ToString() + o["city"].ToString();

    //    return Address;
    //}

    //public static string HttpGet(string Url)
    //{
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
    //    request.Method = "GET";
    //    request.ContentType = "text/html;charset=UTF-8";

    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    Stream myResponseStream = response.GetResponseStream();
    //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
    //    string retString = myStreamReader.ReadToEnd();
    //    myStreamReader.Close();
    //    myResponseStream.Close();

    //    return retString;
    //}
}