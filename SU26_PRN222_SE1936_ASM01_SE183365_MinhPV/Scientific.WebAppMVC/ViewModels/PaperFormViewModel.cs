using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Scientific.WebAppMVC.ViewModels
{
    public class PaperFormViewModel
    {
        public int? PaperId { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        public string? Abstract { get; set; }

        [StringLength(255)]
        public string? Doi { get; set; }

        [Display(Name = "Journal")]
        public int? JournalId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publication date")]
        public DateTime? PublicationDate { get; set; }

        [Range(1900, 2100)]
        [Display(Name = "Publication year")]
        public int? PublicationYear { get; set; }

        [StringLength(50)]
        public string? Volume { get; set; }

        [StringLength(50)]
        public string? Issue { get; set; }

        [StringLength(50)]
        public string? Pages { get; set; }

        [StringLength(500)]
        [Display(Name = "Paper URL")]
        public string? PaperUrl { get; set; }

        [StringLength(500)]
        [Display(Name = "PDF URL")]
        public string? PdfUrl { get; set; }

        [StringLength(100)]
        [Display(Name = "Source name")]
        public string? SourceName { get; set; }

        [Display(Name = "Open access")]
        public bool IsOpenAccess { get; set; }

        public List<SelectListItem> JournalOptions { get; set; } = new();
    }
}
