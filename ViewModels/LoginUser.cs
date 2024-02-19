using System.ComponentModel.DataAnnotations;

namespace HalloDocService.ViewModels
{
    public partial class LoginUser1
    {


        [Required(ErrorMessage = "Email is required.")]
        /*[EmailAddress(ErrorMessage = "Enter Valid Email.")]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "Invalid Email Address.")]*/
        public string? Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        /*     [StringLength(255,MinimumLength=4,ErrorMessage ="Password must be atleast 4 character")]*/
        public string? PasswordHash { get; set; } = null!;
    }
}
