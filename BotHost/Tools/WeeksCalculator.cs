using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotHost.Tools
{
    public class WeeksCalculator
    {
        private static DateTime _startSemestr = new DateTime(2018, 9, 2);
        public static int GetCurrentWeek()
        {
            var days = (DateTime.Now - _startSemestr).Days;
            var week = days / 7;
            var tmp = days % 7;
            week = tmp > 0 ? week + 1 : week;
            return week;
        }
    }
}
