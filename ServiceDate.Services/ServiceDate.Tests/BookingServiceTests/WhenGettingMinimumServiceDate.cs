using NodaTime;
using NSubstitute;
using NUnit.Framework;
using ServiceDate.Services;

namespace ServiceDate.Tests.BookingServiceTests
{
    public class WhenGettingMinimumServiceDate
    {
        private readonly int _workshopId = 123;

        private BookingService GivenABookingService(IClock clock)
        {
            var mockWorkshopDataService = Substitute.For<IWorkshopDataService>();
            mockWorkshopDataService.GetMinimumNoticeDays(123).Returns(2);
            mockWorkshopDataService.IsClosed(123, Arg.Any<LocalDate>()).Returns(false);
            mockWorkshopDataService.IsFullyBooked(123, Arg.Any<LocalDate>()).Returns(false);

            return new BookingService(clock, mockWorkshopDataService, new PublicHolidayService());
        }

        private IClock GivenAClock(Instant now)
        {
            var mockSystemClock = Substitute.For<IClock>();
            mockSystemClock.GetCurrentInstant().Returns(now);

            return mockSystemClock;
        }

        [Test]
        public void MondayBeforeSameDayCutOffReturnsWednesday()
        {
            var utcNow = Instant.FromUtc(2021, 04, 19, 11, 59, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 21, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }

        [Test]
        public void MondayAfterSameDayCutOffReturnsThursday()
        {
            var utcNow = Instant.FromUtc(2021, 04, 19, 12, 0, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 22, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }

        [Test]
        public void FridayBeforeSameDayCutOffReturnsTheFollowingTuesday()
        {
            var utcNow = Instant.FromUtc(2021, 04, 23, 11, 59, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 27, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }

        [Test]
        public void FridayAfterSameDayCutOffReturnsTheFollowingWednesday()
        {
            var utcNow = Instant.FromUtc(2021, 04, 23, 12, 0, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 28, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }

        [Test]
        public void WednesdayBeforeEasterBeforeSameDayCutOffReturnsTheFollowingTuesday()
        {
            var utcNow = Instant.FromUtc(2021, 03, 31, 11, 59, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 6, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }

        [Test]
        public void WednesdayBeforeEasterAfterSameDayCutOffReturnsTheFollowingWednesday()
        {
            var utcNow = Instant.FromUtc(2021, 03, 31, 12, 0, 0);
            var clock = GivenAClock(utcNow);
            var service = GivenABookingService(clock);

            Assert.AreEqual(new LocalDateTime(2021, 4, 7, 0, 0, 0), service.CalculateMinimumServiceDate(_workshopId));
        }
    }
}