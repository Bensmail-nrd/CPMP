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
    public class TaskAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskAssignments
        public async Task<IActionResult> Index()
        {
            var taskAssignments =  _context.TaskAssignments
                .Include(t => t.Task)
                .ThenInclude(u => u.CreatedByNavigation)
                .Where(t=>t.UserId.Equals(int.Parse(HttpContext.Session.GetString("UserId")!))).AsNoTracking();
            
            return View(await taskAssignments.ToListAsync());
        }

        // GET: TaskAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskAssignment = await _context.TaskAssignments
                .Include(t => t.Task)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskAssignment == null)
            {
                return NotFound();
            }

            return View(taskAssignment);
        }

        // GET: TaskAssignments/Create
        public IActionResult Create(int? id)
        {
            ViewData["TaskId"] = id;
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: TaskAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,UserId")] TaskAssignment taskAssignment)
        {
            ModelState.Remove("User");
            ModelState.Remove("Task");
            if (ModelState.IsValid)
            {
                taskAssignment.AssignedAt = DateTime.Now;
                _context.Add(taskAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", taskAssignment.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", taskAssignment.UserId);
            return View(taskAssignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromTask([Bind("TaskId,UserId")] TaskAssignment taskAssignment)
        {
                taskAssignment.AssignedAt = DateTime.Now;
                _context.Add(taskAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit","Tasks", new {id=taskAssignment.TaskId});     
        }

        // GET: TaskAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(_=>_.TaskId==id);
            if (taskAssignment == null)
            {
                return NotFound();
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "Title", taskAssignment.TaskId);
            ViewBag.UserId = new SelectList(_context.Users, "UserId", "Email", taskAssignment.UserId);
            return View(taskAssignment);
        }

        // POST: TaskAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,UserId,AssignedAt")] TaskAssignment taskAssignment)
        {
            if (id != taskAssignment.TaskId)
            {
                return NotFound();
            }
            ModelState.Remove("User");
            ModelState.Remove("Task");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskAssignmentExists(taskAssignment.TaskId))
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
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "Title", taskAssignment.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", taskAssignment.UserId);
            return View(taskAssignment);
        }

        // GET: TaskAssignments/Delete/5
        public async Task<IActionResult> Delete(int? taskId, int? userId)
        {
            if (taskId == null || userId ==null)
            {
                return NotFound();
            }

            var taskAssignment = await _context.TaskAssignments
                .Include(t => t.Task)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == taskId && m.UserId ==userId);
            if (taskAssignment == null)
            {
                return NotFound();
            }

            return View(taskAssignment);
        }

        // POST: TaskAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? taskId, int? userId)
        {
            var taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(m => m.TaskId == taskId && m.UserId == userId);
            if (taskAssignment != null)
            {
                _context.TaskAssignments.Remove(taskAssignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Tasks", new { id = taskId});
        }

        private bool TaskAssignmentExists(int id)
        {
            return _context.TaskAssignments.Any(e => e.TaskId == id);
        }
    }
}
