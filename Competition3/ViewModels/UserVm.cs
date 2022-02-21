using System.ComponentModel.DataAnnotations;
using DNTPersianUtils.Core;

namespace Competition3.ViewModels
{
    public class UserVm
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [ValidIranianMobileNumber]
        public string MobileNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}