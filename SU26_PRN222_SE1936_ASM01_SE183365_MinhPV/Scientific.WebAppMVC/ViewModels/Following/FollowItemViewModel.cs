namespace Scientific.WebAppMVC.ViewModels.Following
{
    public class FollowItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
