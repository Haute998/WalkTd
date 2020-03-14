using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_OrderCreateHelper
    {
        /// <summary>
        /// 订单对象
        /// </summary>
        public jf_lpOrder order { get; set; }
        /// <summary>
        /// 商品集合
        /// </summary>
        public List<jf_Goods> goodsLst { get; set; }

        /// <summary>
        /// 订单创建
        /// </summary>
        /// <param name="mid">商品编号，直接购买单个商品时必填</param>
        /// <param name="mcnt_i">商品数量，直接购买单个商品时必填</param>
        /// <param name="mailid">收货地址编号</param>
        /// <param name="type">类型：mail/cart 即：商品/购物车</param>
        /// <param name="username">用户名</param>
        /// <param name="Remark">买家备注</param>
        /// <returns></returns>
        public string OrderBulid(int mid, int mcnt_i, int mailid, string cartpks, string type, string username, string Remark)
        {
            if (type == "mail")
            {
                C_Consumer user = C_Consumer.GetEntityByUserName(username);

                //单个商品购买模式
                jf_Goods goods = jf_Goods.GetEntityByID(mid);//商品对象
                goods.getcnt = mcnt_i;
                goods.BuyIntegral = goods.SaleIntegral;//当前购买需要的积分
                if (goods.PublishStat == "未上架")
                {
                    return "您下手太慢，该产品已下架，请后续关注该产品。";
                }
                if (goods.Quantity < mcnt_i)
                {
                    if (goods.Quantity <= 0)
                    {
                        return "您下手太慢，已经卖光了";
                    }
                    return "该产品只剩" + goods.Quantity + "件，请减少数量再购买";
                }

                jf_UserMail mail = jf_UserMail.GetEntityByID(mailid);//收货地址对象
                if (mail.UserName != username)
                {
                    return "收货地址有误";
                }
                goodsLst = new List<jf_Goods>();
                goodsLst.Add(goods);
                order = new jf_lpOrder();
                order.DatCreate = DateTime.Now;
                order.OrderName = goods.GoodsName;
                order.MailID = mail.ID;
                order.OrderImgSrc = goods.Main_img;
                order.OrderMan = mail.ContactName;
                order.OrderMobile = mail.ContactMobile;
                order.Address =mail.Province+mail.City+mail.Area+mail.Address;
                order.OrderState = "待支付";
                order.PayState = "未支付";
                order.Postage = 0;
                order.Remark = Remark;
                order.SumIntegral = goods.BuyIntegral * mcnt_i;
                order.UserType = user.Type;
                order.UserName = username;

                //创建订单
                return CreateOrder();
            }
            else
            {
                return "暂不支持该方式购物";
            }
        }


        /// <summary>
        /// 创建订单  在此生成订单号
        /// </summary>
        /// <returns></returns>
        private string CreateOrder()
        {
            if (order == null || goodsLst == null || goodsLst.Count <= 0) { return "创建订单失败"; }
            SqlConnection con = SqlHelper.DefaultConnection;
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                int result = 0;
                order.OrderNo ="lp"+ OnlyNoBulid.NextBillNumber();//生成订单号
                order.ID = order.InsertAndReturnIdentity(tran);
                if (order.ID == 0)
                {
                    tran.Rollback();
                    return "创建订单失败";
                }
                //添加商品快照表数据
                for (int i = 0; i < goodsLst.Count; i++)
                {
                    jf_GoodsSnap goodsSnap = new jf_GoodsSnap();
                    goodsSnap.GoodsID = goodsLst[i].ID;
                    goodsSnap.OrderNo = order.OrderNo;
                    goodsSnap.GoodsName = goodsLst[i].GoodsName;
                    goodsSnap.Main_img = goodsLst[i].Main_img;
                    goodsSnap.Detail = goodsLst[i].Detail;
                    goodsSnap.SaleIntegral = goodsLst[i].SaleIntegral;
                    goodsSnap.BuyIntegral = goodsLst[i].BuyIntegral;//当前购买时的积分
                    goodsSnap.GetCnt = goodsLst[i].getcnt;
                    if (goodsSnap.InsertAndReturnIdentity(tran) <= 0)
                    {
                        tran.Rollback();
                        return "创建订单失败";
                    }

                    string sql = string.Format("update jf_Goods set quantity=quantity-{0} where ID={1}", goodsLst[i].getcnt, goodsLst[i].ID);
                    SqlParameter[] paramters = null;
                    result = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, paramters);
                    if (result <= 0)
                    {
                        tran.Rollback();
                        Log.Instance.Write("更新总库存失败,商品ID：" + goodsLst[i].ID, "CreateOrder_Error");
                        return "创建订单失败";
                    }
                }

                tran.Commit();

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
    }
}
