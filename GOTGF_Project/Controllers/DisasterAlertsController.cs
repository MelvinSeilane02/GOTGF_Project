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
    [Authorize(Roles = "Admin")] // Only Admins can access this controller
    public class DisasterAlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DisasterAlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DisasterAlerts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DisasterAlerts.Include(d => d.Project).Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DisasterAlerts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disasterAlert = await _context.DisasterAlerts
                .Include(d => d.Project)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.AlertID == id);
            if (disasterAlert == null)
            {
                return NotFound();
            }

            return View(disasterAlert);
        }

        // GET: DisasterAlerts/Create
        public IActionResult Create()
        {
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DisasterAlerts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlertID,ProjectID,CreatedBy,AlertMessage,AlertDate")] DisasterAlert disasterAlert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disasterAlert);
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
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", disasterAlert.ProjectID);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", disasterAlert.CreatedBy);
            return View(disasterAlert);
        }

        // GET: DisasterAlerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disasterAlert = await _context.DisasterAlerts.FindAsync(id);
            if (disasterAlert == null)
            {
                return NotFound();
            }
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", disasterAlert.ProjectID);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", disasterAlert.CreatedBy);
            return View(disasterAlert);
        }

        // POST: DisasterAlerts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlertID,ProjectID,CreatedBy,AlertMessage,AlertDate")] DisasterAlert disasterAlert)
        {
            if (id != disasterAlert.AlertID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disasterAlert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisasterAlertExists(disasterAlert.AlertID))
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
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", disasterAlert.ProjectID);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", disasterAlert.CreatedBy);
            return View(disasterAlert);
        }

        // GET: DisasterAlerts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disasterAlert = await _context.DisasterAlerts
                .Include(d => d.Project)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.AlertID == id);
            if (disasterAlert == null)
            {
                return NotFound();
            }

            return View(disasterAlert);
        }

        // POST: DisasterAlerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disasterAlert = await _context.DisasterAlerts.FindAsync(id);
            if (disasterAlert != null)
            {
                _context.DisasterAlerts.Remove(disasterAlert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisasterAlertExists(int id)
        {
            return _context.DisasterAlerts.Any(e => e.AlertID == id);
        }
    }
}
