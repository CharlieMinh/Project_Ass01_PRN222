namespace Scientific.WebAppMVC.ViewModels.Dashboard
{
    public class TrendDetailViewModel
    {
        public int EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;

        public List<string> TrendLabels { get; set; } = new();
        public List<int> PaperCounts { get; set; } = new();
        public List<decimal> TrendScores { get; set; } = new();
        public int TotalPaperCount { get; set; }
        public int TotalCitationCount { get; set; }
        public decimal AverageTrendScore { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
}
