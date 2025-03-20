using Microsoft.AspNetCore.Mvc;

[Route("api/events")]
[ApiController]
public class EventsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetEvents()
    {
        return Ok(_context.Events.ToList());
    }
}
  