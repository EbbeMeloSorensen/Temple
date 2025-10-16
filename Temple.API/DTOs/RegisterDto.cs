using System.ComponentModel.DataAnnotations;

namespace Temple.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        // (?=.*\\d)     At least on number
        // (?=.*[a-z])   At least on lowercase letter
        // (?=.*[A-Z])   At least on uppercase letter
        // .{4,8}        From 4 to 8 characters
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }
}