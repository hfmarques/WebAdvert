using System.ComponentModel.DataAnnotations;

namespace Models.Accounts
{
    public class ConfirmModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
    }
}