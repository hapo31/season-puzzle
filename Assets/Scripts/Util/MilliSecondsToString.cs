using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class MilliSecondsToString
    {
        public static string format(int milliseconds )
        {
            if (milliseconds == 0)
            {
                return "00:00:00";
            }
            var min = milliseconds / 60000;
            var sec = milliseconds >= 1000 ? milliseconds / 1000 % 60 : 0;
            var ms = milliseconds >= 10 ? milliseconds / 10 % 100 : 0;
            return $"{min.ToString("00")}:{sec.ToString("00")}.{ms.ToString("00")}";
        }
    }
}
