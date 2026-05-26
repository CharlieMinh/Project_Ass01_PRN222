using Microsoft.AspNetCore.Mvc.Rendering;

namespace Scientific.WebAppMVC.ViewModels.Dashboard
{
    public class TrendDashboardViewModel
    {
        public int TotalPapers { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalJournals { get; set; }
        public int TotalKeywords { get; set; }
        public int TotalTopics { get; set; }

        public int? SelectedKeywordId { get; set; }
        public string? SelectedKeywordName { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
        public int? LatestTrendYear { get; set; }
        public List<SelectListItem> KeywordOptions { get; set; } = new();

        public List<string> TrendLabels { get; set; } = new();
        public List<int> TrendPaperCounts { get; set; } = new();

        public List<string> TopKeywordLabels { get; set; } = new();
        public List<decimal> TopKeywordScores { get; set; } = new();

        public List<string> TopTopicLabels { get; set; } = new();
        public List<decimal> TopTopicScores { get; set; } = new();

        public List<TopPaperViewModel> TopPapers { get; set; } = new();
    }
}
