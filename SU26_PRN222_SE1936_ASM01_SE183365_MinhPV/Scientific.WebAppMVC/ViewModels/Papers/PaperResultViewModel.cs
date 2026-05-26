namespace Scientific.WebAppMVC.ViewModels.Papers
{
    public class PaperResultViewModel
    {
        public int PaperId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? JournalName { get; set; }
        public int? PublicationYear { get; set; }
        public int CitationCount { get; set; }
        public bool IsOpenAccess { get; set; }
    }
}
