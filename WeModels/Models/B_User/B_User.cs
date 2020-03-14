using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models;

namespace WeModels
{
    public partial class B_User
    {
        /// <summary>
        /// 所有字段
        /// </summary>
        private const string AllFields = "*";
        /// <summary>
        /// 确认密码 注册时有用到
        /// </summary>
        public string ConfirmPwd { get; set; }
      
        /// <summary>
        /// 验证码
        /// </summary>
        public string valiCode { get; set; }

        /// <summary>
        /// 登录后停留时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Dept { get; set; }
        /// <summary>
        /// 是否登录过期
        /// </summary>
        /// <returns></returns>
        public bool IsLoginedTimeOut
        {
            get
            {
                return CurrentTime < DateTime.Now;
            }
        }

        public bool _IsNoError = true;
        /// <summary>
        /// 是否有错误信息
        /// </summary>
        public bool IsNoError
        {
            get
            {
                return _IsNoError;
            }
        }


        private string _ErrorData;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorData
        {
            get { return _ErrorData; }
            set
            {
                _ErrorData = value;
                _IsNoError = string.IsNullOrWhiteSpace(_ErrorData);
            }
        }

        /// <summary>
        /// 当前停留页面
        /// </summary>
        public string CurrentURL { get; set; }

        public B_User CloneB_User()
        {
            B_User usr = new B_User();
            usr.ID = this.ID;
            usr.GuidCode = usr.GuidCode;
            usr.UserName = this.UserName;

            usr.LoginTimes = this.LoginTimes;
            usr.LoginLastDat = this.LoginLastDat;
            usr.LoginLastIp = this.LoginLastIp;
            usr.CurrentTime = this.CurrentTime;
            usr.CurrentURL = this.CurrentURL;
            usr.ErrorData = this.ErrorData;
            return usr;
        }

        /// <summary>
        /// 保存基础数据(根据ID)
        /// </summary>
        /// <returns></returns>
        public bool UpdateBase()
        {
            string strSql = "UPDATE [B_User] SET LoginTimes=@LoginTimes,LoginLastDat=@LoginLastDat,LoginLastIp=@LoginLastIp,GuidCode=@GuidCode WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@LoginTimes", LoginTimes),
                new System.Data.SqlClient.SqlParameter("@LoginLastDat", LoginLastDat),
                new System.Data.SqlClient.SqlParameter("@LoginLastIp", LoginLastIp),
                new System.Data.SqlClient.SqlParameter("@GuidCode", GuidCode),
                new System.Data.SqlClient.SqlParameter("@ID", ID)
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }

        public static B_User GetPassWord(string WxNo)
        {
            string strSql = "select * FROM [B_User] WHERE WxNo=@WxNo AND ID=1 ";
            System.Data.SqlClient.SqlParameter[] paramters = { 
                                                             new System.Data.SqlClient.SqlParameter("@WxNo", WxNo)};

            return DAL.EntityDataHelper.LoadData2Entity<B_User>(strSql, paramters);
        }

