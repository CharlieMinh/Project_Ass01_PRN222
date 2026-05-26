namespace Scientific.WebAppMVC.ViewModels.Notifications
{
    public class NotificationsIndexViewModel
    {
        public List<NotificationItemViewModel> Notifications { get; set; } = new();
        public int UnreadCount => Notifications.Count(x => !x.IsRead);
    }
}
