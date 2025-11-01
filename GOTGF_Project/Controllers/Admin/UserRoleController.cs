using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GOTGF_Project.Data;
using GOTGF_Project.Models; // ApplicationUser, viewmodels
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOTGF_Project.Controllers.Admin
{
    // Only admins can access these user/role management pages
    [Authorize(Roles = "Admin")]
    [Route("admin/users")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserRoleController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
             _logger = logger;
        }

        // GET: admin/users
        // Lists users with current roles and available roles for assignment
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // fetch all users (projection to lightweight viewmodel)
            var users = await _userManager.Users
                .Select(u => new UserRoleViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName // ensure ApplicationUser has FullName property
                })
                .ToListAsync();

            // populate current roles for each user (async loop)
            foreach (var u in users)
            {
                var userEntity = await _userManager.FindByIdAsync(u.Id);
                u.CurrentRoles = await _userManager.GetRolesAsync(userEntity);
            }

            // available roles in the system
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var vm = new UsersIndexViewModel
            {
                Users = users,
                AvailableRoles = roles
            };

            return View(vm); // create Views/Admin/UserRole/Index.cshtml
        }

        // GET: admin/users/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserRoleViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CurrentRoles = await _userManager.GetRolesAsync(user)
            };

            return View(vm); // create Views/Admin/UserRole/Details.cshtml or reuse existing
        }

        // GET: admin/users/edit/{id}
        // Shows the edit form (select new role)
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                SelectedRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            vm.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View(vm); // create Views/Admin/UserRole/Edit.cshtml
        }

        // POST: admin/users/edit
        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                TempData["RoleChangeError"] = "Invalid request. User is missing.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["RoleChangeError"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent admin from changing their own role
            var currentUserId = _userManager.GetUserId(User);
            if (model.Id == currentUserId)
            {
                TempData["RoleChangeError"] = "You cannot change your own role.";
                return RedirectToAction(nameof(Index));
            }

            // If clearing role (empty selection) -> remove all roles
            if (string.IsNullOrEmpty(model.SelectedRole))
            {
                var oldRoles = await _userManager.GetRolesAsync(user);
                if (oldRoles.Any())
                {
                    var removeRes = await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    if (!removeRes.Succeeded)
                    {
                        TempData["RoleChangeError"] = "Failed to remove existing roles.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                TempData["RoleChangeSuccess"] = $"Cleared roles for {user.Email}.";
                return RedirectToAction(nameof(Index));
            }

            // Validate role exists
            if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
            {
                TempData["RoleChangeError"] = $"Role '{model.SelectedRole}' does not exist.";
                return RedirectToAction(nameof(Index));
            }

            // Remove all existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["RoleChangeError"] = "Failed to remove existing roles.";
                return RedirectToAction(nameof(Index));
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!addResult.Succeeded)
            {
                TempData["RoleChangeError"] = "Failed to add the selected role.";
                return RedirectToAction(nameof(Index));
            }

            TempData["RoleChangeSuccess"] = $"Role for {user.Email} updated to {model.SelectedRole}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: admin/users/delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["RoleChangeError"] = "Invalid request. User ID missing.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["RoleChangeError"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent admin from deleting their own account
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                TempData["RoleChangeError"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["RoleChangeSuccess"] = $"User {user.Email} deleted successfully.";
            }
            else
            {
                TempData["RoleChangeError"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(Index));
        }


        // helper: persist audit
        private async Task SaveRoleChangeAuditAsync(string targetUserId, string changedByUserId, string oldRoles, string newRole)
        {
            try
            {
                var log = new RoleChangeLog
                {
                    TargetUserId = targetUserId,
                    ChangedByUserId = changedByUserId,
                    OldRoles = oldRoles,
                    NewRole = newRole,
                    ChangedAt = DateTime.UtcNow
                };
                _context.RoleChangeLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save role change audit for {UserId}", targetUserId);
            }
        }
    }
}
