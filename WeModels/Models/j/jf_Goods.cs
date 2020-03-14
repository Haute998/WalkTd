using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class jf_GoodsSearch : BaseSearch 
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 产品状态
        /// </summary>
        public string PublishStat { get; set; }
    }
    public partial class jf_Goods
    {
        /// <summary>
        /// 当前要购买数量
        /// </summary>
        public int getcnt { get; set; }
        /// <summary>
        /// 购买时的积分
        /// </summary>
        public int BuyIntegral { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string GTypeName { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>

        public DateTime nowDate = DateTime.Now;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get; set; }
     
        /// <summary>
        /// 获取商品库存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetQuantity(int id)
        {
            string sql = "select Quantity from jf_Goods where ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@ID", id)  };
            object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
            string str = obj == null ? "" : obj.ToString();
            int Qua = 0;
            int.TryParse(str, out Qua);
            return Qua;
        }

        /// <summary>
        /// 回滚库存
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="ID">商品编号</param>
        /// <param name="cnt">回滚数量</param>
        /// <returns></returns>
        public static int BackQuantityByID(System.Data.SqlClient.SqlTransaction tran, int ID, int cnt)
        {
            string strSql = "UPDATE [jf_Goods] SET Quantity=Quantity+@Quantity WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",ID),
                new System.Data.SqlClient.SqlParameter("@Quantity",cnt)
            };
            return DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
        }
        /// <summary>
        /// 修改商品 不能修改库存
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = "UPDATE [jf_Goods] SET GoodsName=@GoodsName,Main_img=@Main_img,Detail=@Detail,DetailTemp=@DetailTemp,SaleIntegral=@SaleIntegral,PublishStat=@PublishStat,Quantity=@Quantity WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@GoodsName",_goodsname),
                new System.Data.SqlClient.SqlParameter("@Main_img",_main_img),
                new System.Data.SqlClient.SqlParameter("@Detail",_detail),
                new System.Data.SqlClient.SqlParameter("@DetailTemp",_detailtemp),
                new System.Data.SqlClient.SqlParameter("@SaleIntegral",_saleintegral),
                new System.Data.SqlClient.SqlParameter("@PublishStat",_publishstat),
                 new System.Data.SqlClient.SqlParameter("@Quantity",_quantity)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 添加商品（同时添加图片URL）
        /// </summary>
        /// <returns></returns>
        public int AddGoods(string ext, string UserName)
        {
            SqlConnection con = SqlHelper.DefaultConnection;
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                int rtn = 0;
                rtn = InsertAndReturnIdentity(tran);
                if (rtn == 0)
                {
                    tran.Rollback();
                    Log.Instance.Write("商品表新增失败", "AddGoods_error");
                    return 0;
                }
                _id = rtn;
                _main_img = "/images/jf_Goods/G_" + _id + ext;
                rtn = UpdateByID(tran);
                if (rtn == 0)
                {
                    tran.Rollback();
                    Log.Instance.Write("商品表更新失败", "AddGoods_error");
                    return 0;
                }


                tran.Commit();
                return rtn;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Log.Instance.Write(ex.ToString(), "AddGoods_error");
                return 0;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// 产品详情保存到草稿箱
        /// </summary>
        /// <param name="id"></param>
        /// <param name="detail">详情</param>
        /// <param name="isTemp">是否只更新草稿箱</param>
        /// <returns></returns>
        public static int DetailTempSave(int id, string detail)
        {
            string strSql = string.Empty;
            strSql = "UPDATE [jf_Goods] SET DetailTemp=@DetailTemp WHERE ID=@ID;";

            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@DetailTemp",detail),
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 批量上架或下架
        /// </summary>
        /// <param name="isPublish">1为上架，否则下架</param>
        /// <param name="ids">产品id集合</param>
        /// <returns></returns>
        public static bool toPublishs(int isPublish, int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [jf_Goods] SET PublishStat=@PublishStat WHERE ID in ({0});", idsSql);

            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@PublishStat",isPublish==1?"已上架":"未上架")
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }

  
    }
}
