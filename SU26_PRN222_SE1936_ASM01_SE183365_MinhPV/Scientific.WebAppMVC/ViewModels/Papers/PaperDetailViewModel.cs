namespace Scientific.WebAppMVC.ViewModels.Papers
{
    public class PaperDetailViewModel
    {
        public int PaperId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Abstract { get; set; }
        public string? DOI { get; set; }
        public int? JournalId { get; set; }
        public string? JournalName { get; set; }
        public string? PublisherName { get; set; }
        public int? PublicationYear { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Volume { get; set; }
        public string? Issue { get; set; }
        public string? Pages { get; set; }
        public string? PaperUrl { get; set; }
        public string? PdfUrl { get; set; }
        public string? SourceName { get; set; }
        public bool IsOpenAccess { get; set; }

        public List<string> Authors { get; set; } = new();
        public List<string> Keywords { get; set; } = new();

        public int CitationCount { get; set; }
        public int ViewCount { get; set; }
        public int DownloadCount { get; set; }
        public int BookmarkCount { get; set; }

        public bool IsBookmarkedByCurrentUser { get; set; }
        public bool IsJournalFollowedByCurrentUser { get; set; }
    }
}
