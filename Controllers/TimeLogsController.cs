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
    public class TimeLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TimeLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TimeLogs.Include(t => t.Task).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TimeLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLogs
                .Include(t => t.Task)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TimeLogId == id);
            if (timeLog == null)
            {
                return NotFound();
            }

            return View(timeLog);
        }

        // GET: TimeLogs/Create
        public IActionResult Create()
        {
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: TimeLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeLogId,TaskId,UserId,HoursWorked,LogDate,Notes")] TimeLog timeLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", timeLog.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", timeLog.UserId);
            return View(timeLog);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimeLog(int taskId, decimal hoursWorked, string notes)
        {
            if (ModelState.IsValid)
            {
                var timeLog = new TimeLog
                {
                    TaskId = taskId,
                    UserId = int.Parse(HttpContext.Session.GetString("UserId")!),
                    HoursWorked = hoursWorked,
                    LogDate = DateOnly.FromDateTime(DateTime.Now),
                    Notes = notes
                };
                _context.Add(timeLog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", "Tasks", new { id = taskId });
        }

        // GET: TimeLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLogs.FindAsync(id);
            if (timeLog == null)
            {
                return NotFound();
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", timeLog.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", timeLog.UserId);
            return View(timeLog);
        }

        // POST: TimeLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimeLogId,TaskId,UserId,HoursWorked,LogDate,Notes")] TimeLog timeLog)
        {
            if (id != timeLog.TimeLogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeLogExists(timeLog.TimeLogId))
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
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", timeLog.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", timeLog.UserId);
            return View(timeLog);
        }

        // GET: TimeLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLogs
                .Include(t => t.Task)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TimeLogId == id);
            if (timeLog == null)
            {
                return NotFound();
            }

            return View(timeLog);
        }

        // POST: TimeLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeLog = await _context.TimeLogs.FindAsync(id);
            var taskId = timeLog.TaskId;
            if (timeLog != null)
            {
                _context.TimeLogs.Remove(timeLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","Tasks", new {Id = taskId});
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLogs.Any(e => e.TimeLogId == id);
        }
    }
}
