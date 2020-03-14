using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDALog
    {
        public static void Write(string EventName,string TypeName,string BarCode,string PDAUser,string OtherInfo="",string Result="")
        {   
            PDALog log = new PDALog();
            log.EventName = EventName;
            log.TypeName = TypeName;
            log.BarCode = BarCode;
            log.PDAUser = PDAUser;
            log.OtherInfo = OtherInfo;
            log.Result = Result;
            log.OpearTime = CommonFunc.GetNowTimestamp();

            log.InsertAndReturnIdentity();
        }
    }
}
