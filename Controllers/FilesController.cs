using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CPMP.Models;
using Microsoft.Build.Framework;

namespace CPMP.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Files
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Files.Include(_ => _.Task).Include(_ => _.UploadedByNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Files/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(_ => _.Task)
                .Include(_ => _.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // GET: Files/Create
        public IActionResult Create(int taskId)
        {
            var file = new CPMP.Models.File { TaskId = taskId };
            ViewBag.TaskId = taskId;
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId");
            ViewData["UploadedBy"] = new SelectList(_context.Users, "UserId", "Email");
            return View(file);
        }

        // POST: Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FileId,TaskId,UploadedBy,FileName,FilePath,UploadedAt")] CPMP.Models.File file)
        {
            if (ModelState.IsValid)
            {
                _context.Add(file);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", file.TaskId);
            ViewData["UploadedBy"] = new SelectList(_context.Users, "UserId", "Email", file.UploadedBy);
            return View(file);
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFile(IFormFile file,int taskId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }
            Directory.CreateDirectory("d:/uploads/");
            string filePath = Path.Combine("d:/uploads/", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var tempFile = new CPMP.Models.File
            {
                FileName = file.FileName,
                FilePath = filePath,
                TaskId = taskId,
                UploadedBy = int.Parse(HttpContext.Session.GetString("UserId")!),
                UploadedAt = DateTime.Now
            };
             
            _context.Files.Add(tempFile);
            _context.SaveChanges();
            return RedirectToAction(actionName:"Edit",controllerName: "Tasks",routeValues:new {id=taskId});
        }


        // GET: Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", file.TaskId);
            ViewData["UploadedBy"] = new SelectList(_context.Users, "UserId", "Email", file.UploadedBy);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FileId,TaskId,UploadedBy,FileName,FilePath,UploadedAt")] CPMP.Models.File file)
        {
            if (id != file.FileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(file);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(file.FileId))
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
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", file.TaskId);
            ViewData["UploadedBy"] = new SelectList(_context.Users, "UserId", "Email", file.UploadedBy);
            return View(file);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(_ => _.Task)
                .Include(_ => _.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.FileId == id);
        }
    }
}
