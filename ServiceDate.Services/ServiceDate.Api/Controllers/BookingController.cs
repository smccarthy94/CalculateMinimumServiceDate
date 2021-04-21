using Microsoft.AspNetCore.Mvc;
using ServiceDate.Services;

namespace ServiceDate.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet, Route("minimum-service-date/{workshopId}")]
        public IActionResult GetMinimumServiceDate(long workshopId)
        {
            var minDate = _bookingService.CalculateMinimumServiceDate(workshopId);

            if (minDate == null) return NotFound();

            return Ok(minDate.ToString());
        }
    }
}
