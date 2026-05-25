using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.Services;
using Scientific.WebAppMVC.Authorization;
using Scientific.WebAppMVC.ViewModels;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class UsersController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public UsersController(ScientificJournalTrendDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    (x.Organization != null && x.Organization.Contains(search)));
            }

            ViewBag.Search = search;

            var users = await query
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            return user == null ? NotFound() : View(user);
        }

        public async Task<IActionResult> AssignRoles(int id)
        {
            var user = await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            var selectedIds = user.RoleIdHuyDds.Select(x => x.RoleIdHuyDd).ToList();
            var model = new ManyToManyAssignmentViewModel
            {
                EntityId = user.UserIdHuyDd,
                EntityName = $"{user.FullName} ({user.Email})",
                PageTitle = "Assign Roles",
                OptionLabel = "Roles",
                BackController = "Users",
                RequireAtLeastOne = true,
                SelectedIds = selectedIds,
                Options = await _context.RolesHuyDds
                    .OrderBy(x => x.RoleName)
                    .Select(x => new AssignmentOptionViewModel
                    {
                        Id = x.RoleIdHuyDd,
                        Name = x.RoleName,
                        Description = x.Description,
                        IsSelected = selectedIds.Contains(x.RoleIdHuyDd)
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoles(int id, ManyToManyAssignmentViewModel model)
        {
            if (!model.SelectedIds.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedIds), "Select at least one role.");
            }

            var user = await _context.UsersHuyDds
                .AsTracking()
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return await AssignRoles(id);
            }

            var selectedRoles = await _context.RolesHuyDds
                .AsTracking()
                .Where(x => model.SelectedIds.Contains(x.RoleIdHuyDd))
                .ToListAsync();

            user.RoleIdHuyDds.Clear();
            foreach (var role in selectedRoles)
            {
                user.RoleIdHuyDds.Add(role);
            }

            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Roles assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserAdminFormViewModel
            {
                AvailableRoles = await BuildRoleOptionsAsync(new List<int>())
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAdminFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required when creating a user.");
            }

            if (!model.SelectedRoleIds.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedRoleIds), "Select at least one role.");
            }

            if (await _context.UsersHuyDds.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await BuildRoleOptionsAsync(model.SelectedRoleIds);
                return View(model);
            }

            var selectedRoles = await _context.RolesHuyDds
                .AsTracking()
                .Where(x => model.SelectedRoleIds.Contains(x.RoleIdHuyDd))
                .ToListAsync();

            var user = new UsersHuyDd
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                PasswordHash = UsersHuyDdSevice.HashPassword(model.Password!),
                PhoneNumber = Clean(model.PhoneNumber),
                AvatarUrl = Clean(model.AvatarUrl),
                Organization = Clean(model.Organization),
                AcademicTitle = Clean(model.AcademicTitle),
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            foreach (var role in selectedRoles)
            {
                user.RoleIdHuyDds.Add(role);
            }

            _context.UsersHuyDds.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            var selectedRoleIds = user.RoleIdHuyDds.Select(x => x.RoleIdHuyDd).ToList();
            var model = new UserAdminFormViewModel
            {
                UserId = user.UserIdHuyDd,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                Organization = user.Organization,
                AcademicTitle = user.AcademicTitle,
                IsActive = user.IsActive,
                SelectedRoleIds = selectedRoleIds,
                AvailableRoles = await BuildRoleOptionsAsync(selectedRoleIds)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserAdminFormViewModel model)
        {
            if (model.UserId != id)
            {
                return BadRequest();
            }

            if (!model.SelectedRoleIds.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedRoleIds), "Select at least one role.");
            }

            if (await _context.UsersHuyDds.AnyAsync(x => x.Email == model.Email && x.UserIdHuyDd != id))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await BuildRoleOptionsAsync(model.SelectedRoleIds);
                return View(model);
            }

            var user = await _context.UsersHuyDds
                .AsTracking()
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim();
            user.PhoneNumber = Clean(model.PhoneNumber);
            user.AvatarUrl = Clean(model.AvatarUrl);
            user.Organization = Clean(model.Organization);
            user.AcademicTitle = Clean(model.AcademicTitle);
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = UsersHuyDdSevice.HashPassword(model.Password);
            }

            user.RoleIdHuyDds.Clear();
            var selectedRoles = await _context.RolesHuyDds
                .AsTracking()
                .Where(x => model.SelectedRoleIds.Contains(x.RoleIdHuyDd))
                .ToListAsync();

            foreach (var role in selectedRoles)
            {
                user.RoleIdHuyDds.Add(role);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            return user == null ? NotFound() : View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id.ToString())
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.UsersHuyDds
                .AsTracking()
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.RoleIdHuyDds.Clear();
                _context.UsersHuyDds.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this user because related records exist. Deactivate the account instead.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id.ToString())
            {
                TempData["ErrorMessage"] = "You cannot lock your own account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.UsersHuyDds
                .AsTracking()
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = user.IsActive ? "User unlocked successfully." : "User locked successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<RoleOptionViewModel>> BuildRoleOptionsAsync(List<int> selectedRoleIds)
        {
            return await _context.RolesHuyDds
                .OrderBy(x => x.RoleName)
                .Select(x => new RoleOptionViewModel
                {
                    RoleId = x.RoleIdHuyDd,
                    RoleName = x.RoleName,
                    IsSelected = selectedRoleIds.Contains(x.RoleIdHuyDd)
                })
                .ToListAsync();
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
