using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.DataManager)]
    public class KeywordsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public KeywordsController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.KeywordsLuanNtks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.KeywordName.Contains(search) || (x.Description != null && x.Description.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.KeywordName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var keyword = await _context.KeywordsLuanNtks
                .Include(x => x.PaperIdBaoTgs)
                .Include(x => x.Topics)
                .FirstOrDefaultAsync(x => x.KeywordIdLuanNtk == id);
            return keyword == null ? NotFound() : View(keyword);
        }

        public IActionResult Create() => View(new KeywordsLuanNtk { CreatedAt = DateTime.Now });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KeywordName,Description")] KeywordsLuanNtk keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword.KeywordName))
            {
                ModelState.AddModelError(nameof(keyword.KeywordName), "Keyword name is required.");
            }
            if (await _context.KeywordsLuanNtks.AnyAsync(x => x.KeywordName == keyword.KeywordName))
            {
                ModelState.AddModelError(nameof(keyword.KeywordName), "This keyword already exists.");
            }
            if (!ModelState.IsValid) return View(keyword);

            keyword.KeywordName = keyword.KeywordName.Trim();
            keyword.Description = Clean(keyword.Description);
            keyword.CreatedAt = DateTime.Now;
            _context.KeywordsLuanNtks.Add(keyword);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Keyword created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var keyword = await _context.KeywordsLuanNtks.FindAsync(id);
            return keyword == null ? NotFound() : View(keyword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KeywordIdLuanNtk,KeywordName,Description,CreatedAt")] KeywordsLuanNtk keyword)
        {
            if (id != keyword.KeywordIdLuanNtk) return BadRequest();
            if (string.IsNullOrWhiteSpace(keyword.KeywordName))
            {
                ModelState.AddModelError(nameof(keyword.KeywordName), "Keyword name is required.");
            }
            if (await _context.KeywordsLuanNtks.AnyAsync(x => x.KeywordName == keyword.KeywordName && x.KeywordIdLuanNtk != id))
            {
                ModelState.AddModelError(nameof(keyword.KeywordName), "This keyword already exists.");
            }
            if (!ModelState.IsValid) return View(keyword);

            keyword.KeywordName = keyword.KeywordName.Trim();
            keyword.Description = Clean(keyword.Description);
            _context.Update(keyword);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Keyword updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var keyword = await _context.KeywordsLuanNtks
                .Include(x => x.PaperIdBaoTgs)
                .Include(x => x.Topics)
                .FirstOrDefaultAsync(x => x.KeywordIdLuanNtk == id);
            return keyword == null ? NotFound() : View(keyword);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keyword = await _context.KeywordsLuanNtks.FindAsync(id);
            if (keyword == null) return NotFound();
            try
            {
                _context.KeywordsLuanNtks.Remove(keyword);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Keyword deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this keyword because related records exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
