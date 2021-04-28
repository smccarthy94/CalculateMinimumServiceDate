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

        public static bool IsAfterMidday(this LocalDateTime date)
        {
            return date.Hour >= 12;
        }
    }
}
