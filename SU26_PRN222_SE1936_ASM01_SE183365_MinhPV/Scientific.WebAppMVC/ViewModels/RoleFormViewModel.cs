using System.ComponentModel.DataAnnotations;

namespace Scientific.WebAppMVC.ViewModels
{
    public class RoleFormViewModel
    {
        public int? RoleId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Role name")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
