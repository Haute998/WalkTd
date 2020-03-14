using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class LotteryPrizes 
    {
        public Order order { get; set; }
        
        /// <summary>
        /// 获取某活动奖项个数
        /// </summary>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static int GetPrizesCnt(int ActivityID)
        {
            try
            {
                string sql = "select count(1) from LotteryPrizes where ActivityID=@ActivityID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ActivityID", ActivityID)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string cntStr= obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(cntStr,out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "LotteryPrizes_GetPrizesCnt_error");
                return 0;
            }
        }
        /// <summary>
        /// 获取已编辑完成的奖品
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public static List<LotteryPrizes> GetPrizesByActivityID(int activityid)//
        {
            string strSql = "SELECT * FROM [LotteryPrizes] WHERE ActivityID=@ActivityID and IsFinish=1";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ActivityID", activityid) };

            return DAL.EntityDataHelper.FillData2Entities<LotteryPrizes>(strSql, paramters);
        }

        /// <summary>
        /// 总中奖奖品数
        /// </summary>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static int GetWonPrizesCnt(int ActivityID)
        {
            try
            {
                string sql = "select sum(WonTime) from LotteryPrizes where ActivityID=@ActivityID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ActivityID", ActivityID)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string cntStr = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(cntStr, out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "LotteryPrizes_GetWonPrizesCnt_error");
                return 0;
            }
        }

        /// <summary>
        /// 某个活动今天抽奖次数
        /// </summary>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static int GetPrizesUserCnt(string UserOpenId)
        {
            try
            {
                string sql = "select count(1) from LotteryRecord where UserOpenId=@UserOpenId and DateDiff(dd,Dat,getdate())=0 ";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@UserOpenId", UserOpenId)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string cntStr = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(cntStr, out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "LotteryRecord");
                return 0;
            }
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = "UPDATE [LotteryPrizes] SET PrizeLevel=@PrizeLevel,PrizeName=@PrizeName,PrizeImgUrl=@PrizeImgUrl,WinningRate=@WinningRate,IsFinish=@IsFinish,PrizeQty=@PrizeQty WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@PrizeLevel",_prizelevel),
                new System.Data.SqlClient.SqlParameter("@PrizeName",_prizename),
                new System.Data.SqlClient.SqlParameter("@PrizeImgUrl",_prizeimgurl),
                new System.Data.SqlClient.SqlParameter("@WinningRate",_winningrate),
                new System.Data.SqlClient.SqlParameter("@IsFinish",_isfinish),
                new System.Data.SqlClient.SqlParameter("@PrizeQty",_prizeqty),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        //获取奖品信息
        public static List<LotteryPrizes> GoodList()
        {
            string strSql = @"Select PrizeLevel,PrizeName from LotteryPrizes where 1=1";
            System.Data.SqlClient.SqlParameter[] paramters = {
                                                             //new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName)
                                                             };
            return DAL.EntityDataHelper.FillData2Entities<LotteryPrizes>(strSql, paramters);
        }

        /// <summary>
        /// 创建订单  在此生成订单编号
        /// </summary>
        /// <returns></returns>
        private string CreateOrder()
        {
            if (order == null || PrizeName == null) { return "创建订单失败"; }
            SqlConnection con = SqlHelper.DefaultConnection;
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                order.OrderNo = OnlyNoBulid.NextBillNumber();//生成订单编号
                order.ID = order.InsertAndReturnIdentity(tran);
                if (order.ID == 0)
                {
                    tran.Rollback();
                    return "创建订单失败";
                }
           

                tran.Commit();

                OrderLog.LogAdd(order.OrderName, "奖品", "客户" + order.OrderMan + "领取成功", "Mobile");
                return "ok";

            }
            catch (Exception ex)
            {
                tran.Rollback();
                Log.Instance.Write(ex.Message, "CreateOrder_Error");
                return "创建订单失败";
            }
            finally
            {
                con.Close();
            }


        }
        public static LotteryActivitys GetEntityByPage(int ActivityID)
        {
            string strSql = "SELECT ID,Title,DatB,DatE,IsTimeLimit,IsValid,IsActive,Explain FROM [LotteryActivitys] WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new SqlParameter("@ID", ActivityID) };

            return DAL.EntityDataHelper.LoadData2Entity<LotteryActivitys>(strSql, paramters);
        }
        private static Random rnd = new Random();
        /// <summary>
        /// 获取抽奖结果(概率计算)
        /// </summary>
        /// <param name="cyPrizes">各物品的抽中概率</param>
        /// <returns>返回抽中的物品所在数组的位置</returns>
        private static LotteryPrizes RanPrizes(List<LotteryPrizes> cyPrizes)
        {
            LotteryPrizes result = new LotteryPrizes();
            int n = (int)(cyPrizes.Sum(m=>m.WinningRate) * 1000);           //计算概率总和，放大1000倍
            Random r = rnd;
            decimal x = (decimal)r.Next(0, n) / 1000;       //随机生成0~概率总和的数字

            for (int i = 0; i < cyPrizes.Count(); i++)
            {
                decimal pre = cyPrizes.Take(i).Sum(m=>m.WinningRate);         //区间下界
                decimal next = cyPrizes.Take(i + 1).Sum(m=>m.WinningRate);    //区间上界
                if (x >= pre && x < next)               //如果在该区间范围内，就返回结果退出循环
                {
                    result = cyPrizes[i];
                    break;
                }
            }
            return result;
        }

        public static bool IsNotCanActive()
        {   
            string strSql = "select * from LotteryPrizes where IsNot=1";
            System.Data.SqlClient.SqlParameter[] Parameter = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, Parameter)) > 0 ? true : false;
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="fwm"></param>
        /// <returns></returns>
        public static PrizeAttr toPrizeDraw(string fwm, int ActivityID, string Phone, string Name, string UserOpenId, WXUserInfo UserInfo)
        {
            PrizeAttr prizeAttr = new PrizeAttr();
            prizeAttr.IsPrize = false;
            prizeAttr.IsNot = false;

            try
            {
                // UserOpenId = "123";

                if (string.IsNullOrWhiteSpace(UserOpenId))
                {
                    prizeAttr.ResultStr = "抽奖失败，无法获取到帐户信息!";
                    return prizeAttr;
                }
                int setnum = BaseParameters.getUpdate("Luckytimes");///抽奖次数
                int num = LotteryPrizes.GetPrizesUserCnt(UserOpenId);
                if (num >= setnum)
                {
                    prizeAttr.ResultStr = "您已超出当日最大抽奖次数!";
                    return prizeAttr;
                }

                LotteryActivitys activ = LotteryActivitys.GetEntityByID(ActivityID);

                if (activ == null)
                {
                    prizeAttr.ResultStr = "活动不存在";
                    return prizeAttr;
                }
                if (activ.IsActive == false)
                {
                    prizeAttr.ResultStr = "活动还没上线呢";
                    return prizeAttr;
                }
                if (activ.IsTimeLimit)
                {
                    if (activ.DatB > DateTime.Now)
                    {
                        prizeAttr.ResultStr = "活动还没开始呢";
                        return prizeAttr;
                    }
                    if (activ.DatE < DateTime.Now)
                    {
                        prizeAttr.ResultStr = "活动已经结束啦";
                        return prizeAttr;
                    }
                }

                Scale scale = Scale.GetCAntiFake(fwm);
                if (scale == null)
                {
                    prizeAttr.ResultStr = "防伪码不存在，无抽奖权限";
                    return prizeAttr;
                }
                //if (!(scale != null && scale.CodeState == "已出货"))
                //{
                //    prizeAttr.ResultStr = "防伪码不存在或未出货";
                //    return prizeAttr;
                //}
                else if (scale.StateID == 4)
                {
                    prizeAttr.ResultStr = "此防伪码禁用，无抽奖权限";
                    return prizeAttr;
                }
                else if (scale.StateID == 7)        // 已出货
                {
                    LotteryRecord record = LotteryRecord.GetRecByIntegralCode(fwm);
                    if (record != null)
                    {
                        prizeAttr.ResultStr = "此防伪码已抽过奖";
                        return prizeAttr;
                    }
                }
                else
                {
                    prizeAttr.ResultStr = "防伪码不存在或未出货，无抽奖权限";
                    return prizeAttr;
                }


                MainShow UserScale = Scale.GetScaleCount();
                // 按概率的奖品
                List<LotteryPrizes> ListPrizes = LotteryPrizes.GetPrizesByActivityID(ActivityID);//所有奖品（编辑完成的）
                int allWonCnt = ListPrizes.Sum(m => m.WonTime);         //总中奖奖品数
                int thisTime = allWonCnt + 1;                       //本次中奖是第几次
                int codecnt = UserScale.ScaleCount;

                LotteryPrizes prize = RanPrizes(ListPrizes, UserScale);

                using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
                {
                    conn.Open();
                    System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                    try
                    {
                        string strSql = "UPDATE [LotteryPrizes] SET WonTime=WonTime+1 WHERE ID=@ID;";
                        System.Data.SqlClient.SqlParameter[] paramters ={
                           new System.Data.SqlClient.SqlParameter("@ID",prize.ID)
                       };
                        DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
                        //添加中奖记录
                        LotteryRecord rec = new LotteryRecord();
                        rec.ActivityID = activ.ID;
                        rec.ActivityTitle = activ.Title;
                        rec.Dat = DateTime.Now;
                        rec.IntegralCode = fwm;
                        rec.PrizeID = prize.ID;
                        rec.PrizeImgUrl = prize.PrizeImgUrl;
                        rec.PrizeLevel = prize.PrizeLevel;
                        rec.PrizeName = prize.PrizeName;
                        //rec.Name = Name;
                        //rec.Phone = Phone;
                        rec.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                        rec.WinningRate = prize.WinningRate;
                        rec.States = "未发放";
                        rec.UserOpenId = UserOpenId;
                        rec.IsNot = prize.IsNot;
                        rec.UserWxName = UserInfo.nickname;
                        rec.UserWxImg = UserInfo.headimgurl;
                        prizeAttr.ID = rec.InsertAndReturnIdentity(tran);

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        DAL.Log.Instance.Write(ex.ToString(), "toPrizeDraw_Error");
                        prizeAttr.ResultStr = "活动异常，请稍候再试";
                        return prizeAttr;
                    }
                    finally
                    {
                        tran.Dispose();
                        conn.Close();
                    }
                }

                prizeAttr.PrizeID = prize.ID;
                prizeAttr.IsPrize = true;
                prizeAttr.IsNot = prize.IsNot;
                return prizeAttr;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "toPrizeDraw_Error");
                prizeAttr.ResultStr = "活动异常，请稍候再试";
                return prizeAttr;
            }
        }

        private static LotteryPrizes RanPrizes(List<LotteryPrizes> ParamList, MainShow UserScale)
        {
            int MaxValue = UserScale.CanLotteryCount;

            List<WParam> ListParam = new List<WParam>();

            //int maxValue = 0;
            //double d = 0;
            int P = 1;
            for (int i = 0; i < ParamList.Count; i++)
            {
                int canQty = ParamList[i].PrizeQty - ParamList[i].WonTime;

                if (canQty > 0)
                {
                    WParam wp = new WParam();
                    wp.ID = ParamList[i].ID;
                    wp.MinValue = P;
                    wp.MaxValue = P + canQty - 1;
                    P = P + canQty;
                    ListParam.Add(wp);
                }
            }

            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            Random r = new Random(iSeed);
            int Rand = r.Next(1, MaxValue);

            bool IsCan = false;
            int PrizeID = 0;

            for (int i = 0; i < ListParam.Count; i++)
            {
                if ((Rand > ListParam[i].MinValue && Rand < ListParam[i].MaxValue) || ListParam[i].MinValue == Rand)
                {
                    PrizeID = ListParam[i].ID;
                    IsCan = true;
                    break;
                }
            }

            if (IsCan && PrizeID != 0) return ParamList.FirstOrDefault(m => m.ID == PrizeID);
            else return ParamList.FirstOrDefault(m => m.IsNot == true);
        }
    }
}
