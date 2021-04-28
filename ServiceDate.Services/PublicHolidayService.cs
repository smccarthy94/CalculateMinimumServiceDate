using System.Linq;
using NodaTime;

namespace ServiceDate.Services
{
    public interface IPublicHolidayService
    {
        LocalDate[] GetPublicHolidays();
        LocalDate NextPublicHoliday(LocalDateTime after);
        bool IsPublicHoliday(LocalDateTime date);
    }

    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly LocalDate[] _publicHolidays = {
            new (2021, 04, 02),
            new (2021, 04, 03),
            new (2021, 04, 04),
            new (2021, 04, 05),
            new (2021, 04, 26) // anzac day
        };

        public LocalDate[] GetPublicHolidays()
        {
            return _publicHolidays.OrderBy(d => d).ToArray();
        }

        public LocalDate NextPublicHoliday(LocalDateTime after)
        {
            return GetPublicHolidays().FirstOrDefault(d => d > after.Date);
        }

        public bool IsPublicHoliday(LocalDateTime date)
        {
            return GetPublicHolidays().Any(ph => ph == date.Date);
        }
    }
}
