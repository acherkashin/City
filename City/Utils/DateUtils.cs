using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Utils
{
    public class DateUtils
    {
        public static string GetMonth(int month)
        {
            if (month == 1)
                return "ЯНВАРЯ";
            if (month == 2)
                return "ФЕВРАЛЯ";
            if (month == 3)
                return "МАРТА";
            if (month == 4)
                return "АПРЕЛЯ";
            if (month == 5)
                return "МАЯ";
            if (month == 6)
                return "ИЮНЯ";
            if (month == 7)
                return "ИЮЛЯ";
            if (month == 8)
                return "АВГУСТА";
            if (month == 9)
                return "СЕНТЯБРЯ";
            if (month == 10)
                return "ОКТЯБРЯ";
            if (month == 11)
                return "НОЯБРЯ";
            if (month == 12)
                return "ДЕКАБРЯ";

            return "";
        }
    }
}
