using System.ComponentModel.DataAnnotations;

namespace HWPLoginControl.Models
{
    public class ForgottenPasswordEmail
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
