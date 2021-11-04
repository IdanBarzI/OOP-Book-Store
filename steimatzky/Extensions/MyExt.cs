using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steimatzky.Extensions
{
    public static class MyExt
    {
        public static bool IsEmpty(this string str) =>
            string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Extension Using Parse to int
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static int ToInt(this string str)//check
        {
            if (str.IsEmpty() || str == null)
            {
                return 0;
            }
            return int.Parse(str);
        }

        /// <summary>
        /// Extension Using Parse to double
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static double ToDouble(this string str)
        {
            if (str.IsEmpty() || str == null)
            {
                return 0;
            }
            return double.Parse(str);
        }

        public static int GetQuntityRemoved(this string str)
        {
            string[] words = str.Split(',');
            foreach (var word in words)
            {
                if (word.Contains("Quntity removed:"))
                {
                    string str1 = (word.Substring(17).Replace(',', ' '));
                    int res = str1.ToInt();
                    return res;
                }
            }
            return 0;
        }
    }
}
