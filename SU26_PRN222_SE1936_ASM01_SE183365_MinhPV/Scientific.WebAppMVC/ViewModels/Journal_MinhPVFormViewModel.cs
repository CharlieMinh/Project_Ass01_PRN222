using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Scientific.WebAppMVC.ViewModels
{
    public class Journal_MinhPVFormViewModel
    {
        public int? JournalId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Journal name")]
        public string JournalName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Issn { get; set; }

        [StringLength(50)]
        public string? Eissn { get; set; }

        [Display(Name = "Publisher")]
        public int? PublisherId { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(500)]
        [Display(Name = "Website URL")]
        public string? WebsiteUrl { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Impact factor")]
        public decimal? ImpactFactor { get; set; }

        [StringLength(50)]
        public string? Ranking { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> PublisherOptions { get; set; } = new();
    }
}