        /// <summary>
        /// 更新GUID
        /// </summary>
        /// <returns></returns>
        public bool UpdateGuidCode()
        {
            string strSql = "UPDATE [B_User] SET GuidCode=@GuidCode WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@GuidCode", GuidCode),
                new System.Data.SqlClient.SqlParameter("@ID", ID)
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }

        /// <summary>
        /// 绑定微信账号
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static int BindWxByUserName(string UserName, string C_UserName)
        {
            string strSql = "UPDATE [B_User] SET C_UserName=@C_UserName WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@C_UserName",C_UserName),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 根据主键更新行(并更新到在线用户)
        /// </summary>
        /// <returns></returns>
        public bool UpdateByIDForRefresh()
        {
            if (UpdateByID() > 0)
            {
                B_User tmp = B_UserManager.GetB_User(u => u.ID == this.ID);
                if (tmp != null && tmp.IsLoginedTimeOut == false)
                {
                    tmp.PassWord = this.PassWord;
                    tmp.IsValid = this.IsValid;
                    if (tmp.IsValid == false)
                    {
                        tmp.ErrorData = "您已被管理员设置为禁用状态！";
                    }

                    tmp.BirthDay = this.BirthDay;
                    tmp.Mobile = this.Mobile;
                }
            }
            return true;
        }

        /// <summary>
        /// 根据UserName返回用户对象
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static B_User GetB_UserByUserName(string userName, Func<B_User, bool> check = null)
        {
            string strSql = "SELECT " + AllFields + " FROM [B_User] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", userName) };
            B_User usr = DAL.EntityDataHelper.LoadData2Entity<B_User>(strSql, paramters);
            if (usr != null)
            {
                if (check != null)
                {
                    return check(usr) ? usr : null;
                }
                else
                {
                    return usr;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据OpenID返回用户对象
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static B_User GetB_UserByOpenID(string OpenID)
        {
            string strSql = "SELECT " + AllFields + " FROM [B_User] WHERE OpenID=@OpenID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@OpenID", OpenID) };
            B_User usr = DAL.EntityDataHelper.LoadData2Entity<B_User>(strSql, paramters);
            if (usr != null)
            {
                return usr;
            }
            return null;
        }

        /// <summary>
        /// 获取当前最大的用户ID
        /// </summary>
        /// <returns></returns>
        public static int GetTopUseID()
        {
            string strSql = "SELECT ISNULL(MAX(ID),1000) FROM [B_User]";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }
        
        /// <summary>
        /// 获取该用户名使用人数
        /// </summary>
        /// <param name="NickName"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static int GetUserCountByUserName(string UserName,int id=0)
        {
            string strSql = "SELECT Count(1) FROM [B_User] WHERE UserName=@UserName and ID<>@id";
            System.Data.SqlClient.SqlParameter[] paramters = { 
                                                                 new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                  new System.Data.SqlClient.SqlParameter("@id", id)
                                                             };
            object usrCount = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return usrCount == null ? 0 : Convert.ToInt32(usrCount);
        }


        /// <summary>
        /// 获取用户表
        /// </summary>
        /// <param name="dicWhere"></param>
        /// <returns></returns>
        public static List<B_User> GetEntitys(Dictionary<string, object> dicWhere)
        {
            string strSql = "SELECT " + AllFields + " FROM [B_User] WHERE 1=1";
            if (dicWhere.Count > 0)
            {
                StringBuilder sbAndWhere = new StringBuilder();
                foreach (string colName in dicWhere.Keys)
                {
                    sbAndWhere.Append(" and " + colName + "=@" + colName);
                }
                strSql = strSql + sbAndWhere.ToString();
            }
            return DAL.EntityDataHelper.FillData2Entities<B_User>(strSql, dicWhere);
        }

        /// <summary>
        /// 获取用户数目
        /// </summary>
        /// <param name="dicWhere"></param>
        /// <returns></returns>
        public static int GetCount(Dictionary<string, object> dicWhere)
        {
            string strSql = "SELECT COUNT(ID) FROM [B_User] WHERE 1=1";
            if (dicWhere.Count > 0)
            {
                StringBuilder sbAndWhere = new StringBuilder();
                foreach (string colName in dicWhere.Keys)
                {
                    sbAndWhere.Append(" and " + colName + "=@" + colName);
                }
                strSql = strSql + sbAndWhere.ToString();
            }
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, dicWhere);
        }




        /// <summary>
        /// 根据主键删除一条数据
        /// （当数据库表第一行是int自增主键时生成此方法）
        /// </summary>
        private static int DeleteByID(int id, System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "DELETE FROM [B_User] WHERE ID=" + id;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);
            return cnt;
        }

        public static int EditPortrait(string UserName, string PortraitUrl)
        {
            string strSql = "UPDATE [B_User] SET PortraitUrl=@PortraitUrl WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@PortraitUrl",PortraitUrl),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 后台修改用户基本信息
        /// </summary>
        /// <returns></returns>
        public int UserEditByID()
        {
            string strSql = "UPDATE [B_User] SET Name=@Name,PassWord=@PassWord,Mobile=@Mobile,WxNo=@WxNo,Email=@Email,IsValid=@IsValid,RoleID=@RoleID,DeptID=@DeptID WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@Name",_name),
                new System.Data.SqlClient.SqlParameter("@PassWord",_password),
                new System.Data.SqlClient.SqlParameter("@Mobile",_mobile),
                new System.Data.SqlClient.SqlParameter("@WxNo",_wxno),
                new System.Data.SqlClient.SqlParameter("@Email",_email),
                new System.Data.SqlClient.SqlParameter("@IsValid",_isvalid),
                new System.Data.SqlClient.SqlParameter("@RoleID",_roleid),
                new System.Data.SqlClient.SqlParameter("@DeptID",_deptid),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);


            return cnt;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int EditPwdByUserName(string pwd,string username)
        {
            string strSql = "UPDATE [B_User] SET PassWord=@PassWord WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@PassWord",pwd),
                new System.Data.SqlClient.SqlParameter("@UserName",username)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static List<B_User> GetAllB_User()
        {
            string strSql = "SELECT a.*,(case when a.DeptID=0 then '总部' else b.Name end) as Dept FROM [B_User] as a left join Supplier as b on a.DeptID=b.ID";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<B_User>(strSql, paramters);
        }
    }
}
