namespace Scientific.WebAppMVC.ViewModels.Dashboard
{
    public class TrendingItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int RecordYear { get; set; }
        public int PaperCount { get; set; }
        public int CitationCount { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal TrendScore { get; set; }
    }
}
