using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WeModels
{
    public partial class Scale : BaseSearch
    {
        public string keyword { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string O_ID { get; set; }
        public int cs { get; set; }
        public int SmallQty { get; set; }
        public string ProductNumber { get; set; }
        public string SupplierName { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }

        public static int UpdateStatByID(int id, string stat, System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "UPDATE [Scale] SET exchangestat=@exchangestat WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@exchangestat",stat)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }

   
        /// <summary>
        /// 领取积分 返回ok为成功 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="IntegralCode"></param>
        /// <returns>ok|...   fail|...       ok时返回获得积分数量</returns>
        public static string GetIntegral(string UserName, string IntegralCode, string type)
        {
            C_Consumer user = C_Consumer.GetEntityByUserName(UserName);//钱包
            Scale codeModel = null;
            if (user.Type == "消费者")
            {

                codeModel = Scale.GetEntityBySmallCode(IntegralCode);
            }
            else 
            {
                codeModel = Scale.GetEntityBycxyCode(IntegralCode);
            }
            if (codeModel == null)
            {
                Log.Instance.Write(IntegralCode, "toGet_NoQRCode");
                return "fail|积分码不存在呀";
            }

            if (codeModel.IsIntegral)
            {
                return "fail|该积分码已被使用啦";
            }


            string CodePrefix = IntegralCode.SubStringSafe(0, 4);//积分码前缀

            SYSICodeIntegralSet ICodeIntegralSet = SYSICodeIntegralSet.GetEntityByCodePrefix(CodePrefix);
            if (ICodeIntegralSet == null)
            {
                return "fail|该积分码暂时没有积分，请稍候再试！";
            }


            C_UserIntegralCode userCode = new C_UserIntegralCode();
            userCode.Dat = DateTime.Now;
            userCode.IntegralType = type;
            userCode.IntegralCode = codeModel.AntiCode;
            userCode.IntegralCodeID = codeModel.ID;
            userCode.UserName = UserName;
            userCode.OldIntegral =user.jf;
            userCode.ThisIntegral = ICodeIntegralSet.IntegralCnt;//获得积分
            userCode.NewIntegral = user.jf + ICodeIntegralSet.IntegralCnt;
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    int rtn = 0;
                    int C_InteCodeID = 0;//积分收入记录表ID
                    rtn = userCode.InsertAndReturnIdentity(tran);
                    C_InteCodeID = rtn;//C_UserIntegralCode表ID
                    if (rtn <= 0)
                    {
                        tran.Rollback();
                        DAL.Log.Instance.Write("C_UserIntegralCode表新增失败：" + Newtonsoft.Json.JsonConvert.SerializeObject(userCode), "GetIntegral_Error");
                        return "fail|领取积分失败";
                    }
                    rtn = Scale.UpdateStatByID(codeModel.ID, "已使用", tran);
                    if (rtn <= 0)
                    {
                        tran.Rollback();
                        DAL.Log.Instance.Write("SYSIntegralCode表更新状态为已使用失败", "GetIntegral_Error");
                        return "fail|领取积分失败";
                    }
                    rtn = C_Consumer.addIntegral(UserName, ICodeIntegralSet.IntegralCnt, tran);
                    if (rtn <= 0)
                    {
                        tran.Rollback();
                        DAL.Log.Instance.Write("积分账户增加失败", "GetIntegral_Error");
                        return "fail|领取积分失败";
                    }

                    j_jflog log = new j_jflog();
                    log.Code = IntegralCode;
                    log.Dat = DateTime.Now;
                    log.jf = ICodeIntegralSet.IntegralCnt;
                    log.logContents = user.UserName + "使用积分码" + IntegralCode + "获得" + ICodeIntegralSet.IntegralCnt + "积分";
                    log.Type = "领取";
                    log.UserName = user.UserName;
                    log.UserType = user.Type;
                    log.InsertAndReturnIdentity(tran);


                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "GetIntegral_Error");
                    return "fail|领取积分失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return "ok|" + ICodeIntegralSet.IntegralCnt;
        }

        public static Scale GetEntityByAntiCode(string AntiCode)
        {
            string strSql = "select * from Scale where ID in(select ScaleId from Scale_Anti where AntiCode=@AntiCode)";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@AntiCode", AntiCode) };

            return DAL.EntityDataHelper.LoadData2Entity<Scale>(strSql, paramters);
        }

        public static Scale GetEntityBySmallCode(string SmallCode)
        {
            string strSql = "SELECT * FROM [Scale] WHERE AntiCode=@AntiCode";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@AntiCode", SmallCode) };

            return DAL.EntityDataHelper.LoadData2Entity<Scale>(strSql, paramters);
        }

        public static Scale GetScaleSmallCode(string SmallCode)
        {
            string strSql = "select * from Scale where ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode)";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@SmallCode", SmallCode) };

            return DAL.EntityDataHelper.LoadData2Entity<Scale>(strSql, paramters);
        }
   

        public static Scale GetEntityBycxyCode(string cxyCode)
        {
            string strSql = "SELECT * FROM [Scale] WHERE cxyCode=@cxyCode";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@cxyCode", cxyCode) };

            return DAL.EntityDataHelper.LoadData2Entity<Scale>(strSql, paramters);
        }

        /// <summary>
        /// 判断是否重复导入
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool GetBoolCodeRepeat(string[] list)
        {
            string SqlStr = "SELECT COUNT(*) FROM SCALE WHERE BigCode=@BigCode AND SmallCode=@SmallCode AND AntiCode=@AntiCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@BigCode",list[0]),
                 new System.Data.SqlClient.SqlParameter("@SmallCode",list[1]),
                 new System.Data.SqlClient.SqlParameter("@AntiCode",list[2])
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? false : true;
        }

        /// <summary>
        /// 扫描获取我的条码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static List<BarCode> GetScanCode(string code, string username)
        {
            try
            {
                string strSQL = "SELECT SCALE.SmallCode FROM SCALE inner join ScaleOutStoke on SCALE.SmallCode=ScaleOutStoke.SmallCode and [State]='启用' and Consignee=@UserName " +
                                "WHERE SCALE.BIGCODE=@Code or scale.MiddleCode=@Code or SCALE.SmallCode=@Code";
                System.Data.SqlClient.SqlParameter[] Parameter ={
                                                        new System.Data.SqlClient.SqlParameter("@UserName",username),
                                                        new System.Data.SqlClient.SqlParameter("@Code",code)
                                                    };
                return DAL.EntityDataHelper.FillData2Entities<BarCode>(strSQL, Parameter);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "获取扫描条码错误");
                return null;
            }
        }

        /// <summary>
        /// 判断是否入库
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetBoolInCode(string code)
        {
            string SqlStr = "SELECT COUNT(*) FROM SCALE WHERE SmallCode=@SmallCode and CodeState='已启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@SmallCode",code)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? false : true;
        }
        /// <summary>
        /// 获取大标
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Scale> GetBigScaleList(string code)
        {
            string SqlStr = "SELECT * FROM SCALE WHERE BIGCODE=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }
        /// <summary>
        /// 获取小标
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Scale> GetSmallScaleList(string code)
        {
            string SqlStr = "SELECT * FROM SCALE WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        /// <summary>
        /// 获取条码列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Scale> GetBarCodeList(string code)
        {
            string strSQL = "SELECT distinct * FROM SCALE " +
                        "WHERE ID in(select ScaleId from Scale_Big where BigCode=@Code) or " +
                        "ID in (select ScaleId from Scale_Middle where MiddleCode=@Code) or " +
                        "ID in(select ScaleId from Scale_Small where SmallCode=@Code)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@Code",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(strSQL, Parameter);
        }

        public static bool GetBoolSmallScaleCount(string code)
        {
            return GetSmallScaleList(code).Count > 0 ? true : false;
        }
        public static int GetUpdateScaleInState(string code)
        {
            string SqlStr = "Update SCALE set CodeState='已入库' WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }

        public static int UpdateScaleInState(string CodeSet, string UName, string IntoOrderNo, string ProductNo)
        {
            string SqlStr = "Update SCALE set StateID=6,IsInto=1,IntoPDAUser=@UName,IntoTime=dbo.GetNowTimestamp(),IntoOrderNo=@IntoOrderNo,ProductNo=@ProductNo WHERE StateID in (0,2,3) and SmallCode in (" + CodeSet + ")";
            System.Data.SqlClient.SqlParameter[] Parameter = {
                                                                new System.Data.SqlClient.SqlParameter("@IntoOrderNo",IntoOrderNo),
                                                                new System.Data.SqlClient.SqlParameter("@ProductNo",ProductNo),
                                                                new System.Data.SqlClient.SqlParameter("@UName",UName),
                                                             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }

        public static int GetUpdateScaleRtnState(string code)
        {
            string SqlStr = "Update SCALE set CodeState='已退货' WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }

        public static int UpdateScaleRtnState(string CodeSet, string RtnOrderNo, string UName)
        {
            string SqlStr = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,IsPDAOpera,Shipper,Consignee,OutOrderNo,OutTime,OutWay,RtnWay) " +
                            "select @OrderNo,b.BigCode,b.MiddleCode,b.SmallCode,b.AntiCode,b.ProductNo,@OperaUser,@IsPDAOpera,b.Shipper,b.Consignee,b.OutOrderNo,b.CreateTime,b.OutWay,1 " +
                            "from ScaleOut_Small as a left join ScaleOutStoke as b on a.OutId=b.ID where a.SmallCode in (" + CodeSet + "); " +
                            "delete ScaleOutStoke where ID in(select OutId from ScaleOut_Small where SmallCode in(" + CodeSet + "));" +
                            "update Scale set IsOut=0,StateID=9 where ID in(select ScaleId from Scale_Small where SmallCode in(" + CodeSet + "));";
            System.Data.SqlClient.SqlParameter[] Parameter = {
                                                                new System.Data.SqlClient.SqlParameter("@OrderNo",RtnOrderNo),
                                                                new System.Data.SqlClient.SqlParameter("@OperaUser",UName),
                                                                new System.Data.SqlClient.SqlParameter("@IsPDAOpera",false),
                                                                new System.Data.SqlClient.SqlParameter("@OutWay",1),
                                                             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }
        public static int GetUpdateScaleOutState(string code)
        {
            string SqlStr = "Update SCALE set CodeState='已出货' WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }
        public static int UpdateScaleOutState(string CodeSet, string UName,string OutOrderNo, string CUserName)
        {
            string SqlStr = "insert into ScaleOutStoke(Shipper,BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,Consignee,State,CreateTime,OutOrderNo,OutWay) " +
                        "select '总部',BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,@UserName,'启用',dbo.GetNowTimestamp(),@OutOrderNo,@OutWay " +
                        "from [Scale] where IsOut=0 and ID in(select ScaleId from Scale_Small where SmallCode in (" + CodeSet + "));";
            SqlStr += "Update SCALE set StateID=7,IsOut=1,OutPDAUser=@UName,OutTime=dbo.GetNowTimestamp(),OutOrderNo=@OutOrderNo,UserName=@UserName WHERE StateID in (0,2,3,6,9) and ID in (select ScaleId from Scale_Small where SmallCode in (" + CodeSet + "));";

            System.Data.SqlClient.SqlParameter[] Parameter = {
                                                                 new System.Data.SqlClient.SqlParameter("@UserName",CUserName),
                                                                 new System.Data.SqlClient.SqlParameter("@OutOrderNo",OutOrderNo),
                                                                 new System.Data.SqlClient.SqlParameter("@UName",UName),
                                                                 new System.Data.SqlClient.SqlParameter("@OutWay",1),
                                                             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }

        public static MainShow GetScaleCount()
        {
            //MainShow main = new MainShow();

            //main.CustomerCount = C_User.GetUserCount();
            //main.InScaleCount = GetIntoStockQty();
            //main.OutScaleCount = GetOutStockQty();
            //main.ScaleCount = main.InScaleCount - main.OutScaleCount;

            //return main;

            System.Data.SqlClient.SqlParameter[] Parameter = null;
            string strSQL = "select CustomerCount, InScaleCount,OutScaleCount,ScaleCount,CanLotteryCount from dbo.GetSysCount()";
            return DAL.EntityDataHelper.LoadData2Entity<MainShow>(strSQL, Parameter);
        }

        /// <summary>
        /// 统计入库数
        /// </summary>
        /// <returns></returns>
        public static int GetIntoStockQty()
        {
            string SqlStr = "select COUNT(*) from Scale where IsInto=1";
            SqlParameter[] Parameter = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter));
        }

        /// <summary>
        /// 统计出库数
        /// </summary>
        /// <returns></returns>
        public static int GetOutStockQty()
        {
            string SqlStr = "select COUNT(*) from Scale where IsOut=1";
            SqlParameter[] Parameter = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter));
        }
        public static bool GetAntiFake(string anti)
        {
            string SqlStr = "SELECT COUNT(*) FROM SCALE WHERE AntiCode=@anti";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@anti",anti)
             };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter)) > 0 ? true : false;
        }
        /// <summary>
        /// 获取防伪查询次数
        /// </summary>
        /// <param name="anti"></param>
        /// <returns></returns>
        public static Scale GetCAntiFake(string anti)
        {
            string SqlStr = "SELECT * FROM SCALE WHERE AntiCode=@AntiCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@AntiCode",anti)
             };
            return DAL.EntityDataHelper.LoadData2Entity<Scale>(SqlStr, Parameter);
        }

        /// <summary>
        /// 小标获取数据
        /// </summary>
        /// <param name="smallcode"></param>
        /// <returns></returns>
        public static Scale GetScaleForSmall(string smallcode)
        {
            string SqlStr = "select b.* from Scale_Small as a left join Scale as b on a.ScaleId=b.ID where a.SmallCode=@SmallCode";
            SqlParameter[] Parameter ={
                      new SqlParameter("@SmallCode",smallcode)
             };
            return DAL.EntityDataHelper.LoadData2Entity<Scale>(SqlStr, Parameter);
        }

        /// <summary>
        /// 查出中标关联大标
        /// </summary>
        /// <param name="middlecode"></param>
        /// <returns></returns>
        public static Scale GetMiddleLinkBigScale(string middlecode)
        {
            string SqlStr = "select top 1 * from Scale where IsLinkBig=1 and ID = (select top 1 ScaleId from Scale_Middle where MiddleCode=@MiddleCode)";
            SqlParameter[] Parameter ={
                      new SqlParameter("@MiddleCode",middlecode)
             };
            return DAL.EntityDataHelper.LoadData2Entity<Scale>(SqlStr, Parameter);
        }
        
        /// <summary>
        /// 中标获取数据
        /// </summary>
        /// <param name="middlecode"></param>
        /// <param name="IsNotOut">是否未出货</param>
        /// <returns></returns>
        public static List<Scale> GetScaleForMiddle(string middlecode)
        {
            string SqlStr = "select * from Scale where ID in (select ScaleId from Scale_Middle where MiddleCode=@MiddleCode) ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@MiddleCode",middlecode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        /// <summary>
        /// 大标获取数据
        /// </summary>
        /// <param name="BigCode"></param>
        /// <returns></returns>
        public static List<Scale> GetScaleForBig(string bigcode)
        {
            string SqlStr = "select * from Scale where ID in (select ScaleId from Scale_Big where BigCode=@BigCode)";
            SqlParameter[] Parameter ={
                      new SqlParameter("@BigCode",bigcode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        /// <summary>
        /// 检测出货订单是否存在
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public static bool CheckOutOrderNo(string OrderNo)
        {
            string SqlStr = "SELECT COUNT(*) FROM SCALE WHERE OutOrderNo=@OutOrderNo";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@OutOrderNo",OrderNo)
             };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter)) > 0 ? true : false;
        }

        /// <summary>
        /// 根据Middle更新行
        /// </summary>
        public int UpdateByMiddle()
        {
            string strSql = @"UPDATE [Scale] SET StateID=@StateID,BigCode=@BigCode,IsLinkBig=@IsLinkBig,LinkBigTime=@LinkBigTime,
                            LinkBigPDAUser=@LinkBigPDAUser,SupplierId=@SupplierId,ProductNo=@ProductNo,LinkBigOrderNo=@LinkBigOrderNo,
                            IsInto=@IsInto,IntoOrderNo=@IntoOrderNo,IntoTime=@IntoTime,IntoPDAUser=@IntoPDAUser
                            WHERE ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@IsLinkBig",_islinkbig),
                new System.Data.SqlClient.SqlParameter("@LinkBigTime",_linkbigtime),
                new System.Data.SqlClient.SqlParameter("@LinkBigPDAUser",_linkbigpdauser),
                new System.Data.SqlClient.SqlParameter("@SupplierId",_supplierid),
                new System.Data.SqlClient.SqlParameter("@ProductNo",_productno),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
                new System.Data.SqlClient.SqlParameter("@LinkBigOrderNo",_linkbigorderno),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
                new System.Data.SqlClient.SqlParameter("@IntoOrderNo",_intoorderno),
                new System.Data.SqlClient.SqlParameter("@IntoTime",_intotime),
                new System.Data.SqlClient.SqlParameter("@IntoPDAUser",_intopdauser),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 中标入库
        /// </summary>
        public int MiddleIntoStock()
        {
            string strSql = "UPDATE [Scale] SET IsInto=@IsInto,IntoPDAUser=@IntoPDAUser,IntoTime=@IntoTime,ProductNo=@ProductNo,IntoOrderNo=@IntoOrderNo,SupplierId=@SupplierId,StateID=@StateID WHERE ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
                new System.Data.SqlClient.SqlParameter("@IntoPDAUser",_intopdauser),
                new System.Data.SqlClient.SqlParameter("@ProductNo",_productno),
                new System.Data.SqlClient.SqlParameter("@IntoTime",_intotime),
                new System.Data.SqlClient.SqlParameter("@IntoOrderNo",_intoorderno),
                new System.Data.SqlClient.SqlParameter("@SupplierId",_supplierid),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 大标入库
        /// </summary>
        public int BigIntoStock()
        {
            string strSql = "UPDATE [Scale] SET IsInto=@IsInto,IntoPDAUser=@IntoPDAUser,IntoTime=@IntoTime,ProductNo=@ProductNo,IntoOrderNo=@IntoOrderNo,SupplierId=@SupplierId,StateID=@StateID WHERE ID in(select ScaleId from Scale_Big where BigCode=@BigCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
                new System.Data.SqlClient.SqlParameter("@IntoPDAUser",_intopdauser),
                new System.Data.SqlClient.SqlParameter("@ProductNo",_productno),
                new System.Data.SqlClient.SqlParameter("@IntoTime",_intotime),
                new System.Data.SqlClient.SqlParameter("@IntoOrderNo",_intoorderno),
                new System.Data.SqlClient.SqlParameter("@SupplierId",_supplierid),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 中标出货
        /// </summary>
        public int MiddleOutStock()
        {

            string strSql = "UPDATE [Scale] SET StateID=@StateID,IsOut=@IsOut,OutTime=@OutTime,UserName=@UserName,OutPDAUser=@OutPDAUser,OutOrderNo=@OutOrderNo,OutWay=2,ProductNo=(case when ltrim(ProductNo)='' or ProductNo is null then @ProductNo else ProductNo end) " +
                            "WHERE IsOut=0 and ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            strSql += "insert into ScaleOutStoke(Shipper,BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,Consignee,State,CreateTime,OutOrderNo,OutWay) " +
                        "select '总部',BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,@UserName,'启用',@OutTime,@OutOrderNo,@OutWay " +
                        "from [Scale] where IsOut=1 and OutWay=2 and ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@IsOut",_isout),
                new System.Data.SqlClient.SqlParameter("@OutTime",_outtime),
                new System.Data.SqlClient.SqlParameter("@UserName",_username),
                new System.Data.SqlClient.SqlParameter("@OutPDAUser",_outpdauser),
                new System.Data.SqlClient.SqlParameter("@OutOrderNo",_outorderno),
                new System.Data.SqlClient.SqlParameter("@OutWay",_outway),
                new System.Data.SqlClient.SqlParameter("@ProductNo",_productno),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 大标出货
        /// </summary>
        public int BigOutStock()
        {
            string strSql = "UPDATE [Scale] SET StateID=@StateID,IsOut=@IsOut,OutTime=@OutTime,UserName=@UserName,OutPDAUser=@OutPDAUser,OutOrderNo=@OutOrderNo,OutWay=3,ProductNo=(case when ltrim(ProductNo)='' or ProductNo is null then @ProductNo else ProductNo end) WHERE IsOut=0 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode);";
            strSql += "insert into ScaleOutStoke(Shipper,BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,Consignee,State,CreateTime,OutOrderNo,OutWay)" +
            "select '总部',BigCode,SmallCode,MiddleCode,AntiCode,ProductNo,@UserName,'启用',@OutTime,@OutOrderNo,@OutWay " +
            "from [Scale] where IsOut=1 and OutWay=3 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@IsOut",_isout),
                new System.Data.SqlClient.SqlParameter("@OutTime",_outtime),
                new System.Data.SqlClient.SqlParameter("@UserName",_username),
                new System.Data.SqlClient.SqlParameter("@OutPDAUser",_outpdauser),
                new System.Data.SqlClient.SqlParameter("@OutOrderNo",_outorderno),
                new System.Data.SqlClient.SqlParameter("@OutWay",_outway),
                new System.Data.SqlClient.SqlParameter("@ProductNo",_productno),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 解除中大标关联
        /// </summary>
        /// <returns></returns>
        public int RemoveMiddleAndBigCode()
        {
            string strSql = "UPDATE [Scale] SET BigCode=@BigCode,IsLinkBig=@IsLinkBig,LinkBigPDAUser=@LinkBigPDAUser,LinkBigTime=@LinkBigTime,StateID=@StateID,IsInto=@IsInto WHERE ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@IsLinkBig",_islinkbig),
                new System.Data.SqlClient.SqlParameter("@LinkBigPDAUser",_linkbigpdauser),
                new System.Data.SqlClient.SqlParameter("@LinkBigTime",_linkbigtime),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 移除中标入库
        /// </summary>
        /// <returns></returns>
        public int RemoveMiddleInto()
        {
            string strSql = "UPDATE [Scale] SET IsInto=@IsInto,IntoPDAUser=@IntoPDAUser,IntoTime=@IntoTime,StateID=@StateID WHERE ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
                new System.Data.SqlClient.SqlParameter("@IntoPDAUser",_intopdauser),
                new System.Data.SqlClient.SqlParameter("@IntoTime",_intotime),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 移除大标入库
        /// </summary>
        /// <returns></returns>
        public int RemoveBigInto()
        {
            string strSql = "UPDATE [Scale] SET IsInto=@IsInto,IntoPDAUser=@IntoPDAUser,IntoTime=@IntoTime,StateID=@StateID WHERE ID in(select ScaleId from Scale_Big where BigCode=@BigCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@IsInto",_isinto),
                new System.Data.SqlClient.SqlParameter("@IntoPDAUser",_intopdauser),
                new System.Data.SqlClient.SqlParameter("@IntoTime",_intotime),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 移除中标出货
        /// </summary>
        /// <returns></returns>
        public int RemoveMiddleOut()
        {
            string strSql = "UPDATE [Scale] SET IsOut=@IsOut,OutPDAUser=@OutPDAUser,OutTime=@OutTime,StateID=@StateID,OutWay=0 WHERE OutWay=2 and ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode);delete [ScaleOutStoke] WHERE OutWay=2 and ID in(select OutId from ScaleOut_Middle where MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",_middlecode),
                new System.Data.SqlClient.SqlParameter("@IsOut",_isout),
                new System.Data.SqlClient.SqlParameter("@OutPDAUser",_outpdauser),
                new System.Data.SqlClient.SqlParameter("@OutTime",_outtime),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 移除大标出货
        /// </summary>
        /// <returns></returns>
        public int RemoveBigOut()
        {
            string strSql = "UPDATE [Scale] SET IsOut=@IsOut,OutPDAUser=@OutPDAUser,OutTime=@OutTime,StateID=@StateID,OutWay=0 WHERE OutWay=3 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode);delete [ScaleOutStoke] WHERE OutWay=3 and ID in(select OutId from ScaleOut_Big where BigCode=@BigCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",_bigcode),
                new System.Data.SqlClient.SqlParameter("@IsOut",_isout),
                new System.Data.SqlClient.SqlParameter("@OutPDAUser",_outpdauser),
                new System.Data.SqlClient.SqlParameter("@OutTime",_outtime),
                new System.Data.SqlClient.SqlParameter("@StateID",_stateid),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 小标退货
        /// </summary>
        public int SmallReturnStock(ScaleRtnStoke RtnObj)
        {
            string strSql = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,IsPDAOpera,Shipper,Consignee,OutOrderNo,OutTime,OutWay,RtnWay)" +
                            "select @OrderNo,b.BigCode,b.MiddleCode,b.SmallCode,b.AntiCode,b.ProductNo,@OperaUser,@IsPDAOpera,b.Shipper,b.Consignee,b.OutOrderNo,b.CreateTime,b.OutWay,@RtnWay " +
                            "from ScaleOut_Small as a left join ScaleOutStoke as b on a.OutId=b.ID where a.SmallCode=@SmallCode;";
            strSql += "delete ScaleOutStoke where ID in(select OutId from ScaleOut_Small where SmallCode=@SmallCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@SmallCode",RtnObj.SmallCode),
                new System.Data.SqlClient.SqlParameter("@OrderNo",RtnObj.OrderNo),
                new System.Data.SqlClient.SqlParameter("@OperaUser",RtnObj.OperaUser),
                new System.Data.SqlClient.SqlParameter("@IsPDAOpera",RtnObj.IsPDAOpera),
                new System.Data.SqlClient.SqlParameter("@RtnWay",RtnObj.RtnWay),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 中标退货
        /// </summary>
        public int MiddleReturnStock(ScaleRtnStoke RtnObj)
        {
            string strSql = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,IsPDAOpera,Shipper,Consignee,OutOrderNo,OutTime,OutWay,RtnWay)" +
                            "select @OrderNo,b.BigCode,b.MiddleCode,b.SmallCode,b.AntiCode,b.ProductNo,@OperaUser,@IsPDAOpera,b.Shipper,b.Consignee,b.OutOrderNo,b.CreateTime,b.OutWay,@RtnWay " +
                            "from ScaleOut_Middle as a left join ScaleOutStoke as b on a.OutId=b.ID where b.OutWay>1 and a.MiddleCode=@MiddleCode;";
            strSql += "delete ScaleOutStoke where ID in(select OutId from ScaleOut_Middle where OutWay>1 and MiddleCode=@MiddleCode);";
            strSql += "update Scale set IsOut=0,StateID=9 where ID in(select ScaleId from Scale_Middle where OutWay>1 and MiddleCode=@MiddleCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@MiddleCode",RtnObj.MiddleCode),
                new System.Data.SqlClient.SqlParameter("@OrderNo",RtnObj.OrderNo),
                new System.Data.SqlClient.SqlParameter("@OperaUser",RtnObj.OperaUser),
                new System.Data.SqlClient.SqlParameter("@IsPDAOpera",RtnObj.IsPDAOpera),
                new System.Data.SqlClient.SqlParameter("@RtnWay",RtnObj.RtnWay),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 大标退货
        /// </summary>
        public int BigReturnStock(ScaleRtnStoke RtnObj)
        {
            string strSql = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,IsPDAOpera,Shipper,Consignee,OutOrderNo,OutTime,OutWay,RtnWay)" +
                            "select @OrderNo,b.BigCode,b.MiddleCode,b.SmallCode,b.AntiCode,b.ProductNo,@OperaUser,@IsPDAOpera,b.Shipper,b.Consignee,b.OutOrderNo,b.CreateTime,b.OutWay,@RtnWay " +
                            "from ScaleOut_Big as a left join ScaleOutStoke as b on a.OutId=b.ID where b.OutWay=3 and a.BigCode=@BigCode;";
            strSql += "delete ScaleOutStoke where OutWay=3 and ID in(select OutId from ScaleOut_Big where BigCode=@BigCode);";
            strSql += "update Scale set IsOut=0,StateID=9 where OutWay=3 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode);";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@BigCode",RtnObj.BigCode),
                new System.Data.SqlClient.SqlParameter("@OrderNo",RtnObj.OrderNo),
                new System.Data.SqlClient.SqlParameter("@OperaUser",RtnObj.OperaUser),
                new System.Data.SqlClient.SqlParameter("@IsPDAOpera",RtnObj.IsPDAOpera),
                new System.Data.SqlClient.SqlParameter("@RtnWay",RtnObj.RtnWay),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public int OrderNoReturnStock(ScaleRtnStoke RtnObj)
        {
            string strSql = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,IsPDAOpera,Shipper,Consignee,OutOrderNo,OutTime,OutWay,RtnWay)" +
                            "select @OrderNo,b.BigCode,b.MiddleCode,b.SmallCode,b.AntiCode,b.ProductNo,@OperaUser,@IsPDAOpera,b.Shipper,b.Consignee,b.OutOrderNo,b.CreateTime,b.OutWay,b.OutWay " +
                            "from ScaleOut_Big as a left join ScaleOutStoke as b on a.OutId=b.ID where b.OutOrderNo=@OrderNo;";
            strSql += "delete ScaleOutStoke where OutOrderNo=@OrderNo;";
            strSql += "update Scale set IsOut=0,StateID=9 where OutOrderNo=@OrderNo;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@OrderNo",RtnObj.OrderNo),
                new System.Data.SqlClient.SqlParameter("@OperaUser",RtnObj.OperaUser),
                new System.Data.SqlClient.SqlParameter("@IsPDAOpera",RtnObj.IsPDAOpera),
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static List<Scale> GetSmallCodeInfo(string SmallCode)
        {
            string SqlStr = "select * From Scale where ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode) ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@SmallCode",SmallCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        public static List<Scale> GetMiddleCodeInfo(string MiddleCode)
        {
            string SqlStr = "select * From Scale where ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode) ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@MiddleCode",MiddleCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        public static List<Scale> GetBigCodeInfo(string BigCode)
        {
            string SqlStr = "select * From Scale where ID in(select ScaleId from Scale_Big where BigCode=@BigCode) ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@BigCode",BigCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        public static List<Scale> GetMiddleCodeUnboxingQuery(string MiddleCode)
        {
            string SqlStr = "select distinct BigCode,MiddleCode,StateID,COUNT(MiddleCode) as SmallQty,IsLinkBig,IsLinkMid " +
                            "From Scale where ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode) "+
                            "group by BigCode,StateID,MiddleCode,IsLinkBig,IsLinkMid  ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@MiddleCode",MiddleCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        public static List<Scale> GetBigCodeUnboxingQuery(string BigCode)
        {
            string SqlStr = "select distinct BigCode,MiddleCode,StateID,COUNT(MiddleCode) as SmallQty,IsLinkBig,IsLinkMid " +
                            "From Scale where ID in(select ScaleId from Scale_Big where BigCode=@BigCode) " +
                            "group by BigCode,StateID,MiddleCode,IsLinkBig,IsLinkMid ";
            SqlParameter[] Parameter ={
                      new SqlParameter("@BigCode",BigCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }

        public static int BatchRemoveIntoSmall(string IDSet)
        {
            string strSql = "update Scale set IsInto=0,StateID=(case when IsLinkBig=1 then 3 else case when IsLinkMid=1 then 2 else 0 end end) where ID in(" + IDSet + ") ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int BatchRemoveOutSmall(string CodeSet)
        {
            string strSql = "update Scale set IsOut=0,StateID=6 where ID in(select ScaleId from Scale_Small where SmallCode in (" + CodeSet + "));delete ScaleOutStoke where ID in(select OutId from ScaleOut_Small where SmallCode in (" + CodeSet + "))";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int BatchUnboxingSmall(string IDSet)
        {
            string strSql = "update Scale set BigCode='',MiddleCode='',IsLinkMid=0,IsLinkBig=0,StateID=0 where ID in(" + IDSet + ") ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int BatchUnboxingMiddle(string MiddleSet)
        {
            string strSql = "update Scale set BigCode='',IsLinkBig=0,StateID=(case when StateID=3 then case when IsLinkMid=1 then 2 else 0 end else StateID end) " +
                            "where ID in(select ScaleId from Scale_Middle where MiddleCode in(" + MiddleSet + "))";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 获取入库统计数量
        /// </summary>
        /// <returns></returns>
        public static ScaleCodeCount GetIntoStockCount(string where)
        {
            string SqlStr = "select COUNT(distinct case when BigCode<>'' then BigCode else null end) as BigCount," +
                            "COUNT(distinct case when MiddleCode<>'' then MiddleCode else null end) as MiddleCount," +
                            "COUNT(SmallCode) as SmallCount from Scale where IsInto=1 " + where;
            System.Data.SqlClient.SqlParameter[] Parameter =null;
            return DAL.EntityDataHelper.LoadData2Entity<ScaleCodeCount>(SqlStr, Parameter);
        }

        public static ScaleCodeCount GetStockCount(string where)
        {
            string SqlStr = "select COUNT(SmallCode) as SmallCount,sum(b.kw) as SumPrice " +
                            "from Scale as a left join Product as b on a.ProductNo=b.ProductNumber " +
                            "where StateID in (6,9) " + where;
            System.Data.SqlClient.SqlParameter[] Parameter =null;
            return DAL.EntityDataHelper.LoadData2Entity<ScaleCodeCount>(SqlStr, Parameter);
        }
    }

    public class ScaleCodeCount
    {
        public int BigCount { get; set; }
        public int MiddleCount { get; set; }
        public int SmallCount { get; set; }
        public decimal SumPrice { get; set; }
    }
    public class ScaleCode_Simple
    {
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        public string SmallCode { get; set; }
    }
    public class BigScale
    {
        public string ProductNo { get; set; }
        /// <summary>
        /// 小标数量
        /// </summary>
        public int SmallCount { get; set; }
        public decimal money { get; set; }
        /// <summary>
        /// 大标
        /// </summary>
        public string BigCode { get; set; }
        /// <summary>
        /// 小标
        /// </summary>
        public string SmallCode { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImg { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductNumber { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DatCreate {
            get { return CommonFunc.GetDateTimeFromTimestamp(CreateTime); }
            set { DatCreate = value; }
        }
    }
    public class BigScaleSearch : BaseSearch
    {
        public string BigCode { get; set; }
        public string SmallCode { get; set; }
        public string CK { get; set; }
        public string RtnCK { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string DatCreateB { get; set; }
        public string DatCreateE { get; set; }
        public string KeyWord { get; set; }
        public string PUserName { get; set; }
        public string ProductNumber { get; set; }
        public string OutOrderNo { get; set; }
        public string OrderNo { get; set; }
    }

    public class BarCode
    {
        public int ID { get; set; }
        public string SmallCode { get; set; }
    }
}
