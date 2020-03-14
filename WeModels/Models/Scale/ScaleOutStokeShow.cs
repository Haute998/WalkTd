using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class ScaleOutStokeShow : BaseSearch
    {
        /// <summary>
        /// 大标
        /// </summary>
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }
        /// <summary>
        /// 小标
        /// </summary>
        public string SmallCode { get; set; }
        /// <summary>
        /// 防伪标
        /// </summary>
        public string AntiCode { get; set; }
        /// <summary>
        /// 出货时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OutOrderNo { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public string ProductImg { get; set; }
        public decimal kw { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 发货人
        /// </summary>
        public string Shipper { get; set; }
        /// <summary>
        /// 搜素开始时间
        /// </summary>
        public DateTime DatStart { get; set; }
        /// <summary>
        /// 搜索结束时间
        /// </summary>
        public DateTime DatEnd { get; set; }
        /// <summary>
        /// 时间条件
        /// </summary>
        public string DatKey { get; set; }
        /// <summary>
        /// 多条件查询
        /// </summary>
        public string keyword { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// 产品价格
        /// </summary>
        public string Province { get; set; }
        public string City { get; set; }
        public string DatCreateB { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string DatCreateE { get; set; }
        public int Qty { get; set; }
    }
    /// <summary>
    /// 出货详情
    /// </summary>
    public class ScaleOutStokeDetail
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string OutOrderNo { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 发货人编号
        /// </summary>
        public string Shipper { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
    }
    public class OutScaleDetail
    {
        public int Qty { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string OutOrderNo { get; set; }
        /// <summary>
        /// 小标
        /// </summary>
        public string SmallCode { get; set; }
        public string BigCode { get; set; }
        public string MiddleCode { get; set; }

        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
    }

    /// <summary>
    /// 出货记录
    /// </summary>
    public class OutStokeRecord
    { 
        public int Qty{get;set;}
        public string Shipper{get;set;}
        public string ProductNumber{get;set;}
        public string Name{get;set;}
        public int CreateTime{get;set;}
        public string OutOrderNo{get;set;}
        public string ProductName{get;set;}
        public string ProductImg{get;set;}
        public string Consignee { get; set; }
    }
}
