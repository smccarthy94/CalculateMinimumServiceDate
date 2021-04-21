using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using ServiceDate.Services;

namespace ServiceDate.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => 
                    services.AddSingleton<IPublicHolidayService, PublicHolidayService>()
                            .AddScoped<IWorkshopDataService, WorkshopDataService>()
                            .AddScoped<IBookingService, BookingService>()
                            .AddScoped<IClock>(_ => new ZonedClock(SystemClock.Instance, DateTimeZone.Utc, CalendarSystem.Iso))
                    )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
