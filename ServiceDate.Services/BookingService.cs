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
            if (from.LocalDateTime.IsAfterMidday()) minimumNoticeDays++;

            // init the date ranges for checking
            var minimumDate = from.Date;
            var maximumDate = from.Date.PlusDays(90);

            var daysSinceLastHoliday = -1;
            var extraDaysNotice = 0;

            // get next date after minimum notice period.
            if (from.Date.PlusDays(minimumNoticeDays) > minimumDate)
            {
                minimumDate = minimumDate.NextWeekdayAfterBusinessDays(minimumNoticeDays);
            }

            // loop until we reach the maximum possible date or find a valid date.
            while ((!IsDateValid(minimumDate, workshopId) || extraDaysNotice > 0) && minimumDate < maximumDate)
            {
                // track when the most recent public holiday fell on.
                if (IsPublicHoliday(minimumDate)) daysSinceLastHoliday = 0;
                if (daysSinceLastHoliday > -1) daysSinceLastHoliday++;

                minimumDate = minimumDate.PlusDays(1);

                // if date is still not valid, loop.
                if (!IsDateValid(minimumDate, workshopId)) continue;

                // if we found a valid date, perform our last adjustments.
                if (daysSinceLastHoliday == 1 && from.LocalDateTime.IsAfterMidday())
                {
                    extraDaysNotice += 1;
                    daysSinceLastHoliday = -1;
                }

                // append notice if required.
                if (extraDaysNotice > 0 && IsDateValid(minimumDate.PlusDays(extraDaysNotice), workshopId))
                {
                    minimumDate = minimumDate.PlusDays(extraDaysNotice);
                    extraDaysNotice = 0;
                }
            }

            // if no possible days in the next 3 months.
            if (minimumDate >= maximumDate && !IsDateValid(minimumDate, workshopId)) return null;

            return minimumDate.AtMidnight();
        }

        private bool IsPublicHoliday(LocalDate date)
        {
            return _publicHolidayService.IsPublicHoliday(date.AtMidnight());
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
