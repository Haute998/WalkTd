using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.Models.C_UserModel;

namespace WeModels
{
    public partial class C_UserUpGrade
    {
        /// <summary>
        /// 上级ID
        /// </summary>
        public int P_UserID { get; set; }
        /// <summary>
        /// 原来代理级别名称
        /// </summary>
        public string OldUserTypeName { get; set; }
        /// <summary>
        /// 申请的代理级别名称
        /// </summary>
        public string NewUserTypeName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string C_Name { get; set; }

        /// <summary>
        /// 获取单条记录 拓展版  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static C_UserUpGrade GetEntityMoreByID(int id)
        {
            string strSql = "SELECT [C_UserUpGrade].*,ISNULL(C_User.ID,0) P_UserID FROM [C_UserUpGrade] left join C_User on [C_UserUpGrade].ParentUser=C_User.UserName WHERE C_UserUpGrade.ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserUpGrade>(strSql, paramters);
        }


        public static List<C_UserUpGrade> GetListByAuditStat(string AuditStat, string UserName)
        {
            string strSql = "SELECT * FROM [C_UserUpGrade] where UserName=@UserName and AuditStat=@AuditStat";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@AuditStat",AuditStat)
            };

            return DAL.EntityDataHelper.FillData2Entities<C_UserUpGrade>(strSql, paramters);
        }


        public static List<C_UserUpGrade> GetDetailListByAuditStat(string AuditStat, string UserName)
        {
            string strSql = @"SELECT [C_UserUpGrade].*,t1.Name OldUserTypeName,t2.Name NewUserTypeName FROM [C_UserUpGrade] 
                                      left join C_UserType t1 on [C_UserUpGrade].OldUserTypeID=t1.ID
                                      left join C_UserType t2 on [C_UserUpGrade].NewUserTypeID=t2.ID 
                                      where UserName=@UserName and AuditStat=@AuditStat";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                new System.Data.SqlClient.SqlParameter("@AuditStat",AuditStat)
            };

            return DAL.EntityDataHelper.FillData2Entities<C_UserUpGrade>(strSql, paramters);
        }
        /// <summary>
        /// 申请升级
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="C_UserTypeID"></param>
        /// <returns></returns>
        public static string toUpGrade(string UserName, int C_UserTypeID)
        {
            try
            {
                List<C_UserUpGrade> doings = C_UserUpGrade.GetListByAuditStat("未审核", UserName);

                if (doings.Count > 0)
                {
                    return "您有一个升级申请正在审核中，暂时无法提交新的申请";
                }

                C_UserVM user = C_UserVM.GetVMByUserName(UserName);//自己

                int MaxLevel = C_UserType.GetMaxLevel();//最大等级
                C_UserVM P_User = C_UserVM.GetVMByID(user.Chief);//上级
                string yuanUser = "";
                if (user.Chief == 0)
                {
                    yuanUser = "0";
                }
                else
                {
                    yuanUser = P_User.UserName;
                }



                int P_Chief = 0;
                if (P_User != null)
                {
                    P_Chief = P_User.ID;//上级ID
                }


                C_UserType userType = C_UserType.GetEntityByLever(C_UserTypeID);
                if (userType == null)
                {
                    return "您所选的等级已被删除";
                }

                while (true)
                {
                    if (userType.Lever == MaxLevel || P_User == null)
                    {
                        P_User = null;
                        P_Chief = 0;
                        break;
                    }
                    if (userType.Lever <= P_User.userTypeLever)
                    {
                        P_User = C_UserVM.GetVMByID(P_User.Chief);
                        if (P_User == null)
                        {
                            P_User = null;
                            P_Chief = 0;
                            break;
                        }
                        P_Chief = P_User.ID;
                    }
                    else
                    {
                        break;
                    }
                }




                C_UserUpGrade model = new C_UserUpGrade();
                model.DatApply = DateTime.Now;
                model.AuditStat = "未审核";
                model.OldUserTypeID = user.C_UserTypeID;
                model.NewUserTypeID = C_UserTypeID;
                model.ParentUser = (P_Chief == 0 ? "" : C_User.GetUserNameByID(P_Chief));
                model.UserName = UserName;
                model.OldParentUser = yuanUser;
                model.InsertAndReturnIdentity();

                return "ok";
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "toUpGrade_error");
                return "申请遇到问题";
            }
        }
        ///获取跨级申请

        public static int GetCountUpGrade(string UserName)
        {
            string SqlStr = "SELECT count(*) FROM C_UserUpGrade where OldParentUser=@UserName and ParentUser!=OldParentUser and AuditStat='未审核'";
            System.Data.SqlClient.SqlParameter[] SqlParameter = {
                                                                  new System.Data.SqlClient.SqlParameter("@UserName",UserName)
                                                                };
        return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr,SqlParameter));
        }

        /// <summary>
        /// 获取未审核订单(红点)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetC_UserByUpGrade(string AuditStat)
        {
            string strSql = "SELECT Count(*) FROM [C_UserUpGrade] WHERE   ParentUser='' and AuditStat=@AuditStat";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@AuditStat", AuditStat) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
    }
}
