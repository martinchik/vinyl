using NCrontab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Vinyl
{
    public static class Repeat
    {
        public static IEnumerable<DateTime> Immediately()
        {
            yield return DateTime.UtcNow;
        }

        public static IEnumerable<DateTime> Than(this IEnumerable<DateTime> first, IEnumerable<DateTime> second)
        {
            return first.Concat(second);
        }

        public static IEnumerable<DateTime> Cron(string expression, params object[] p)
        {
            var schedule = CrontabSchedule.TryParse(string.Format(expression, p));

            var startFrom = DateTime.MinValue;
            while (true)
            {
                var now = DateTime.UtcNow;
                if (startFrom == DateTime.MinValue)
                {
                    startFrom = now;
                }

                var next = schedule.GetNextOccurrence(startFrom);

                if (next < now.AddSeconds(30))
                {
                    next = now.AddSeconds(30);
                }

                yield return next;

                startFrom = next;
            }
        }

        public static class Each
        {
            public static IEnumerable<DateTime> Day(int day)
            {
                return day > 0 ? Cron("1 0 */{0} * *", day) : Cron("1 0 * * *");
            }

            public static IEnumerable<DateTime> Hours(int hour = 0)
            {
                return hour > 0 ? Cron("1 */{0} * * *", hour) : Cron("1 * * * *");
            }

            public static IEnumerable<DateTime> Minutes(int minute, int shift = 0)
            {
                return minute > 0 ? Cron("{1}/{0} * * * *", minute, shift == 0 ? "*" : shift.ToString(CultureInfo.InvariantCulture)) : Cron("* * * * *");
            }
        }

        public static class Localize
        {
            public static int HourInDanishTime(int hour)
            {
                var danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                var danishTimeZoneOffsetHours = danishTimeZone.GetUtcOffset(DateTime.Now).Hours;
                var utcHours = hour - danishTimeZoneOffsetHours;

                return utcHours;
            }
        }

        public static class Twice
        {
            public static IEnumerable<DateTime> PerDay(int atHour1 = 0, int atHour2 = 1)
            {
                return atHour1 > 0 && atHour2 > 0 ? Cron("1 {0},{1} * * *", atHour1, atHour2) : Cron("1 0,1 * * *");
            }
        }

        public static class Thrice
        {
            public static IEnumerable<DateTime> PerDay(int atHour1 = 0, int atHour2 = 1, int atHour3 = 2)
            {
                return atHour1 > 0 && atHour2 > 0 && atHour3 > 0 ? Cron("1 {0},{1},{2} * * *", atHour1, atHour2, atHour3) : Cron("1 0,1,2 * * *");
            }
        }

        public static class Quarce
        {
            public static IEnumerable<DateTime> PerDay(int atHour1 = 0, int atHour2 = 1, int atHour3 = 2, int atHour4 = 3)
            {
                return atHour1 > 0 && atHour2 > 0 && atHour3 > 0 ? Cron("1 {0},{1},{2},{3} * * *", atHour1, atHour2, atHour3, atHour4) : Cron("1 0,1,2,4 * * *");
            }
        }

        public static class Once
        {
            public static IEnumerable<DateTime> PerDay(int atHour = 0)
            {
                return atHour > 0 ? Cron("1 {0} * * *", atHour) : Cron("1 0 * * *");
            }

            public static IEnumerable<DateTime> PerHour(int atMinute = 0)
            {
                return atMinute > 0 ? Cron("{0} * * * *", atMinute) : Cron("1 * * * *");
            }
        }
    }
}
