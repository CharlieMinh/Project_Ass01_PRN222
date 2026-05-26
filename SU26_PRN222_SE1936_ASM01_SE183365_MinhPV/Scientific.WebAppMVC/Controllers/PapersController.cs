using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.Authorization;
using Scientific.WebAppMVC.ViewModels;
using Scientific.WebAppMVC.ViewModels.Papers;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    public class PapersController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public PapersController(ScientificJournalTrendDBContext context) => _context = context;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] PaperSearchViewModel model)
        {
            model.Journals = await BuildSearchJournalOptionsAsync();

            if (!HasSearchFilters(model))
            {
                model.Results = new List<PaperResultViewModel>();
                return View(model);
            }

            var query = _context.PapersBaoTgs
                .AsNoTracking()
                .Include(x => x.JournalIdMinhPvNavigation)
                .Include(x => x.PaperMetric)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.SearchText))
            {
                var searchText = model.SearchText.Trim();
                query = query.Where(x =>
                    x.Title.Contains(searchText) ||
                    (x.Abstract != null && x.Abstract.Contains(searchText)) ||
                    x.Keywords.Any(k => k.KeywordName.Contains(searchText)));
            }

            if (!string.IsNullOrWhiteSpace(model.AuthorName))
            {
                var authorName = model.AuthorName.Trim();
                query = query.Where(x => x.PaperAuthorsBaoTgs
                    .Any(pa => pa.AuthorIdBaoTgNavigation.FullName.Contains(authorName)));
            }

            if (model.JournalId.HasValue)
            {
                query = query.Where(x => x.JournalIdMinhPv == model.JournalId.Value);
            }

            if (model.PublicationYear.HasValue)
            {
                query = query.Where(x => x.PublicationYear == model.PublicationYear.Value);
            }

            model.Results = await query
                .OrderByDescending(x => x.PublicationYear)
                .ThenBy(x => x.Title)
                .Select(x => new PaperResultViewModel
                {
                    PaperId = x.PaperIdBaoTg,
                    Title = x.Title,
                    JournalName = x.JournalIdMinhPvNavigation != null ? x.JournalIdMinhPvNavigation.JournalName : null,
                    PublicationYear = x.PublicationYear,
                    CitationCount = x.PaperMetric != null ? x.PaperMetric.CitationCount : 0,
                    IsOpenAccess = x.IsOpenAccess
                })
                .ToListAsync();

            return View(model);
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.PapersBaoTgs.Include(x => x.JournalIdMinhPvNavigation).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    (x.Doi != null && x.Doi.Contains(search)) ||
                    (x.JournalIdMinhPvNavigation != null && x.JournalIdMinhPvNavigation.JournalName.Contains(search)));
            }
            ViewBag.Search = search;
            return View(await query.OrderByDescending(x => x.PublicationYear).ThenBy(x => x.Title).ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var paper = await _context.PapersBaoTgs
                .AsNoTracking()
                .Include(x => x.JournalIdMinhPvNavigation)
                    .ThenInclude(x => x.Publisher)
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);

            if (paper == null)
            {
                return NotFound();
            }

            var authors = await _context.PaperAuthorsBaoTgs
                .AsNoTracking()
                .Where(x => x.PaperIdBaoTg == id)
                .OrderBy(x => x.AuthorOrder)
                .Select(x => x.AuthorIdBaoTgNavigation.FullName)
                .ToListAsync();

            var keywords = await _context.PapersBaoTgs
                .AsNoTracking()
                .Where(x => x.PaperIdBaoTg == id)
                .SelectMany(x => x.Keywords)
                .OrderBy(x => x.KeywordName)
                .Select(x => x.KeywordName)
                .ToListAsync();

            var metric = await _context.PaperMetrics
                .AsTracking()
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);

            if (metric == null)
            {
                metric = new PaperMetric
                {
                    PaperIdBaoTg = id,
                    CitationCount = 0,
                    ViewCount = 0,
                    DownloadCount = 0,
                    BookmarkCount = 0,
                    LastUpdated = DateTime.Now
                };
                _context.PaperMetrics.Add(metric);
            }

            metric.ViewCount += 1;
            metric.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            var currentUserId = GetCurrentUserId();
            var isBookmarked = false;
            if (currentUserId.HasValue)
            {
                isBookmarked = await _context.Bookmarks
                    .AsNoTracking()
                    .AnyAsync(x => x.PaperIdBaoTg == id && x.UserIdHuyDd == currentUserId.Value);
            }

            var model = new PaperDetailViewModel
            {
                PaperId = paper.PaperIdBaoTg,
                Title = paper.Title,
                Abstract = paper.Abstract,
                DOI = paper.Doi,
                JournalName = paper.JournalIdMinhPvNavigation?.JournalName,
                PublisherName = paper.JournalIdMinhPvNavigation?.Publisher?.PublisherName,
                PublicationYear = paper.PublicationYear,
                PublicationDate = paper.PublicationDate?.ToDateTime(TimeOnly.MinValue),
                Volume = paper.Volume,
                Issue = paper.Issue,
                Pages = paper.Pages,
                PaperUrl = paper.PaperUrl,
                PdfUrl = paper.PdfUrl,
                SourceName = paper.SourceName,
                IsOpenAccess = paper.IsOpenAccess,
                Authors = authors,
                Keywords = keywords,
                CitationCount = metric.CitationCount,
                ViewCount = metric.ViewCount,
                DownloadCount = metric.DownloadCount,
                BookmarkCount = metric.BookmarkCount,
                IsBookmarkedByCurrentUser = isBookmarked
            };

            return View(model);
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> AssignAuthors(int id)
        {
            var paper = await _context.PapersBaoTgs
                .Include(x => x.PaperAuthorsBaoTgs)
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);

            if (paper == null)
            {
                return NotFound();
            }

            var selectedIds = paper.PaperAuthorsBaoTgs.Select(x => x.AuthorIdBaoTg).ToList();
            var model = new ManyToManyAssignmentViewModel
            {
                EntityId = paper.PaperIdBaoTg,
                EntityName = paper.Title,
                PageTitle = "Assign Authors",
                OptionLabel = "Authors",
                BackController = "Papers",
                SelectedIds = selectedIds,
                Options = await _context.AuthorsBaoTgs
                    .OrderBy(x => x.FullName)
                    .Select(x => new AssignmentOptionViewModel
                    {
                        Id = x.AuthorIdBaoTg,
                        Name = x.FullName,
                        Description = x.Affiliation,
                        IsSelected = selectedIds.Contains(x.AuthorIdBaoTg)
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> AssignAuthors(int id, ManyToManyAssignmentViewModel model)
        {
            var paperExists = await _context.PapersBaoTgs.AnyAsync(x => x.PaperIdBaoTg == id);
            if (!paperExists)
            {
                return NotFound();
            }

            var existingLinks = await _context.PaperAuthorsBaoTgs
                .Where(x => x.PaperIdBaoTg == id)
                .ToListAsync();

            _context.PaperAuthorsBaoTgs.RemoveRange(existingLinks);

            var selectedAuthorIds = model.SelectedIds
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var order = 1;
            foreach (var authorId in selectedAuthorIds)
            {
                _context.PaperAuthorsBaoTgs.Add(new PaperAuthorsBaoTg
                {
                    PaperIdBaoTg = id,
                    AuthorIdBaoTg = authorId,
                    AuthorOrder = order++,
                    IsCorrespondingAuthor = false
                });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Authors assigned successfully. Author order was assigned automatically.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> AssignKeywords(int id)
        {
            var paper = await _context.PapersBaoTgs
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);

            if (paper == null)
            {
                return NotFound();
            }

            var selectedIds = paper.Keywords.Select(x => x.KeywordIdLuanNtk).ToList();
            var model = new ManyToManyAssignmentViewModel
            {
                EntityId = paper.PaperIdBaoTg,
                EntityName = paper.Title,
                PageTitle = "Assign Keywords",
                OptionLabel = "Keywords",
                BackController = "Papers",
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
        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> AssignKeywords(int id, ManyToManyAssignmentViewModel model)
        {
            var paper = await _context.PapersBaoTgs
                .AsTracking()
                .Include(x => x.Keywords)
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);

            if (paper == null)
            {
                return NotFound();
            }

            var selectedKeywords = await _context.KeywordsLuanNtks
                .AsTracking()
                .Where(x => model.SelectedIds.Contains(x.KeywordIdLuanNtk))
                .ToListAsync();

            paper.Keywords.Clear();
            foreach (var keyword in selectedKeywords)
            {
                paper.Keywords.Add(keyword);
            }

            paper.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Keywords assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Create()
        {
            return View(new PaperFormViewModel { JournalOptions = await BuildJournalOptionsAsync() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Create(PaperFormViewModel model)
        {
            if (model.PublicationYear == null && model.PublicationDate.HasValue)
            {
                model.PublicationYear = model.PublicationDate.Value.Year;
            }
            if (!ModelState.IsValid)
            {
                model.JournalOptions = await BuildJournalOptionsAsync();
                return View(model);
            }

            var paper = new PapersBaoTg
            {
                Title = model.Title.Trim(),
                Abstract = Clean(model.Abstract),
                Doi = Clean(model.Doi),
                JournalIdMinhPv = model.JournalId,
                PublicationDate = model.PublicationDate.HasValue ? DateOnly.FromDateTime(model.PublicationDate.Value) : null,
                PublicationYear = model.PublicationYear,
                Volume = Clean(model.Volume),
                Issue = Clean(model.Issue),
                Pages = Clean(model.Pages),
                PaperUrl = Clean(model.PaperUrl),
                PdfUrl = Clean(model.PdfUrl),
                SourceName = Clean(model.SourceName),
                IsOpenAccess = model.IsOpenAccess,
                CreatedByHuyDd = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            _context.PapersBaoTgs.Add(paper);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Paper created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Edit(int id)
        {
            var paper = await _context.PapersBaoTgs.FindAsync(id);
            if (paper == null) return NotFound();

            var model = new PaperFormViewModel
            {
                PaperId = paper.PaperIdBaoTg,
                Title = paper.Title,
                Abstract = paper.Abstract,
                Doi = paper.Doi,
                JournalId = paper.JournalIdMinhPv,
                PublicationDate = paper.PublicationDate?.ToDateTime(TimeOnly.MinValue),
                PublicationYear = paper.PublicationYear,
                Volume = paper.Volume,
                Issue = paper.Issue,
                Pages = paper.Pages,
                PaperUrl = paper.PaperUrl,
                PdfUrl = paper.PdfUrl,
                SourceName = paper.SourceName,
                IsOpenAccess = paper.IsOpenAccess,
                JournalOptions = await BuildJournalOptionsAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Edit(int id, PaperFormViewModel model)
        {
            if (model.PaperId != id) return BadRequest();
            if (model.PublicationYear == null && model.PublicationDate.HasValue)
            {
                model.PublicationYear = model.PublicationDate.Value.Year;
            }
            if (!ModelState.IsValid)
            {
                model.JournalOptions = await BuildJournalOptionsAsync();
                return View(model);
            }

            var paper = await _context.PapersBaoTgs.AsTracking().FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);
            if (paper == null) return NotFound();

            paper.Title = model.Title.Trim();
            paper.Abstract = Clean(model.Abstract);
            paper.Doi = Clean(model.Doi);
            paper.JournalIdMinhPv = model.JournalId;
            paper.PublicationDate = model.PublicationDate.HasValue ? DateOnly.FromDateTime(model.PublicationDate.Value) : null;
            paper.PublicationYear = model.PublicationYear;
            paper.Volume = Clean(model.Volume);
            paper.Issue = Clean(model.Issue);
            paper.Pages = Clean(model.Pages);
            paper.PaperUrl = Clean(model.PaperUrl);
            paper.PdfUrl = Clean(model.PdfUrl);
            paper.SourceName = Clean(model.SourceName);
            paper.IsOpenAccess = model.IsOpenAccess;
            paper.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Paper updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> Delete(int id)
        {
            var paper = await _context.PapersBaoTgs
                .Include(x => x.JournalIdMinhPvNavigation)
                .FirstOrDefaultAsync(x => x.PaperIdBaoTg == id);
            return paper == null ? NotFound() : View(paper);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPolicies.DataManager)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paper = await _context.PapersBaoTgs.FindAsync(id);
            if (paper == null) return NotFound();
            try
            {
                _context.PapersBaoTgs.Remove(paper);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Paper deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this paper because related records exist.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static bool HasSearchFilters(PaperSearchViewModel model)
        {
            return !string.IsNullOrWhiteSpace(model.SearchText) ||
                !string.IsNullOrWhiteSpace(model.AuthorName) ||
                model.JournalId.HasValue ||
                model.PublicationYear.HasValue;
        }

        private async Task<List<SelectListItem>> BuildSearchJournalOptionsAsync()
        {
            return await _context.JournalsMinhPvs
                .AsNoTracking()
                .OrderBy(x => x.JournalName)
                .Select(x => new SelectListItem { Value = x.JournalIdMinhPv.ToString(), Text = x.JournalName })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildJournalOptionsAsync()
        {
            var items = await _context.JournalsMinhPvs
                .OrderBy(x => x.JournalName)
                .Select(x => new SelectListItem { Value = x.JournalIdMinhPv.ToString(), Text = x.JournalName })
                .ToListAsync();
            items.Insert(0, new SelectListItem { Value = "", Text = "-- No journal --" });
            return items;
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
