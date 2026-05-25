using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class PublishersController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public PublishersController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Publishers.Include(x => x.JournalsMinhPvs).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.PublisherName.Contains(search) || (x.Country != null && x.Country.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.PublisherName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var publisher = await _context.Publishers
                .Include(x => x.JournalsMinhPvs)
                .FirstOrDefaultAsync(x => x.PublisherId == id);
            return publisher == null ? NotFound() : View(publisher);
        }

        public IActionResult Create() => View(new Publisher { CreatedAt = DateTime.Now });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherName,Country,WebsiteUrl")] Publisher publisher)
        {
            if (string.IsNullOrWhiteSpace(publisher.PublisherName))
            {
                ModelState.AddModelError(nameof(publisher.PublisherName), "Publisher name is required.");
            }
            if (!ModelState.IsValid) return View(publisher);

            publisher.PublisherName = publisher.PublisherName.Trim();
            publisher.Country = Clean(publisher.Country);
            publisher.WebsiteUrl = Clean(publisher.WebsiteUrl);
            publisher.CreatedAt = DateTime.Now;
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Publisher created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            return publisher == null ? NotFound() : View(publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PublisherId,PublisherName,Country,WebsiteUrl,CreatedAt")] Publisher publisher)
        {
            if (id != publisher.PublisherId) return BadRequest();
            if (string.IsNullOrWhiteSpace(publisher.PublisherName))
            {
                ModelState.AddModelError(nameof(publisher.PublisherName), "Publisher name is required.");
            }
            if (!ModelState.IsValid) return View(publisher);

            publisher.PublisherName = publisher.PublisherName.Trim();
            publisher.Country = Clean(publisher.Country);
            publisher.WebsiteUrl = Clean(publisher.WebsiteUrl);
            _context.Update(publisher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Publisher updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _context.Publishers
                .Include(x => x.JournalsMinhPvs)
                .FirstOrDefaultAsync(x => x.PublisherId == id);
            return publisher == null ? NotFound() : View(publisher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();
            try
            {
                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Publisher deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this publisher because journals are using it.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
