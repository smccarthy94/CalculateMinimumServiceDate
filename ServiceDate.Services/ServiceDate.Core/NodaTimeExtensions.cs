using System;

namespace ServiceDate.Core
{
    public static class NodaTimeExtensions
    {
        public static DateTimeOffset StartOfDay(this DateTimeOffset date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static bool IsWeekend(this DateTimeOffset date)
        {
            if (date.LocalDateTime.DayOfWeek == DayOfWeek.Sunday) return true;
            if (date.LocalDateTime.DayOfWeek == DayOfWeek.Saturday) return true;
            return false;
        }
    }
}
