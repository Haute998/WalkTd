using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class Winning
    {
        public static WParam GetWNumber(List<WParam> ParamList)
        {
            WParam wp = new WParam();
            double d = 0;
            int P = 0;
            for (int i = 0; i < ParamList.Count; i++)
            {
                d += ParamList[i].Bility;
                if (d > 1)
                {
                    ParamList[i].Bility = ParamList[i].Bility - (d - 1);
                    d = 1;
                }

                if (d <= 1)
                {
                    int MValue = Convert.ToInt32(ParamList[i].Bility * 10000);
                    ParamList[i].MinValue = P;
                    P += MValue;
                    ParamList[i].MaxValue = P;
                }
            }

            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            Random r = new Random(iSeed);
            int Rand = r.Next(1, 10000);

            for (int i = 0; i < ParamList.Count; i++)
            {
                if (Rand > ParamList[i].MinValue && Rand < ParamList[i].MaxValue)
                {
                    double dvalue = 0;
                    if (ParamList[i].MaxPrice != ParamList[i].MinPrice)
                    {
                        Random rt = new Random(iSeed);
                        dvalue = ParamList[i].MinPrice + rt.NextDouble() * (ParamList[i].MaxPrice - ParamList[i].MinPrice);
                    }
                    else dvalue = ParamList[i].MinPrice;

                    ParamList[i].WPrice = dvalue;
                    wp = ParamList[i];
                    break;
                }
            }

            return wp;
        }
    }

    public class WParam
    {
        public int ID{get;set;}                     // 行ID
        public double MinPrice{get;set;}            // 最小金额
        public double MaxPrice { get; set; }        // 最大金额
        public double Bility{get;set;}              // 概率
        public double WPrice { get; set; }          // 中奖金额
        public int MinValue { get; set; }           // 最小取值
        public int MaxValue { get; set; }           // 最大取值
        public string prize { get; set; }           
    }
}
