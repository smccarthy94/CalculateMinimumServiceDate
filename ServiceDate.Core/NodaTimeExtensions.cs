using NodaTime;

namespace ServiceDate.Core
{
    public static class NodaTimeExtensions
    {
        public static bool IsWeekend(this LocalDate date)
        {
            switch (date.DayOfWeek)
            {
                case IsoDayOfWeek.Saturday:
                case IsoDayOfWeek.Sunday:
                    return true;
                default:
                    return false;
            }
        }

        public static LocalDate NextBusinessDayAfter(this LocalDate date, int days = 0)
        {
            while (days > 0 || date.IsWeekend())
            {
                if (!date.IsWeekend()) days--;
                date = date.PlusDays(1);
            }

            return date;
        }

        public static bool IsAfterMidday(this LocalDateTime date)
        {
            return date.Hour >= 12;
        }
    }
}
