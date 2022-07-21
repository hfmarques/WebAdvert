using System.ComponentModel.DataAnnotations;

namespace Models.Accounts
{
    public class NewPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required] [Display(Name = "Token")] public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and its confirmation do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}