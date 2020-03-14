using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/Customer")]
    public class CustomerController : ApiBaseController
    {
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAllList()
        {
            RequestResult result = new RequestResult();
            try
            {
                List<C_Interface> CUser = C_User.Getusername();

                List<Customer> CusList = new List<Customer>();
                foreach (C_Interface user in CUser)
                {
                    Customer cus = new Customer();
                    cus.CusNo = user.UserName;
                    cus.CusName = user.Name;
                    CusList.Add(cus);
                }

                result.data = CusList;
                result.message = "成功";
                result.success = true;

                PDALog.Write("获取客户列表", "获取", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取客户列表出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUpdateNewList(int Timestamp)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<C_Interface> CUser = C_User.Getusername(Timestamp);

                List<Customer> CusList = new List<Customer>();
                foreach (C_Interface user in CUser)
                {
                    Customer cus = new Customer();
                    cus.CusNo = user.UserName;
                    cus.CusName = user.Name;
                    CusList.Add(cus);
                }

                result.data = CusList;
                result.message = "成功";
                result.success = true;

                PDALog.Write("更新客户列表", "获取", "", PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("Timestamp:{0}", Timestamp), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取最新客户列表出错：" + ex.Message, "PDA上传出错");
            }
            return result;
        }

        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult QueryList(string KeyWords)
        {
            RequestResult result = new RequestResult();
            try
            {
                List<C_Interface> CUser = C_User.Getusername(KeyWords);

                List<Customer> CusList = new List<Customer>();
                foreach (C_Interface user in CUser)
                {
                    Customer cus = new Customer();
                    cus.CusNo = user.UserName;
                    cus.CusName = user.Name;
                    CusList.Add(cus);
                }

                result.data = CusList;
                result.message = "成功";
                result.success = true;

                PDALog.Write("查询客户", "查询", "", PdaUser.PUserName + "-" + PdaUser.PRealName, string.Format("KeyWords:{0}", KeyWords), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("客户查询出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }

        private class Customer
        {
            public string CusNo { get; set; }
            public string CusName { get; set; }
        }
    }
}
