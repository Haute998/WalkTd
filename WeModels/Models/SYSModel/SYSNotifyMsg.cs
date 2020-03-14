using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSNotifyMsgSearch : BaseSearch
    {
        /// <summary>
        /// 阅读状态  1为已读，0为未读
        /// </summary>
        public string ReadState { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
    public partial class SYSNotifyMsg
    {
        /// <summary>
        /// 获取未读消息数
        /// </summary>
        /// <returns></returns>
        public static int GetUnReadMsgCount(string username)
        {
            string strSql = "SELECT count(1) FROM [SYSNotifyMsg] where IsRead=0 and UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserName",username)
            };
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }
        /// <summary>
        /// 设为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int ReadMsgByID(int id)
        {
            string strSql = "UPDATE [SYSNotifyMsg] SET IsRead=1 WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="MsgName"></param>
        /// <param name="MsgContent"></param>
        public static void sendSysMsg(string MsgCode, string MsgContent)
        {
            try
            {
                SYSNotify notify = SYSNotify.GetEntityByMsgCode(MsgCode);

                SYSNotifyMsg msg = new SYSNotifyMsg();
                msg.MsgContent = MsgContent;
                msg.MsgType = notify.MsgType;

                notify.Subscriber = notify.Subscriber.TrimStart(',').TrimEnd(',');
                string[] userArray = notify.Subscriber.Split(',');
                for (int i = 0; i < userArray.Length; i++)
                {
                    msg.Dat = DateTime.Now;
                    msg.UserName = userArray[i];
                    msg.InsertAndReturnIdentity();
                }
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "sendSysMsg_error");
            }

        }
    }
}
