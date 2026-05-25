using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class CategoriesController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public CategoriesController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.CategoriesMinhPvs.Include(x => x.JournalIdMinhPvs).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.CategoryName.Contains(search) || (x.Description != null && x.Description.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.CategoryName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.CategoriesMinhPvs
                .Include(x => x.JournalIdMinhPvs)
                .FirstOrDefaultAsync(x => x.CategoryIdMinhPv == id);
            return category == null ? NotFound() : View(category);
        }

        public IActionResult Create() => View(new CategoriesMinhPv { CreatedAt = DateTime.Now });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Description")] CategoriesMinhPv category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError(nameof(category.CategoryName), "Category name is required.");
            }
            if (await _context.CategoriesMinhPvs.AnyAsync(x => x.CategoryName == category.CategoryName))
            {
                ModelState.AddModelError(nameof(category.CategoryName), "This category already exists.");
            }
            if (!ModelState.IsValid) return View(category);

            category.CategoryName = category.CategoryName.Trim();
            category.Description = Clean(category.Description);
            category.CreatedAt = DateTime.Now;
            _context.CategoriesMinhPvs.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.CategoriesMinhPvs.FindAsync(id);
            return category == null ? NotFound() : View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryIdMinhPv,CategoryName,Description,CreatedAt")] CategoriesMinhPv category)
        {
            if (id != category.CategoryIdMinhPv) return BadRequest();
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError(nameof(category.CategoryName), "Category name is required.");
            }
            if (await _context.CategoriesMinhPvs.AnyAsync(x => x.CategoryName == category.CategoryName && x.CategoryIdMinhPv != id))
            {
                ModelState.AddModelError(nameof(category.CategoryName), "This category already exists.");
            }
            if (!ModelState.IsValid) return View(category);

            category.CategoryName = category.CategoryName.Trim();
            category.Description = Clean(category.Description);
            _context.Update(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.CategoriesMinhPvs
                .Include(x => x.JournalIdMinhPvs)
                .FirstOrDefaultAsync(x => x.CategoryIdMinhPv == id);
            return category == null ? NotFound() : View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.CategoriesMinhPvs.FindAsync(id);
            if (category == null) return NotFound();
            try
            {
                _context.CategoriesMinhPvs.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this category because journals are using it.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
