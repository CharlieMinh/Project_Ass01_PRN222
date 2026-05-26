using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.ViewModels.Dashboard;

namespace Scientific.WebAppMVC.Controllers
{
    [AllowAnonymous]
    public class DashboardController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public DashboardController(ScientificJournalTrendDBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index(int? keywordId, int? fromYear, int? toYear)
        {
            var latestTrendYear = await _context.TrendRecords
                .AsNoTracking()
                .MaxAsync(x => (int?)x.RecordYear);

            var selectedKeywordId = keywordId ?? await ResolveDefaultKeywordIdAsync(latestTrendYear);
            var normalizedFromYear = fromYear;
            var normalizedToYear = toYear;

            if (normalizedFromYear.HasValue && normalizedToYear.HasValue && normalizedFromYear > normalizedToYear)
            {
                (normalizedFromYear, normalizedToYear) = (normalizedToYear, normalizedFromYear);
            }

            var model = new TrendDashboardViewModel
            {
                TotalPapers = await _context.PapersBaoTgs.AsNoTracking().CountAsync(),
                TotalAuthors = await _context.AuthorsBaoTgs.AsNoTracking().CountAsync(),
                TotalJournals = await _context.JournalsMinhPvs.AsNoTracking().CountAsync(),
                TotalKeywords = await _context.KeywordsLuanNtks.AsNoTracking().CountAsync(),
                TotalTopics = await _context.TopicsLuanNtks.AsNoTracking().CountAsync(),
                SelectedKeywordId = selectedKeywordId,
                SelectedKeywordName = await GetKeywordNameAsync(selectedKeywordId),
                FromYear = normalizedFromYear,
                ToYear = normalizedToYear,
                LatestTrendYear = latestTrendYear,
                KeywordOptions = await BuildKeywordOptionsAsync(selectedKeywordId)
            };

            if (selectedKeywordId.HasValue)
            {
                var trendQuery = _context.TrendRecords
                    .AsNoTracking()
                    .Where(x => x.KeywordId == selectedKeywordId.Value);

                if (normalizedFromYear.HasValue)
                {
                    trendQuery = trendQuery.Where(x => x.RecordYear >= normalizedFromYear.Value);
                }

                if (normalizedToYear.HasValue)
                {
                    trendQuery = trendQuery.Where(x => x.RecordYear <= normalizedToYear.Value);
                }

                var yearlyTrends = await trendQuery
                    .GroupBy(x => x.RecordYear)
                    .Select(g => new
                    {
                        Year = g.Key,
                        PaperCount = g.Sum(x => x.PaperCount)
                    })
                    .OrderBy(x => x.Year)
                    .ToListAsync();

                model.TrendLabels = yearlyTrends.Select(x => x.Year.ToString()).ToList();
                model.TrendPaperCounts = yearlyTrends.Select(x => x.PaperCount).ToList();
            }

            if (latestTrendYear.HasValue)
            {
                var topKeywords = await _context.TrendRecords
                    .AsNoTracking()
                    .Where(x => x.RecordYear == latestTrendYear.Value && x.KeywordId != null)
                    .GroupBy(x => new { x.KeywordId, x.Keyword!.KeywordName })
                    .Select(g => new
                    {
                        Name = g.Key.KeywordName,
                        Score = g.Max(x => x.TrendScore ?? 0)
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(10)
                    .ToListAsync();

                model.TopKeywordLabels = topKeywords.Select(x => x.Name).ToList();
                model.TopKeywordScores = topKeywords.Select(x => x.Score).ToList();

                var topTopics = await _context.TrendRecords
                    .AsNoTracking()
                    .Where(x => x.RecordYear == latestTrendYear.Value && x.TopicId != null)
                    .GroupBy(x => new { x.TopicId, x.Topic!.TopicName })
                    .Select(g => new
                    {
                        Name = g.Key.TopicName,
                        Score = g.Max(x => x.TrendScore ?? 0)
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(10)
                    .ToListAsync();

                model.TopTopicLabels = topTopics.Select(x => x.Name).ToList();
                model.TopTopicScores = topTopics.Select(x => x.Score).ToList();
            }

            model.TopPapers = await _context.PapersBaoTgs
                .AsNoTracking()
                .Include(x => x.JournalIdMinhPvNavigation)
                .Include(x => x.PaperMetric)
                .OrderByDescending(x => x.PaperMetric != null ? x.PaperMetric.CitationCount : 0)
                .ThenByDescending(x => x.PaperMetric != null ? x.PaperMetric.BookmarkCount : 0)
                .Take(10)
                .Select(x => new TopPaperViewModel
                {
                    PaperId = x.PaperIdBaoTg,
                    Title = x.Title,
                    JournalName = x.JournalIdMinhPvNavigation != null ? x.JournalIdMinhPvNavigation.JournalName : null,
                    PublicationYear = x.PublicationYear,
                    CitationCount = x.PaperMetric != null ? x.PaperMetric.CitationCount : 0,
                    BookmarkCount = x.PaperMetric != null ? x.PaperMetric.BookmarkCount : 0
                })
                .ToListAsync();

            return View(model);
        }

        private async Task<int?> ResolveDefaultKeywordIdAsync(int? latestTrendYear)
        {
            var artificialIntelligenceKeywordId = await _context.KeywordsLuanNtks
                .AsNoTracking()
                .Where(x => x.KeywordName == "Artificial Intelligence")
                .Select(x => (int?)x.KeywordIdLuanNtk)
                .FirstOrDefaultAsync();

            if (artificialIntelligenceKeywordId.HasValue)
            {
                return artificialIntelligenceKeywordId;
            }

            var aiKeywordId = await _context.TrendRecords
                .AsNoTracking()
                .Where(x => x.KeywordId != null && x.Keyword!.KeywordName.Contains("AI"))
                .OrderByDescending(x => x.RecordYear)
                .ThenByDescending(x => x.TrendScore ?? 0)
                .Select(x => x.KeywordId)
                .FirstOrDefaultAsync();

            if (aiKeywordId.HasValue)
            {
                return aiKeywordId;
            }

            return await _context.TrendRecords
                .AsNoTracking()
                .Where(x => x.KeywordId != null && (!latestTrendYear.HasValue || x.RecordYear == latestTrendYear.Value))
                .OrderByDescending(x => x.TrendScore ?? 0)
                .Select(x => x.KeywordId)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> GetKeywordNameAsync(int? keywordId)
        {
            if (!keywordId.HasValue)
            {
                return null;
            }

            return await _context.KeywordsLuanNtks
                .AsNoTracking()
                .Where(x => x.KeywordIdLuanNtk == keywordId.Value)
                .Select(x => x.KeywordName)
                .FirstOrDefaultAsync();
        }

        private async Task<List<SelectListItem>> BuildKeywordOptionsAsync(int? selectedKeywordId)
        {
            return await _context.KeywordsLuanNtks
                .AsNoTracking()
                .Where(x => x.TrendRecords.Any())
                .OrderBy(x => x.KeywordName)
                .Select(x => new SelectListItem
                {
                    Value = x.KeywordIdLuanNtk.ToString(),
                    Text = x.KeywordName,
                    Selected = selectedKeywordId.HasValue && x.KeywordIdLuanNtk == selectedKeywordId.Value
                })
                .ToListAsync();
        }
    }
}
