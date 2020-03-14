using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WeModels
{
    public class MProductSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductNumber { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 产品添加时间-开始
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 产品添加时间-结束
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 产品状态
        /// </summary>
        public string States { get; set; }

        /// <summary>
        /// 查询产品名称
        /// </summary>
        public string GoodsName { get; set; }

        public static string StrWhere(MProductSearch condition)
        {
            string where = string.Empty;
            //产品编号
            if (!string.IsNullOrWhiteSpace(condition.ProductNumber))
            {
                where += " and ProductNumber like '%" + Common.Filter(condition.ProductNumber) + "%' ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (ProductNumber like '%{0}%' or States like '%{0}%' or ProductName like '%{0}%') ", condition.keyword);
            }
            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.AddTime))
            {
                where += string.Format(" and AddTime >='{0} 00:00:00' ", Common.Filter(condition.AddTime));
            }
            if (!string.IsNullOrWhiteSpace(condition.EndTime))
            {
                where += string.Format(" and AddTime <'{0} 23:59:59' ", Common.Filter(condition.EndTime));
            }
            if (!string.IsNullOrWhiteSpace(condition.States))
            {
                where += string.Format(" and States ='{0}' ", Common.Filter(condition.States));
            }
            return where;
        }
    }
    public partial class Product
    {
        /// <summary>
        /// 当前购买数量  临时使用
        /// </summary>
        public int getcnt { get; set; }
        /// <summary>
        /// 当前购买价格   临时使用
        /// </summary>
        public decimal buyprice { get; set; }

       // 数量
        public int count { get; set; }

        public int EditImgTxtByID()
        {
            string strSql = "UPDATE [Product] SET ImgTxtContent=@ImgTxtContent,ImgTxtTmp=@ImgTxtTmp WHERE ProductID=@ProductID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ProductID",ProductID),
                new System.Data.SqlClient.SqlParameter("@ImgTxtContent",ImgTxtContent),
                new System.Data.SqlClient.SqlParameter("@ImgTxtTmp",ImgTxtTmp),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        public static bool toPDA(string isPublish, int ids)
        {
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [PDA] SET State=@State WHERE ID={0};", ids);

            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@State",isPublish)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        public static Product GetProductNo(string ProductNo)
        {
            string strSql = "SELECT ProductID,ProductNumber,ProductName,ProductImg,AddTime,States,ImgTxtContent,ImgTxtTmp,bianma,kw,UpdateTime FROM [Product] WHERE ProductNumber=@ProductNumber";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ProductNumber", ProductNo) };

            return DAL.EntityDataHelper.LoadData2Entity<Product>(strSql, paramters);
        }
        /// <summary>
        /// 草稿箱保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public static int DetailTempSave(int id, string tmp)
        {
            string strSql = "UPDATE [Product] SET ImgTxtTmp=@ImgTxtTmp WHERE ProductID=@ProductID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ProductID",id),
                new System.Data.SqlClient.SqlParameter("@ImgTxtTmp",tmp),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }


        /// <summary>
        /// 上架或下架
        /// </summary>
        /// <param name="isPublish">产品状态</param>
        /// <param name="ids">产品ID集合</param>
        /// <returns></returns>
        public static bool toPublish(string isPublish, int ids)
        {
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [Product] SET States=@States WHERE ProductID={0};", ids);

            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@States",isPublish=="上架"?"已上架":"未上架")
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        /// <summary>
        /// 批量上架或下架
        /// </summary>
        /// <param name="isPublish">产品状态</param>
        /// <param name="ids">产品ID集合</param>
        /// <returns></returns>
        public static bool toPublishs(string isPublish, int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [Product] SET States=@States WHERE ProductID in ({0});", idsSql);

            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@States",isPublish=="已上架"?"已上架":"未上架")
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">产品ID集合</param>
        /// <returns></returns>
        public static bool DelProducts(int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("DELETE FROM [Product] WHERE ProductID in ({0});", idsSql);

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql);
            return cnt > 0;
        }
        /// <summary>
        /// 查询字段值是否存在
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static int Whetherthere(string field, string value, int id)
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT * FROM [Product] WHERE {0}='{1}' and ProductID<>{2} ", field, value, id);
            object obj = DAL.SqlHelper.ExecuteScalar(strSql);
            string str = obj == null ? "" : obj.ToString();
            int Qua = 0;
            int.TryParse(str, out Qua);
            return Qua;
        }
        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="p">产品Model</param>
        /// <returns></returns>
        public static bool ProductAdd(Product p)
        {
            string strSql = string.Empty;
               strSql = string.Format("INSERT [Product](ProductNumber,ProductName,ProductImg,bianma,kw,States) VALUES('{0}','{1}','{2}','{3}','{4}','{5}');", p.ProductNumber, p.ProductName, p.ProductImg, p.bianma,p.kw, p.States);
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql);
            return cnt > 0;
        }
        /// <summary>
        /// 修改产品信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool ProductUpdate(Product p)
        {
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [Product] SET ProductNumber=@ProductNumber,ProductName=@ProductName,ProductImg=@ProductImg,bianma=@bianma,States=@States WHERE ProductID=@ProductID");
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ProductNumber",p.ProductNumber),
                new System.Data.SqlClient.SqlParameter("@ProductName",p.ProductName),
                new System.Data.SqlClient.SqlParameter("@ProductImg",p.ProductImg),
                new System.Data.SqlClient.SqlParameter("@bianma",p.bianma),
                new System.Data.SqlClient.SqlParameter("@States",p.States),
                new System.Data.SqlClient.SqlParameter("@ProductID",p.ProductID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        public static List<P_Interface> GetProduct()
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT ProductNumber,ProductName FROM Product Order by AddTime desc ");
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<P_Interface>(strSql, paramters);
        }

        public static List<P_Interface> GetProduct(string KeyWords)
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT ProductNumber,ProductName FROM Product where ProductNumber+ProductName like '%{0}%' Order by AddTime desc", KeyWords);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<P_Interface>(strSql, paramters);
        }

        public static List<P_Interface> GetProduct(int Timestamp)
        {
            string strSql = string.Empty;
            strSql = string.Format("SELECT ProductNumber,ProductName FROM Product where UpdateTime>=@UpdateTime Order by AddTime desc");
            SqlParameter[] paramters = { new SqlParameter("@UpdateTime", Timestamp) };
            return DAL.EntityDataHelper.FillData2Entities<P_Interface>(strSql, paramters);
        }

        public static bool IsSysProduct(string ProductNumber)
        {
            string SqlStr = "SELECT COUNT(*) FROM Product WHERE ProductNumber=@ProductNumber";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@ProductNumber",ProductNumber),
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }

        public static List<Product> GetPageProductList(int PageSize, int PageIndex, string keyword, out int totalCount)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where = string.Format(" ProductNumber+ProductName like '%{0}%'", Common.Filter(keyword));       
            }

            string strSql = "exec dbo.Common_PageList 'Product','*','ProductID desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };

            List<Product> OrderList = DAL.EntityDataHelper.FillData2Entities<Product>(strSql, paramters);

            strSql = "select count(*) from Product " + (string.IsNullOrEmpty(where) ? "" : "where" + where);
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;
        }
    }
    public class P_Interface
    {
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        
    }
}
