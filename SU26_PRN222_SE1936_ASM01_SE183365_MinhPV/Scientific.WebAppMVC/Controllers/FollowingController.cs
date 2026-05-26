using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.ViewModels.Following;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize]
    public class FollowingController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public FollowingController(ScientificJournalTrendDBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var model = new FollowingIndexViewModel
            {
                Keywords = await _context.UserFollowKeywords
                    .AsNoTracking()
                    .Where(x => x.UserIdHuyDd == userId.Value)
                    .OrderBy(x => x.Keyword.KeywordName)
                    .Select(x => new FollowItemViewModel
                    {
                        Id = x.KeywordId,
                        Name = x.Keyword.KeywordName,
                        Description = x.Keyword.Description,
                        FollowedAt = x.CreatedAt
                    })
                    .ToListAsync(),

                Topics = await _context.UserFollowTopics
                    .AsNoTracking()
                    .Where(x => x.UserIdHuyDd == userId.Value)
                    .OrderBy(x => x.Topic.TopicName)
                    .Select(x => new FollowItemViewModel
                    {
                        Id = x.TopicId,
                        Name = x.Topic.TopicName,
                        Description = x.Topic.Description,
                        FollowedAt = x.CreatedAt
                    })
                    .ToListAsync(),

                Journals = await _context.UserFollowJournals
                    .AsNoTracking()
                    .Where(x => x.UserIdHuyDd == userId.Value)
                    .OrderBy(x => x.JournalIdMinhPvNavigation.JournalName)
                    .Select(x => new FollowItemViewModel
                    {
                        Id = x.JournalIdMinhPv,
                        Name = x.JournalIdMinhPvNavigation.JournalName,
                        Description = x.JournalIdMinhPvNavigation.Description,
                        FollowedAt = x.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost("Following/FollowKeyword/{keywordId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowKeyword(int keywordId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var keyword = await _context.KeywordsLuanNtks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.KeywordIdLuanNtk == keywordId);

            if (keyword == null)
            {
                return NotFound();
            }

            var exists = await _context.UserFollowKeywords
                .AnyAsync(x => x.UserIdHuyDd == userId.Value && x.KeywordId == keywordId);

            if (!exists)
            {
                _context.UserFollowKeywords.Add(new UserFollowKeyword
                {
                    UserIdHuyDd = userId.Value,
                    KeywordId = keywordId,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"You are now following keyword: {keyword.KeywordName}.";
            }
            else
            {
                TempData["ErrorMessage"] = "You already follow this keyword.";
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost("Following/UnfollowKeyword/{keywordId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnfollowKeyword(int keywordId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var follow = await _context.UserFollowKeywords
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == userId.Value && x.KeywordId == keywordId);

            if (follow != null)
            {
                _context.UserFollowKeywords.Remove(follow);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Keyword unfollowed successfully.";
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost("Following/FollowTopic/{topicId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowTopic(int topicId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var topic = await _context.TopicsLuanNtks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TopicIdLuanNtk == topicId);

            if (topic == null)
            {
                return NotFound();
            }

            var exists = await _context.UserFollowTopics
                .AnyAsync(x => x.UserIdHuyDd == userId.Value && x.TopicId == topicId);

            if (!exists)
            {
                _context.UserFollowTopics.Add(new UserFollowTopic
                {
                    UserIdHuyDd = userId.Value,
                    TopicId = topicId,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"You are now following topic: {topic.TopicName}.";
            }
            else
            {
                TempData["ErrorMessage"] = "You already follow this topic.";
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost("Following/UnfollowTopic/{topicId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnfollowTopic(int topicId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var follow = await _context.UserFollowTopics
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == userId.Value && x.TopicId == topicId);

            if (follow != null)
            {
                _context.UserFollowTopics.Remove(follow);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Topic unfollowed successfully.";
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost("Following/FollowJournal/{journalId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowJournal(int journalId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var journal = await _context.JournalsMinhPvs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == journalId);

            if (journal == null)
            {
                return NotFound();
            }

            var exists = await _context.UserFollowJournals
                .AnyAsync(x => x.UserIdHuyDd == userId.Value && x.JournalIdMinhPv == journalId);

            if (!exists)
            {
                _context.UserFollowJournals.Add(new UserFollowJournal
                {
                    UserIdHuyDd = userId.Value,
                    JournalIdMinhPv = journalId,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"You are now following journal: {journal.JournalName}.";
            }
            else
            {
                TempData["ErrorMessage"] = "You already follow this journal.";
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost("Following/UnfollowJournal/{journalId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnfollowJournal(int journalId, string? returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var follow = await _context.UserFollowJournals
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == userId.Value && x.JournalIdMinhPv == journalId);

            if (follow != null)
            {
                _context.UserFollowJournals.Remove(follow);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Journal unfollowed successfully.";
            }

            return RedirectBack(returnUrl);
        }

        private IActionResult RedirectBack(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
        }
    }
}
