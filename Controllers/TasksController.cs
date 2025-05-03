using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CPMP.Models;

namespace CPMP.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tasks.
                Include(t => t.Project).
                Include(t => t.Status).
                AsNoTracking().
                Where(_=>_.CreatedBy==int.Parse(HttpContext.Session.GetString("UserId")!));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Name");
            ViewData["StatusId"] = new SelectList(_context.TaskStatuses, "TaskStatusId", "Name");
            CPMP.Models.Task task = new CPMP.Models.Task
            {
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StatusId = _context.TaskStatuses.Find(2)!.TaskStatusId,
            };
            return View(task);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,ProjectId,Title,Description,CreatedBy,StatusId,DueDate,CreatedAt")] CPMP.Models.Task task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                TempData["ToastrType"] = "success";
                TempData["ToastrMessage"] = "The Task has been created";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.TaskStatuses, "TaskStatusId", "Name", task.StatusId);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(_ => _.Files).ThenInclude(_ => _.UploadedByNavigation)
                .Include(_ => _.TimeLogs)
                .Include(_ => _.TaskComments)
                .Include(_ => _.TaskAssignments)
                .ThenInclude(_=>_.User)
                .FirstOrDefaultAsync(_=>_.TaskId==id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["Users"] = new SelectList(_context.Users.Where(_=>!_.TaskAssignments.Any(_=>_.TaskId==id)), "UserId", "Email", task.CreatedBy);

            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Email", task.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.TaskStatuses, "TaskStatusId", "Name", task.StatusId);
            if (Request.Headers.Referer.Last().Contains("TaskAssignment"))
            {
                TempData["From"] = "TaskAssignment";
                TempData.Keep();
            }
            return View(task);
        }


        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,ProjectId,Title,Description,CreatedBy,StatusId,DueDate,CreatedAt")] CPMP.Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }
            var a = TempData["From"];
            ModelState.Remove("ProjectId"); 
            ModelState.Remove("DueDate"); 
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (TempData["From"]!=null && TempData["From"]!.ToString()== "TaskAssignment")
                {
                    return RedirectToAction("Index", "TaskAssignments");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Email", task.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.TaskStatuses, "TaskStatusId", "Name", task.StatusId);
            TempData.Keep();
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
