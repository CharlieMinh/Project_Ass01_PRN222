using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;
using Scientific.WebAppMVC.ViewModels;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class RolesController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public RolesController(ScientificJournalTrendDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.RolesHuyDds
                .Include(x => x.UserIdHuyDds)
                .OrderBy(x => x.RoleName)
                .ToListAsync();

            return View(roles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await _context.RolesHuyDds
                .Include(x => x.UserIdHuyDds)
                .FirstOrDefaultAsync(x => x.RoleIdHuyDd == id);

            return role == null ? NotFound() : View(role);
        }

        public IActionResult Create()
        {
            return View(new RoleFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (await _context.RolesHuyDds.AnyAsync(x => x.RoleName == model.RoleName))
            {
                ModelState.AddModelError(nameof(model.RoleName), "This role name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = new RolesHuyDd
            {
                RoleName = model.RoleName.Trim(),
                Description = Clean(model.Description),
                CreatedAt = DateTime.Now
            };

            _context.RolesHuyDds.Add(role);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Role created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.RolesHuyDds.FirstOrDefaultAsync(x => x.RoleIdHuyDd == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(new RoleFormViewModel
            {
                RoleId = role.RoleIdHuyDd,
                RoleName = role.RoleName,
                Description = role.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleFormViewModel model)
        {
            if (model.RoleId != id)
            {
                return BadRequest();
            }

            if (await _context.RolesHuyDds.AnyAsync(x => x.RoleName == model.RoleName && x.RoleIdHuyDd != id))
            {
                ModelState.AddModelError(nameof(model.RoleName), "This role name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _context.RolesHuyDds
                .AsTracking()
                .FirstOrDefaultAsync(x => x.RoleIdHuyDd == id);

            if (role == null)
            {
                return NotFound();
            }

            role.RoleName = model.RoleName.Trim();
            role.Description = Clean(model.Description);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.RolesHuyDds
                .Include(x => x.UserIdHuyDds)
                .FirstOrDefaultAsync(x => x.RoleIdHuyDd == id);

            return role == null ? NotFound() : View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.RolesHuyDds
                .AsTracking()
                .Include(x => x.UserIdHuyDds)
                .FirstOrDefaultAsync(x => x.RoleIdHuyDd == id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.UserIdHuyDds.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this role because users are assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            _context.RolesHuyDds.Remove(role);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Role deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
