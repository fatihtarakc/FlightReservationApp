using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModels.Account
{
    public class VerifyPhoneVM
    {
        public string Email { get; set; } = string.Empty;
        public string MaskedPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;
    }
}
