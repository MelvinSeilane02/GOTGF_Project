using GOTGF_Project.Areas.Identity.Pages.Account;
using GOTGF_Project.Data;
using GOTGF_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GOTGF_Project.Controllers
{
    [Authorize(Roles = "Admin,Volunteer")]
    //[Authorize(Roles = "ADMIN")]
    //[Authorize]

    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            // Check what roles are attached to the logged-in user
            var roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value);

            Console.WriteLine("User Roles: " + string.Join(", ", roles));

            var applicationDbContext = _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Reports/Create
        [Authorize(Roles = "Admin,Volunteer")]
        public IActionResult Create()
        {
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName");
            ViewData["GeneratedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportID,ReportType,ReportDate,ProjectID,GeneratedBy")] Report report)
        {
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

            }
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", report.ProjectID);
            ViewData["GeneratedBy"] = new SelectList(_context.Users, "Id", "Id", report.GeneratedBy);
            return View(report);
        }

        // GET: Reports/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", report.ProjectID);
            ViewData["GeneratedBy"] = new SelectList(_context.Users, "Id", "Id", report.GeneratedBy);
            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportID,ReportType,ReportDate,ProjectID,GeneratedBy")] Report report)
        {
            if (id != report.ReportID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.ReportID))
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
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", report.ProjectID);
            ViewData["GeneratedBy"] = new SelectList(_context.Users, "Id", "Id", report.GeneratedBy);
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.ReportID == id);
        }
    }
}
