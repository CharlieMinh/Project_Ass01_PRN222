using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;
using Scientific.WebAppMVC.ViewModels;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.DataManager)]
    public class TopicsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public TopicsController(ScientificJournalTrendDBContext context) => _context = context;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.TopicsLuanNtks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.TopicName.Contains(search) || (x.Description != null && x.Description.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderBy(x => x.TopicName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var topic = await _context.TopicsLuanNtks
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == id);
            return topic == null ? NotFound() : View(topic);
        }

        public async Task<IActionResult> AssignKeywords(int id)
        {
            var topic = await _context.TopicsLuanNtks
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == id);

            if (topic == null)
            {
                return NotFound();
            }

            var selectedIds = topic.Keywords.Select(x => x.KeywordIdLuanNtk).ToList();
            var model = new ManyToManyAssignmentViewModel
            {
                EntityId = topic.TopicIdLuanNtk,
                EntityName = topic.TopicName,
                PageTitle = "Assign Keywords",
                OptionLabel = "Keywords",
                BackController = "Topics",
                SelectedIds = selectedIds,
                Options = await _context.KeywordsLuanNtks
                    .OrderBy(x => x.KeywordName)
                    .Select(x => new AssignmentOptionViewModel
                    {
                        Id = x.KeywordIdLuanNtk,
                        Name = x.KeywordName,
                        Description = x.Description,
                        IsSelected = selectedIds.Contains(x.KeywordIdLuanNtk)
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignKeywords(int id, ManyToManyAssignmentViewModel model)
        {
            var topic = await _context.TopicsLuanNtks
                .AsTracking()
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == id);

            if (topic == null)
            {
                return NotFound();
            }

            var selectedKeywords = await _context.KeywordsLuanNtks
                .AsTracking()
                .Where(x => model.SelectedIds.Contains(x.KeywordIdLuanNtk))
                .ToListAsync();

            topic.Keywords.Clear();
            foreach (var keyword in selectedKeywords)
            {
                topic.Keywords.Add(keyword);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Keywords assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Create() => View(new TopicsLuanNtk { CreatedAt = DateTime.Now });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TopicName,Description")] TopicsLuanNtk topic)
        {
            if (string.IsNullOrWhiteSpace(topic.TopicName))
            {
                ModelState.AddModelError(nameof(topic.TopicName), "Topic name is required.");
            }
            if (await _context.TopicsLuanNtks.AnyAsync(x => x.TopicName == topic.TopicName))
            {
                ModelState.AddModelError(nameof(topic.TopicName), "This topic already exists.");
            }
            if (!ModelState.IsValid) return View(topic);

            topic.TopicName = topic.TopicName.Trim();
            topic.Description = Clean(topic.Description);
            topic.CreatedAt = DateTime.Now;
            _context.TopicsLuanNtks.Add(topic);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Topic created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _context.TopicsLuanNtks.FindAsync(id);
            return topic == null ? NotFound() : View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TopicIdLuanNtk,TopicName,Description,CreatedAt")] TopicsLuanNtk topic)
        {
            if (id != topic.TopicIdLuanNtk) return BadRequest();
            if (string.IsNullOrWhiteSpace(topic.TopicName))
            {
                ModelState.AddModelError(nameof(topic.TopicName), "Topic name is required.");
            }
            if (await _context.TopicsLuanNtks.AnyAsync(x => x.TopicName == topic.TopicName && x.TopicIdLuanNtk != id))
            {
                ModelState.AddModelError(nameof(topic.TopicName), "This topic already exists.");
            }
            if (!ModelState.IsValid) return View(topic);

            topic.TopicName = topic.TopicName.Trim();
            topic.Description = Clean(topic.Description);
            _context.Update(topic);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Topic updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var topic = await _context.TopicsLuanNtks
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == id);
            return topic == null ? NotFound() : View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.TopicsLuanNtks.FindAsync(id);
            if (topic == null) return NotFound();
            try
            {
                _context.TopicsLuanNtks.Remove(topic);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Topic deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this topic because related records exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
