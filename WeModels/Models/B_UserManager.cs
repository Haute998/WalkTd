using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models
{
    /// <summary>
    /// 用户在线管理类
    /// </summary>
    public class B_UserManager
    {
        private static readonly object lockHelper = new object();

        /// <summary>
        /// 应用程序初始化时应超时的时间
        /// </summary>
        private static readonly DateTime initTime = DateTime.MinValue;


        /// <summary>
        /// 用户字典
        /// </summary>
        private static Dictionary<string, B_User> userDictionary;


        static B_UserManager()
        {
            userDictionary = new Dictionary<string, B_User>();
            initTime = DateTime.Now.AddSeconds(System.Web.Security.FormsAuthentication.Timeout.TotalSeconds);
        }



        /// <summary>
        /// 初始化时间是否超时
        /// </summary>
        public static bool IsInitTimeOut
        {
            get
            {
                return initTime < DateTime.Now;
            }
        }






        /// <summary>
        /// 获取用户数（包含超时用户）
        /// </summary>
        public static int UserCount
        {
            get
            {
                return userDictionary.Count;
            }
        }


        /// <summary>
        /// 获取用户在线数
        /// </summary>
        /// <returns></returns>
        public static int GetOnlineCount()
        {
            int count = 0;
            foreach (B_User usr in userDictionary.Values)
            {
                if (usr.IsLoginedTimeOut == false && usr.IsNoError)
                {
                    count++;
                }
            }
            return count;
        }



        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="usr">登录用户</param>
        /// <param name="usrField">登录的字段</param>
        /// <param name="notice">通知原用户下线要求</param>
        /// <returns></returns>
        public static bool Login(B_User usr, Func<B_User, bool, bool> notice)
        {
            B_User dbUser = B_User.GetB_UserByUserName(usr.UserName, tUser => tUser.PassWord == usr.PassWord);
            if (dbUser != null)
            {
                lock (lockHelper)
                {
                    //登录时传参给dbUser
                    dbUser.LoginLastDat = usr.LoginLastDat;
                    dbUser.CurrentTime = usr.CurrentTime;
                    dbUser.LoginLastIp = usr.LoginLastIp;
                    dbUser.LoginTimes = dbUser.LoginTimes + 1;
                    dbUser.GuidCode = usr.GuidCode;
                    dbUser.PortraitUrl = usr.PortraitUrl;

                    bool usrExists = false;
                    B_User olUser = null;
                    if (userDictionary.TryGetValue(dbUser.UserName, out olUser))
                    {
                        //忽略已超时的用户
                        if (olUser.IsLoginedTimeOut)
                        {
                            userDictionary.Remove(dbUser.UserName);
                        }
                        else
                        {
                            usrExists = true;
                        }
                    }
                    
                    //符合业务逻辑条件则移除原用户
                    if (notice(dbUser, usrExists) == false)
                    {
                        return false;
                    }

                    if (usrExists)
                    {
                        userDictionary.Remove(dbUser.UserName);
                    }
                    if (dbUser.UpdateBase())
                    {
                        userDictionary.Add(dbUser.UserName, dbUser);
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 处理应用程序重启还在线的用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static B_User GetB_UserAndLogin(string userName, Func<B_User, bool> check)
        {
            B_User dbUser = B_User.GetB_UserByUserName(userName);
            if (dbUser != null)
            {
                if (check(dbUser))
                {
                    lock (lockHelper)
                    {
                        if (userDictionary.ContainsKey(dbUser.UserName) == false)
                        {
                            dbUser.CurrentTime = DateTime.Now.AddSeconds(System.Web.Security.FormsAuthentication.Timeout.TotalSeconds);
                            userDictionary.Add(dbUser.UserName, dbUser);
                            return dbUser;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 登出一个用户
        /// </summary>
        /// <param name="userName"></param>
        public static void LogOff(string userName)
        {
            B_User usr = GetB_UserAndRefresh(u => u.UserName == userName);
            if (usr != null)
            {
                usr.GuidCode = Guid.NewGuid().ToString("N");
                usr.UpdateGuidCode();
                RemoveUser(usr.UserName);
            }
        }





        /// <summary>
        /// 循环所有用户
        /// </summary>
        /// <param name="each"></param>
        public static List<B_User> GetB_UserList(Func<B_User, bool> each = null)
        {
            List<B_User> listResult = new List<B_User>();
            foreach (B_User usr in userDictionary.Values)
            {
                bool isAdd = false;
                if (each == null || each(usr))
                {
                    isAdd = true;
                }
                if (isAdd)
                {
                    listResult.Add(usr.CloneB_User());
                }
            }
            return listResult;
        }



        /// <summary>
        /// 移除一个登录用户
        /// </summary>
        /// <param name="userName"></param>
        public static void RemoveUser(string userName)
        {
            if (userDictionary.ContainsKey(userName))
            {
                lock (lockHelper)
                {
                    if (userDictionary.ContainsKey(userName))
                    {
                        userDictionary.Remove(userName);
                    }
                }
            }
        }



        /// <summary>
        /// 循环找到相关的用户
        /// </summary>
        /// <param name="each"></param>
        /// <returns></returns>
        public static B_User GetB_User(Predicate<B_User> each)
        {
            if (each != null)
            {
                B_User tUser = null;
                foreach (B_User usr in userDictionary.Values)
                {
                    if (each(usr))
                    {
                        tUser = usr;
                        break;
                    }
                }
                return tUser;
            }
            return null;
        }


        /// <summary>
        /// 循环找到相关的用户(并刷新)
        /// </summary>
        /// <param name="each"></param>
        /// <param name="curUrl"></param>
        /// <returns></returns>
        public static B_User GetB_UserAndRefresh(Predicate<B_User> each, string curUrl = null)
        {
            B_User tmpUser = GetB_User(each);
            if (tmpUser != null && tmpUser.IsLoginedTimeOut == false)
            {
                tmpUser.CurrentTime = DateTime.Now.AddSeconds(System.Web.Security.FormsAuthentication.Timeout.TotalSeconds);
                if (curUrl != null)
                {
                    tmpUser.CurrentURL = curUrl;
                }
            }
            return tmpUser;
        }
    }
}
