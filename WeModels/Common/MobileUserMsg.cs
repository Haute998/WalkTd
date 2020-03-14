using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class MobileUserMsg
    {
        public static int TimeOut = 7200000;

        public static List<CacheMobileUser> CacheMobileUserList = new List<CacheMobileUser>();

        /// <summary>
        /// 判断UserToken是否存在
        /// </summary>
        /// <param name="UserToken"></param>
        /// <returns></returns>
        public static bool IsUserToken(string UserToken)
        {
            bool IsOK = false;

            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                if (CacheMobileUserList[i].UserToken == UserToken)
                {
                    CacheMobileUserList[i].Timestamp = CommonFunc.GetNowMTimestamp();
                    IsOK = true;
                    break;
                }
            }

            return IsOK;
        }

        /// <summary>
        /// 删除超时用户
        /// </summary>
        public static void ClearTimeOutUser()
        {
            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                double millisend = CommonFunc.GetMTimestamp(DateTime.Now) - CacheMobileUserList[i].Timestamp;
                if (millisend > TimeOut)
                {
                    CacheMobileUserList.Remove(CacheMobileUserList[i]);
                }
            }
        }

        /// <summary>
        /// 根据UserToken删除用户
        /// </summary>
        public static bool RemoveUser(string UserToken)
        {
            bool IsOK = false;
            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                if (CacheMobileUserList[i].UserToken == UserToken)
                {
                    CacheMobileUserList.Remove(CacheMobileUserList[i]);
                    IsOK = true;
                }
            }

            return IsOK;
        }

        /// <summary>
        /// 生成UserToken
        /// </summary>
        public static string CreateUserToken()
        {
            IdWorker IdKey = new IdWorker(0,1);
            return IdKey.nextId().ToString();
        }

        /// <summary>
        /// 根据用户Token获取信息
        /// </summary>
        /// <param name="UserToken"></param>
        /// <returns></returns>
        public static CacheMobileUser TokenGetUser(string UserToken)
        {
            CacheMobileUser user = null;
            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                if (CacheMobileUserList[i].UserToken == UserToken)
                {
                    CacheMobileUserList[i].Timestamp = CommonFunc.GetNowMTimestamp();       // 每次请求延长过期时间
                    user = CacheMobileUserList[i];
                }
            }

            return user;
        }

        /// <summary>
        /// 根据用户获取用户信息
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static CacheMobileUser UserNameGetUser(string UserName)
        {
            return CacheMobileUserList.Where(u => u.UserName == UserName).FirstOrDefault();
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static string IsAlreadyLogin(string UserName)
        {
            string UserToken = string.Empty;

            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                if (CacheMobileUserList[i].UserName == UserName)
                {
                    CacheMobileUserList[i].Timestamp = CommonFunc.GetNowMTimestamp();
                    UserToken = CacheMobileUserList[i].UserToken;
                    break;
                }
            }

            return UserToken;
        }

        public static string PDAUserLogin(string UserName)
        {
            string UserToken = string.Empty;

            for (int i = 0; i < CacheMobileUserList.Count; i++)
            {
                if (CacheMobileUserList[i].UserName == UserName)
                {
                    CacheMobileUserList[i].Timestamp = CommonFunc.GetNowMTimestamp();
                    CacheMobileUserList[i].UserToken = UserToken = CreateUserToken();
                    
                    break;
                }
            }

            return UserToken;
        }
    }
    public class CacheMobileUser
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public List<string> AuthCodeList { get; set; }      // 权限集合
        public double Timestamp { get; set; }               // 时间
    }
}
