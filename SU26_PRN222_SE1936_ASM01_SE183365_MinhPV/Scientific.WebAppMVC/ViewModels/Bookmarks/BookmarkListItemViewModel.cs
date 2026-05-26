namespace Scientific.WebAppMVC.ViewModels.Bookmarks
{
    public class BookmarkListItemViewModel
    {
        public int BookmarkId { get; set; }
        public int PaperId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? JournalName { get; set; }
        public int? PublicationYear { get; set; }
        public int CitationCount { get; set; }
        public bool IsOpenAccess { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
