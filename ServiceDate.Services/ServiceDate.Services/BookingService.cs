using NodaTime;
using ServiceDate.Core;

namespace ServiceDate.Services
{
    public interface IBookingService
    {
        LocalDateTime? CalculateMinimumServiceDate(int workshopId);
    }

    public class BookingService : IBookingService
    {
        private readonly IClock _clock;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IWorkshopDataService _workshopDataService;

        private readonly int _daysNoticeBeforePublicHolidays = 2;
        private readonly int _daysBufferAfterPublicHolidays = 1;

		public BookingService(IClock clock, IWorkshopDataService workshopDataService, IPublicHolidayService publicHolidayService)
        {
            _clock = clock;
            _workshopDataService = workshopDataService;
            _publicHolidayService = publicHolidayService;
        }

        public LocalDateTime? CalculateMinimumServiceDate(int workshopId)
        {
            var from = _clock.GetCurrentInstant().InUtc();

			// workshop data
            int minimumNoticeDays = _workshopDataService.GetMinimumNoticeDays(workshopId);

			// init the date ranges for checking
			var minimumDate = from.Date;
			var maximumDate = from.Date.PlusDays(90);

			// state tracking.
			bool isClosed;
			bool isFullyBooked;
			bool isPublicHoliday;

			var publicHolidayDetected = false;
			var daysSinceLastHoliday = -1;
			var extraDaysNotice = 0;
			var beforeNoonCutOff = true;

			if (from.Date.PlusDays(minimumNoticeDays) > minimumDate) // check if workshops minimum days notice is satisfied.
			{
				minimumDate = minimumDate.PlusDays(minimumNoticeDays);

                // offset post weekend minimum date for workshops that require more than one full day of notice.
				if (minimumDate.IsWeekend()) extraDaysNotice++;
            }

			if (from.LocalDateTime.IsAfterMidday()) // cut off at 12pm.
			{
				minimumDate = minimumDate.PlusDays(1);

                beforeNoonCutOff = false;

				// missed cut-off, and today is a friday, min date will need to be a day later to allow enough notice.
				// only apply this extra notice for weekend case if not already considered.
				if (from.Date.PlusDays(minimumNoticeDays).IsWeekend() && extraDaysNotice < 1) 
                    extraDaysNotice++;
            }

			// helper methods.
			bool ShouldIterate()
			{
				ValidateMinimumDate();
				return isClosed || isFullyBooked || isPublicHoliday || minimumDate.IsWeekend();
			}

			bool CanIterate()
			{
				return minimumDate < maximumDate;
			}

			bool RequiresPublicHolidayBuffer()
			{
				return publicHolidayDetected && daysSinceLastHoliday == 1 && !beforeNoonCutOff;
			}

			void ValidateMinimumDate()
			{
				isClosed = _workshopDataService.IsClosed(workshopId, minimumDate);
				isFullyBooked = _workshopDataService.IsFullyBooked(workshopId, minimumDate);
				isPublicHoliday = _publicHolidayService.IsPublicHoliday(minimumDate.AtMidnight());
			}

			// check workshop status and if date is valid so far.
			ValidateMinimumDate();

			var nextPublicHoliday = _publicHolidayService.NextPublicHoliday(from.LocalDateTime);
			// make this return null if no public holiday after X date..
			if (nextPublicHoliday.Year > 1 && (from.Date.PlusDays(_daysNoticeBeforePublicHolidays) >= nextPublicHoliday))
			{
				// public holiday falls within the next 2 days
				publicHolidayDetected = true;
            }

			// replicating similar code block from example seen with Anth, for the sake of this being a somewhat better fit to existing code.
			while ((ShouldIterate() || extraDaysNotice > 0) && CanIterate())
			{
				// track when the most recent public holiday fell on.
				if (isPublicHoliday) daysSinceLastHoliday = 0;
				if (daysSinceLastHoliday > -1) daysSinceLastHoliday++;

				minimumDate = minimumDate.PlusDays(1);

				// if we found a valid date, perform our last adjustments, 
				// and make sure the date THIS produces is still valid, looping if required.
                if (ShouldIterate()) continue;

                if (RequiresPublicHolidayBuffer())
                {
                    extraDaysNotice += _daysBufferAfterPublicHolidays;
                    daysSinceLastHoliday = -1;
                }

                if (extraDaysNotice > 0)
                {
                    minimumDate = minimumDate.PlusDays(extraDaysNotice);
                    extraDaysNotice = 0;
                }
            }

			// if no possible days in the next 3 months.
            if (!CanIterate() && ShouldIterate()) return null;

			return minimumDate.AtMidnight();
		}
    }
}
