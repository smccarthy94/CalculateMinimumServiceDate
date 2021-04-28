using System.Linq;
using NodaTime;

namespace ServiceDate.Services
{
    public interface IPublicHolidayService
    {
        LocalDate[] GetPublicHolidays();
        LocalDate? NextPublicHoliday(LocalDateTime after);
        LocalDate? LastPublicHoliday(LocalDateTime after);
        bool IsPublicHoliday(LocalDateTime date);
    }

    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly LocalDate[] _publicHolidays = {
            new (2021, 04, 02), // easter friday
            new (2021, 04, 03), // easter saturday
            new (2021, 04, 04), // easter sunday
            new (2021, 04, 05), // easter monday
            new (2021, 04, 26), // anzac day
            new (2021, 05, 03) // labor day
        };

        public LocalDate[] GetPublicHolidays()
        {
            return _publicHolidays.OrderBy(d => d).ToArray();
        }

        public LocalDate? NextPublicHoliday(LocalDateTime after)
        {
            return GetPublicHolidays().FirstOrDefault(d => d > after.Date);
        }

        public LocalDate? LastPublicHoliday(LocalDateTime after)
        {
            return GetPublicHolidays().LastOrDefault(d => d < after.Date);
        }

        public bool IsPublicHoliday(LocalDateTime date)
        {
            return GetPublicHolidays().Any(ph => ph == date.Date);
        }
    }
}
