using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CPMP.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(u => u.Roles)
                .Include(u => u.Files)
                .Include(u => u.Tasks)
                .Include(u => u.Notifications)
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.Roles)
                .Include(u => u.Files)
                .Include(u => u.Tasks)
                .Include(u => u.Notifications)
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.UserId.Equals(id!));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,PasswordHash,Email,CreatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.UserId.Equals(id!));
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.AllRoles = await _context.Roles.ToListAsync();
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,PasswordHash,Email,CreatedAt,Roles")] User user, int[] SelectedRoles)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var existingUser = _context.Users.Include(_=>_.Roles).FirstOrDefault(u => u.UserId == user.UserId);
                    var existingRoleIds = existingUser.Roles.Select(r => r.RoleId).ToList();
                    Console.WriteLine($"Existing Role IDs: {string.Join(", ", existingRoleIds)}");
                    var rolesToAdd = SelectedRoles.Except(existingRoleIds);
                    Console.WriteLine($"Roles to Add: {string.Join(", ", rolesToAdd)}");
                    var rolesToRemove = existingRoleIds.Except(SelectedRoles);
                    Console.WriteLine($"Roles to Remove: {string.Join(", ", rolesToRemove)}");

                    foreach (var roleId in rolesToRemove)
                    {
                        var role = existingUser.Roles.FirstOrDefault(r => r.RoleId == roleId);
                        if (role != null)
                        {
                            existingUser.Roles.Remove(role);
                        }
                    }
                    foreach (var roleId in rolesToAdd)
                    {
                        var role = await _context.Roles.FindAsync(roleId);
                        if (role != null)
                        {
                            existingUser.Roles.Add(role);
                        }
                    }

                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
