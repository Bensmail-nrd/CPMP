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
                .Include(u=>u.TaskAssignments).ThenInclude(_=>_.Task).ThenInclude(_=>_.Status) 
                .Include(u=>u.TeamMembers)
                .Include(u => u.Notifications.Where(n => n.IsRead==false))
                .AsNoTracking()
                .FirstOrDefault(_ => _.UserId.Equals(userId));
            return View(user);
        }
        [HttpGet("notes")]
        public IActionResult Notification()
        {
            var notification = _context.Notifications.Where(_=>_.UserId==int.Parse(HttpContext.Session.GetString("UserId")!) && _.IsRead==false).ToList();
            foreach (var item in notification)
            {
                item.IsRead = true;
            }
            _context.SaveChanges();
            return Json(notification);
        }
    }
}
