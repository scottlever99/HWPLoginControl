using System.ComponentModel.DataAnnotations;

namespace HWPLoginControl.Models
{
    public class ForgottenPassword
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Password should be longer than 7 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
