using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeModels
{
    /// <summary>
    ///ModelQueryParam 的摘要说明
    /// </summary>
    public class ModelQueryParam
    {
        private string headCode;

        public string HeadCode
        {
            get { return headCode; }
            set { headCode = value; }
        }

        private string sCode;

        public string SCode
        {
            get { return sCode; }
            set { sCode = value; }
        }
        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
        private string area;

        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        private string province;

        public string Province
        {
            get { return province; }
            set { province = value; }
        }
        private string city;

        public string City
        {
            get { return city; }
            set { city = value; }
        }
        private string service_Provider;

        public string Service_Provider
        {
            get { return service_Provider; }
            set { service_Provider = value; }
        }

        private string statuDesc;

        public string StatuDesc
        {
            get { return statuDesc; }
            set { statuDesc = value; }
        }

        public int VID { get; set; }

        public int Integral { get; set; }
    }
}