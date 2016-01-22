using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Resources
{
    /// <summary>
    /// This class turns an ugly date/time into a prettier one like "yesterday".
    /// It takes a DateTime and formats it according to the following rules:
    /// 
    ///     - less than 10 min ago ====> "just now"
    ///     - today                ====> "3:53 PM"
    ///     - yesterday            ====> "Yesterday - 11:12AM"
    ///     - earlier than that    ====> "2 days ago", "33 days ago", "100 days ago", etc
    /// </summary>
    public static class FriendlyTime
    {
        public static string Format(DateTime input)
        {
            var now = DateTime.Now;
            string output = "";

            if (input > now.AddMinutes(-10))
            {
                output = i18n.Time_JustNow;
            }
            else if (input.Date == now.Date)
            {
                output = input.ToShortTimeString();
            }
            else if (input.Date == now.Date.AddDays(-1))
            {
                output = i18n.Time_Yesterday + " - " + input.ToShortTimeString();
            }
            else
            {
                output = (now.Date - input.Date).Days.ToString() + " " + i18n.Time_DaysAgo;
            }

            return output;
        }
    }
}
