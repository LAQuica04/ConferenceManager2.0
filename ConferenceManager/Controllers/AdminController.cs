using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var users = await _userManager.Users.ToListAsync();
        var events = await _context.Events.ToListAsync();

        ViewBag.TotalUsers = users.Count;
        ViewBag.TotalEvents = events.Count;
        ViewBag.RecentUsers = users.Take(5);

        return View();
    }
}
