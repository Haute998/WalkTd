using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.C_UserModel
{
    public class C_UserUpGradeVMSearch:BaseSearch 
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
    public class C_UserUpGradeVM : C_UserUpGrade
    {
        /// <summary>
        /// 代理姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代理微信号
        /// </summary>
        public string wxNo { get; set; }
        /// <summary>
        /// 代理手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 获取单条申请记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static C_UserUpGradeVM getVMByID(int id) 
        {
            string strSql = @"select C_UserUpGrade.*,C_User.Name,C_User.wxNo,C_User.Phone,C_UserType.Name OldUserTypeName,newType.Name NewUserTypeName
                            from C_UserUpGrade 
                           left join C_User on C_UserUpGrade.UserName=C_User.UserName    
                           left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever
                           left join C_UserType newType on C_UserUpGrade.NewUserTypeID=newType.Lever where C_UserUpGrade.ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserUpGradeVM>(strSql, paramters);
        }
 public static bool GetNoPromote(int[] ID)
        {
            string strSql = @"UPDATE  C_UserUpGrade  SET AuditStat='已取消' WHERE ID=@ID";
                  System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID[0]) };
             return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters)>0?true:false;
        }
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        public static string AuditByIDs(int[] cklst)
        {
            List<C_UserUpGrade> UpGradeLst = new List<C_UserUpGrade>();
            string IDs = "";
            string error = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                IDs += cklst[i] + ",";
                C_UserUpGrade UpGrade = C_UserUpGrade.GetEntityMoreByID(cklst[i]);
                if (UpGrade.P_UserID == 0 && !string.IsNullOrWhiteSpace(UpGrade.ParentUser)) 
                {
                    error = "申请升级遇到了问题";
                    break;
                }
                UpGradeLst.Add(UpGrade);
            }
            if (!string.IsNullOrWhiteSpace(error)) 
            {
                return error;
            }

            IDs = IDs.TrimEnd(',');

            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string strSql = string.Format("UPDATE [C_UserUpGrade] SET AuditStat='已审核',DatAudit=@DatAudit WHERE ID in ({0});", IDs);
                    System.Data.SqlClient.SqlParameter[] paramters ={
                           new System.Data.SqlClient.SqlParameter("@DatAudit",DateTime.Now)
                       };
                    int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);

                    foreach (C_UserUpGrade UpGradeItem in UpGradeLst) 
                    {
                        string UpGradeSql = string.Format("UPDATE [C_User] SET C_UserTypeID={0},Chief={1} WHERE UserName='{2}'", UpGradeItem.NewUserTypeID, UpGradeItem.P_UserID, UpGradeItem.UserName);
                        System.Data.SqlClient.SqlParameter[] paraUpGrade = null;
                        cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text,UpGradeSql, paraUpGrade);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "C_UserUpGradeVM_AuditByIDs_Error");
                    return "审核失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }


            return "ok";

        }


    }
}
