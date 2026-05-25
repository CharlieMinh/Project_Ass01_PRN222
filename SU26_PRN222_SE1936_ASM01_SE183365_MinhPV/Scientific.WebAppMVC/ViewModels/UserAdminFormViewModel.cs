using System.ComponentModel.DataAnnotations;

namespace Scientific.WebAppMVC.ViewModels
{
    public class UserAdminFormViewModel
    {
        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [StringLength(20)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }

        [StringLength(200)]
        public string? Organization { get; set; }

        [StringLength(100)]
        [Display(Name = "Academic title")]
        public string? AcademicTitle { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<int> SelectedRoleIds { get; set; } = new();
        public List<RoleOptionViewModel> AvailableRoles { get; set; } = new();
    }
}
