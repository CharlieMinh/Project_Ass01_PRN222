using System.ComponentModel.DataAnnotations;

namespace Scientific.WebAppMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Organization { get; set; }

        [StringLength(100)]
        [Display(Name = "Academic title")]
        public string? AcademicTitle { get; set; }
    }
}
