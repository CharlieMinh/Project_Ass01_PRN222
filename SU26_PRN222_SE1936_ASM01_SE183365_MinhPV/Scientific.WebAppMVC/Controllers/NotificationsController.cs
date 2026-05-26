using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.WebAppMVC.ViewModels.Notifications;
using System.Security.Claims;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ScientificJournalTrendDBContext _context;

        public NotificationsController(ScientificJournalTrendDBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserIdHuyDd == userId.Value)
                .OrderBy(x => x.IsRead)
                .ThenByDescending(x => x.CreatedAt)
                .Select(x => new NotificationItemViewModel
                {
                    NotificationId = x.NotificationId,
                    Title = x.Title,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    RelatedPaperId = x.RelatedPaperIdBaoTg,
                    RelatedPaperTitle = x.RelatedPaperIdBaoTgNavigation != null ? x.RelatedPaperIdBaoTgNavigation.Title : null,
                    RelatedKeywordId = x.RelatedKeywordId,
                    RelatedKeywordName = x.RelatedKeyword != null ? x.RelatedKeyword.KeywordName : null,
                    RelatedTopicId = x.RelatedTopicId,
                    RelatedTopicName = x.RelatedTopic != null ? x.RelatedTopic.TopicName : null
                })
                .ToListAsync();

            return View(new NotificationsIndexViewModel { Notifications = notifications });
        }

        [HttpPost("Notifications/MarkAsRead/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.UserIdHuyDd == userId.Value);

            if (notification == null)
            {
                return NotFound();
            }

            if (!notification.IsRead)
            {
                await _context.Notifications
                    .Where(x => x.NotificationId == id && x.UserIdHuyDd == userId.Value)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsRead, true));
            }

            TempData["SuccessMessage"] = "Notification marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Notifications/MarkAllAsRead")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Challenge();
            }

            await _context.Notifications
                .Where(x => x.UserIdHuyDd == userId.Value && !x.IsRead)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsRead, true));

            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
        }
    }
}
