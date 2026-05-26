namespace Scientific.WebAppMVC.ViewModels.Dashboard
{
    public class TopPaperViewModel
    {
        public int PaperId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? JournalName { get; set; }
        public int? PublicationYear { get; set; }
        public int CitationCount { get; set; }
        public int BookmarkCount { get; set; }
    }
}
