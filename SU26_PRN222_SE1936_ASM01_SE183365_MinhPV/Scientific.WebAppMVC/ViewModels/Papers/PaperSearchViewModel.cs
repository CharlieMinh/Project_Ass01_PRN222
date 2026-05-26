using Microsoft.AspNetCore.Mvc.Rendering;

namespace Scientific.WebAppMVC.ViewModels.Papers
{
    public class PaperSearchViewModel
    {
        public string? SearchText { get; set; }
        public string? AuthorName { get; set; }
        public int? JournalId { get; set; }
        public int? PublicationYear { get; set; }

        public List<SelectListItem> Journals { get; set; } = new();
        public List<PaperResultViewModel> Results { get; set; } = new();
    }
}
