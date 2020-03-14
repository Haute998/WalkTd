using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_User_Login
    {
        //手机号
        public string Phone { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; } 
    }

    public class C_Interface
    {
        public string UserName { get; set; }
        public string Name { get; set; }

    }

    public class SubUserCount
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public int Count { get; set; }

        public List<ShowSubUser> UserList = new List<ShowSubUser>();
    }
    public class ShowSubUser
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Province{get;set;}
        public string City{get;set;}
        public string Area { get; set; }
        public string Address { get; set; }

    }
    public partial class C_User
    {
        public int count { get; set; }
        public string C_UserTypeName { get; set; }

        /// <summary>
        /// 读取所有实例，限制10万条。
        /// </summary>
        public static List<C_User> GetEntitysPDA()
        {
            string strSql = "SELECT TOP 100000 useNum,ID,UserName,Name,Phone,PassWord FROM [C_User] where state='已审核' and chief=0 ";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 余额增加 【事务版】
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="UserName"></param>
        /// <param name="money">要增加的余额</param>
        /// <returns></returns>
        public static int addMoneyByUserName(System.Data.SqlClient.SqlTransaction tran,string UserName,decimal money)
        {
            string strSql = "UPDATE [C_User] SET Money+=@Money WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@Money",money)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 余额减少 【事务版】
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="UserName"></param>
        /// <param name="money">要减少的余额</param>
        /// <returns></returns>
        public static int reduceMoneyByUserName(System.Data.SqlClient.SqlTransaction tran, string UserName, decimal money)
        {
            string strSql = "UPDATE [C_User] SET Money-=@Money WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@Money",money)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }



        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public static C_User GetLoginByPassWord(string Phone, string PassWord)
        {
            string strSql = "select * FROM [C_User] WHERE Phone=@Phone and PassWord=@PassWord and state<>'已删除'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Phone", Phone),
                                                             new System.Data.SqlClient.SqlParameter("@PassWord", PassWord)};

            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }
  
        public static C_User GetC_UserByUserName(string UserName)
        {
            string strSql = "select * from C_User where UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };
            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 根据上级获取数量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetChiefInt(int ID)
        {
            string StrSq = "SELECT count(1) FROM C_User WHERE CHIEF=@ID";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            object rtn = DAL.SqlHelper.ExecuteScalar(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }


        /// <summary>
        /// 修改成黑名单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetUpdateStateBlack(int ID)
        {
            string StrSq = "UPDATE C_User SET state='黑名单' WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            object rtn = DAL.SqlHelper.ExecuteNonQuery(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }
        /// <summary>
        /// 修改成白名单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetUpdateStateWhite(int ID)
        {
            string StrSq = "UPDATE C_User SET state='已审核' WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            object rtn = DAL.SqlHelper.ExecuteNonQuery(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }
        /// <summary>
        /// 删除到回收站
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetUpdateStateDel(int ID)
        {
            string StrSq = "UPDATE C_User SET state='已删除' WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            object rtn = DAL.SqlHelper.ExecuteNonQuery(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }
        /// <summary>
        /// 转移下级代理
        /// </summary>
        /// <param name="ID">原上级ID</param>
        /// <param name="OldId">现ID</param>
        /// <returns></returns>
        public static int GetUpdateChief(int ID, int OldId)
        {
            string StrSq = "UPDATE C_User SET Chief=@ID WHERE Chief=@OldId";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID),new System.Data.SqlClient.SqlParameter("@OldId",OldId)

            };
            object rtn = DAL.SqlHelper.ExecuteNonQuery(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }
        /// <summary>
        /// 修改上级
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OldId"></param>
        /// <returns></returns>
        public static int GetUpdateOldChief(int ID, int OldId)
        {
            string StrSq = "UPDATE C_User SET Chief=@ID WHERE ID=@OldId";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",ID),new System.Data.SqlClient.SqlParameter("@OldId",OldId)

            };
            object rtn = DAL.SqlHelper.ExecuteNonQuery(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }
        /// <summary>
        /// 获取代理机构树信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<C_User> GetOptionzTreeMenu(int ID)
        {
            string strSql = "SELECT * FROM [C_User] WHERE Chief=@ID and state='已审核'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID) };
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 获取未审核上级(红点)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetC_UserCircles(int ID)
        {
            string strSql = "SELECT Count(*) FROM [C_User] WHERE Chief=@ID and state='未审核'";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }


        public static int GetNameCnt(string Name)
        {
            string strSql = "SELECT count(1) FROM [C_User] where Name=@Name";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Name", Name) };
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }

        /// <summary>
        /// 获取未审核代理 非直属(红点)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetC_UserBySta(string state)
        {
            string strSql = "SELECT Count(*) FROM [C_User] WHERE Chief<>0 and state=@state";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@state", state) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
        /// <summary>
        /// 获取代理机构树信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetUserCount()
        {
            string strSql = "SELECT count(*) FROM [C_User] WHERE  state='已审核'";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
        public static int YJGetUserCount()
        {
            string strSql = " select  COUNT(s.id) counts from ScaleOutStoke s left join Product p on p.ProductNumber=s.P_ID where s.State='启用' and s.Consignee in (select UserName from C_User where state='已审核' and C_UserTypeID=1 ) ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
        /// <summary>
        /// 结构树内容
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GetOptionTreeMenu(int ID)
        {
            List<C_User> list = GetOptionzTreeMenu(ID);
            List<Ztree> ztree = new List<Ztree>();
            foreach (C_User item in list)
            {
                string ImgUrl = item.PortraitUrl;
                string NickName = item.Name;
                string C_UserTypeName = item.C_UserTypeName;
                ztree.Add(new Ztree
                {
                    id = item.ID,
                    name = "<img src='/images/27.png'></img><span>" + NickName + "</span><span>" + C_UserTypeName + "</span>",
                    isParent = GetOptionzTreeMenu(item.ID).Count > 0 ? true : false

                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(ztree);
        }
        /// <summary>
        /// 判断是否有推荐下级
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<C_User> GetOptionzDisTreeMenu(string UserName)
        {
            string strSql = "SELECT * FROM [C_User] WHERE Introducer=@UserName and state='已审核' and C_UserTypeID=1";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }
        ///<summary>
        /// 获取所有直属于总部的合伙人所有代理
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<C_User> GetOptionDisDrectzTreeMenu()
        {
            string strSql = "SELECT * FROM [C_User] WHERE Chief=0 and C_UserTypeID=1 and state='已审核'";
            System.Data.SqlClient.SqlParameter[] paramters = {null};
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }
        /// <summary>
        /// 获取分销结构树
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GetOptionDisTreeMenu(string ID)
        {
            List<C_User> list = new List<C_User>();
            list = C_User.GetOptionzDisTreeMenu(ID);
            if (string.IsNullOrWhiteSpace(ID))
            {
                //获取所有合伙人
                list = C_User.GetOptionDisDrectzTreeMenu();
            }
            List<RebateZree> ztree = new List<RebateZree>();
            foreach (C_User item in list)
            {
                string ImgUrl = item.PortraitUrl;
                string NickName = item.Name;
                ztree.Add(new RebateZree
                {
                    id = item.UserName,
                    name = "<img src='" + ImgUrl + "'></img><span>" + NickName + "</span>",
                    isParent = GetOptionzDisTreeMenu(item.UserName).Count > 0 ? true : false

                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(ztree);
        }

        /// <summary>
        /// 获取当前最大的用户ID
        /// </summary>
        /// <returns></returns>
        public static int GetTopUseID()
        {
            string strSql = "SELECT ISNULL(MAX(ID),1000) FROM [C_User]";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }
        public static C_User GetC_UserBy(string keyWay)
        {
            string strSql = "select * from C_User where 1=1 " + keyWay;
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.LoadData2Entity<C_User>(strSql, paramters);
        }

        /// <summary>
        /// 结构树用户信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static SearchZtree GetOptionTreeUserMsg(int ID)
        {
            string strSql = " select c.ID ID,c.CardUrl CardUrl, c.Name Name,c.PortraitUrl PortraitUrl,c.Phone Phone,c.Card Card,c.C_UserTypeID C_UserTypeID,c.DatCreate DatCreate,c.DatVerify DatVerify,c.state state,c.Identifier Identifier,c.Chief Chief,t.Name LevelName from C_User as  c left join C_UserType as t on t.Lever=c.C_UserTypeID where c.ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID) };
            SearchZtree UserMsg = DAL.EntityDataHelper.LoadData2Entity<SearchZtree>(strSql, paramters);
            if (UserMsg == null)
            {
                return null;
            }
            UserMsg.ChiefCount = Convert.ToInt32(DAL.SqlHelper.ExecuteScalar("SELECT count(1) FROM [C_User] where Chief=" + ID + ""));
            return UserMsg;

        }
        /// <summary>
        /// 结构树用户信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<C_User> GetAuthorityUser()
        {
            string strSql = "select * from C_User where state=@state and C_UserTypeID=1 Order by Name";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@state", "已审核") };
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }
        public static List<C_User> GetAuthorityUserSch()
        {
            string strSql = "select * from C_User where state=@state and C_UserTypeID=1";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@state", "已审核") };
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }

        /// <summary>
        /// 代理柱状图
        /// </summary>
        /// <returns></returns>
        public static string GetC_UserCountCennsus()
        {
            string strSql = "select count(c.C_UserTypeID) Count,c.C_UserTypeID C_UserTypeID,isnull(t.Name,'其他') name from C_User c left join C_UserType t on  c.C_UserTypeID=t.Lever where c.state='已审核' group by c.C_UserTypeID,t.Name order by C_UserTypeID ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<C_CountCensus> list = DAL.EntityDataHelper.FillData2Entities<C_CountCensus>(strSql, paramters);
            List<Census> censusList = new List<Census>();
            foreach (C_CountCensus item in list)
            {
                censusList.Add(new Census
                {
                    name = item.name,
                    data = new int[] { item.Count }
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }
        public static string GetPieC_UserCountCensus()
        {
            string strSql = "select count(c.C_UserTypeID) Count,c.C_UserTypeID C_UserTypeID,t.Name name from C_User c left join C_UserType t on  c.C_UserTypeID=t.Lever where c.state='已审核' group by c.C_UserTypeID,t.Name order by c.C_UserTypeID";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<C_CountCensus> list = DAL.EntityDataHelper.FillData2Entities<C_CountCensus>(strSql, paramters);
            List<PieCensus> censusList = new List<PieCensus>();
            int count = C_User.GetUserCount();
            foreach (C_CountCensus item in list)
            {
                censusList.Add(new PieCensus
                {
                    name = item.name,
                    y = item.Count% count 
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }

        public static string YJGetPieC_UserCountCensus(string key)
        {
            string where = "select UserName from C_User where state='已审核' and C_UserTypeID=1 and UserName!='m2000' ";
            if (!string.IsNullOrWhiteSpace(key))
            {
                where = "select UserName from C_User where state='已审核' and C_UserTypeID=1 and UserName!='m2000' and (Name like '%" + key + "%' or Phone like '%" + key + "%' )"; 
            }
            string strSql = "select  COUNT(s.id) Count,p.ProductName name from ScaleOutStoke s left join Product p on p.ProductNumber=s.P_ID where s.State='启用' and s.Consignee in ("+where+") group by p.ProductName";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<U_PCensus> list = DAL.EntityDataHelper.FillData2Entities<U_PCensus>(strSql, paramters);
            List<PieCensus> censusList = new List<PieCensus>();
            int count = C_User.YJGetUserCount();
            foreach (U_PCensus item in list)
            {
                censusList.Add(new PieCensus
                {
                    name = item.name,
                    y = item.Count % count
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }



        public static string GetPieCustomerCountCensus(string type)
        {
            string strSql = @"select count(c.Sex) Count,(case when c.Sex='' then '未知' else c.Sex end) name
                                from C_Consumer c 
                                where c.[Type]='" +type+@"' group by c.Sex order by c.Sex";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<CustomerCountCensus> list = DAL.EntityDataHelper.FillData2Entities<CustomerCountCensus>(strSql, paramters);
            List<PieCensus> censusList = new List<PieCensus>();
            int count = C_Consumer.GetCCount(type);
            foreach (CustomerCountCensus item in list)
            {
                censusList.Add(new PieCensus
                {
                    name = item.name,
                    y = item.Count % count
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }

        /// <summary>
        /// 分销结构树
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static SearchZtree GetOptionDisTreeUserMsg(string UserName)
        {
            string strSql = " select c.ID ID,c.CardUrl CardUrl, c.Name Name,c.PortraitUrl PortraitUrl,c.Phone Phone,c.Card Card,c.C_UserTypeID C_UserTypeID,c.DatCreate DatCreate,c.DatVerify DatVerify,c.state state,c.Identifier Identifier,c.Chief Chief,t.Name LevelName from C_User as  c left join C_UserType as t on t.Lever=c.C_UserTypeID where c.UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };
            SearchZtree UserMsg = DAL.EntityDataHelper.LoadData2Entity<SearchZtree>(strSql, paramters);
            if (UserMsg == null)
            {
                return null;
            }
            return UserMsg;

        }


        /// <summary>
        /// 更新最新登录时间
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="DatLoginChange"></param>
        /// <returns></returns>
        public static int DatLoginChangeByUserName(string UserName)
        {
            string strSql = "UPDATE [C_User] SET DatLogin=@DatLogin WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@DatLogin",DateTime.Now)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 更新最新修改密码时间
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="DatPwdChange"></param>
        /// <returns></returns>
        public static int DatPwdChangeByUserName(string UserName)
        {
            string strSql = "UPDATE [C_User] SET DatPwdChange=@DatPwdChange WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@DatPwdChange",DateTime.Now),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        public static List<C_Interface> Getusername()
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT UserName,Name FROM C_User where Chief=0");
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<C_Interface>(strSql, paramters);
        }

        public static List<C_Interface> Getusername(string KeyWords)
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT UserName,Name FROM C_User where UserName+Name like '%{0}%'", KeyWords);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<C_Interface>(strSql, paramters);
        }

        public static List<C_Interface> Getusername(int Timestamp)
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT UserName,Name FROM C_User where Chief=0 and state<>'已删除' and UpdateTime>=@UpdateTime");
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UpdateTime", Timestamp) };
            return DAL.EntityDataHelper.FillData2Entities<C_Interface>(strSql, paramters);
        }

        /// <summary>
        /// 获取所有下级用户
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static List<C_User> GetAllSubUser(int UserId)
        {
            string strSql = "select * from C_User where ID in(select UserID from dbo.[GetAllSubUser](@UserId)) where ID<>@UserId";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserId", UserId) };
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }

        /// <summary>
        /// 获取直属下级列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static List<ShowSubUser> GetDirectSubUser(int UserId, string KeyWords, int PageSize, int PageIndex, out int totalCount)
        {
            string where = string.Empty;

            where = " Chief=" + UserId.ToString();
            
            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" and UserName+Name+[Address] like '%" + KeyWords + "%'";
            }

            string strSql = "exec dbo.Common_PageList 'View_C_User','*','UserName',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };

            List<ShowSubUser> OrderList = DAL.EntityDataHelper.FillData2Entities<ShowSubUser>(strSql, paramters);

            strSql = "select count(*) from View_C_User where " + where;
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;
        }

        /// <summary>
        /// 获取用户集合
        /// </summary>
        /// <param name="UserNameSet"></param>
        /// <returns></returns>
        public static List<C_User> GetUserSet(string UserNameSet)
        {
            string strSql = "SELECT * FROM [C_User] WHERE UserName in (" + UserNameSet + ")";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
        }

    }
}
