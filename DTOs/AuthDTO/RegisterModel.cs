using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.AuthDTO
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Bakery name is required.")]
        [StringLength(200, ErrorMessage = "The bakery name must be at most 200 characters long.")]
        public string BakeryName { get; set; } = null!;
    }
}