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
    public class VolunteerAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VolunteerAssignments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.VolunteerAssignments.Include(v => v.Project).Include(v => v.Volunteer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: VolunteerAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerAssignment = await _context.VolunteerAssignments
                .Include(v => v.Project)
                .Include(v => v.Volunteer)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);
            if (volunteerAssignment == null)
            {
                return NotFound();
            }

            return View(volunteerAssignment);
        }

        // GET: VolunteerAssignments/Create
        [Authorize(Roles = "Admin,Volunteer")]
        public IActionResult Create()
        {
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName");
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "VolunteerID", "UserId");
            return View();
        }

        // POST: VolunteerAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignmentID,VolunteerID,ProjectID,AssignedDate")] VolunteerAssignment volunteerAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volunteerAssignment);
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
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", volunteerAssignment.ProjectID);
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "VolunteerID", "UserId", volunteerAssignment.VolunteerID);
            return View(volunteerAssignment);
        }

        // GET: VolunteerAssignments/Edit/5
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerAssignment = await _context.VolunteerAssignments.FindAsync(id);
            if (volunteerAssignment == null)
            {
                return NotFound();
            }
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", volunteerAssignment.ProjectID);
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "VolunteerID", "UserId", volunteerAssignment.VolunteerID);
            return View(volunteerAssignment);
        }

        // POST: VolunteerAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentID,VolunteerID,ProjectID,AssignedDate")] VolunteerAssignment volunteerAssignment)
        {
            if (id != volunteerAssignment.AssignmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteerAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerAssignmentExists(volunteerAssignment.AssignmentID))
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
            ViewData["ProjectID"] = new SelectList(_context.Projects, "ProjectID", "ProjectName", volunteerAssignment.ProjectID);
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "VolunteerID", "UserId", volunteerAssignment.VolunteerID);
            return View(volunteerAssignment);
        }

        // GET: VolunteerAssignments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerAssignment = await _context.VolunteerAssignments
                .Include(v => v.Project)
                .Include(v => v.Volunteer)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);
            if (volunteerAssignment == null)
            {
                return NotFound();
            }

            return View(volunteerAssignment);
        }

        // POST: VolunteerAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteerAssignment = await _context.VolunteerAssignments.FindAsync(id);
            if (volunteerAssignment != null)
            {
                _context.VolunteerAssignments.Remove(volunteerAssignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerAssignmentExists(int id)
        {
            return _context.VolunteerAssignments.Any(e => e.AssignmentID == id);
        }
    }
}
