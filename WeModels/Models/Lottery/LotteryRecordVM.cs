using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class LotteryRecordVM
    {
        /// <summary>
        /// 中奖纪录
        /// </summary>
        public LotteryRecord rec { get; set; }
        /// <summary>
        /// 中奖人信息
        /// </summary>
        public C_UserWxVM user { get; set; }
        /// <summary>
        /// 读取记录
        /// </summary>
        /// <param name="serialNumber"></param>
        public void LoadRecord(string serialNumber)
        {
            rec = LotteryRecord.GetRecBySerialNumber(serialNumber);
            user =new C_UserWxVM();
            user.LoadUserVMByUserName(rec.UserName);
        }
    }
}
