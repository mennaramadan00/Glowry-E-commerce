using System.ComponentModel.DataAnnotations;

namespace Ghazal.ViewModel
{
    public class SignInViewModel
    {
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "*")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }

        [Display(Name ="Remember me !")]
        public bool RememberMe {  get; set; }

    }
}
