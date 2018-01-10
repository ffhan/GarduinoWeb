using System.ComponentModel.DataAnnotations;

namespace GarduinoAPI.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
