using CPMP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPMP.Controllers
{
	public class AccountController : Controller
	{
		readonly private ApplicationDbContext _context;
		public AccountController(ApplicationDbContext context)
		{
			   _context = context;
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUserViewModel model)
        {
            if (ModelState.IsValid)
            {
				var user = _context.Users.Include(_ => _.Roles).FirstOrDefault(_=>_.Email == model.Email);
				if (user == null)
					{ 
					TempData["Error"] = "Invalid email or password";
					return View(model); }
                var passwordIsCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
				if (passwordIsCorrect)
				{
					HttpContext.Session.SetString("UserId", user.UserId.ToString());
					HttpContext.Session.SetString("Username", user.Username);
					HttpContext.Session.SetString("Email", user.Email);
                    foreach (var item in user.Roles)
                    {
                        HttpContext.Session.SetString(item.Name, "1");
                    }
                    return RedirectToAction("Index", "Home");
				}
                    TempData["Error"] = "Invalid email or password";
				return View(model);

                
            }
			return View(model);

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }

        [HttpGet]
        public IActionResult Register()
		{
			return View();
		}

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
				var existingUser = _context.Users.FirstOrDefault(u => (u.Email == model.Email) || (u.Username==model.Username));
				if (existingUser != null)
                {
                    TempData["Error"] = "Email or username already exists";
                    return View(model);
                }
				var role = _context.Roles.Find(3);
				var user = new User
				{
					Email = model.Email,
					Username = model.Username,
					Roles = [role],
					PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
				};

				_context.Users.Add(user);
				_context.SaveChanges();


				return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
			return RedirectToAction("Login");

        }
        // GET: LoginController
        public ActionResult Index()
		{
			return View();
		}

		// GET: LoginController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: LoginController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: LoginController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: LoginController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: LoginController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: LoginController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: LoginController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
