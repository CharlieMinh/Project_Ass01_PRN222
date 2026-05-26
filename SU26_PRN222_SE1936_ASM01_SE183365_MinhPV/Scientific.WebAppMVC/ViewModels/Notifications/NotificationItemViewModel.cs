namespace Scientific.WebAppMVC.ViewModels.Notifications
{
    public class NotificationItemViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RelatedPaperId { get; set; }
        public string? RelatedPaperTitle { get; set; }
        public int? RelatedKeywordId { get; set; }
        public string? RelatedKeywordName { get; set; }
        public int? RelatedTopicId { get; set; }
        public string? RelatedTopicName { get; set; }
    }
}
