using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class EventController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Events.ToListAsync());
    }

    [Authorize(Roles = "Administrator,Speaker")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Administrator,Speaker")]
    public async Task<IActionResult> Create(Event eventModel)
    {
        if (ModelState.IsValid)
        {
            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(eventModel);
    }

    [Authorize(Roles = "Administrator,Speaker")]
    public async Task<IActionResult> Edit(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        return eventModel == null ? NotFound() : View(eventModel);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Speaker")]
    public async Task<IActionResult> Edit(int id, Event eventModel)
    {
        if (id != eventModel.Id) return NotFound();

        _context.Update(eventModel);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel != null)
        {
            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Search(string keyword)
    {
        var events = _context.Events
            .Where(e => e.Title.Contains(keyword) || e.Location.Contains(keyword))
            .ToList();
        return View("Index", events);
    }
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.Speaker)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }
    // 🆕 Администраторски панел за одобрение на регистрации
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PendingRegistrations()
    {
        var pendingRegistrations = await _context.Registrations
            .Where(r => !r.Confirmed)
            .Include(r => r.User)
            .Include(r => r.Event)
            .ToListAsync();

        return View(pendingRegistrations);
    }

    // 🆕 Одобряване на регистрация
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> ApproveRegistration(int registrationId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);
        if (registration != null)
        {
            registration.Confirmed = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(PendingRegistrations));
    }

    // 🆕 Отказване на регистрация
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> RejectRegistration(int registrationId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);
        if (registration != null)
        {
            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(PendingRegistrations));
    }
}
