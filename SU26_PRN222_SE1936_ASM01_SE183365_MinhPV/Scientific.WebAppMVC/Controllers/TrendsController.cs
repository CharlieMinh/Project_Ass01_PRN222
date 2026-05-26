using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.ViewModels.Dashboard;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    [AllowAnonymous]
    public class TrendsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public TrendsController(ScientificJournalTrendDBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Keyword(int id)
        {
            var keyword = await _context.KeywordsLuanNtks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.KeywordIdLuanNtk == id);

            if (keyword == null)
            {
                return NotFound();
            }

            var records = await _context.TrendRecords
                .AsNoTracking()
                .Where(x => x.KeywordId == id)
                .GroupBy(x => x.RecordYear)
                .Select(g => new
                {
                    Year = g.Key,
                    PaperCount = g.Sum(x => x.PaperCount),
                    CitationCount = g.Sum(x => x.CitationCount),
                    TrendScore = g.Max(x => x.TrendScore ?? 0)
                })
                .OrderBy(x => x.Year)
                .ToListAsync();

            var currentUserId = GetCurrentUserId();
            var model = new TrendDetailViewModel
            {
                EntityId = keyword.KeywordIdLuanNtk,
                EntityName = keyword.KeywordName,
                EntityType = "Keyword",
                TrendLabels = records.Select(x => x.Year.ToString()).ToList(),
                PaperCounts = records.Select(x => x.PaperCount).ToList(),
                TrendScores = records.Select(x => x.TrendScore).ToList(),
                TotalPaperCount = records.Sum(x => x.PaperCount),
                TotalCitationCount = records.Sum(x => x.CitationCount),
                AverageTrendScore = records.Any() ? records.Average(x => x.TrendScore) : 0,
                IsFollowedByCurrentUser = currentUserId.HasValue &&
                    await _context.UserFollowKeywords
                        .AsNoTracking()
                        .AnyAsync(x => x.UserIdHuyDd == currentUserId.Value && x.KeywordId == id)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Topic(int id)
        {
            var topic = await _context.TopicsLuanNtks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == id);

            if (topic == null)
            {
                return NotFound();
            }

            var records = await _context.TrendRecords
                .AsNoTracking()
                .Where(x => x.TopicId == id)
                .GroupBy(x => x.RecordYear)
                .Select(g => new
                {
                    Year = g.Key,
                    PaperCount = g.Sum(x => x.PaperCount),
                    CitationCount = g.Sum(x => x.CitationCount),
                    TrendScore = g.Max(x => x.TrendScore ?? 0)
                })
                .OrderBy(x => x.Year)
                .ToListAsync();

            var currentUserId = GetCurrentUserId();
            var model = new TrendDetailViewModel
            {
                EntityId = topic.TopicIdLuanNtk,
                EntityName = topic.TopicName,
                EntityType = "Topic",
                TrendLabels = records.Select(x => x.Year.ToString()).ToList(),
                PaperCounts = records.Select(x => x.PaperCount).ToList(),
                TrendScores = records.Select(x => x.TrendScore).ToList(),
                TotalPaperCount = records.Sum(x => x.PaperCount),
                TotalCitationCount = records.Sum(x => x.CitationCount),
                AverageTrendScore = records.Any() ? records.Average(x => x.TrendScore) : 0,
                IsFollowedByCurrentUser = currentUserId.HasValue &&
                    await _context.UserFollowTopics
                        .AsNoTracking()
                        .AnyAsync(x => x.UserIdHuyDd == currentUserId.Value && x.TopicId == id)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TrendingTopics()
        {
            var latestYear = await _context.TrendRecords
                .AsNoTracking()
                .MaxAsync(x => (int?)x.RecordYear);

            var items = new List<TrendingItemViewModel>();
            if (latestYear.HasValue)
            {
                var topics = await _context.TrendRecords
                    .AsNoTracking()
                    .Where(x => x.TopicId != null && x.RecordYear == latestYear.Value)
                    .GroupBy(x => new { x.TopicId, x.Topic!.TopicName })
                    .Select(g => new TrendingItemViewModel
                    {
                        Id = g.Key.TopicId!.Value,
                        Name = g.Key.TopicName,
                        Type = "Topic",
                        RecordYear = latestYear.Value,
                        PaperCount = g.Sum(x => x.PaperCount),
                        CitationCount = g.Sum(x => x.CitationCount),
                        GrowthRate = g.Max(x => x.GrowthRate ?? 0),
                        TrendScore = g.Max(x => x.TrendScore ?? 0)
                    })
                    .OrderByDescending(x => x.TrendScore)
                    .Take(10)
                    .ToListAsync();

                var keywords = await _context.TrendRecords
                    .AsNoTracking()
                    .Where(x => x.KeywordId != null && x.RecordYear == latestYear.Value)
                    .GroupBy(x => new { x.KeywordId, x.Keyword!.KeywordName })
                    .Select(g => new TrendingItemViewModel
                    {
                        Id = g.Key.KeywordId!.Value,
                        Name = g.Key.KeywordName,
                        Type = "Keyword",
                        RecordYear = latestYear.Value,
                        PaperCount = g.Sum(x => x.PaperCount),
                        CitationCount = g.Sum(x => x.CitationCount),
                        GrowthRate = g.Max(x => x.GrowthRate ?? 0),
                        TrendScore = g.Max(x => x.TrendScore ?? 0)
                    })
                    .OrderByDescending(x => x.TrendScore)
                    .Take(10)
                    .ToListAsync();

                items = topics.Concat(keywords)
                    .OrderByDescending(x => x.TrendScore)
                    .ThenBy(x => x.Name)
                    .ToList();
            }

            return View(items);
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
        }
    }
}
