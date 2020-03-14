using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeModels
{
    public static class ColorImg
    {

        public  static  string str = "";
        public static int[] array = new int[16];
        public static System.Drawing.Color[] array1 = new System.Drawing.Color[16];
        public static string[] array2 = new string[16];
        public static System.Drawing.Color adsa1(string a0, int c)
        {
            System.Drawing.Color c0;
            string b4 = a0.ToString().Substring(a0.ToString().Length - 1, 1);
            if (b4 == "0")//红色#ff0000
            { c0 = Color.FromArgb(255, 231, 120, 23); }
            else if (b4 == "1")//蓝色#0000ff
            {
                c0 = Color.FromArgb(255, 255, 0, 255);

            }
            else if (b4 == "2")//绿色#00ff00
            { c0 = Color.FromArgb(255, 8, 84, 8); }
            else if (b4 == "3")//橙色ff6100
            { c0 = Color.FromArgb(255, 189, 169, 19); }
            else if (b4 == "4")//金黄色ffD700
            { c0 = Color.FromArgb(255, 30, 144, 255); }
            else if (b4 == "5")//金黄色ffD700
            { c0 = Color.FromArgb(255, 0, 0, 255); }
            else if (b4 == "6")//黑色#000000
            { c0 = Color.FromArgb(255, 0, 0, 0); }
            else if (b4 == "7")//金黄色ffD700
            { c0 = Color.FromArgb(255, 227, 85, 106); }
            else if (b4 == "8")//金黄色ffD700
            { c0 = Color.FromArgb(255, 255, 0, 0); }

            else //金黄色ffD700
            { c0 = Color.FromArgb(255, 0, 255, 127); }



            return c0;
        }

       

    }
}
