using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.DataManager)]
    public class AuthorsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public AuthorsController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.AuthorsBaoTgs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.FullName.Contains(search) || (x.Affiliation != null && x.Affiliation.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.FullName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var author = await _context.AuthorsBaoTgs
                .Include(x => x.AuthorMetric)
                .Include(x => x.PaperAuthorsBaoTgs).ThenInclude(x => x.PaperIdBaoTgNavigation)
                .FirstOrDefaultAsync(x => x.AuthorIdBaoTg == id);
            return author == null ? NotFound() : View(author);
        }

        public IActionResult Create() => View(new AuthorsBaoTg { CreatedAt = DateTime.Now });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Email,Affiliation,Country,Orcid,GoogleScholarUrl,ScopusId")] AuthorsBaoTg author)
        {
            if (string.IsNullOrWhiteSpace(author.FullName))
            {
                ModelState.AddModelError(nameof(author.FullName), "Full name is required.");
            }
            if (!ModelState.IsValid) return View(author);

            author.FullName = author.FullName.Trim();
            author.Email = Clean(author.Email);
            author.Affiliation = Clean(author.Affiliation);
            author.Country = Clean(author.Country);
            author.Orcid = Clean(author.Orcid);
            author.GoogleScholarUrl = Clean(author.GoogleScholarUrl);
            author.ScopusId = Clean(author.ScopusId);
            author.CreatedAt = DateTime.Now;
            _context.AuthorsBaoTgs.Add(author);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Author created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.AuthorsBaoTgs.FindAsync(id);
            return author == null ? NotFound() : View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorIdBaoTg,FullName,Email,Affiliation,Country,Orcid,GoogleScholarUrl,ScopusId,CreatedAt")] AuthorsBaoTg author)
        {
            if (id != author.AuthorIdBaoTg) return BadRequest();
            if (string.IsNullOrWhiteSpace(author.FullName))
            {
                ModelState.AddModelError(nameof(author.FullName), "Full name is required.");
            }
            if (!ModelState.IsValid) return View(author);

            author.FullName = author.FullName.Trim();
            author.Email = Clean(author.Email);
            author.Affiliation = Clean(author.Affiliation);
            author.Country = Clean(author.Country);
            author.Orcid = Clean(author.Orcid);
            author.GoogleScholarUrl = Clean(author.GoogleScholarUrl);
            author.ScopusId = Clean(author.ScopusId);
            _context.Update(author);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Author updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.AuthorsBaoTgs
                .Include(x => x.PaperAuthorsBaoTgs)
                .FirstOrDefaultAsync(x => x.AuthorIdBaoTg == id);
            return author == null ? NotFound() : View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.AuthorsBaoTgs.FindAsync(id);
            if (author == null) return NotFound();
            try
            {
                _context.AuthorsBaoTgs.Remove(author);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Author deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this author because related records exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
