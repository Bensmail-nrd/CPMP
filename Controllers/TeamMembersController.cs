using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CPMP.Models;

namespace CPMP.Controllers
{
    public class TeamMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeamMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TeamMembers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TeamMembers.Include(t => t.Team).Include(t => t.User).AsNoTracking();
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TeamMembers/Details/5
        public async Task<IActionResult> Details(int teamId, int userId)
        {

            var teamMember = await _context.TeamMembers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId==userId);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        // GET: TeamMembers/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: TeamMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamId,UserId,RoleInTeam")] TeamMember teamMember)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(teamMember);
                var team = _context.Teams.AsNoTracking().Include(_ => _.TeamMembers).FirstOrDefault(t => t.TeamId == teamMember.TeamId);
                foreach(var item in team.TeamMembers)
                {
                    if(item.UserId == teamMember.UserId)
                    {
                        ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", teamMember.TeamId);
                        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", teamMember.UserId);
                        ViewData["Error"] = "This user has already been included in the selected team";
                        return View(teamMember);
                    }
                }
                var teamName = team.Name;
                var notification = new Notification
                {
                    UserId = teamMember.UserId,
                    Message = $"You have been added to the team \"{teamName}\" as ({teamMember.RoleInTeam}).",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", teamMember.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", teamMember.UserId);
            return View(teamMember);
        }

        // GET: TeamMembers/Edit/5
        public async Task<IActionResult> Edit(int teamId, int userId)
        {
            var teamMember = await _context.TeamMembers.Include(_=>_.User).Include(_=>_.Team).FirstOrDefaultAsync(_=>_.TeamId==teamId && _.UserId==userId);
            if (teamMember == null)
            {
                return NotFound();
            }
			ViewBag.roles = new SelectList(_context.RolesInTeams, "Name", "Name", teamMember.RoleInTeam);
            return View(teamMember);
        }

        // POST: TeamMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("TeamId,UserId,RoleInTeam")] TeamMember teamMember)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamMemberExists(teamMember.TeamId))
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
            ViewBag.roles = new SelectList(_context.RolesInTeams, "Name", "Name", teamMember.RoleInTeam);
            return View(teamMember);
        }

        // GET: TeamMembers/Delete/5
        public async Task<IActionResult> Delete(int teamId, int userId)
        {

            var teamMember = await _context.TeamMembers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId==userId);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        // POST: TeamMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int teamId, int userId)
        {
            var teamMember = await _context.TeamMembers.FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);
            if (teamMember != null)
            {
                _context.TeamMembers.Remove(teamMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","Teams", new { id = teamId});
        }

        private bool TeamMemberExists(int id)
        {
            return _context.TeamMembers.Any(e => e.TeamId == id);
        }
    }
}
