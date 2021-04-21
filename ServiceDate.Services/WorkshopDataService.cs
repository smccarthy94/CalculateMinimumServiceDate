using NodaTime;

namespace ServiceDate.Services
{
    public interface IWorkshopDataService
    {
        int GetMinimumNoticeDays(long workshopId);
        bool IsClosed(long workshopId, LocalDate date);
        bool IsFullyBooked(long workshopId, LocalDate date);
    }

    public class WorkshopDataService : IWorkshopDataService
    {
        public int GetMinimumNoticeDays(long workshopId) => 2;

        public bool IsClosed(long workshopId, LocalDate date) => false;

        public bool IsFullyBooked(long workshopId, LocalDate date) => false;
    }
}
