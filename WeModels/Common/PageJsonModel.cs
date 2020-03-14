using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    /// <summary>
    /// 公共分页类
    /// </summary>
    public class PageJsonModel
    {
        /// <summary>
        /// 表或表集合【必传】
        /// </summary>
        public string strForm { get; set; }
        /// <summary>
        /// 页码即第几页，从1开始，默认1
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 主键字段  写一个
        /// </summary>
        public string primaryField { get; set; }
        /// <summary>
        /// 条件 如： and 2=1  ,字符首必须要and
        /// </summary>
        public string strWhere { get; set; }
        /// <summary>
        /// 字段，默认*
        /// </summary>
        public string strSelect { get; set; }
        /// <summary>
        /// 排序字段 如：ID DESC，默认 ID DESC
        /// </summary>
        public string strOrder { get; set; }
        /// <summary>
        /// 一页显示数量，默认20
        /// </summary>
        public int pageSize { get; set; }
    }
    public class PageJsonModel<T> : PageJsonModel
    {
        /// <summary>
        /// 返回错误信息 为空则为成功
        /// </summary>
        public string errorMsg { get; set; }
        public JsonResponse<T> pageResponse { get; set; }
        public void LoadList()
        {
            JsonResponse<T> res = new JsonResponse<T>();
            if (string.IsNullOrWhiteSpace(strSelect))
            {
                strSelect = "*";
            }
            if (string.IsNullOrWhiteSpace(strOrder))
            {
                strOrder = "ID DESC";
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 5;
            }
            res.pageSize = pageSize;
            string cntSql = "";
            string strSql = "";
            try
            {
                //总条数
                cntSql = string.Format("select count(1) from {0} where 1=1 {1}", strForm, strWhere);
                System.Data.SqlClient.SqlParameter[] cntParam = null;
                object obj = SqlHelper.ExecuteScalar(cntSql, cntParam);
                res.totalCnt = obj != null ? Convert.ToInt32(obj) : 0;

                strSql = getSqlStrLow();
                System.Data.SqlClient.SqlParameter[] paramters = { 
    			new System.Data.SqlClient.SqlParameter("@pageIndex",pageIndex),
    			new System.Data.SqlClient.SqlParameter("@pageSize",pageSize)};
                res.RtnList = EntityDataHelper.FillData2Entities<T>(strSql, paramters);//数据list集合
                res.thisCnt = res.RtnList.Count;
                res.pageIndex = pageIndex;
                res.totalPage = (res.totalCnt / res.pageSize) + (res.totalCnt % res.pageSize == 0 ? 0 : 1);
                int pageRowCount = (res.pageIndex - 1) * res.pageSize;
                res.rowBegin = res.totalCnt > 0 ? pageRowCount + 1 : 0;
                pageRowCount = res.pageIndex * res.pageSize;
                res.rowEnd = pageRowCount < res.totalCnt ? pageRowCount : res.totalCnt;
                pageResponse = res;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                pageResponse = res;
                Log.Instance.Write("cntSql：" + cntSql + "，strSql：" + strSql+"。" + ex.ToString(), "PageJsonModel_LoadList_error");
            }
        }

        /// <summary>
        /// 获取List  没有totalPage、rowBegin、rowEnd
        /// </summary>
        public void LoadListNoCnt()
        {
            JsonResponse<T> res = new JsonResponse<T>();
            if (string.IsNullOrWhiteSpace(strSelect))
            {
                strSelect = "*";
            }
            if (string.IsNullOrWhiteSpace(strOrder))
            {
                strOrder = "ID DESC";
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            res.pageSize = pageSize;
            try
            {

                string strSql = getSqlStrLow();
                System.Data.SqlClient.SqlParameter[] paramters = { 
    			new System.Data.SqlClient.SqlParameter("@pageIndex",pageIndex),
    			new System.Data.SqlClient.SqlParameter("@pageSize",pageSize)};
                res.RtnList = EntityDataHelper.FillData2Entities<T>(strSql, paramters);//数据list集合
                res.thisCnt = res.RtnList.Count;
                res.pageIndex = pageIndex;
                int pageRowCount = (res.pageIndex - 1) * res.pageSize;
                pageRowCount = res.pageIndex * res.pageSize;
                pageResponse = res;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                pageResponse = res;
                Log.Instance.Write(ex.ToString(), "PageJsonModel_LoadListNoCnt_error");
            }
        }


        /// <summary>
        /// 获取分页sql
        /// </summary>
        /// <returns></returns>
        private string getSqlStr()
        {
            return string.Format("SELECT {0} FROM {1} where 1=1 {2} ORDER BY {3} OFFSET (@pageIndex -1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY", strSelect, strForm, strWhere, strOrder);
        }

        /// <summary>
        /// 获取分页sql 用于sql2008
        /// </summary>
        /// <returns></returns>
        private string getSqlStrLow()
        {
            return string.Format(@"
                                    SELECT * FROM (
                                    SELECT {0} ,
                                    row_number() over(order by {1}) r 
                                    FROM {2} where 1=1 {3} ) t 
                                    where t.r <= (@pageSize*@pageIndex) and t.r > (@pageSize*(@pageIndex-1))", strSelect, strOrder, strForm, strWhere);
        }

//        private string getSqlStrLow_new()
//        {
//            if (string.IsNullOrWhiteSpace(primaryField)) 
//            {
//                primaryField = "ID";
//            }
//            return string.Format(@"
//                                    SELECT TOP @pageSize *
//FROM {0}
//WHERE ({1} >
//          (
//
//		  
//		  SELECT MAX(T.id)
//         FROM (SELECT TOP {2}
//		           {1}
//                 FROM {0}
//                 ORDER BY {4}) AS T
//
//				 
//				 ))
//ORDER BY ID", strForm,primaryField,pageSize*(pageIndex-1),strSelect, strOrder, strWhere);
//        }


    }
    public class JsonResponse<T>
    {
        /// <summary>
        /// 返回的list集合
        /// </summary>
        public List<T> RtnList { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        public int totalCnt { get; set; }
        /// <summary>
        /// 当前所取数据条数
        /// </summary>
        public int thisCnt { get; set; }
        /// <summary>
        /// 总页码
        /// </summary>
        public int totalPage { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 当前是第几条数据开始
        /// </summary>
        public int rowBegin { get; set; }
        /// <summary>
        /// 当前是第几条结束
        /// </summary>
        public int rowEnd { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int pageSize { get; set; }
        public decimal SumPrice { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        /// 
        public int BigCount { get; set; }
        public int MiddleCount { get; set; }
        public int SmallCount { get; set; }
    }
}
