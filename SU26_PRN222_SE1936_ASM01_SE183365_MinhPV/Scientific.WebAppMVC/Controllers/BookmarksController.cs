using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.ViewModels.Bookmarks;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public BookmarksController(ScientificJournalTrendDBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var bookmarks = await _context.Bookmarks
                .AsNoTracking()
                .Where(x => x.UserIdHuyDd == userId.Value)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new BookmarkListItemViewModel
                {
                    BookmarkId = x.BookmarkId,
                    PaperId = x.PaperIdBaoTg,
                    Title = x.PaperIdBaoTgNavigation.Title,
                    JournalName = x.PaperIdBaoTgNavigation.JournalIdMinhPvNavigation != null
                        ? x.PaperIdBaoTgNavigation.JournalIdMinhPvNavigation.JournalName
                        : null,
                    PublicationYear = x.PaperIdBaoTgNavigation.PublicationYear,
                    CitationCount = x.PaperIdBaoTgNavigation.PaperMetric != null
                        ? x.PaperIdBaoTgNavigation.PaperMetric.CitationCount
                        : 0,
                    IsOpenAccess = x.PaperIdBaoTgNavigation.IsOpenAccess,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return View(new BookmarksIndexViewModel { Bookmarks = bookmarks });
        }

        [HttpPost("Bookmarks/Add/{paperId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int paperId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var paperExists = await _context.PapersBaoTgs.AnyAsync(x => x.PaperIdBaoTg == paperId);
            if (!paperExists)
            {
                return NotFound();
            }

            var exists = await _context.Bookmarks
                .AnyAsync(x => x.UserIdHuyDd == userId.Value && x.PaperIdBaoTg == paperId);

            if (exists)
            {
                TempData["ErrorMessage"] = "This paper is already in your bookmarks.";
                return RedirectBack(returnUrl, paperId);
            }

            _context.Bookmarks.Add(new Bookmark
            {
                UserIdHuyDd = userId.Value,
                PaperIdBaoTg = paperId,
                CreatedAt = DateTime.Now
            });

            var metric = await GetOrCreatePaperMetricAsync(paperId);
            metric.BookmarkCount += 1;
            metric.LastUpdated = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Paper bookmarked successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Could not save this bookmark. It may already exist.";
            }

            return RedirectBack(returnUrl, paperId);
        }

        [HttpPost("Bookmarks/Remove/{paperId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int paperId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == userId.Value && x.PaperIdBaoTg == paperId);

            if (bookmark == null)
            {
                TempData["ErrorMessage"] = "This paper is not in your bookmarks.";
                return RedirectBack(returnUrl, paperId);
            }

            _context.Bookmarks.Remove(bookmark);

            var metric = await _context.PaperMetrics.FirstOrDefaultAsync(x => x.PaperIdBaoTg == paperId);
            if (metric != null)
            {
                metric.BookmarkCount = Math.Max(0, metric.BookmarkCount - 1);
                metric.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bookmark removed successfully.";

            return RedirectBack(returnUrl, paperId);
        }

        private async Task<PaperMetric> GetOrCreatePaperMetricAsync(int paperId)
        {
            var metric = await _context.PaperMetrics.FirstOrDefaultAsync(x => x.PaperIdBaoTg == paperId);
            if (metric != null)
            {
                return metric;
            }

            metric = new PaperMetric
            {
                PaperIdBaoTg = paperId,
                CitationCount = 0,
                ViewCount = 0,
                DownloadCount = 0,
                BookmarkCount = 0,
                LastUpdated = DateTime.Now
            };
            _context.PaperMetrics.Add(metric);
            return metric;
        }

        private IActionResult RedirectBack(string? returnUrl, int paperId)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Papers", new { id = paperId });
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
        }
    }
}
