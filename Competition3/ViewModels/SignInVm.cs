using System.ComponentModel.DataAnnotations;

namespace Competition3.ViewModels
{
    public class SignInVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}