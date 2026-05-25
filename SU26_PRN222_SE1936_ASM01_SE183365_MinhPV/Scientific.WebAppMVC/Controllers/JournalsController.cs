using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;
using Scientific.WebAppMVC.ViewModels;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class JournalsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public JournalsController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.JournalsMinhPvs.Include(x => x.Publisher).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.JournalName.Contains(search) ||
                    (x.Issn != null && x.Issn.Contains(search)) ||
                    (x.Eissn != null && x.Eissn.Contains(search)) ||
                    (x.Publisher != null && x.Publisher.PublisherName.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.JournalName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var journal = await _context.JournalsMinhPvs
                .Include(x => x.Publisher)
                .Include(x => x.CategoryIdMinhPvs)
                .Include(x => x.PapersBaoTgs)
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);
            return journal == null ? NotFound() : View(journal);
        }

        public async Task<IActionResult> AssignCategories(int id)
        {
            var journal = await _context.JournalsMinhPvs
                .Include(x => x.CategoryIdMinhPvs)
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);

            if (journal == null)
            {
                return NotFound();
            }

            var selectedIds = journal.CategoryIdMinhPvs.Select(x => x.CategoryIdMinhPv).ToList();
            var model = new ManyToManyAssignmentViewModel
            {
                EntityId = journal.JournalIdMinhPv,
                EntityName = journal.JournalName,
                PageTitle = "Assign Categories",
                OptionLabel = "Categories",
                BackController = "Journals",
                SelectedIds = selectedIds,
                Options = await _context.CategoriesMinhPvs
                    .OrderBy(x => x.CategoryName)
                    .Select(x => new AssignmentOptionViewModel
                    {
                        Id = x.CategoryIdMinhPv,
                        Name = x.CategoryName,
                        Description = x.Description,
                        IsSelected = selectedIds.Contains(x.CategoryIdMinhPv)
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCategories(int id, ManyToManyAssignmentViewModel model)
        {
            var journal = await _context.JournalsMinhPvs
                .AsTracking()
                .Include(x => x.CategoryIdMinhPvs)
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);

            if (journal == null)
            {
                return NotFound();
            }

            var selectedCategories = await _context.CategoriesMinhPvs
                .AsTracking()
                .Where(x => model.SelectedIds.Contains(x.CategoryIdMinhPv))
                .ToListAsync();

            journal.CategoryIdMinhPvs.Clear();
            foreach (var category in selectedCategories)
            {
                journal.CategoryIdMinhPvs.Add(category);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Categories assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Create()
        {
            var model = new JournalFormViewModel
            {
                PublisherOptions = await BuildPublisherOptionsAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JournalFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PublisherOptions = await BuildPublisherOptionsAsync();
                return View(model);
            }

            var journal = new JournalsMinhPv
            {
                JournalName = model.JournalName.Trim(),
                Issn = Clean(model.Issn),
                Eissn = Clean(model.Eissn),
                PublisherId = model.PublisherId,
                Country = Clean(model.Country),
                WebsiteUrl = Clean(model.WebsiteUrl),
                ImpactFactor = model.ImpactFactor,
                Ranking = Clean(model.Ranking),
                Description = Clean(model.Description),
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.JournalsMinhPvs.Add(journal);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Journal created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var journal = await _context.JournalsMinhPvs.FindAsync(id);
            if (journal == null) return NotFound();

            var model = new JournalFormViewModel
            {
                JournalId = journal.JournalIdMinhPv,
                JournalName = journal.JournalName,
                Issn = journal.Issn,
                Eissn = journal.Eissn,
                PublisherId = journal.PublisherId,
                Country = journal.Country,
                WebsiteUrl = journal.WebsiteUrl,
                ImpactFactor = journal.ImpactFactor,
                Ranking = journal.Ranking,
                Description = journal.Description,
                IsActive = journal.IsActive,
                PublisherOptions = await BuildPublisherOptionsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JournalFormViewModel model)
        {
            if (model.JournalId != id) return BadRequest();
            if (!ModelState.IsValid)
            {
                model.PublisherOptions = await BuildPublisherOptionsAsync();
                return View(model);
            }

            var journal = await _context.JournalsMinhPvs.AsTracking().FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);
            if (journal == null) return NotFound();

            journal.JournalName = model.JournalName.Trim();
            journal.Issn = Clean(model.Issn);
            journal.Eissn = Clean(model.Eissn);
            journal.PublisherId = model.PublisherId;
            journal.Country = Clean(model.Country);
            journal.WebsiteUrl = Clean(model.WebsiteUrl);
            journal.ImpactFactor = model.ImpactFactor;
            journal.Ranking = Clean(model.Ranking);
            journal.Description = Clean(model.Description);
            journal.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Journal updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var journal = await _context.JournalsMinhPvs
                .Include(x => x.Publisher)
                .Include(x => x.PapersBaoTgs)
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);
            return journal == null ? NotFound() : View(journal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journal = await _context.JournalsMinhPvs.FindAsync(id);
            if (journal == null) return NotFound();
            try
            {
                _context.JournalsMinhPvs.Remove(journal);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Journal deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this journal because related records exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> BuildPublisherOptionsAsync()
        {
            var items = await _context.Publishers
                .OrderBy(x => x.PublisherName)
                .Select(x => new SelectListItem { Value = x.PublisherId.ToString(), Text = x.PublisherName })
                .ToListAsync();
            items.Insert(0, new SelectListItem { Value = "", Text = "-- No publisher --" });
            return items;
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
