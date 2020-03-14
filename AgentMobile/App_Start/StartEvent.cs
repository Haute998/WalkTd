using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeModels;

namespace AgentMobile
{
    public class StartEvent
    {
        public StartEvent()
        {
            PDAUserMsg.Param = PDAParam.GetEntityByID(1);
        }
    }
}