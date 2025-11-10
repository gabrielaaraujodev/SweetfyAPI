using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.AuthDTO
{
    public class LoginModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
