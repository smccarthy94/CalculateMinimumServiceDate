using System;

namespace ServiceDate.Services
{
    public interface IBookingService
    {
        DateTimeOffset CalculateMinimumServiceDate();
    }

    public class BookingService : IBookingService
    {
        public DateTimeOffset CalculateMinimumServiceDate()
        {
            throw new NotImplementedException();
        }
    }
}
