using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    /// <summary>
    /// 搜索条件
    /// </summary>
    public class BaseParametersSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
    //List_User_DistinctBy_userId比较器，继承自IEqualityComparer接口。
    public class List_Para_DistinctBy_Type : IEqualityComparer<BaseParameters>
    {
        public bool Equals(BaseParameters x, BaseParameters y)
        {
            if (x.Type == y.Type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(BaseParameters obj)
        {
            return 0;
        }
    }
    public partial class BaseParameters
    {
        /// <summary>
        /// 获取带% 的参数  转化为数字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal getRate(string name)
        {
            string str= BaseParameters.GetVal(name);
            str = str.Replace(" ","");
            if (str.Contains("%")) 
            {
                str = str.Replace("%","");
            }
            decimal de = 0;
            decimal.TryParse(str,out de);
            return de / 100;
        }
        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal getPrice(string name) 
        {
            string str = BaseParameters.GetVal(name);
            str = str.Replace(" ", "");
            decimal de = 0;
            decimal.TryParse(str, out de);
            return de;
        }

        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int getUpdate(string name)
        {
            string str = BaseParameters.GetVal(name);
            str = str.Replace(" ", "");
            int de = 0;
            int.TryParse(str, out de);
            return de;
        }

        public static List<BaseParameters> GetEntitysAllSort()
        {
            string strSql = "SELECT TOP 100000 * FROM [BaseParameters] order by Type,Sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<BaseParameters>(strSql, paramters);
        }
        /// <summary>
        /// 根据唯一标识获取参数
        /// </summary>
        /// <param name="ParametersKey"></param>
        /// <returns></returns>
        public static BaseParameters GetEntityByParametersKey(string ParametersKey)
        {
            string strSql = "SELECT * FROM [BaseParameters] WHERE ParametersKey=@ParametersKey";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ParametersKey", ParametersKey) };

            return DAL.EntityDataHelper.LoadData2Entity<BaseParameters>(strSql, paramters);
        }

        /// <summary>
        /// 获取显示的参数
        /// </summary>
        /// <returns></returns>
        public static List<BaseParameters> GetEntitysIsShow()
        {
            string strSql = "SELECT TOP 100000 * FROM [BaseParameters] where IsShow=1 order by Type,Sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<BaseParameters>(strSql, paramters);
        }
        /// <summary>
        /// 获取直售代理账号
        /// </summary>
        /// <returns></returns>
        public static string getOfficialAgent() 
        {
            string OfficialAgent= BaseParameters.GetVal("OfficialAgent");
            if (string.IsNullOrWhiteSpace(OfficialAgent)) 
            {
                return "官网";
            }
            return OfficialAgent;
        }
        /// <summary>
        /// 获取某个参数值
        /// </summary>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        public static string GetVal(string paramKey)
        {
            try
            {
                string sql = "select top 1 ParametersVal from BaseParameters where ParametersKey=@paramKey and IsValid=1 ";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@paramKey", paramKey)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                return obj != null ? obj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "BaseParameters_GetVal_error");
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据类别获取参数列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<BaseParameters> GetParasByType(string type)
        {
            string strSql = "SELECT * FROM [BaseParameters] where Type=@Type";
            System.Data.SqlClient.SqlParameter[] paramters =  {
              new System.Data.SqlClient.SqlParameter("@Type", type)  };

            return DAL.EntityDataHelper.FillData2Entities<BaseParameters>(strSql, paramters);
        }
        /// <summary>
        /// 修改系统参数【维护版】
        /// </summary>
        /// <returns></returns>
        public int SYSEditByID()
        {
            string strSql = "UPDATE [BaseParameters] SET Type=@Type,NickName=@NickName,Sort=@Sort,Remark=@Remark,IsShow=@IsShow WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@Type",Type),
                new System.Data.SqlClient.SqlParameter("@NickName",NickName),
                new System.Data.SqlClient.SqlParameter("@Sort",_sort),
                new System.Data.SqlClient.SqlParameter("@Remark",_remark),
                new System.Data.SqlClient.SqlParameter("@IsShow",IsShow),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 修改系统参数
        /// </summary>
        /// <returns></returns>
        public static int EditByID(string ParametersKey, string ParametersVal)
        {
            string strSql = "UPDATE [BaseParameters] SET ParametersVal=@ParametersVal WHERE ParametersKey=@ParametersKey;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ParametersKey",ParametersKey),
                new System.Data.SqlClient.SqlParameter("@ParametersVal",ParametersVal)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 修改参数值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ParametersVal"></param>
        /// <returns></returns>
        public static int EditParaValByID(int id, string ParametersVal)
        {
            string strSql = "UPDATE [BaseParameters] SET ParametersVal=@ParametersVal WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@ParametersVal",ParametersVal)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
