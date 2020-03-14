using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class exportHelper
    {
        public string[] field { get; set; }

        public string[] list { get; set; }



        public static exportHelper getexport(string[] oldfield,string[] oldlist,string url,string UserName) 
        {
            exportHelper hel = new exportHelper();

            List<string> fields = new List<string>();
            List<string> lists = new List<string>();


            List<TableViewConfig> configs = TableViewConfig.GetEntitys(url,UserName);
            if (configs != null && configs.Count>0)
            {
                configs = configs.OrderBy(m => m.Sort).ToList();

                foreach (TableViewConfig config in configs)
                {
                    int i=0;
                    foreach (var item in oldlist)
                    {
                        if (item == config.ShowName)
                        {
                            lists.Add(item);
                            fields.Add(oldfield[i]);
                            i++;
                            break;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }



                hel.field = fields.ToArray() ;
                hel.list = lists.ToArray();
                return hel;

            }
            else 
            {
                hel.field = oldfield;
                hel.list = oldlist;
                return hel;
            }



        }
    }
}
