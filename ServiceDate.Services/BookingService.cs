using NodaTime;
using ServiceDate.Core;

namespace ServiceDate.Services
{
    public interface IBookingService
    {
        LocalDateTime? CalculateMinimumServiceDate(long workshopId);
    }

    public class BookingService : IBookingService
    {
        private readonly IClock _clock;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IWorkshopDataService _workshopDataService;

        public BookingService(IClock clock, IWorkshopDataService workshopDataService, IPublicHolidayService publicHolidayService)
        {
            _clock = clock;
            _workshopDataService = workshopDataService;
            _publicHolidayService = publicHolidayService;
        }

        public LocalDateTime? CalculateMinimumServiceDate(long workshopId)
        {
            var from = _clock.GetCurrentInstant().InUtc();

            // workshop data
            var minimumNoticeDays = _workshopDataService.GetMinimumNoticeDays(workshopId);

            // init the date ranges for checking
            var minimumDate = from.Date;
            var maximumDate = from.Date.PlusDays(90);

            // get next date after minimum notice period.
            if (from.LocalDateTime.IsAfterMidday()) minimumNoticeDays++;

            if (from.Date.PlusDays(minimumNoticeDays) > minimumDate) 
                minimumDate = minimumDate.NextBusinessDayAfter(minimumNoticeDays);

            // loop until we reach the maximum possible date or find a valid date.
            while (!IsDateValid(minimumDate, workshopId) && minimumDate < maximumDate)
            {
                minimumDate = minimumDate.PlusDays(1);

                // if we found a valid date, perform our last adjustments.
                if (from.LocalDateTime.IsAfterMidday() &&
                    IsDateValid(minimumDate, workshopId) &&
                    DaysSinceLastPublicHoliday(minimumDate) == 1)
                {
                    minimumDate = minimumDate.PlusDays(1);
                }
            }

            // if no possible days in the next 3 months.
            if (minimumDate >= maximumDate && !IsDateValid(minimumDate, workshopId)) return null;

            return minimumDate.AtMidnight();
        }

        private int DaysSinceLastPublicHoliday(LocalDate date)
        {
            var lastHol = _publicHolidayService.LastPublicHoliday(date.AtMidnight());

            if (!lastHol.HasValue) return -1;

            return Period.Between(lastHol.Value, date, PeriodUnits.Days).Days;
        }

        private bool IsDateValid(LocalDate date, long workshopId)
        {
            return !_workshopDataService.IsClosed(workshopId, date) &&
                   !_workshopDataService.IsFullyBooked(workshopId, date) &&
                   !_publicHolidayService.IsPublicHoliday(date.AtMidnight()) &&
                   !date.IsWeekend();
        }
    }
}
