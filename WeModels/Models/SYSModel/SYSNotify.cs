using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSNotifySearch : BaseSearch
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string keyword { get; set; }
    }

    public partial class SYSNotify
    {
        /// <summary>
        /// 除当前用户外的其他订阅者
        /// </summary>
        public string elseSubscriber { get; set; }
        /// <summary>
        /// 根据消息代号查找记录
        /// </summary>
        /// <param name="MsgCode"></param>
        /// <returns></returns>
        public static SYSNotify GetEntityByMsgCode(string MsgCode)
        {
            string strSql = "SELECT top 1 * FROM [SYSNotify] WHERE MsgCode=@MsgCode";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@MsgCode", MsgCode) };

            return DAL.EntityDataHelper.LoadData2Entity<SYSNotify>(strSql, paramters);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool ToDels(int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("DELETE FROM [SYSNotify] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = "UPDATE [SYSNotify] SET MsgType=@MsgType,MsgCode=@MsgCode,Remark=@Remark WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@MsgType",_msgtype),
                new System.Data.SqlClient.SqlParameter("@MsgCode",_msgcode),
                new System.Data.SqlClient.SqlParameter("@Remark",_remark)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserName"></param>
        /// <param name="type">1:订阅，0:取消订阅</param>
        /// <returns></returns>
        public static int SubscriberByID(int id, string UserName, int type)
        {
            SYSNotify model = SYSNotify.GetEntityByID(id);
            if (model == null)
            {
                return 0;
            }
            string Subscriber = "";
            Subscriber = model.Subscriber;

            if (Subscriber.Contains("," + UserName + ","))
            {
                type = 0;
            }
            if (string.IsNullOrWhiteSpace(Subscriber))
            {
                if (type == 1)
                {
                    Subscriber = "," + UserName + ",";
                }
            }
            else
            {
                if (type == 1)
                {
                    Subscriber = Subscriber + UserName + ",";
                }
                else if (type == 0)
                {
                    Subscriber = Subscriber.Replace("," + UserName + ",", ",");
                    if (Subscriber == ",")
                    {
                        Subscriber = "";
                    }
                }
            }

            string strSql = "UPDATE [SYSNotify] SET Subscriber=@Subscriber WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@Subscriber",Subscriber)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
