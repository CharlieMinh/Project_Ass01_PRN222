namespace Scientific.WebAppMVC.ViewModels.Following
{
    public class FollowingIndexViewModel
    {
        public List<FollowItemViewModel> Keywords { get; set; } = new();
        public List<FollowItemViewModel> Topics { get; set; } = new();
        public List<FollowItemViewModel> Journals { get; set; } = new();
    }
}
