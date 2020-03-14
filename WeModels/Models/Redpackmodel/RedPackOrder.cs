using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class RedPackOrder
    {
        public static int FinishStatByID(int id)
        {
            string strSql = "UPDATE [RedPackOrder] SET Stat=@Stat WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@Stat","已完成")
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
     

        /// <summary>
        /// 抢红包   返回结果示例  ok|20    fail|抢红包失败
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IntegralCode"></param>
        /// <returns></returns>
        public static string getRedPack(int id, string IntegralCode, string username, string ip)
        {
            LotteryActivitys act = LotteryActivitys.GetEntityByID(id);
            if (act == null)
            {
                return "fail|活动不存在";
            }
            if (act.IsActive == false)
            {
                return "fail|活动还没上线呢";
            }
            if (act.IsTimeLimit)
            {
                if (act.DatB > DateTime.Now)
                {
                    return "fail|活动还没开始呢";
                }
                if (act.DatE < DateTime.Now)
                {
                    return "fail|活动已经结束啦";
                }
            }

            C_UserWxVM user = new C_UserWxVM();
            user.LoadUserVMByUserName(username);

            DAL.Log.Instance.Write(user.userWxInfo.openid, "用户oppenid");

            if (user.userWxInfo == null)
            {
                return "fail|获取您的信息失败，请重新进入";
            }

            SYSIntegralCode code = SYSIntegralCode.GetEntityByIntegralCode(IntegralCode);
            
            if (code == null)
            {
                return "fail|抽奖码不存在";
            }

            List<ScaleOutStoke> modleout = ScaleOutStoke.GetListByAntiCode(IntegralCode);

            if (modleout.Count<1)
            {
                DAL.Log.Instance.Write(string.Format("用户：{0}| 防伪码：{1}|时间：{2}", username, IntegralCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "未出货防伪码抽奖");
                return "fail|红包尚未激活";
            }

            List<LotteryRecord> cords = LotteryRecord.GetCntUserNameIntegralCode(username, IntegralCode);
            if (cords.Count > 0) 
            {
                return "fail|红包派完啦！";
            }
            SYSIntegralCodeArea areamodel = SYSIntegralCodeArea.GetEntityByID(code.AreaID);
            if (areamodel == null)
            {
                return "fail|很遗憾，您什么都没抢到";
            }

            List<LotteryActivitysRedPack> p = LotteryActivitysRedPack.GetEntityActArea(id, code.AreaID);

            SYSIntegralCodeArea la = SYSIntegralCodeArea.GetEntitysl(code.AreaID);

            if (code.RedCnted >= la.cnt) 
            {
                return "fail|你来晚了，该红包已经抢完了哦";
            }
            int LAARPP_ID = 0;//概率配置ID
            decimal hongbao = 0;//最终红包金额
            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            rngServiceProvider.GetBytes(randomBytes);
            Int32 result = BitConverter.ToInt32(randomBytes, 0);
            Random rdm = new Random(result + 1);

            int redomNum = rdm.Next(10000);

            List<ValueAre> RdmAreList=new List<ValueAre>();
            int Loca=0;

            foreach (LotteryActivitysRedPack lottery in p)
            {
                ValueAre are = new ValueAre();
                are.StartVal = Loca;

                int LocaVal = (int)(lottery.Rate * 100);
                Loca += LocaVal;
                are.EndVal = Loca;

                if (redomNum >= are.StartVal && redomNum <= are.EndVal)
                {
                    Random ran2 = new Random();

                    decimal SeqValue = Math.Abs(lottery.MaxPrice - lottery.MinPrice);
                    SeqValue = SeqValue * 100;

                    int IRdm = rdm.Next((int)SeqValue);
                    double AdValue = (double)IRdm / 100.0;

                    hongbao = lottery.MinPrice + (decimal)AdValue;
                    if (hongbao > lottery.MaxPrice || hongbao < lottery.MinPrice)
                    {
                        hongbao = lottery.MinPrice;
                    }
                    break;
                }
            }

            if (hongbao <= 0)
            {
                SYSIntegralCode.StateByID(code.ID, "已抽奖", LAARPP_ID);
                LotteryRecord log = new LotteryRecord();
                log.ActivityID = act.ID;
                log.ActivityTitle = act.Title;
                log.Dat = DateTime.Now;
                log.IntegralCode = code.IntegralCode;
                log.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                log.UserName = username;
                log.redArea = areamodel.AreaName;
                log.redMoney = hongbao;
                log.InsertAndReturnIdentity();

                return "fail|" + act.FailMsg;
            }

            RedPackOrder order = new RedPackOrder();
            order.act_name = act.Title;
            order.OrderNo = "red" + DAL.OnlyNoBulid.NextBillNumber();
            order.remark = "获得红包";
            order.scene_id = "PRODUCT_3";
            order.total_amount = hongbao;
            order.total_num = 1;
            order.Stat = "未完成";
            order.wishing = act.Title;
            order.ID = order.InsertAndReturnIdentity();


            try
            {

                WXVariousApi api = new WXVariousApi();
                api.LoadWxConfigIncidentalAccess_token();
                WxData response = api.sendredpackToUser(order, user.userWxInfo.openid, ip);

                if (response == null)
                {
                    return "fail|领取红包失败，请稍候再试";
                }
                //成功通过
                if (response.GetValue("return_code").ToString() == "SUCCESS" && response.GetValue("result_code").ToString() == "SUCCESS")
                {
                                                             
                    FinishStatByID(order.ID);
                    SYSIntegralCode.StateByID(code.ID, "已抽奖", LAARPP_ID);
                    SYSIntegralCode.AddRedCntedByID(code.ID);
                    LotteryRecord log = new LotteryRecord();
                    log.ActivityID = act.ID;
                    log.ActivityTitle = act.Title;
                    log.Dat = DateTime.Now;
                    log.IntegralCode = code.IntegralCode;
                    log.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                    log.UserName = username;
                    log.redMoney = hongbao;
                    log.redArea = areamodel.AreaName;
                    log.InsertAndReturnIdentity();
                    return "ok|" + hongbao;

                }
                else
                {
                    return "fail|" + response.GetValue("err_code_des").ToString();

                }

                //*********************************************************************************
                // QQ.SendQQMailTo("获得红包" + hongbao + "元");
                //FinishStatByID(order.ID);
                //SYSIntegralCode.StateByID(code.ID, "已抽奖", LAARPP_ID);
                //SYSIntegralCode.AddRedCntedByID(code.ID);
                //LotteryRecord log = new LotteryRecord();
                //log.ActivityID = act.ID;
                //log.ActivityTitle = act.Title;
                //log.Dat = DateTime.Now;
                //log.IntegralCode = code.IntegralCode;
                //log.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                //log.UserName = username;
                //log.redMoney = hongbao;
                //log.redArea = areamodel.AreaName;
                //log.InsertAndReturnIdentity();
                //*************************************************************************************
                //return "ok|" + hongbao;

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "getRedPack_error");
                return "fail|商户还未配置红包账户，请稍候再试";
            }
        }

        /// <summary>
        /// 分享红包  返回结果示例  ok|20    fail|抢红包失败
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IntegralCode"></param>
        /// <returns></returns>
        public static string getRedSharePack(int id, string IntegralCode, string username, string ip)
        {
            LotteryActivitys act = LotteryActivitys.GetEntityByID(id);
            if (act == null)
            {
                return "fail|活动不存在";
            }
            if (act.IsActive == false)
            {
                return "fail|活动还没上线呢";
            }
            if (act.IsTimeLimit)
            {
                if (act.DatB > DateTime.Now)
                {
                    return "fail|活动还没开始呢";
                }
                if (act.DatE < DateTime.Now)
                {
                    return "fail|活动已经结束啦";
                }
            }

            C_UserWxVM user = new C_UserWxVM();
            user.LoadUserVMByUserName(username);

            if (user.userWxInfo == null)
            {
                return "fail|获取您的信息失败，请重新进入";
            }
            RedPackShare redmodel = RedPackShare.GetEntityByCode(IntegralCode);
            if (redmodel == null)
            {
                return "fail|你来晚了，该红包已经抢完了哦";
            }
            if (redmodel.ReceiveCnt >= redmodel.RedCnt)
            {
                return "fail|你来晚了，该红包已经抢完了哦！";
            }

            RedPackLottery code = RedPackLottery.GetEntityByUserName(username);
            if (code != null)
            {
                return "fail|您已经参与过此活动";
            }
            LotteryActivitysAreaRedPack LAARP = LotteryActivitysAreaRedPack.GetEntityActivityID(id);
            //int LAARPP_ID = 0;//概率配置ID
            //decimal hongbao = 0;//最终红包金额
            //#region 红包金额设置
            List<LotteryActivitysAreaRedPackPrice> p = LotteryActivitysAreaRedPackPrice.GetEntitysByLAARP_ID(LAARP.ID);
            //if (p == null || p.Count <= 0)
            //{
            //    hongbao = 0;
            //}
            //else
            //{

                //p = p.OrderBy(m => m.ID).ToList();
                //Random ran = new Random();
                //int RandKey = ran.Next(p[0].ID, p[p.Count - 1].ID);

                //do
                //{
                //    if (p.Count <= 0)
                //    {
                //        break;
                //    }


                    //List<ValueAre> ListParam = new List<ValueAre>();
                    //foreach (LotteryActivitysAreaRedPackPrice i in p)
                    //{
                    //    ValueAre pm = new ValueAre();
                    //    pm.ID = i.ID;
                    //    pm.Bility = Convert.ToDouble(i.Rate) / 100;
                    //    pm.MaxPrice = Convert.ToDouble(i.MaxPrice);
                    //    pm.MinPrice = Convert.ToDouble(i.MinPrice);
                    //    ListParam.Add(pm);
                    //}
                    //WParam param = Winning.GetWNumber(ListParam);

                    //hongbao = Convert.ToDecimal(param.WPrice); //Convert.ToDecimal(param.MinPrice);
                    //break;



                //} while (true);


            //}

            #region
            int LAARPP_ID = 0;//概率配置ID
            decimal hongbao = 0;//最终红包金额
            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            rngServiceProvider.GetBytes(randomBytes);
            Int32 result = BitConverter.ToInt32(randomBytes, 0);
            Random rdm = new Random(result + 1);

            int redomNum = rdm.Next(10000);

            List<ValueAre> RdmAreList=new List<ValueAre>();
            int Loca=0;

            foreach (LotteryActivitysAreaRedPackPrice lottery in p)
            {
                ValueAre are = new ValueAre();
                are.StartVal = Loca;

                int LocaVal = (int)(lottery.Rate * 100);
                Loca += LocaVal;
                are.EndVal = Loca;

                if (redomNum >= are.StartVal && redomNum <= are.EndVal)
                {
                    Random ran2 = new Random();

                    decimal SeqValue = Math.Abs(lottery.MaxPrice - lottery.MinPrice);
                    SeqValue = SeqValue * 100;

                    int IRdm = rdm.Next((int)SeqValue);
                    double AdValue = (double)IRdm / 100.0;

                    hongbao = lottery.MinPrice + (decimal)AdValue;
                    if (hongbao > lottery.MaxPrice || hongbao < lottery.MinPrice)
                    {
                        hongbao = lottery.MinPrice;
                    }
                    break;
                }
            }

            #endregion


            //List<LotteryActivitysAreaRedPackPrice> p = LotteryActivitysAreaRedPackPrice.GetEntitysByLAARP_ID(LAARP.ID);
            //if (p == null || p.Count <= 0)
            //{
            //    hongbao = 0;
            //}
            //else
            //{

            //    p = p.OrderBy(m => m.ID).ToList();
            //    Random ran = new Random();
            //    int RandKey = ran.Next(p[0].ID, p[p.Count - 1].ID);

            //    do
            //    {
            //        if (p.Count <= 0)
            //        {
            //            break;
            //        }
            //        LotteryActivitysAreaRedPackPrice thisp = p.FirstOrDefault(m => m.ID == RandKey);


            //        List<LotteryActivitysAreaRedPackPrice> thisg = LotteryActivitysAreaRedPackPrice.GetLoginBygl();
            //        List<WParam> ListParam = new List<WParam>();
            //        foreach (LotteryActivitysAreaRedPackPrice i in thisg)
            //        {
            //            WParam pm = new WParam();
            //            pm.ID = i.ID;
            //            pm.Bility = Convert.ToDouble(i.Rate) / 100;
            //            pm.MaxPrice = Convert.ToDouble(i.MaxPrice);
            //            pm.MinPrice = Convert.ToDouble(i.MinPrice);
            //            ListParam.Add(pm);
            //        }


            //        WParam param = Winning.GetWNumber(ListParam);

            //        hongbao = Convert.ToDecimal(param.MinPrice);
            //        break;



            //    } while (true);
            //}



            if (hongbao <= 0 && hongbao > 2)
            {

                LotteryRecord log = new LotteryRecord();
                log.ActivityID = act.ID;
                log.ActivityTitle = act.Title;
                log.Dat = DateTime.Now;
                log.IntegralCode = IntegralCode;
                log.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                log.UserName = username;
                log.redArea = "";// areamodel.AreaName;
                log.redMoney = hongbao;
                log.InsertAndReturnIdentity();

                return "fail|" + act.FailMsg;
            }

            List<LotteryRecord> BigScale = LotteryRecord.GetcountNumber(hongbao);

            RedPackOrder order = new RedPackOrder();
            order.act_name = act.Title;
            order.OrderNo = "red" + DAL.OnlyNoBulid.NextBillNumber();
            order.remark = "分享关注红包";
            order.scene_id = "PRODUCT_3";
            order.total_amount = hongbao;
            order.total_num = 1;
            order.Stat = "未完成";
            order.wishing = act.Title;


            order.ID = order.InsertAndReturnIdentity();

            try
            {

                WXVariousApi api = new WXVariousApi();
                api.LoadWxConfigIncidentalAccess_token();
                WxData response = api.sendredpackToUser(order, user.userWxInfo.openid, ip);

                if (response == null)
                {
                    return "fail|领取红包失败，请稍候再试";
                }
                //成功通过
                if (response.GetValue("return_code").ToString() == "SUCCESS" && response.GetValue("result_code").ToString() == "SUCCESS")
                {
                    FinishStatByID(order.ID);

                    SYSIntegralCode model = new SYSIntegralCode();
                    model.IntegralCode = IntegralCode;
                    model.RedCnted = 1;
                    model.LAARPP_ID = LAARPP_ID;
                    model.State = "已抽奖";
                    model.Dat = DateTime.Now;
                    model.InsertAndReturnIdentity();


                    redmodel.ReceiveCnt += 1;
                    redmodel.UpdateByID();
                    RedPackLottery mod = new RedPackLottery();
                    mod.Addtime = DateTime.Now;
                    mod.Code = IntegralCode;
                    mod.UserName = username;
                    mod.InsertAndReturnIdentity();

                    LotteryRecord log = new LotteryRecord();
                    log.ActivityID = act.ID;
                    log.ActivityTitle = act.Title;
                    log.Dat = DateTime.Now;
                    log.IntegralCode = IntegralCode;
                    log.SerialNumber = DAL.OnlyNoBulid.NextBillNumber();
                    LotteryRecord L = LotteryRecord.GetCntIntegralCodetg(IntegralCode);
                    if (L != null)
                    {
                        log.chief = L.UserName;
                    }
                    else
                    {
                        log.chief = username;
                    }
                    log.UserName = username;
                    log.redMoney = hongbao;
                    log.redArea = "";               // areamodel.AreaName;
                    log.InsertAndReturnIdentity();
                    return "ok|" + hongbao;

                }
                else
                {
                    return "fail|" + response.GetValue("err_code_des").ToString();
                }
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "getRedPack_error");
                return "fail|商户还未配置红包账户，请稍候再试";
            }
        }

        private class ValueAre
        {
            public int StartVal { get; set; }
            public int EndVal { get; set; }
        }
    }
}
