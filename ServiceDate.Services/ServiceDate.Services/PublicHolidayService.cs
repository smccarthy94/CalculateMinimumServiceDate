using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDate.Services
{
    public interface IPublicHolidayService
    {
        DateTime[] GetPublicHolidays();
        DateTime NextPublicHoliday(DateTime after);
        bool IsPublicHoliday(DateTimeOffset date);
    }

    class PublicHolidayService : IPublicHolidayService
    {
        private readonly DateTime[] _publicHolidays = new DateTime[] {
            new DateTime(2021, 04, 02),
            new DateTime(2021, 04, 03),
            new DateTime(2021, 04, 04),
            new DateTime(2021, 04, 05)
        };

        DateTime[] GetPublicHolidays() { return _publicHolidays.OrderBy(d => d).ToArray(); }
        DateTime NextPublicHoliday(DateTime after) { return GetPublicHolidays().FirstOrDefault(d => d > after); }

        bool IsWeekend(DateTimeOffset date)
        {
            return new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(date.LocalDateTime.DayOfWeek);
        }

        bool IsPublicHoliday(DateTimeOffset date)
        {
            return _publicHolidays.Any(ph => StartOfDay(ph) == StartOfDay(date.LocalDateTime));
        }
    }
}
