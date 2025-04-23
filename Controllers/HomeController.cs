using CPMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPMP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = _context.Users.Include(u => u.Roles)
                .Include(u => u.Files)
                .Include(u => u.Tasks)
                .Include(u => u.Notifications.Where(n => n.IsRead==false))
                .AsNoTracking()
                .FirstOrDefault(_ => _.UserId.Equals(userId));
            return View(user);
        }
    }
}
